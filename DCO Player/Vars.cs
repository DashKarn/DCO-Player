using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCO_Player
{
    public static class Vars
    {
        public static UserPlaylist playlist;

        public static List<Tuple<Guid, string>> allPlaylists;

        public static string AppPath = AppDomain.CurrentDomain.BaseDirectory;   // Путь до файла приложения

        public static Guid Id_playlist; // Буфер индекса

        public static List<Composition> current = new List<Composition>(); // Буфер файлов воспроизведения

        public static List<Composition> search = new List<Composition>(); // Буфер файлов воспроизведения

        public static List<Composition> StreamTracklist = new List<Composition>(); // Трэклист

        public static int CurrentTrackNumber;   // Корректирующая переменная
    }
}
