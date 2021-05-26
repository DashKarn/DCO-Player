using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DCO_Player
{
    /// <summary>
    /// Логика взаимодействия для Composition.xaml
    /// </summary>
    public partial class Composition : UserControl
    {
        public Song song_;
        public bool search = false;
        public int n = -1;
        ComboBox Playlists = null;
        List<Tuple<Guid, string>> Id_playlists = new List<Tuple<Guid, string>>();

        public Composition()
        {
            InitializeComponent();
        }


        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            MusicStream.Stop();
            MusicStream.StreamLineStop(sender);
            Start.Visibility = Visibility.Visible;
            Stop.Visibility = Visibility.Collapsed;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            MusicStream.Stop();
            if (Vars.playlist.songs.Count > 0 && !search
                || Vars.search.Count > 0 && search)
            {
                if (search)
                    Vars.StreamTracklist = Vars.search;
                else
                {
                    Vars.StreamTracklist = new List<Composition>(Vars.current);
                    Vars.Id_playlist = Vars.playlist.Id_playlist;
                }
                int i = 0;
                foreach (var s in Vars.StreamTracklist)
                {
                    if (s.song_ == song_)
                        Vars.CurrentTrackNumber = i;
                    i++;
                }
                n = Vars.CurrentTrackNumber;
            }
            MusicStream.PlayPlayer(song_.path, MusicStream.Volume);

            MusicStream.Play();
            MusicStream.StreamLineStart(CompositionName.Text, ArtistName.Text, sender);
            Start.Visibility = Visibility.Collapsed;
            Stop.Visibility = Visibility.Visible;
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            BorderComposition.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5AE3DB"));
            CompositionName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#F0F0F0"));
            ArtistName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#C2C2C2"));
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderComposition.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#303030"));
            CompositionName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            ArtistName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#7E7E7E"));
        }
        private void Playlists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;

            foreach (var i in Id_playlists)
            {
                if (cmb.SelectedValue.ToString() == i.Item2)
                {
                    Database.InsertSongToPlaylist(song_, i.Item1);
                }
            }
            Playlists.Visibility = Visibility.Collapsed;

        }

        private void Drop_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            List<Song> new_songs = new List<Song>();
            foreach(var s in Vars.playlist.songs)
            {
                if(s != song_)
                {
                    s.n_sequence = i;
                    new_songs.Add(s);
                    i++;
                }
            }
            Vars.playlist.lastUpdate = DateTime.Now;
            Vars.playlist.songs = new_songs;
            if (Database.UpdatePlaylist(Vars.playlist, null, true))
                if (Firebase.AddPlaylist(Vars.playlist))
                {
                    Vars.playlist.lastSync = DateTime.Now;
                    Database.UpdatePlaylist(Vars.playlist);
                    Firebase.AddPlaylist(Vars.playlist);
                }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

            Playlists = new ComboBox();
            myGrid.Children.Add(Playlists);
            Grid.SetColumn(Playlists, 2);
            Playlists.Name = "Playlists";
            Playlists.SelectionChanged += Playlists_SelectionChanged;
            Playlists.Height = 26;
            Playlists.Margin = new Thickness(286, 16, 0, 16);
            Playlists.Visibility = Visibility.Collapsed;
            Id_playlists = Vars.allPlaylists;
            Playlists.Items.Add(Vars.allPlaylists.ConvertAll(pl => pl.Item2));
            Playlists.Visibility = Visibility.Visible;
        }
    }
}
