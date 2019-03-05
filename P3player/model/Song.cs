using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3player.model
{
    class Song
    {
        public String songName;
        public String fullPath;
        public String author;
        public long time;
        public int sign; //标志位，判断是否播放过
        public Song(String songName, String fullPath, String author, long time,int sign)
        {
            this.songName = songName;
            this.fullPath = fullPath;
            this.author = author;
            this.time = time;
            this.sign = 0;
        }

        public bool IsRecommInfo { get; internal set; }

        public String ToString()
        {
            return author + "#" + songName+"#" +fullPath;
        }
    }
}
