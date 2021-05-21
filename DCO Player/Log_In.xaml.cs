using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;


namespace DCO_Player
{
    /// <summary>
    /// Логика взаимодействия для Log_In.xaml
    /// </summary>
    public partial class Log_In : Page
    {
        string connectionString;

        Guid id { get; set; }
        string name { get; set; }
        string surname { get; set; }
        string login { get; set; }
        string password { get; set; }
        string createDate { get; set; }
        string imageSrc { get; set; }
        int cash { get; set; }

        Regex RLogin = new Regex(@"(\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,6})");
        //Regex RPassword = new Regex(@"(?=.*[0-9])(?=.*[!@#$%^&*])(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z!@#$%^&*]{9,}");
        Regex RPassword = new Regex(@"[A-Za-z\d@$!%_*\-#?&]{8,}$");

        bool BLogin = false;
        bool BPassword = false;

        public Log_In()
        {
            InitializeComponent();
        }

        private void Log_In_Click(object sender, RoutedEventArgs e)
        {

            try {
                if (RLogin.IsMatch(Login.Text))
                {
                    BLogin = true;
                    login = Login.Text;
                }
                else
                {
                    MessageBox.Show("Поле должно содержать правильное имя почты");
                }

                if (RPassword.IsMatch(CPassword.Password))
                {
                    BPassword = true;
                    password = CPassword.Password;
                }
                else
                {
                    MessageBox.Show("Пароль не соответсвует правилам пароля");
                }

                if (BLogin && BPassword)
                {
                    connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    string sqlExpression = "SELECT * FROM Users WHERE Login = '" + login + "'";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlExpression, connection);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows) // если есть данные
                        {
                            reader.Read();
                            if (reader.GetValue(4).ToString() == password)
                            {
                                id = (Guid)reader.GetValue(0);
                                name = reader.GetValue(1).ToString();
                                surname = reader.GetValue(2).ToString();
                                createDate = reader.GetValue(5).ToString();
                                imageSrc = reader.GetValue(6).ToString();
                            }
                            else
                                MessageBox.Show("Неправильный пароль!");

                            reader.Close();
                        }
                        else
                            MessageBox.Show("Такого пользователя нет!");
                    }


                }
            }
            catch
            {
                MessageBox.Show("Отсутствует подключение к базе данных,\n проверьте соединение на сервере");

            }

            Profile.Id_user = id;
            Profile.name = name;
            Profile.surname = surname;
            Profile.createDate = createDate;
            Profile.imageSrc = imageSrc;

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Start.Instance.Close();
        }
    }
}
