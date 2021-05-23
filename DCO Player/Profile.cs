using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace DCO_Player
{
    [FirestoreData]
    static class Profile
    {
        [FirestoreProperty("Id_user")]
        public static Guid Id_user { get; set; }        // Идентификатор 

        [FirestoreProperty("Login")]
        public static string login { get; set; }

        [FirestoreProperty("Password")]
        public static string password { get; set; }

        [FirestoreProperty("Name")]
        public static string name { get; set; }         // Имя 

        [FirestoreProperty("Surname")]
        public static string surname { get; set; }      // Фамилия 

        [FirestoreProperty("Create_date")]
        public static DateTime createDate { get; set; }   // Дата регистрации

        [FirestoreProperty("User_image_source")]
        public static string imageSrc { get; set; }     // Аватар

        [FirestoreProperty("Subscription_Date")]
        public static DateTime subscriptionDate { get; set; }   // Дата окончания подписки

        [FirestoreProperty("N_GB")]
        public static int gbs { get; set; }   // Количество купленных гигабайт

        [FirestoreProperty("GB_Date")]
        public static DateTime GBDate { get; set; }   // Дата окончания купленного места

    }
}
