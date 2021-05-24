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
            bool get_db;
            List<Song> songs = new List<Song>();
            (get_db, songs) = Database.GetSongs(Id_playlist);
            if (!get_db)
                return;

            Playlist playlist = new Playlist(); // Получаем новую страницу с плейлистом

            Vars.files.Clear();
            Vars.id_playlist = Id_playlist;

            foreach (var song in songs)
            {
                Composition composition = new Composition(); // Создаем образ контрола с альбомом
                composition.Margin = new Thickness(0, 15, 0, 0);

                composition.Id_composition = song.Id_song;
                composition.CompositionName.Text = song.name;
                composition.ArtistName.Text = song.artist;
                playlist.PlaylistName = PlaylistName;
                Vars.files.Add(Tuple.Create(song.Id_song, song.path)); // Записываем пути для воспроизведения композиций текущего альбома
                playlist.WPP.Children.Add(composition); // Добавляем контрол на страницу  
            }
            Instance.NavigationService.Navigate(playlist);
        }        
    }
}
