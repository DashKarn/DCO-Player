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
    /// Логика взаимодействия для Statictics.xaml
    /// </summary>
    public partial class Statictics : Page
    {
        public Statictics()
        {
            InitializeComponent();

            AccountName.Text = Profile.name + " " + Profile.surname;     
            AccountLogin.Text = Profile.login;
            if(Profile.imageSrc != "")
            {
                AccountImage.ImageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + Profile.imageSrc, UriKind.Absolute)); // Изображение профиля
            }
            
            CreateDate.Content = Profile.createDate;
            Update();            
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

        private void Update_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Update();
        }

        private void Update()
        {
            List<UserPlaylist> playlists;
            bool db;
            (db, playlists) = Database.GetPlaylists();

            long sec_playlists = 0;
            HashSet<Guid> songs = new HashSet<Guid>();
            HashSet<string> albums = new HashSet<string>();
            HashSet<string> artists = new HashSet<string>();

            foreach(var pl in playlists)
            {
                foreach(var song in pl.songs)
                {
                    sec_playlists += song.length;
                    songs.Add(song.Id_song);
                    artists.Add(song.artist);
                }
            }
            Albums.Content = albums.Count.ToString();
            Artists.Content = artists.Count.ToString();
            Songs.Content = songs.Count.ToString();
            Playlists.Content = playlists.Count.ToString();
            NumberOfHoursPlaylists.Content = (sec_playlists / 360).ToString();
        }
    }
}
