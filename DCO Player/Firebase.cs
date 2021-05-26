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

            Profile.Id_user = Guid.Parse(snapshot.GetValue<string>("Id_user"));
            Profile.name = snapshot.GetValue<string>("Name");
            Profile.surname = snapshot.GetValue<string>("Surname");
            Profile.login = snapshot.GetValue<string>("Login");
            Profile.password = snapshot.GetValue<string>("Password");
            Profile.createDate = DateTime.Parse(snapshot.GetValue<string>("Create_date"));
            Profile.imageSrc = snapshot.GetValue<string>("User_image_source");
            Profile.subscriptionDate = DateTime.Parse(snapshot.GetValue<string>("Subscription_date"));
            Profile.gbs = snapshot.GetValue<int>("N_GB");
            Profile.GBDate = DateTime.Parse(snapshot.GetValue<string>("GB_date"));
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
            coll_ref = db.Collection("Radio");
            QuerySnapshot snapshot = coll_ref.GetSnapshotAsync().Result;

            foreach (var doc in snapshot.Documents)
            {
                RadioStation station = new RadioStation();
                station.Id = doc.GetValue<int>("Id_radio");
                station.name = doc.GetValue<string>("Radio");
                station.descr = doc.GetValue<string>("Description");
                station.stream = doc.GetValue<string>("Stream");
                station.page = doc.GetValue<string>("Page");
                station.imageSrc = doc.GetValue<string>("Radio_image_source");

                radioStations.Add(station);
            }
            return radioStations;
        }

        public static UserPlaylist GetPlaylist(string Id_playlist)
        {
            string[] split1 = { " , " };
            string[] split2 = { " = " };
            UserPlaylist playlist = new UserPlaylist();
            doc_ref = db.Collection("Playlists/" + Profile.login + "/Playlists").Document(Id_playlist);
            DocumentSnapshot doc = doc_ref.GetSnapshotAsync().Result;
            if (!doc.Exists)
                return null;
            playlist.Id_user = Guid.Parse(doc.GetValue<string>("Id_user"));
            playlist.Id_playlist = Guid.Parse(doc.GetValue<string>("Id_playlist"));
            playlist.name = doc.GetValue<string>("Name");
            playlist.lastUpdate = DateTime.Parse(doc.GetValue<string>("Last_update"));
            playlist.lastSync = DateTime.Parse(doc.GetValue<string>("Last_sync"));
            playlist.imageSrc = doc.GetValue<string>("Playlist_image_source");
            var s = doc.GetValue<string>("Songs");
            List<Tuple<Guid, int>> songs = new List<Tuple<Guid, int>>();
            if (s != "")
                songs = s.Split(split1, StringSplitOptions.None)
                        .Select(p => p.Split(split2, StringSplitOptions.None))
                        .Select(p => new Tuple<Guid, int>(Guid.Parse(p[0]), int.Parse(p[1])))
                        .ToList();
            songs = songs.OrderByDescending(p => p.Item2).ToList();
            playlist.songs = GetSongs(songs.Select(p => p.Item1).ToList());

            return playlist;

        }
        public static List<UserPlaylist> GetPlaylists(bool deleted = false)
        {
            List<UserPlaylist> playlists = new List<UserPlaylist>();
            coll_ref = db.Collection("Playlists/" + Profile.login + "/Playlists");
            QuerySnapshot snapshot = coll_ref.GetSnapshotAsync().Result;

            foreach (var doc in snapshot.Documents)
            {
                UserPlaylist playlist = new UserPlaylist();
                playlist = GetPlaylist(doc.GetValue<string>("Id_playlist"));
                if (playlist.name == UserPlaylist.NULL_NAME && deleted)
                    playlists.Add(playlist);
                else
                    continue;
            }
            return playlists;
        }

        public static bool AddPlaylist(UserPlaylist playlist)
        {
            return AddPlaylists(new List<UserPlaylist> { playlist });
        }
        public static bool AddPlaylists(List<UserPlaylist> playlists)
        {
            int count = 0;
            string songs;

            foreach (var playlist in playlists)
            {
                var doc_name = playlist.Id_playlist.ToString();
                doc_ref = db.Collection("Playlists/" + Profile.login + "/Playlists").Document(doc_name);
                if (playlist.songs.Count > 0)
                {
                    songs = string.Join(" , ", playlist.songs.Select(kv => kv.Id_song.ToString() + " = " + kv.n_sequence).ToArray());
                }
                else
                    songs = "";

                Dictionary<string, object> pl = new Dictionary<string, object>
            {
                { "Id_user", playlist.Id_user.ToString() },
                { "Id_playlist", playlist.Id_playlist.ToString() },
                { "Name", playlist.name },
                { "Last_update", playlist.lastUpdate.ToString() },
                { "Last_sync", DateTime.Now.ToString() },
                { "Playlist_image_source", playlist.imageSrc },
                { "Songs", songs }
            };
                doc_ref.SetAsync(pl);

                if (doc_ref.GetSnapshotAsync().Result.Exists)
                    count++;
            }
            if (count == playlists.Count)
                return true;
            else
                return false;
        }

        public static bool DeletePlaylist(Guid id)
        {
            doc_ref = db.Collection("Playlists/" + Profile.login + "/Playlists").Document(id.ToString());
            doc_ref.DeleteAsync();
            DocumentSnapshot snapshot = doc_ref.GetSnapshotAsync().Result;
            if (snapshot.Exists)
                return false;
            else
                return true;
        }
        
        public static List<Song> GetSongs(List<Guid> Id_songs = null, bool all = false)
        {
            List<Song> songs = new List<Song>();
            List<Song> songs_playlist = new List<Song>();
            Song s;
            coll_ref = db.Collection("Songs");
            QuerySnapshot snapshot = coll_ref.GetSnapshotAsync().Result;
            int i = 0;
            foreach (var doc in snapshot.Documents)
            {
                Song song = new Song();
                song.Id_song = Guid.Parse(doc.GetValue<string>("Id_song"));
                song.is_local = doc.GetValue<bool>("Is_local");
                song.full_name = doc.GetValue<string>("Full_name");
                song.name = doc.GetValue<string>("Name");
                song.artist = doc.GetValue<string>("Artist");
                song.album = doc.GetValue<string>("Album");
                song.length = doc.GetValue<int>("Length");
                song.path = doc.GetValue<string>("Path");
                songs.Add(song);
            }
            if (all)
                return songs;
            else
            {
                foreach(var id in Id_songs)
                {
                    s = songs.First(o => o.Id_song == id);
                    s.n_sequence = i;
                    songs_playlist.Add(s);
                    i++;
                }
                return songs_playlist;
            }
        }

        public static bool AddSong(Song song)
        {
            return AddSongs(new List<Song> { song });
        }
        public static bool AddSongs(List<Song> songs)
        {
            int count = 0;

            foreach (var song in songs)
            {
                doc_ref = db.Collection("Songs/" + Profile.login + "/Songs").Document(song.Id_song.ToString());

                Dictionary<string, object> s = new Dictionary<string, object>
            {
                { "Id_song", song.Id_song.ToString() },
                { "Is_local", song.is_local.ToString() },
                { "Full_name", song.full_name },
                { "Name", song.name },
                { "Artist", song.artist },
                { "Album", song.album },
                { "Length", song.length },
                { "Path", song.path }
            };
                doc_ref.SetAsync(s);

                if (doc_ref.GetSnapshotAsync().Result.Exists)
                    count++;
            }
            if (count == songs.Count)
                return true;
            else
                return false;
        }
        public static bool DeleteSong(Song song)
        {
            return DeleteSongs(new List<Song> { song });
        }
        public static bool DeleteSongs(List<Song> songs)
        {
            int count = 0;

            foreach (var song in songs)
            {
                doc_ref = db.Collection("Songs/" + Profile.login + "/Songs").Document(song.Id_song.ToString());
                doc_ref.DeleteAsync();
                if (doc_ref.GetSnapshotAsync().Result.Exists)
                    count++;
            }
            if (count == songs.Count)
                return true;
            else
                return false;
        }
    }
}
