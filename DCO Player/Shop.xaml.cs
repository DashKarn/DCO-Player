using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Логика взаимодействия для Shop.xaml
    /// </summary>
    public partial class Shop : Page
    {
        string connectionString;

        string sqlExpression = "SELECT Id_artist, Artist, Album, Price, Album_image_source, Id_albums FROM Artists, Albums Where Artists.Id_artists = Albums.Id_artist"; // Делаем запрос к исполнителям

        public void Search(string sqlExpression)
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read())
                    {
                        {
                            AlbumControl albumControl = new AlbumControl(); // Создаем образ контрола с альбомом

                            albumControl.Margin = new Thickness(32);

                            albumControl.InstanceShop = this;

                            albumControl.ArtistName.Text = reader.GetValue(1).ToString(); // Передаем имя Исполнителя в контрол
                            albumControl.AlbumName.Text = reader.GetValue(2).ToString(); // Передаем имя Альбома в контрол
                            albumControl.Price.Content = "$" + reader.GetValue(3).ToString(); // Передаем цену в альбом
                            albumControl.price = (int)reader.GetValue(3);
                            albumControl.Id_albums = (int)reader.GetValue(5);
                            albumControl.Image.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + reader.GetValue(4).ToString(), UriKind.Absolute)); // Передаем картинку в альбом

                            WPS.Children.Add(albumControl); // Добавляем контрол на страницу
                        }
                    }
                }
                reader.Close();
            }
        }

        public Shop()
        {
            InitializeComponent();
            
            Search(sqlExpression);
        }

        private static TextBlock FindTextBlock(object panel)
        {
            foreach (UIElement child in ((Panel)((ComboBoxItem)panel).Content).Children)
            {
                if (child is TextBlock)
                {
                    return (TextBlock)child;
                }
            }

            throw new Exception("TextBlock ненашёлся");
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            // Страна
            string CountrySql;
            if (Country != null && FindTextBlock(Country.SelectedItem).Text == "ALL COUNTRIES")
            {
                CountrySql = "SELECT Id_artist, Artist, Album, Price, Album_image_source, Id_albums FROM Artists, Albums Where Artists.Id_artists = Albums.Id_artist";
            }
            else if (Country != null && FindTextBlock(Country.SelectedItem).Text == "UNITED KINGDOM")
            {
                CountrySql = "SELECT Id_artist, Artist, Album, Price, Album_image_source, Id_albums FROM Artists, Albums WHERE Artists.Id_artists = Albums.Id_artist and Artists.Id_country in (SELECT Id_country FROM Artists where Id_country like 'UK')";
            }
            else if (Country != null && FindTextBlock(Country.SelectedItem).Text == "CZECH REPUBLIC")
            {
                CountrySql = "SELECT Id_artist, Artist, Album, Price, Album_image_source, Id_albums FROM Artists, Albums WHERE Artists.Id_artists = Albums.Id_artist and Artists.Id_country in (SELECT Id_country FROM Artists where Id_country like 'Czech_Republic')";
            }
            else
            {
                CountrySql = "SELECT Id_artist, Artist, Album, Price, Album_image_source, Id_albums FROM Artists, Albums WHERE Artists.Id_artists = Albums.Id_artist and Artists.Id_country in (SELECT Id_country FROM Artists where Id_country like '" + FindTextBlock(Country.SelectedItem).Text + "') ";
            }

            // Цена
            string PriceSql;
            string regex = @"^\d{1,}$";
            if (PriceFirst.Text != "" && PriceSecond.Text != "" && Regex.IsMatch(PriceFirst.Text, regex) && Regex.IsMatch(PriceSecond.Text, regex))
            {
                PriceSql = "SELECT Id_artist, Artist, Album, Price, Album_image_source, Id_albums FROM Artists, Albums WHERE Artists.Id_artists = Albums.Id_artist and Price between " + PriceFirst.Text + " and " + PriceSecond.Text;
            }
            else if (PriceFirst.Text != "" && PriceSecond.Text == "" && Regex.IsMatch(PriceFirst.Text, regex))
            {
                PriceSql = "SELECT Id_artist, Artist, Album, Price, Album_image_source, Id_albums FROM Artists, Albums WHERE Artists.Id_artists = Albums.Id_artist and Price between " + PriceFirst.Text + " and 100";
            }
            else if (PriceFirst.Text == "" && PriceSecond.Text != ""  && Regex.IsMatch(PriceSecond.Text, regex))
            {
                PriceSql = "SELECT Id_artist, Artist, Album, Price, Album_image_source, Id_albums FROM Artists, Albums WHERE Artists.Id_artists = Albums.Id_artist and Price between 0 and " + PriceSecond.Text;
            }
            else
            {
                PriceSql = "SELECT Id_artist, Artist, Album, Price, Album_image_source, Id_albums FROM Artists, Albums WHERE Artists.Id_artists = Albums.Id_artist";
            }

            // Жанр

            string GenreSql;
            if (AllGenres.IsChecked == true)
            {
                GenreSql = "SELECT Id_artist, Artist, Album, Price, Album_image_source, Id_albums FROM Artists, Albums Where Artists.Id_artists = Albums.Id_artist";
            }
            else if (Rock.IsChecked == true || Metal.IsChecked == true || Pop.IsChecked == true || PunkRock.IsChecked == true || PowerMetal.IsChecked == true)
            {
                List<ToggleButton> b = new List<ToggleButton>() { AllGenres, Rock, Metal, Pop, PunkRock, PowerMetal };
                GenreSql = "SELECT Id_artist, Artist, Album, Price, Album_image_source, Id_albums FROM Artists, Albums WHERE Artists.Id_artists = Albums.Id_artist and Artists.Id_artists in (SELECT Id_artist FROM Genre where Id_genre in (SELECT Id_genres FROM Genres where Genres in ('";
                List<string> st = new List<string>();
                int i = 0;
                foreach (ToggleButton a in b) // Заполняем список жанрами выбранными
                {
                    if ((bool)a.IsChecked)
                    {
                        st.Add((string)a.Content);
                    }
                }
                string[] s = new string[st.Count];
                foreach (string a in st) // Заполняем из списка массив чтобы я мог нормально преобразовать этот ***** массив в строку
                {
                    s[i] = a;
                    i++;
                }
                GenreSql += string.Join("','", s) + "')))";
            }
            else
            {
                GenreSql = "SELECT Id_artist, Artist, Album, Price, Album_image_source, Id_albums FROM Artists, Albums Where Artists.Id_artists = Albums.Id_artist";
            }

            sqlExpression = CountrySql + " INTERSECT " + PriceSql + " INTERSECT " + GenreSql;
            WPS.Children.Clear();
            Search(sqlExpression);
        }
    }
}
