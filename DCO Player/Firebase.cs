using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using Google.Apis.Storage.v1.Data;

namespace DCO_Player
{
    static class Firebase
    {
        static string project_Id = "dco-player-74813";
        static string bucketName = "dco-player-74813.appspot.com";

        static FirestoreDb db;
        static DocumentReference user_ref;
        static Dictionary<string, object> user = new Dictionary<string, object>
            {
                { "Id_user", Profile.Id_user },
                { "Name", Profile.name },
                { "Surname", Profile.surname },
                { "Login", Profile.login },
                { "Password", Profile.password },
                { "Create_date", Profile.createDate },
                { "User_image_source", Profile.imageSrc },
                { "Subscription_Date", Profile.subscriptionDate },
                { "N_GB", Profile.gbs },
                { "GB_Date", Profile.GBDate }
            };
        public static void Init()
        {
            string credential_path = @"D:\!! New diplom\dco-player-74813-c9cace3048cd.json";

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);

            db = FirestoreDb.Create(project_Id);


          //  StorageClient storage = StorageClient.Create();
          //  Bucket bucket = storage.CreateBucket(project_Id, bucketName);
          //  foreach (var b in storage.ListBuckets(project_Id))
          //      Console.WriteLine(b.Name);
        }

        public static void AddUser()
        {
            user_ref = db.Collection("Users").Document(Profile.login);
            user_ref.SetAsync(user);
        }

        public static void UpdateUser()
        {
            user_ref = db.Collection("Users").Document(Profile.login);
            user_ref.SetAsync(user);
        }

        public static bool CheckUser(string login)
        {
            user_ref = db.Collection("Users").Document(login);
            DocumentSnapshot snapshot = user_ref.GetSnapshotAsync().Result;
            if (snapshot.Exists)
                return true;
            else
                return false;
        }

        public static bool GetUser(string login)
        {
            user_ref = db.Collection("Users").Document(login);
            DocumentSnapshot snapshot = user_ref.GetSnapshotAsync().Result;
            if (!snapshot.Exists)
                return false;

            Profile.Id_user = snapshot.GetValue<Guid>("Id_user");
            Profile.name = snapshot.GetValue<string>("Name");
            Profile.surname = snapshot.GetValue<string>("Surname");
            Profile.login = snapshot.GetValue<string>("Login");
            Profile.password = snapshot.GetValue<string>("Password");
            Profile.createDate = snapshot.GetValue<DateTime>("Create_date");
            Profile.imageSrc = snapshot.GetValue<string>("User_image_source");
            Profile.subscriptionDate = snapshot.GetValue<DateTime>("Subscription_Date");
            Profile.gbs = snapshot.GetValue<int>("N_GB");
            Profile.GBDate = snapshot.GetValue<DateTime>("GB_date");
            return true;
        }
        public static void DeleteUser()
        {
            user_ref = db.Collection("Users").Document(Profile.login);
            user_ref.DeleteAsync();
        }
    }
}
