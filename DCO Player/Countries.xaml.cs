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
using System.Reflection;

namespace DCO_Player
{
    /// <summary>
    /// Логика взаимодействия для Countries.xaml
    /// </summary>
    public partial class Countries : Page
    {
        string connectionString;

        public void Art(Path sender)
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlExpressionFirst = "SELECT * FROM Artists"; // Делаем запрос к странам
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpressionFirst, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    Artists artists = new Artists(); // Получаем новую страницу с артистами

                    while (reader.Read())
                    {
                        if (sender.Name == reader.GetValue(0).ToString()) // Проверяем на совпадение страну исполнителя
                        {
                            Artist artistControl = new Artist(); // Создаем образ контрола с именем исполнителя

                            artistControl.Instance = artists.Instance;

                            artistControl.Margin = new Thickness(42, 15, 42, 0);

                            artistControl.ArtistName.Text = reader.GetValue(2).ToString(); // Передаем имя в контрол
                            artistControl.Id_artists = (int)reader.GetValue(1); // Передаем айдишник в контрол
                            artists.WPA.Children.Add(artistControl); // Добавляем контрол на страницу
                        }

                    }
                    this.NavigationService.Navigate(artists); // Переходим на страницу
                }
                reader.Close();
            }
        }

        public Countries()
        {
            InitializeComponent();
        }

        private new void MouseEnter(object sender, MouseEventArgs e)
        {
            Path s = (Path)sender;
            var imageBrush = new ImageBrush();
            s.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
            imageBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Flag/" + s.Name + ".png"));
            s.Fill = imageBrush;
            Canvas.SetZIndex(s, 1);
        }

        private new void MouseLeave(object sender, MouseEventArgs e)
        {
            Path s = (Path)sender;
            SolidColorBrush Brush = new SolidColorBrush();
            s.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#434242"));
            Brush.Color = Colors.Transparent;
            s.Fill = Brush;
            Canvas.SetZIndex(s, 0);
        }

        private new void MouseDown(object sender, MouseButtonEventArgs e)
        {
            Art((Path)sender);
        }
    }
}
