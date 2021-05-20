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
using System.Windows.Threading;

namespace DCO_Player
{
    /// <summary>
    /// Логика взаимодействия для Composition.xaml
    /// </summary>
    public partial class Composition : UserControl
    {
        public int Id_composition { get; set; }

        ComboBox Playlists = null;

        List<Tuple<Guid, string>> Id_playlist = new List<Tuple<Guid, string>>();

        public Composition()
        {
            InitializeComponent();
        }


        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            MusicStream.Stop();
            MusicStream.StreamLineStop(sender);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            MusicStream.Stop();
            if (Vars.files.Count != 0)
            {
                Vars.Tracklist = Vars.files;
                Vars.Id_album = Vars.id_album;     
                int i = 0;
                foreach(var a in Vars.Tracklist)
                {
                    if(a.Item1 == Id_composition)
                    {
                        Vars.CurrentTrackNumber = i;
                        string current = a.Item2; // Ссылка на файл
                        MusicStream.PlayPlayer(current, MusicStream.Volume);
                    }
                    i++;
                }
            }
            MusicStream.StreamLineStart(CompositionName.Text, ArtistName.Text, sender);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlExpression = "SELECT Name, Id_playlist FROM Playlists, Users WHERE Users.Id_user = Playlists.Id_user and Users.Id_user = " + Profile.Id_users; // Делаем запрос к плейлистам
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows){ // если есть данные
                    Playlists = new ComboBox();
                    myGrid.Children.Add(Playlists);
                    Grid.SetColumn(Playlists, 2);
                    Playlists.Name = "Playlists";
                    Playlists.SelectionChanged += Playlists_SelectionChanged;
                    Playlists.Height = 26;
                    Playlists.Margin = new Thickness(286, 16, 0, 16);
                    Playlists.Visibility = Visibility.Collapsed;
                    while (reader.Read())
                    {
                        Playlists.Items.Add(reader.GetValue(0).ToString());
                        Id_playlist.Add(Tuple.Create((Guid)reader.GetValue(1), reader.GetValue(0).ToString()));
                    }
                }
                reader.Close();
            }
            if(Playlists != null)
                Playlists.Visibility = Visibility.Visible;
        }

        private void Playlists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;

            int len, N_sequence = 0;

            string connectionString;
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlExpression_song = "INSERT INTO Playlist_songs (Id_playlist, Id_song, N_sequence) VALUES (@Id_playlist, @Id_song, @N_sequence)";
            string sqlExpression_max = "SELECT * FROM Playlist_songs WHERE Playlist_songs.Id_playlist = ";
            string sqlExpression_playlist = "UPDATE Playlists SET Last_update = @Last_update  WHERE Playlists.Id_playlist = ";

            foreach (var i in Id_playlist)
            {
                if (cmb.SelectedValue.ToString() == i.Item2)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlExpression_max + i.Item1.ToString(), connection);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        { // если есть данные
                            while (reader.Read())
                            {
                                len = (int)reader.GetValue(2);
                                if (len > N_sequence)
                                    N_sequence = len;
                            }
                        }
                        else
                            N_sequence = 0;

                        reader.Close();
                    }

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlExpression_song, connection);

                        command.Parameters.Add(new SqlParameter("@Id_playlist", i.Item1));
                        command.Parameters.Add(new SqlParameter("@Id_song", Id_composition));
                        command.Parameters.Add(new SqlParameter("@N_sequence", N_sequence));

                        int number = command.ExecuteNonQuery();
                        //MessageBox.Show("Добавлено объектов: {0}", number.ToString());
                    }

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlExpression_playlist + i.Item1.ToString(), connection);

                        command.Parameters.Add(new SqlParameter("@Last_update", DateTime.Now));

                        command.ExecuteNonQuery();
                    }
                }
            }
            Playlists.Visibility = Visibility.Collapsed;
        }
    }
}
