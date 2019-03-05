using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace P3player.model
{
    class songLrc
    {
        //private List<String> lrclist = new List<String>();
        //读取歌词
        public struct TLrc
        {
            public int ms;//毫秒  
            public string lrc;//对应的歌词  
        }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string LrcBy { get; set; }
        public string Offset { get; set; }

        
    }
}
