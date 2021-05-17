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
    /// Логика взаимодействия для Search.xaml
    /// </summary>
    public partial class Search : Page
    {
        public void Srch(string sqlExpression)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read())
                    {
                        if (MainWindow.Instance.SearchContent.Text.ToUpper() == reader.GetValue(6).ToString().ToUpper())
                        {
                            AlbumControl albumControl = new AlbumControl(); // Создаем образ контрола с альбомом

                            albumControl.Margin = new Thickness(32);

                            albumControl.InstanceSearch = this;

                            albumControl.ArtistName.Text = reader.GetValue(1).ToString(); // Передаем имя Исполнителя в контрол
                            albumControl.AlbumName.Text = reader.GetValue(2).ToString(); // Передаем имя Альбома в контрол
                            albumControl.Price.Content = "$" + reader.GetValue(3).ToString(); // Передаем цену в альбом
                            albumControl.price = (int)reader.GetValue(3);
                            albumControl.Id_albums = (int)reader.GetValue(5);
                            albumControl.Image.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + reader.GetValue(4).ToString(), UriKind.Absolute)); // Передаем картинку в альбом

                            WPS.Children.Add(albumControl); // Добавляем контрол на страницу
                        }

                    }
                }
                reader.Close();
            }
        }

        public Search()
        {
            InitializeComponent();

            if (MainWindow.Instance.Albums.IsChecked == true && MainWindow.Instance.Compositions.IsChecked == false)
            {
                string sqlExpression = "SELECT Id_artist, Artist, Album, Price, Album_image_source, Albums.Id_albums, Album FROM Artists, Albums Where Artists.Id_artists = Albums.Id_artist"; // Делаем запрос к Альбомам и композициям
                Srch(sqlExpression);
            }
 
            if (MainWindow.Instance.Albums.IsChecked == false && MainWindow.Instance.Compositions.IsChecked == true)
            {
                string sqlExpression = "SELECT Id_artist, Artist, Album, Price, Album_image_source, Albums.Id_albums, Composition FROM Artists, Albums, Album Where Artists.Id_artists = Albums.Id_artist and Albums.Id_albums = Album.Id_album"; // Делаем запрос к Альбомам и композициям
                Srch(sqlExpression);
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
