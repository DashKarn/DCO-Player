using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using Google.Apis.Storage.v1.Data;
using System.Data.SqlClient;

namespace DCO_Player
{
    static class Firebase
    {
        static string project_Id = "dco-player-74813";
       // static string bucketName = "dco-player-74813.appspot.com";

        static FirestoreDb db;
        static CollectionReference coll_ref;
        static DocumentReference doc_ref;
        public static void Init()
        {
          //  string credential_path = @"D:dco-player-74813.json";

          //  Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);

          //  db = FirestoreDb.Create(project_Id);


          ////  StorageClient storage = StorageClient.Create();
          ////  Bucket bucket = storage.CreateBucket(project_Id, bucketName);
          ////  foreach (var b in storage.ListBuckets(project_Id))
          ////      Console.WriteLine(b.Name);
        }

        public static void AddUser()
        {
            doc_ref = db.Collection("Users").Document(Profile.login);
            Dictionary<string, object> user = new Dictionary<string, object>
            {
                { "Id_user", Profile.Id_user.ToString() },
                { "Name", Profile.name },
                { "Surname", Profile.surname },
                { "Login", Profile.login },
                { "Password", Profile.password },
                { "Create_date", Profile.createDate.ToString() },
                { "User_image_source", Profile.imageSrc },
                { "Subscription_Date", Profile.subscriptionDate.ToString() },
                { "N_GB", Profile.gbs },
                { "GB_Date", Profile.GBDate.ToString() }
            };
            doc_ref.SetAsync(user);
        }

        public static void UpdateUser()
        {
            doc_ref = db.Collection("Users").Document(Profile.login);
            Dictionary<string, object> user = new Dictionary<string, object>
            {
                { "Id_user", Profile.Id_user.ToString() },
                { "Name", Profile.name },
                { "Surname", Profile.surname },
                { "Login", Profile.login },
                { "Password", Profile.password },
                { "Create_date", Profile.createDate.ToString() },
                { "User_image_source", Profile.imageSrc },
                { "Subscription_Date", Profile.subscriptionDate.ToString() },
                { "N_GB", Profile.gbs },
                { "GB_Date", Profile.GBDate.ToString() }
            };
            doc_ref.UpdateAsync(user);
        }

        public static bool CheckUser(string login)
        {
            doc_ref = db.Collection("Users").Document(login);
            DocumentSnapshot snapshot = doc_ref.GetSnapshotAsync().Result;
            if (snapshot.Exists)
                return true;
            else
                return false;
        }

        public static bool GetUser(string login)
        {
            doc_ref = db.Collection("Users").Document(login);
            DocumentSnapshot snapshot = doc_ref.GetSnapshotAsync().Result;
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
            doc_ref = db.Collection("Users").Document(Profile.login);
            doc_ref.DeleteAsync();
        }

        public static void toserver(RadioStation reader)
        {
            doc_ref = db.Collection("Radio").Document(reader.name);
            Dictionary<string, object> radio = new Dictionary<string, object>
            {
                { "Id_radio", reader.Id.ToString() },
                { "Radio", reader.name },
                { "Description", reader.descr },
                { "Stream", reader.stream },
                { "Page", reader.page },
                { "Radio_image_source", reader.imageSrc }
            };
            doc_ref.CreateAsync(radio);
        }

        public static List<RadioStation> GetRadio()
        {
            List<RadioStation> radioStations = new List<RadioStation>();
            ////coll_ref = db.Collection("Radio");
            ////QuerySnapshot snapshot = coll_ref.GetSnapshotAsync().Result;

            //foreach (var doc in snapshot.Documents)
            //{
            //    RadioStation station = new RadioStation();
            //    station.Id = doc.GetValue<Guid>("Id_radio");
            //    station.name = doc.GetValue<string>("Radio");
            //    station.descr = doc.GetValue<string>("Description");
            //    station.stream = doc.GetValue<string>("Stream");
            //    station.page = doc.GetValue<string>("Page");
            //    station.imageSrc = doc.GetValue<string>("Radio_image_source");

            //    radioStations.Add(station);
            //}
            return radioStations;
        }

        public static List<UserPlaylist> GetPlaylists(bool songs = false)
        {
            List<UserPlaylist> playlists = new List<UserPlaylist>();
            //coll_ref = db.Collection("Playlists/" + Profile.login + "/Playlists");
            //QuerySnapshot snapshot = coll_ref.GetSnapshotAsync().Result;

            //foreach (var doc in snapshot.Documents)
            //{
            //    UserPlaylist playlist = new UserPlaylist();

            //    playlist.Id_user = doc.GetValue<Guid>("Id_user");
            //    playlist.Id_playlist = doc.GetValue<Guid>("Id_playlist");
            //    playlist.name = doc.GetValue<string>("Name");
            //    playlist.lastUpdate = doc.GetValue<DateTime>("Last_update");
            //    playlist.lastSync = doc.GetValue<DateTime>("Last_sync");
            //    playlist.imageSrc = doc.GetValue<string>("Playlist_image_source");



            //    playlists.Add(playlist);
            //}
            return playlists;
        }

        public static bool AddPlaylist(UserPlaylist playlist)
        {
            return AddPlaylists(new List<UserPlaylist> { playlist });
        }
        public static bool AddPlaylists(List<UserPlaylist> playlists)
        {
            //int count = 0;
            //coll_ref = db.Collection("Playlists/" + Profile.login);

            //foreach (var playlist in playlists)
            //{
            //    doc_ref = db.Collection("Playlists/" + Profile.login + "/Playlists").Document(playlist.Id_playlist.ToString());

            //    Dictionary<string, object> pl = new Dictionary<string, object>
            //{
            //    { "Id_user", playlist.Id_user.ToString() },
            //    { "Id_playlist", playlist.Id_playlist.ToString() },
            //    { "Name", playlist.name },
            //    { "Last_update", playlist.lastUpdate.ToString() },
            //    { "Last_sync", DateTime.Now.ToString() },
            //    { "Playlist_image_source", playlist.imageSrc }
            //};
            //    doc_ref.SetAsync(pl);

            //    if (doc_ref.GetSnapshotAsync().Result.Exists)
            //        count++;
            //}
            //if (count == playlists.Count)
            //    return true;
            //else
                return false;
        }

        public static bool DeletePlaylist(Guid id)
        {
            //doc_ref = db.Collection("Playlists/" + Profile.login + "/Playlists").Document(id.ToString());
            //doc_ref.DeleteAsync();
            //DocumentSnapshot snapshot = doc_ref.GetSnapshotAsync().Result;
            //if (snapshot.Exists)
            //    return false;
            //else
                return true;
        }
    }
}
