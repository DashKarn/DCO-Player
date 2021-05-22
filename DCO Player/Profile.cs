using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCO_Player
{
    static class Profile
    {
        public static Guid Id_user { get; set; }        // Идентификатор 
        public static string name { get; set; }         // Имя 
        public static string surname { get; set; }      // Фамилия 
        public static DateTime createDate { get; set; }   // Дата регистрации
        public static string imageSrc { get; set; }     // Аватар
        public static DateTime subscriptionDate { get; set; }   // Дата окончания подписки
        public static int gbs { get; set; }   // Количество купленных гигабайт
        public static DateTime GBDate { get; set; }   // Дата окончания купленного места

    }
}
