using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Threading;

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
            List<Compositions> compositions = new List<Compositions>();

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
