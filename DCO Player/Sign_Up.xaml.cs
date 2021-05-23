using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Windows.Media.Imaging;

namespace DCO_Player
{
    /// <summary>
    /// Логика взаимодействия для Sign_Up.xaml
    /// </summary>
    public partial class Sign_Up : Page
    {
        Guid Id_user = Guid.NewGuid();
        string name { get; set; }
        string surname { get; set; }
        string login { get; set; }
        string password { get; set; }

        DateTime createDate = DateTime.Now;
        string imageSrc { get; set; }

        Regex RName = new Regex("^([А-я]|[A-z]){1,19}$");
        Regex RSurname = new Regex("^([А-я]|[A-z]){1,19}$");
        Regex RLogin = new Regex(@"(\w+@[a-z_]+?\.[a-z]{2,6})");
        Regex RPassword = new Regex(@"[A-Za-z\d@$!%_*\-#?&]{8,}$");

        bool BName = false;
        bool BSurname = false;
        bool BLogin = false;
        bool BPassword = false;

        CroppedBitmap cb;

        public Sign_Up()
        {
            InitializeComponent();
        }

        private void Sign_Up_Click(object sender, RoutedEventArgs e)
        {
            Firebase.Init();
            bool load = false;

                if (RName.IsMatch(Name.Text))
                {
                    BName = true;
                    name = Name.Text;
                }
                else
                {
                    MessageBox.Show("Поле Name должно содержать буквы кириллического либо латинского алфавита");
                }

                if (RSurname.IsMatch(Surname.Text))
                {
                    BSurname = true;
                    surname = Surname.Text;
                }
                else
                {
                    MessageBox.Show("Поле Surname должно содержать буквы кириллического либо латинского алфавита");
                }

                if (RLogin.IsMatch(Login.Text))
                {
                    BLogin = true;
                    login = Login.Text;
                }
                else
                {
                    MessageBox.Show("Поле login должно содержать имя почты");
                }

                if (RPassword.IsMatch(Password.Password))
                {
                }
                else
                    MessageBox.Show("Пароль должен содержать не менее 8 символов и включать спецсимволы, загланые буквы, числа");

                if (RPassword.IsMatch(CPassword.Password))
                {
                    if (CPassword.Password == Password.Password)
                    {
                        BPassword = true;
                        password = Password.Password;
                    }
                    else
                    {
                        MessageBox.Show("Пароли не совпадают");
                    }
                }
                else
                {
                    MessageBox.Show("Пароль должен содержать не менее 8 символов, включать спецсимволы, загланые буквы, числа");
                }

            if (BName && BSurname && BLogin && BPassword)
            {
                if (cb != null)
                {
                    BitmapEncoder encoder = new PngBitmapEncoder(); // Создаем новый образ кодировщика
                    encoder.Frames.Add(BitmapFrame.Create(cb)); // кодируем наше обрезанное изображение в png и далее ниже его сохраняем

                    using (var fileStream = new System.IO.FileStream(Environment.CurrentDirectory + "/Images/Profiles/Avatar/" + Name.Text + "_" + Surname.Text + ".png", System.IO.FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }

                    imageSrc = "/Images/Profiles/Avatar/" + Name.Text + "_" + Surname.Text + ".png"; // Ссылка на аватар
                }
                else
                {
                    imageSrc = "";
                }

                try
                {
                    SqlDataReader reader = Database.GetUser(login);
                    if (reader.HasRows || Firebase.CheckUser(login)) // если есть данные
                    {
                        MessageBox.Show("Пользователь " + login + "уже существует");
                        load = false;
                        throw new Exception();
                    }
                    reader.Close();

                    Profile.Id_user = Id_user;
                    Profile.name = name;
                    Profile.login = login;
                    Profile.password = password;
                    Profile.surname = surname;
                    Profile.createDate = createDate;
                    Profile.imageSrc = imageSrc;
                    Profile.subscriptionDate = DateTime.MinValue;
                    Profile.gbs = 0;
                    Profile.GBDate = DateTime.MinValue;

                    Database.InsertUser();
                    Firebase.AddUser();

                    load = true;
                }
                catch
                {
                    MessageBox.Show("Отсутствует подключение к базе данных,\n проверьте соединение на сервере");
                    load = false;
                }
            }

            if (load)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Start.Instance.Close();
            }

        }

        private void Photo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|Portable Network Graphic (*.png)|*.png";
                if (openFileDialog.ShowDialog() == true)
                {
                    Uri uri = new Uri(openFileDialog.FileName); // Получаем ссылку на файл (картинку)

                    System.Windows.Controls.Image croppedImage = new System.Windows.Controls.Image();
                    BitmapImage bm = new BitmapImage(uri); // Создаем новый образ битового изображения
                    cb = new CroppedBitmap(
                       bm,
                       new Int32Rect((int)(((int)bm.PixelWidth - 600) / 2), (int)(((int)bm.PixelHeight - 600) / 2), 600, 600));       // Выбираем настройки обрезки

                    Photo.Background = new ImageBrush(cb);
                }
            }
            catch
            {
                MessageBox.Show("Изображение должно быть 600х600 пикселей");
            }

        }
    }
}

