using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCO_Player
{
    public class Song
    {
        public Guid Id_song { get; set; }

        public bool is_local = true;
        public string full_name { get; set; }
        public string name { get; set; }
        public string artist { get; set; }
        public string album { get; set; }
        public int length { get; set; }
        public string path { get; set; }
        public int n_sequence { get; set; }
    }
}
