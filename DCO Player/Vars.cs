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

        public static int Id_album; // Буфер индекса

        public static int id_album; // Индекс

        public static List<Tuple<int, string>> files = new List<Tuple<int, string>>(); // Буфер файлов воспроизведения

        public static List<Tuple<int, string>> Tracklist = new List<Tuple<int, string>>(); // Трэклист

        public static int CurrentTrackNumber;   // Корректирующая переменная
    }
}
