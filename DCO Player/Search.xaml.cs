using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DCO_Player
{
    /// <summary>
    /// Логика взаимодействия для Search.xaml
    /// </summary>
    public partial class Search : Page
    {
        public void Srch(bool song, bool album, bool artist)
        {
            List<Song> all_songs;
            bool db;
            (db, all_songs) = Database.GetAllSongs();
            if (!db)
            {
                MessageBox.Show("Нет соединения с БД");
                return;
            }
            bool match = false;
            string str = MainWindow.Instance.SearchContent.Text.ToLower();

            Vars.search.Clear();
            foreach (var s in all_songs)
            {
                match = false;
                if (song)
                    if (s.name.ToLower().Contains(str))
                        match = true;
                if (album)
                    if (s.album.ToLower().Contains(str))
                        match = true;
                if (artist)
                    if (s.artist.ToLower().Contains(str))
                        match = true;

                if (match)
                {
                    Composition composition = new Composition(); // Создаем образ контрола 
                    composition.Margin = new Thickness(0, 15, 0, 0);
                    composition.search = true;
                    composition.song_ = s;
                    composition.CompositionName.Text = s.name;
                    composition.ArtistName.Text = s.artist;
                    composition.Drop.Visibility = Visibility.Hidden;
                    composition.Duration.Text = TimeSpan.FromSeconds(s.length).ToString(@"mm\:ss");
                    Vars.search.Add(composition); // Записываем пути для воспроизведения композиций текущего альбома
                    WPM.Children.Add(composition); // Добавляем контрол на страницу 
                }
            }

        }
        public Search()
        {
            InitializeComponent();

            if (!MainWindow.Instance.Albums.IsChecked.Value && !MainWindow.Instance.Songs.IsChecked.Value && !MainWindow.Instance.Artists.IsChecked.Value)
                Srch(true, true, true);
            else
                Srch(MainWindow.Instance.Songs.IsChecked.Value,
                    MainWindow.Instance.Albums.IsChecked.Value,
                    MainWindow.Instance.Artists.IsChecked.Value);
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
