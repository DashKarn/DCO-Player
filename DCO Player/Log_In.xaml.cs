﻿using System;
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
        DateTime createDate { get; set; }
        string imageSrc { get; set; }
        DateTime subDate { get; set; }
        int gbs { get; set; }
        DateTime gbDate { get; set; }

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
            bool load = false;
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
                try
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
                                MessageBox.Show("1");
                                id = (Guid)reader.GetValue(0);
                                name = reader.GetValue(1).ToString();
                                surname = reader.GetValue(2).ToString();
                                createDate = (DateTime)reader.GetValue(5);
                                imageSrc = reader.GetValue(6).ToString();
                                subDate = (DateTime)reader.GetValue(7);
                                gbs = (int)reader.GetValue(8);
                                gbDate = (DateTime)reader.GetValue(9);

                                load = true;
                            }
                            else
                            {
                                MessageBox.Show("Неправильный пароль!");
                                load = false;
                            }

                            reader.Close();
                        }
                        else
                        {
                            MessageBox.Show("Такого пользователя нет!");
                            load = false;
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Отсутствует подключение к базе данных,\n проверьте соединение на сервере");
                    MessageBox.Show(ex.Message);
                    load = false;
                }
            }


            if (load)
            {
                Profile.Id_user = id;
                Profile.name = name;
                Profile.surname = surname;
                Profile.createDate = createDate;
                Profile.imageSrc = imageSrc;
                Profile.subscriptionDate = subDate;
                Profile.gbs = gbs;
                Profile.GBDate = gbDate;

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Start.Instance.Close();
            }
        }
    }
}
