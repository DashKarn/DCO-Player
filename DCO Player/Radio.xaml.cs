using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Net;
using System.Windows.Threading;

namespace DCO_Player
{
    /// <summary>
    /// Логика взаимодействия для Radio.xaml
    /// </summary>
    /// 
    public class RadioStation
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string descr { get; set; }
        public string page { get; set; }
        public string stream { get; set; }
        public string imageSrc { get; set; }
    }
    public class RadioComposition
    {
        public string Time { get; set; }
        public string Artist { get; set; }
        public string Track { get; set; }
    }
    public partial class Radio : Page
    {
        public List<RadioComposition> Composition(string page, RadioControl RC)
        {
            string str, time = "", artist = "", track = "";
            MatchCollection times;
            MatchCollection names;
            List<RadioComposition> compositions = new List<RadioComposition>();

            try
            {
                WebClient web = new WebClient();    // Веб клиент, для получения
                web.Encoding = Encoding.UTF8;       // разметки страницы в формате
                str = web.DownloadString(page);     // utf-8

                Regex First = new Regex(@"name"">.{1,120}[\d]{2}:[\d]{2}<");  // Регулярка для получения необработанных записей треков из html
                Regex Second = new Regex(@"[\d]{2}:[\d]{2}");                           // Регулярка для получения времени из списка необработанных записей
                Regex Third = new Regex(@"(?<=>).{1,70}(?= - )");                      // Регулярка для получения имени исполнителя из списка необработанных записей
                Regex Fourth = new Regex(@"(?<= - )[^<]{1,30}");                    // Регулярка для получения имени композиции из списка необработанных записей

                MatchCollection firstMatches = First.Matches(str);
                foreach (Match match in firstMatches)
                {
                    times = Second.Matches(match.Value);    // Получаем время композиции из списка необработанных
                    foreach (Match t in times)              // треков, путем получения элемента и дальнейшего
                        time = t.Value;                     // прогона через цикл и получения времени

                    names = Third.Matches(match.Value);     // Получаем исполнителя композиции из списка необработанных
                    foreach (Match n in names)              // треков, путем получения элемента и дальнейшего
                        artist = n.Value;                   // прогона через цикл и получения исполнителя

                    names = Fourth.Matches(match.Value);    // Получаем имя композиции из списка необработанных
                    foreach (Match n in names)              // треков, путем получения элемента и дальнейшего
                        track = n.Value;                    // прогона через цикл и получения имени

                    compositions.Add(new RadioComposition()
                    {
                        Time = time,
                        Artist = artist,
                        Track = track
                    });
                }
                RC.CompositionName.Text = compositions[0].Track;
                RC.ArtistName.Text = compositions[0].Artist;


            }
            catch
            {
               // MessageBox.Show("Отстутствует подключение к сети интернет");
            }
            return compositions;
        }

        public Radio()
        {
            InitializeComponent();

            bool get_db;
            List<RadioStation> radiostations = new List<RadioStation>();

            (get_db, radiostations) = Database.GetRadio();
            if (radiostations.Count == 0)
            {
                radiostations = Firebase.GetRadio();
                if (radiostations.Count > 0 && get_db)
                    Database.InsertRadio(radiostations);
            }
            if (radiostations.Count > 0)
            {
                foreach (var station in radiostations)
                {
                    Firebase.toserver(station);

                    RadioControl RC = new RadioControl();


                    List<RadioComposition> compositions = Composition(station.page, RC);
                    RC.Margin = new Thickness(42, 31, 42, 0);
                    RC.ImageRadio.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + station.imageSrc, UriKind.Absolute));
                    RC.RadiostationName.Text = station.name;
                    RC.src = station.stream;

                    RC.ImageRadio.MouseDown += RC_MouseDown;

                    WPR.Children.Add(RC);
                    DispatcherTimer timer = new DispatcherTimer(); // Создание таймера обновлений композиций через 30s

                    timer.Tick += Timer_Tick;
                    timer.Interval = new TimeSpan(0, 0, 10);
                    timer.Start();

                    void Timer_Tick(object sender, EventArgs e) // событие обновления таймера
                    {
                        Composition(station.page, RC);
                    }

                    void RC_MouseDown(object sender, MouseButtonEventArgs e)
                    {
                        RadioIn radioIn = new RadioIn();
                        int ind = 1;

                        radioIn.ImageRadio.Source = RC.ImageRadio.Source;           // Ресурс изображения
                        radioIn.RadiostationName.Text = RC.RadiostationName.Text;   // Название радиостанции
                        radioIn.RadiostationDescription.Text = station.descr;         // Описание
                        foreach (RadioComposition comp in compositions)
                        {
                            RadioPlaylistControl RPC = new RadioPlaylistControl();

                            RPC.Margin = new Thickness(42, 0, 42, 15);// Отбивка
                            RPC.Index.Text = ind.ToString(); ind++; // Индекс плейлиста
                            RPC.CompositionName.Text = comp.Track;  // Композиция
                            RPC.ArtistName.Text = comp.Artist;      // Исполнитель
                            RPC.Time.Text = comp.Time;              // Время начала исполнения

                            radioIn.SPPlaylist.Children.Add(RPC);   // Добавление в плейлист
                        }

                        this.NavigationService.Navigate(radioIn);
                    }

                }
            }
            else
                MessageBox.Show("Не удаётся загрузить радиостанции. \n" +
                    "Отсутствует подключение к базе данных и интернет соединение");
        }
    }
}
