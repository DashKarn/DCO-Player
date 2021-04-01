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
    /// Логика взаимодействия для PlaylistControl.xaml
    /// </summary>
    public partial class PlaylistControl : UserControl
    {
        public My_playlists Instance { get; set; }

        public int Id_playlist { get; set; }

        public PlaylistControl()
        {
            InitializeComponent();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlExpressionFirst = "SELECT Id_playlist, Album.Id_composition, Composition_source, Composition, Artist FROM Playlist, Playlists, Album, Albums, Artists WHERE Playlists.Id_playlists = Playlist.Id_playlist and Album.Id_composition = Playlist.Id_composition and Albums.Id_albums = Album.Id_album and Artists.Id_artists = Albums.Id_artist and Playlists.Id_user = " + Profile.Id_users; // Делаем запрос к исполнителям
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpressionFirst, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    Playlist playlist = new Playlist(); // Получаем новую страницу с плейлистом

                    Vars.files.Clear();
                    Vars.id_album = Id_playlist;

                    while (reader.Read())
                    {
                        if (Id_playlist == (int)reader.GetValue(0)) // Проверка на совпадение ключей альбома
                        {
                            Composition composition = new Composition(); // Создаем образ контрола с альбомом

                            composition.Margin = new Thickness(0, 15, 0, 0);

                            composition.Id_composition = (int)reader.GetValue(1);
                            composition.CompositionName.Text = reader.GetValue(3).ToString();
                            composition.ArtistName.Text = reader.GetValue(4).ToString();
                            playlist.PlaylistName = PlaylistName;

                            Vars.files.Add(Tuple.Create((int)reader.GetValue(1), Environment.CurrentDirectory + reader.GetValue(2).ToString())); // Записываем пути для воспроизведения композиций текущего альбома

                            playlist.WPP.Children.Add(composition); // Добавляем контрол на страницу
                        }
                    }
                    Instance.NavigationService.Navigate(playlist);
                }
                reader.Close();
            }
        }
    }
}
