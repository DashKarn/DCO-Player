using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace DCO_Player
{
    /// <summary>
    /// Логика взаимодействия для Playlist.xaml
    /// </summary>
    public partial class Playlist : Page
    {
        public Playlist()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            bool db;
            List<Song> songs = new List<Song>();
            List<Song> songs_all = new List<Song>();
            List<Tuple<Guid, string>> paths;

            new Thread(() =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                    dialog.InitialDirectory = "C:\\Users";
                    dialog.Multiselect = true;
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        (db, paths) = Database.GetSongsPath();
                        if (!db)
                            MessageBox.Show("Отсутствует подключение к БД");
                        else
                        {
                            foreach (string file in dialog.FileNames)
                            {
                                Song song = new Song();
                                song.addProperties(file);
                                if (paths.Any(m => m.Item2 == file))
                                    song.Id_song = paths.First(m => m.Item2 == file).Item1;
                                else
                                {
                                    song.Id_song = Guid.NewGuid();
                                    songs_all.Add(song);
                                }
                                songs.Add(song);
                            }
                            Vars.playlist.lastUpdate = DateTime.Now;
                            if (Database.InsertSongs(songs_all))
                                Firebase.AddSongs(songs_all);
                            if (Database.UpdatePlaylist(Vars.playlist, songs, true))
                            {
                                (db, songs) = Database.GetSongsFromPlaylist(Vars.playlist.Id_playlist);
                                if (db)
                                {
                                    Vars.playlist.songs = songs;
                                    Firebase.AddPlaylist(Vars.playlist);
                                }
                            }
                        }
                    }
                }));

            }).Start();
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
