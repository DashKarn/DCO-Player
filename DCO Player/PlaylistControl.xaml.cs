using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DCO_Player
{
    /// <summary>
    /// Логика взаимодействия для PlaylistControl.xaml
    /// </summary>
    public partial class PlaylistControl : UserControl
    {
        public My_playlists Instance { get; set; }

        public UserPlaylist playlist_ { get; set; }

        public PlaylistControl()
        {
            InitializeComponent();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            bool get_db;
            List<Song> songs = new List<Song>();
            (get_db, songs) = Database.GetSongsFromPlaylist(playlist_.Id_playlist);
            if (!get_db)
                return;

            Playlist playlist = new Playlist(); // Получаем новую страницу с плейлистом
            playlist.PlaylistName.Content = playlist_.name;
            Vars.playlist = playlist_;
            Vars.current.Clear();
            foreach (var song in songs)
            {
                Composition composition = new Composition(); // Создаем образ контрола 
                composition.Margin = new Thickness(0, 15, 0, 0);

                composition.song_ = song;
                composition.CompositionName.Text = song.name;
                composition.ArtistName.Text = song.artist;
                composition.Duration.Text = TimeSpan.FromSeconds(song.length).ToString(@"mm\:ss");
                playlist.PlaylistName = PlaylistName;
                playlist.WPP.Children.Add(composition); // Добавляем контрол на страницу  
                Vars.current.Add(composition);
            }
            Vars.playlist.songs = new List<Song>(songs);
            Instance.NavigationService.Navigate(playlist);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Database.DeletePlaylist(playlist_))
                if (Firebase.DeletePlaylist(playlist_.Id_playlist))
                    Database.DeletePlaylist(playlist_, false);
        }
    }
}
