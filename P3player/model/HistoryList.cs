using P3player.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3player.model
{
    class HistoryList
    {
        private List<Song> hisList = new List<Song>();
        public HistoryList()
        {
            Path();  //路径初始化
            writehis(); //将记录写入txt文件
        }
        public void writehis()
        {
            string path = "H:/G音乐/history.txt";
            StreamReader objReader = new StreamReader(path,Encoding.Default);
            string sLine = "";
            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null && !sLine.Equals(""))
                {
                    String[] s = sLine.Split('#');
                    Song song = new Song(s[1], s[2], s[0], 0, 0);
                    hisList.Add(song);
                    
                }
            }
            objReader.Close();
        }
        public void Path()
        {
            if (!File.Exists("H:/G音乐/history.txt"))
            {
                FileStream fs = new FileStream("H:\\G音乐/history.txt", FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(fs);
                sw.Close();
                fs.Close();
            }
        
        }

        public void AddHistory(Song song)
        {
            bool i = true;
            int index = 0;
            foreach(Song s in hisList)
            {
                if (s.fullPath .Equals( song.fullPath))
                {
                    i = false;
                    break;
                }
                index++;
            }
            if (i)
            {
                hisList.Insert(0, song);
            }
            else
            {
                hisList.RemoveAt(index);
                hisList.Insert(0, song);
            }

            sava2HisFile();//刷新his文件
        }
        public void sava2HisFile()
        {
            string newTxtPath = "H:/G音乐/history.txt";//创建txt文件的具体路径，我这里在选中的路径中创建名为depth的txt文件
            StreamWriter sw = new StreamWriter(newTxtPath, false, Encoding.Default);//实例化StreamWriter
            //中间参数属性如果此值为false，则创建一个新文件，如果存在原文件，则覆盖。
            //如果此值为true，则打开文件保留原来数据，如果找不到文件，则创建新文件。
            foreach(Song s in hisList)
            {
                sw.Write(s.ToString()+"\n");
            }
            sw.Flush();
            sw.Close();

        }

        public List<Song> getList()
        {
            return hisList;
        }
    }
}
