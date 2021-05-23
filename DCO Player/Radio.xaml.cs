using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;

namespace DCO_Player
{
    /// <summary>
    /// Логика взаимодействия для Radio.xaml
    /// </summary>
    public class Compositions
    {
        public string Time { get; set; }
        public string Artist { get; set; }
        public string Track { get; set; }
    }
    public partial class Radio : Page
    {

        string connectionString;

        public List<Compositions> Composition(string page, RadioControl RC)
        {
            string str, time = "", artist = "", track = "";
            MatchCollection times;
            MatchCollection names;
            List<Compositions> compositions = new List<Compositions>();

            try
            {
                WebClient web = new WebClient();    // Веб клиент, для получения
                web.Encoding = Encoding.UTF8;       // разметки страницы в формате
                str = web.DownloadString(page);     // utf-8

                Regex First = new Regex(@"<td>[\d]{2}:[\d]{2}<\/td>.{1,120}<\/span>");  // Регулярка для получения необработанных записей треков из html
                Regex Second = new Regex(@"[\d]{2}:[\d]{2}");                           // Регулярка для получения времени из списка необработанных записей
                Regex Third = new Regex(@"(?<=out"">).{1,70}(?= - )");                      // Регулярка для получения имени исполнителя из списка необработанных записей
                Regex Fourth = new Regex(@"(?<= - ).{1,70}(?=<\/span>)");                    // Регулярка для получения имени композиции из списка необработанных записей

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

                    compositions.Add(new Compositions()
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
                //MessageBox.Show("Отстутствует подключение к сети интернет");
            }
            return compositions;
        }

        public Radio()
        {
            InitializeComponent();

            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlExpression = "SELECT * FROM Radio";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read())
                    {
                        string page = "";
                        RadioControl RC = new RadioControl();

                        string Description = reader.GetValue(2).ToString();
            
                        page = reader.GetValue(4).ToString();
                        List<Compositions> compositions = Composition(page, RC);
                        RC.Margin = new Thickness(42, 31, 42, 0);
                        RC.ImageRadio.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + reader.GetValue(5).ToString(), UriKind.Absolute));
                        RC.RadiostationName.Text = reader.GetValue(1).ToString();
                        RC.src = reader.GetValue(3).ToString();

                        RC.ImageRadio.MouseDown += RC_MouseDown;

                        WPR.Children.Add(RC);

                        void RC_MouseDown(object sender, MouseButtonEventArgs e)
                        {
                            RadioIn radioIn = new RadioIn();
                            int ind = 1;

                            radioIn.ImageRadio.Source = RC.ImageRadio.Source;           // Ресурс изображения
                            radioIn.RadiostationName.Text = RC.RadiostationName.Text;   // Название радиостанции
                            radioIn.RadiostationDescription.Text = Description;         // Описание
                            foreach(Compositions comp in compositions)
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
                reader.Close();
            }
        }

        
    }
}
