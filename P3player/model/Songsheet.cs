using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3player.model
{
    class Songsheet
    {
        private List<Song> sheetList = new List<Song>();
        public Songsheet()
        {
            Path();  //路径初始化
        }
        public void Path()
        {
            if (!File.Exists("H:/G音乐/Songsheet.txt"))
            {
                FileStream fs = new FileStream("H:\\G音乐/Songsheet.txt", FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(fs);
                sw.Close();
                fs.Close();
            }
        }
    }
}
