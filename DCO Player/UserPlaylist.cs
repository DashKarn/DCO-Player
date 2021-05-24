using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCO_Player
{
    public class UserPlaylist
    {
        public const string NULL_NAME = "1A5f8r9Dddw8ds58FEKaE58dwbf";
        public Guid Id_user { get; set; }
        public Guid Id_playlist { get; set; }
        public string name { get; set; }
        public DateTime lastUpdate { get; set; }
        public DateTime lastSync { get; set; }
        public string imageSrc { get; set; }
        public List<Song> songs { get; set; }
    }
}
