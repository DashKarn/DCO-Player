using System;
using System.IO;
using TagLib;

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

        public void addProperties(string path)
        {
            var tfile = TagLib.File.Create(path);
            this.is_local = true;
            this.full_name = Path.GetFileNameWithoutExtension(path);
            this.name = tfile.Tag.Title;
            if (tfile.Tag.FirstArtist == null || tfile.Tag.FirstArtist == "")
                this.artist = tfile.Tag.FirstPerformer;
            else
                this.artist = tfile.Tag.FirstArtist;
            this.album = tfile.Tag.Album;
            this.length = (int)tfile.Properties.Duration.TotalSeconds;
            this.path = path;

            if (album == null)
                album = "";
            string[] sep = { " - " };
            string[] words = full_name.Split(sep, System.StringSplitOptions.RemoveEmptyEntries);
            if (name == "" || name == null)
            {
                if (words.Length == 1)
                    name = full_name;
                else
                    name = words[0];
            }
            if (artist == "" || artist == null)
            {
                if (words.Length == 1)
                    artist = full_name;
                else
                    artist = words[1];
            }
        }
    }
}
