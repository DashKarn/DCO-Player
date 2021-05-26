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
            InitializeComponent();

            bool get_db;
            List<UserPlaylist> playlists;

            (get_db, playlists) = Database.GetPlaylists(false);
            if (playlists.Count == 0)
            {
                playlists = Firebase.GetPlaylists();
                if (playlists.Count > 0 && get_db)
                    Database.InsertPlaylists(playlists);
            }
            Vars.allPlaylists = playlists.Select(p => new Tuple<Guid, string>(p.Id_playlist, p.name)).ToList();
            if (playlists.Count > 0)
            {
                foreach (var playlist in playlists)
                {
                    if (playlist.name == UserPlaylist.NULL_NAME)
                        continue;

                    PlaylistControl playlistControl = new PlaylistControl(); // Создаем образ контрола с плейлистом
                    playlistControl.Margin = new Thickness(64, 35, 0, 29);
                    playlistControl.Instance = this;
                    playlistControl.playlist_ = playlist;
                    playlistControl.PlaylistName.Content = playlist.name; // Передаем имя плейлиста в контрол
                    if (playlist.imageSrc != "")
                    {
                        playlistControl.Image.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + playlist.imageSrc, UriKind.Absolute)); // Передаем картинку в плейлист
                    }
                    WPM.Children.Add(playlistControl); // Добавляем контрол на страницу                       
                }
            }
            else
                if (!get_db)
                    MessageBox.Show("Не получается зарузить плейлисты. \n" +
                        "Отсутствует подключение к базе данных и интернет соединение");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new CreatePlaylist().Show();
           // NavigationService.Navigate(new My_playlists());
        }
        private void Sync_MouseDown(object sender, MouseButtonEventArgs e)
        {
            bool get_db;
            List<UserPlaylist> playlists_fb = new List<UserPlaylist>();
            var ids_fb = playlists_fb.Select(f => f.Id_playlist).ToList();
            List<UserPlaylist> playlists_db = new List<UserPlaylist>();
            var ids_db = playlists_db.Select(f => f.Id_playlist).ToList();
            List<Song> songs_db = new List<Song>();
            List<Song> songs_fb = new List<Song>();

            (get_db, songs_db) = Database.GetAllSongs();
            if (!get_db)
                return;
            songs_fb = Firebase.GetSongs(null, true);

            playlists_fb = Firebase.GetPlaylists();
            if (playlists_fb.Count == 0)
                return;

            (get_db, playlists_db) = Database.GetPlaylists(deleted : true);
            if (!get_db)
                return;

            //upload to server
            foreach (var playlist in playlists_db)
            {
                if (playlist.name == UserPlaylist.NULL_NAME)
                {
                    if (Firebase.DeletePlaylist(playlist.Id_playlist))
                        Database.DeletePlaylist(playlist, false);
                    continue;
                }
                if (!ids_fb.Contains(playlist.Id_playlist) ||
                    playlist.lastUpdate > playlist.lastSync)
                {
                    playlist.lastSync = DateTime.Now;
                    if (Firebase.AddPlaylist(playlist))
                        Database.UpdatePlaylist(playlist);
                }
            }
            // Download to local DB
            foreach (var playlist in playlists_fb)
            {
                if (playlist.name == UserPlaylist.NULL_NAME)
                {
                    if (Database.DeletePlaylist(playlist, false))
                        Firebase.DeletePlaylist(playlist.Id_playlist);
                    continue;
                }
                if (!ids_db.Contains(playlist.Id_playlist))
                {
                    playlist.lastSync = DateTime.Now;
                    if (Database.InsertPlaylist(playlist))
                        Firebase.AddPlaylist(playlist);
                    continue;
                }
                if (playlist.lastSync > playlists_db.First(f => f.Id_playlist == playlist.Id_playlist).lastSync)
                {
                    playlist.lastSync = DateTime.Now;
                    if (Database.UpdatePlaylist(playlist))
                        Firebase.AddPlaylist(playlist);
                }
            }
        }
    }
}
