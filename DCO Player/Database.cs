using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;

namespace DCO_Player
{
    static class Database
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public static SqlDataReader GetUser(string login)
        {
            string sqlExpression = "SELECT * FROM Users WHERE Login = '" + login + "'";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            return command.ExecuteReader();
        }

        public static void InsertUser()
        {
            string sqlExpression = "INSERT INTO Users (Id_user, Name, Surname, Login, Password, Create_date, User_image_source, Subscription_date, N_GB, GB_date) VALUES" +
                    " (@Id_user, @Name, @Surname, @Login, @Password, @Create_date, @User_image_source, @Subscription_date, @N_GB, @GB_date)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.Add(new SqlParameter("@Id_user", Profile.Id_user));
                command.Parameters.Add(new SqlParameter("@Name", Profile.name));
                command.Parameters.Add(new SqlParameter("@Surname", Profile.surname));
                command.Parameters.Add(new SqlParameter("@Login", Profile.login));
                command.Parameters.Add(new SqlParameter("@Password", Profile.password));
                command.Parameters.Add(new SqlParameter("@Create_date", Profile.createDate.ToString("d")));
                command.Parameters.Add(new SqlParameter("@User_image_source", Profile.imageSrc));
                command.Parameters.Add(new SqlParameter("@Subscription_date", DateTime.MinValue.ToString("d")));
                command.Parameters.Add(new SqlParameter("@N_GB", "0"));
                command.Parameters.Add(new SqlParameter("@GB_date", DateTime.MinValue.ToString("d")));
                connection.Close();
            }
        }

    }
}
