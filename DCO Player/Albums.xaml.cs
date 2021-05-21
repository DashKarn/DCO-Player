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
    /// Логика взаимодействия для Albums.xaml
    /// </summary>
    public partial class Albums : Page
    {

        public Albums()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlExpression = "SELECT Id_artist, Artist, Album, Price, Album_image_source, Albums.Id_albums FROM Artists, Albums, Purchased_albums Where Artists.Id_artists = Albums.Id_artist and Purchased_albums.Id_albums = Albums.Id_albums and Purchased_albums.Id_user = " + Profile.Id_user; // Делаем запрос к исполнителям
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read())
                    {
                            AlbumControl albumControl = new AlbumControl(); // Создаем образ контрола с альбомом

                            albumControl.Margin = new Thickness(32);

                            albumControl.InstanceAlbums = this;

                            albumControl.ArtistName.Text = reader.GetValue(1).ToString(); // Передаем имя Исполнителя в контрол
                            albumControl.AlbumName.Text = reader.GetValue(2).ToString(); // Передаем имя Альбома в контрол
                            albumControl.Price.Content = "OK"; // Передаем цену в альбом
                            albumControl.price = (int)reader.GetValue(3);
                            albumControl.Id_albums = (Guid)reader.GetValue(5);
                            albumControl.Image.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + reader.GetValue(4).ToString(), UriKind.Absolute)); // Передаем картинку в альбом

                            this.WPA.Children.Add(albumControl); // Добавляем контрол на страницу
                    }
                }
                reader.Close();
            }
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("No entries in back navigation history.");
            }
        }
    }
}
