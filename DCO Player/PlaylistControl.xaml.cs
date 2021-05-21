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

        public Guid Id_playlist { get; set; }

        public PlaylistControl()
        {
            InitializeComponent();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlExpressionFirst = "SELECT Playlist_songs.Id_playlist, Playlist_songs.Id_song, Songs.Path, Songs.Name, Songs.Artist " + 
                "FROM Playlist_songs, Playlists, Songs WHERE Playlists.Id_playlist = Playlist_songs.Id_playlist and Playlist_songs.Id_song = Songs.Id_song  and Playlists.Id_user = @Id_user"; // Делаем запрос к исполнителям
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpressionFirst, connection);
                command.Parameters.Add(new SqlParameter("@Id_user", Profile.Id_user));
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    Playlist playlist = new Playlist(); // Получаем новую страницу с плейлистом

                    Vars.files.Clear();
                    Vars.id_playlist = Id_playlist;

                    while (reader.Read())
                    {
                        if (Id_playlist == (Guid)reader.GetValue(0)) // Проверка на совпадение ключей альбома
                        {
                            Composition composition = new Composition(); // Создаем образ контрола с альбомом

                            composition.Margin = new Thickness(0, 15, 0, 0);

                            composition.Id_composition = (Guid)reader.GetValue(1);
                            composition.CompositionName.Text = reader.GetValue(3).ToString();
                            composition.ArtistName.Text = reader.GetValue(4).ToString();
                            playlist.PlaylistName = PlaylistName;

                            Vars.files.Add(Tuple.Create((Guid)reader.GetValue(1), reader.GetValue(2).ToString())); // Записываем пути для воспроизведения композиций текущего альбома

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
