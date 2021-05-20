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
    /// Логика взаимодействия для Artist.xaml
    /// </summary>
    public partial class Artist : UserControl
    {
        public int Id_artists { get; set; }

        public Artists Instance { get; set; }

        string connectionString;

        public Artist()
        {
            InitializeComponent();
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            BorderArtist.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5AE3DB"));
            ArtistName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#F0F0F0"));
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderArtist.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#303030"));
            ArtistName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
        }

        private void ArtistName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //TextBlock s = (TextBlock)sender;

            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlExpressionFirst = "SELECT Id_artist, Artist, Album, Price, Album_image_source, Id_albums FROM Artists, Albums Where Artists.Id_artists = Albums.Id_artist"; // Делаем запрос к исполнителям
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpressionFirst, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    CountryAlbums albums = new CountryAlbums(); // Получаем новую страницу с альбомами
                    albums.Artist.Content = ArtistName.Text; // Именуем страницу

                    while (reader.Read())
                    {
                        if (Id_artists == (int)reader.GetValue(0)) // Проверяем на совпадение исполнителя
                        {
                            AlbumControl albumControl = new AlbumControl(); // Создаем образ контрола с альбомом

                            albumControl.Margin = new Thickness(32);

                            albumControl.InstanceCountry = albums.Instance;

                            albumControl.ArtistName.Text = reader.GetValue(1).ToString(); // Передаем имя Исполнителя в контрол
                            albumControl.AlbumName.Text = reader.GetValue(2).ToString(); // Передаем имя Альбома в контрол
                            albumControl.Price.Content = "$" + reader.GetValue(3).ToString(); // Передаем цену в альбом
                            albumControl.price = (int)reader.GetValue(3);
                            albumControl.Id_albums = (int)reader.GetValue(5);
                            albumControl.Image.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + reader.GetValue(4).ToString(), UriKind.Absolute)); // Передаем картинку в альбом

                            albums.WPA.Children.Add(albumControl); // Добавляем контрол на страницу
                        }

                    }
                    Instance.NavigationService.Navigate(albums); // Переходим на страницу
                }
                reader.Close();
            }
        }
    }
}
