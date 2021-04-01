using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCO_Player
{
    static class Profile
    {
        public static int Id_users { get; set; }        // Идентификатор 
        public static string name { get; set; }         // Имя 
        public static string surname { get; set; }      // Фамилия 
        public static string createDate { get; set; }   // Дата регистрации
        public static string imageSrc { get; set; }     // Аватар
        public static int cash { get; set; }            // Счет

    }
}
