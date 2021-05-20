using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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

namespace DCO_Player
{
    /// <summary>
    /// Логика взаимодействия для My_playlists.xaml
    /// </summary>
    public partial class My_playlists : Page
    {
        public My_playlists()
        {
            //InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlExpression = "SELECT * FROM Playlists"; // Делаем запрос к плейлистам
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read())
                    {
                        if(Profile.Id_users == (Guid)reader.GetValue(0)){
                            PlaylistControl playlistControl = new PlaylistControl(); // Создаем образ контрола с плейлистом

                            playlistControl.Margin = new Thickness(64, 35, 0, 29);

                            playlistControl.Instance = this;
                            playlistControl.Id_playlist = (Guid)reader.GetValue(1);

                            playlistControl.PlaylistName.Content = reader.GetValue(5).ToString(); // Передаем имя плейлиста в контрол
                            if(reader.GetValue(2).ToString() != "")
                            {
                                playlistControl.Image.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + reader.GetValue(5).ToString(), UriKind.Absolute)); // Передаем картинку в плейлист
                            }
                            WPM.Children.Add(playlistControl); // Добавляем контрол на страницу
                        }
                    }
                }
                reader.Close();
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new CreatePlaylist().Show();
        }
<<<<<<< HEAD

        private void Sync_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
=======
>>>>>>> 614100299399cdaad423c1deaae47e9c9aa1208b
    }
}
