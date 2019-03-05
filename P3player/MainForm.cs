using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using P3player.model;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;

namespace P3player
{
    public partial class MainForm : Form
    {
        int currentIndex = 0;
        int sign = 0;
        

        private List<Song> songList = new List<Song>();


        private HistoryList historyList = new HistoryList(); //历史

        private Songsheet songsheet = new Songsheet();//歌单

        private songLrc songLrc = new songLrc(); //歌词

        public DrawMode OwnerDrawVariable { get; private set; }

        public MainForm()
        {
            InitializeComponent();
            initPath();  //listbox1获取歌曲列表
            listBox2.Hide();
        }


        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        //播放歌曲并记录历史
        private void PlaySong(List<Song> list)
        {
            int index = listBox1.SelectedIndex;
            if (index < 0) { }
            else
            {
                Song s = list[index];

                player.Ctlcontrols.currentItem = player.currentPlaylist.Item[index];

                player.Ctlcontrols.play();
                //记录播放历史
                historyList.AddHistory(s);

            }

        }

        public void initPath()
        {
            player.windowlessVideo = true;//双击全屏
            //listBox1.Items.Clear();  //清空控件
            string url = "H:/网易云音乐下载";
            DirectoryInfo folder = new DirectoryInfo(url);
            foreach (FileInfo file in folder.GetFiles("*.mp3"))
            {
                WMPLib.IWMPMedia media = player.newMedia(file.FullName);
                player.currentPlaylist.appendItem(media);
                double time = media.duration;
                String author= media.getItemInfo("Author"); ;
                String title =media.getItemInfo("Title");
                Song song = new Song(title, file.FullName, author, (long)time, sign);
                songList.Add(song);
                listBox1.Items.Add("  " + author + "             " + title + "            " + time);
            }
        }
        


        private void listBox1_DoubleClick(object sender, EventArgs e)

        {
            if (sign == 0)
                PlaySong(songList);
            else
                PlaySong(historyList.getList());

            //显示歌词
            string file = null;
            GetSong();
            //启动一个新的进程
            showLrcTh = new Thread(new ThreadStart(showLrc));
            showLrcTh.Start();

        }
        Thread showLrcTh;
        //本地音乐
        public void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            sign = 0;
            listBox1.Items.Clear();  //清空控件
            foreach (Song s in songList)
            {
                listBox1.Items.Add("  " + s.author + "             " + s.songName + "             " + s.time);
            }
        }
        //播放历史
        public void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            sign = 1;
            listBox1.Items.Clear();  //清空控件
            //显示播放历史
            foreach (Song song in historyList.getList())
            {
                listBox1.Items.Add(song.author + "                " + song.songName);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    


        public List<string> lrcList = new List<string>();
        //显示歌词
        private int count = 0;
        private void button2_Click(object sender, EventArgs e)
        {
            count++;
            int a = count % 2;
            switch (a)
            {
                case 0:
                    listBox2.Hide();
                    break;
                case 1:
                    listBox2.Show();

                    break;
            }

        }
        private int getCurrentIndex(String path)
        {
            for(int i = 0; i < songList.Count; i++)
            {
                if (path.Equals(songList[i].fullPath)){
                    return i;
                }
            }
            return -1;
        }

        //获得当前正在播放的歌曲
        public void GetSong()
        {
            //获取歌词路径
            int index = getCurrentIndex(player.currentMedia.sourceURL);
            if (index < 0)
                return;
            string songpath = songList[index].ToString();
            string s = Path.GetFileNameWithoutExtension(songpath);
            s += ".lrc";
            //MessageBox.Show(s);


            //判断当前播放的的歌曲是否有歌词文件
            if (File.Exists(s))
            {
                string path = songList[index].fullPath;
                string n = Path.GetFileNameWithoutExtension(path);
                //string z = Path.
                DirectoryInfo info = new DirectoryInfo(n);
                string P= info.Parent.FullName;//E文件夹路径 C:\A\B\C\D\E
                n += ".lrc";
                string p1 = Path.Combine(P,n);
                Getlrc(p1);
            }
            else//否则只显示歌曲名字
            {
                string l = Path.GetFileNameWithoutExtension(songpath);
                listBox2.Items.Clear();
                listBox2.Items.Add("                     "+l);
            }
        }
        Dictionary<double, string> dicword = new Dictionary<double, string>();
        public void Getlrc(string path)
        {
            dicword.Clear();
            songLrc lrc = new songLrc(); //lrc表
            StreamReader L = File.OpenText(path);
            string nextLine;
            while ((nextLine = L.ReadLine()) != null)
            {
                if (nextLine.Trim().Equals(""))
                {
                    continue;
                }
                if (nextLine.StartsWith("[ti:"))
                {
                    lrc.Title = SplitInfo(nextLine);
                }
                else if (nextLine.StartsWith("[ar:"))
                {
                    lrc.Artist = SplitInfo(nextLine);
                }
                else if (nextLine.StartsWith("[al:"))
                {
                    lrc.Album = SplitInfo(nextLine);
                }
                else if (nextLine.StartsWith("[by:"))
                {
                    lrc.LrcBy = SplitInfo(nextLine);
                }
                else if (nextLine.StartsWith("[offset:"))
                {
                    lrc.Offset = SplitInfo(nextLine);
                }
                else
                {
                    try
                    {
                        Regex regexword = new Regex(@".*\](.*)");
                        Match mcw = regexword.Match(nextLine);
                        string word = mcw.Groups[1].Value;
                        Regex regextime = new Regex(@"\[([0-9.:]*)\]", RegexOptions.Compiled);
                        MatchCollection mct = regextime.Matches(nextLine);
                        foreach (Match item in mct)
                        {
                            double time = TimeSpan.Parse("00:" + item.Groups[1].Value).TotalSeconds;
                            dicword.Add(time, word);
                        }
                    }
                    catch
                    {
                        continue;
                    }

                }
                
            }
            L.Close();
        }
        static string SplitInfo(string line)
        {
            return line.Substring(line.IndexOf(":") + 1).TrimEnd(']');
        }
        private double nowPosition = 0;
        public void showLrc()
        {
            int i ;
            List<double> times = new List<double>(dicword.Keys);
            List<String> lrcs = new List<string>(dicword.Values);
            //寻找当前歌词位置
            for (i = times.Count-1; i>=0&&times[i] > nowPosition; i--) ;

            i = i < 0 ? 0 : i;
            Control.CheckForIllegalCrossThreadCalls = false;
            while (times.Count!=0&&true)
            {
                listBox2.Items.Clear();
                listBox2.Items.Add(lrcs[i]);

                if (i == times.Count-1)
                    break;
                Thread.Sleep((int)(times[i + 1] - times[i])*1000);
                i++;
            }
        }

        private void player_PositionChange(object sender, AxWMPLib._WMPOCXEvents_PositionChangeEvent e)
        {
            showLrcTh.Abort();//先终止该线程
            nowPosition = player.Ctlcontrols.currentPosition;
            Console.Write(nowPosition.ToString());
            showLrcTh = new Thread(new ThreadStart(showLrc));
            showLrcTh.Start();//最后启动该线程
        }
    }
}
