using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCO_Player
{
    public static class Vars
    {
        public static string AppPath = AppDomain.CurrentDomain.BaseDirectory;   // Путь до файла приложения

        public static Guid Id_playlist; // Буфер индекса

        public static Guid id_playlist; // Индекс

        public static List<Tuple<Guid, string>> files = new List<Tuple<Guid, string>>(); // Буфер файлов воспроизведения

        public static List<Tuple<Guid, string>> Tracklist = new List<Tuple<Guid, string>>(); // Трэклист

        public static int CurrentTrackNumber;   // Корректирующая переменная
    }
}
