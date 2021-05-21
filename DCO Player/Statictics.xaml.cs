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

            AccountName.Text = Profile.name + " " + Profile.surname;        // Имя и фамилия в профиле
            if(Profile.imageSrc != "")
            {
                AccountImage.ImageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + Profile.imageSrc, UriKind.Absolute)); // Изображение профиля
            }
            
            CreateDate.Content = Profile.createDate;

            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlExpressionFirst = "select Count(*) from Purchased_albums where Purchased_albums.Id_user = " + Profile.Id_user; // Делаем запрос 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpressionFirst, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read())
                    {
                        AlbumsContent.Content = reader.GetValue(0).ToString();
                    }
                }
                reader.Close();
            }

            sqlExpressionFirst = "SELECT top(1) Artist, Count(Artist), Id_country [Count] FROM Artists, Albums, Purchased_albums Where Artists.Id_artists = Albums.Id_artist and Purchased_albums.Id_albums = Albums.Id_albums and Purchased_albums.Id_user = " + Profile.Id_user + " GROUP BY Artist, Id_country ORDER BY Count(Artist) asc"; // Делаем запрос 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpressionFirst, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read())
                    {
                        FavoriteArtistsContent.Content = reader.GetValue(0).ToString();
                        FavoriteCountriesContent.Content = reader.GetValue(2).ToString();
                    }
                }
                reader.Close();
            }

            sqlExpressionFirst = "select isnull(sum(Duration),0) from Albums, Purchased_albums where Albums.Id_albums = Purchased_albums.Id_albums and Purchased_albums.Id_user = " + Profile.Id_user; // Делаем запрос 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpressionFirst, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read())
                    {
                        double count = Math.Round((double)((int)reader.GetValue(0)) / 3600.0, 1);
                        NumberOfHoursContent.Content = count.ToString() + "h";
                    }
                }
                reader.Close();
            }

            sqlExpressionFirst = "select Count(*) from Playlists where Playlists.Id_user = " + Profile.Id_user; // Делаем запрос 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpressionFirst, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read())
                    {
                        PlaylistsContent.Content = reader.GetValue(0).ToString();
                    }
                }
                reader.Close();
            }
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
