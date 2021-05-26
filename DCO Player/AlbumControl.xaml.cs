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
    /// Логика взаимодействия для AlbumControl.xaml
    /// </summary>
    public partial class AlbumControl : UserControl
    {
        public int price { get; set; }
        public Guid Id_albums { get; set; }
        public List<Guid> purchasedAlbums = new List<Guid>();
        public bool Correct = true;

      //  public CountryAlbums InstanceCountry { get; set; }
     //   public Albums InstanceAlbums { get; set; }
        public Search InstanceSearch { get; set; }

        public AlbumControl()
        {
            InitializeComponent();
        }
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            Line.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5AE3DB"));
            AlbumName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#F0F0F0"));
            ArtistName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#C2C2C2"));
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            Line.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#616161"));
            AlbumName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            ArtistName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#7E7E7E"));
        }

        private void Price_Click(object sender, RoutedEventArgs e)
        {
            string connectionString;
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlExpression = "INSERT INTO Purchased_albums (Id_user, Id_albums) VALUES" +
                " (@Id_user, @Id_albums)";

            purchasedAlbums = Contr();
            foreach (Guid i in purchasedAlbums)
            {
                if (Id_albums == i)
                {
                    Correct = false;
                }
            }

            if (Correct)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);

                    command.Parameters.Add(new SqlParameter("@Id_user", Profile.Id_user));
                    command.Parameters.Add(new SqlParameter("@Id_albums", Id_albums));

                    int number = command.ExecuteNonQuery();
                    //MessageBox.Show("Добавлено объектов: {0}", number.ToString());
                }
                Button Sender = (Button)sender;
                Sender.Content = "OK";
            }
            else
            {
                MessageBox.Show("Этот альбом был ранее преобретен");
            }
            
        }

        public List<Guid> Contr()
        {
            List<Guid> list = new List<Guid>();
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlExpression = "SELECT Id_albums FROM Purchased_albums Where Purchased_albums.Id_user = " + Profile.Id_user; // Делаем запрос к преобретенным альбомам
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
                            list.Add((Guid)reader.GetValue(0)); // Если вышел за границы массива то ***** посмотри на этот ******* GetValue(n) и сравни n с количеством столбцов в твоем запросе
                        }

                    }
                }
                reader.Close();
            }
            return list;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {/*
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlExpressionFirst = "SELECT Id_albums, Id_composition, Composition_source, Composition, Artist, Album_image_source_record, Album, Description FROM Album, Artists, Albums where Artists.Id_artists = Albums.Id_artist and Albums.Id_albums = Album.Id_album"; // Делаем запрос к исполнителям
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpressionFirst, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    Album album = new Album(); // Получаем новую страницу с альбомом

                    int i = 0;

                    Vars.files.Clear();
                    Vars.id_playlist = Id_albums;

                    while (reader.Read())
                    {
                        if(Id_albums == (Guid)reader.GetValue(0)) // Проверка на совпадение ключей альбома
                        {
                            if (Id_albums == (Guid)reader.GetValue(0) && i < 1) // Ставим флаг для единоразового исполнения данной операции
                            {
                                album.AlbumImage.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + reader.GetValue(5).ToString(), UriKind.Absolute));
                                album.Description.Text = reader.GetValue(7).ToString();
                                album.Name.Text = reader.GetValue(6).ToString();
                                i++;
                            }

                            Composition composition = new Composition(); // Создаем образ контрола с альбомом

                            composition.Margin = new Thickness(0, 15, 0, 0);

                            composition.Id_composition = (Guid)reader.GetValue(1);
                            composition.CompositionName.Text = reader.GetValue(3).ToString();
                            composition.ArtistName.Text = reader.GetValue(4).ToString();

                            Vars.files.Add(Tuple.Create((Guid)reader.GetValue(1), Environment.CurrentDirectory + reader.GetValue(2).ToString())); // Записываем пути для воспроизведения композиций текущего альбома

                            album.WPA.Children.Add(composition); // Добавляем контрол на страницу
                        }
                    }

                    try
                    {
                        if (InstanceCountry != null)
                        {
                            InstanceCountry.NavigationService.Navigate(album);
                        }
                        else if (InstanceSearch != null)
                        {
                            InstanceSearch.NavigationService.Navigate(album);
                        }
                        else
                        {
                           // InstanceAlbums.NavigationService.Navigate(album);
                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                reader.Close();
            }*/
        }
    }
}
