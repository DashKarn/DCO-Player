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
            string sqlExpression = "SELECT * FROM Users WHERE Login = @login";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            command.Parameters.Add(new SqlParameter("@login", login));
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
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public static (bool, List<RadioStation>) GetRadio()
        {
            List<RadioStation> radioStations = new List<RadioStation>();
            bool db;
            string sqlExpression = "SELECT * FROM Radio";
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RadioStation station = new RadioStation();
                        station.Id = (Guid)reader.GetValue(0);
                        station.name = reader.GetValue(1).ToString();
                        station.descr = reader.GetValue(2).ToString();
                        station.stream = reader.GetValue(3).ToString();
                        station.page = reader.GetValue(4).ToString();
                        station.imageSrc = reader.GetValue(5).ToString();

                        radioStations.Add(station);
                    }
                }
                db = true;
                reader.Close();
                connection.Close();
            }
            catch
            {
                db = false;
            }
            return (db, radioStations);
        }

        public static void InsertRadio(List<RadioStation> radioStations)
        {
            string sqlExpression = "INSERT INTO Radio (Id_Radio, Radio, Description, Stream, Page, Radio_image_source) VALUES" +
                    " (@Id_Radio, @Radio, @Description, @Stream, @Page, @Radio_image_source)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (var station in radioStations)
                {
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.Parameters.Add(new SqlParameter("@Id_Radio", station.Id));
                    command.Parameters.Add(new SqlParameter("@Radio", station.name));
                    command.Parameters.Add(new SqlParameter("@Description", station.descr));
                    command.Parameters.Add(new SqlParameter("@Stream", station.stream));
                    command.Parameters.Add(new SqlParameter("@Page", station.page));
                    command.Parameters.Add(new SqlParameter("@Radio_image_source", station.imageSrc));
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public static bool DeleteRadio(string name)
        {
            try
            {
                string sqlExpression = "DELETE FROM Radio WHERE Radio.Radio = " + name;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static (bool, List<UserPlaylist>) GetPlaylists()
        {
            List<UserPlaylist> playlists = new List<UserPlaylist>();
            bool db;
            string sqlExpression = "SELECT * FROM Playlists WHERE Playlists.Id_user = " + Profile.Id_user; // Делаем запрос к плейлистам
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read())
                    {
                        UserPlaylist playlist = new UserPlaylist();

                        playlist.Id_user = (Guid)reader.GetValue(0);
                        playlist.Id_playlist = (Guid)reader.GetValue(1);
                        playlist.name = reader.GetValue(2).ToString();
                        playlist.lastUpdate = (DateTime)reader.GetValue(3);
                        playlist.lastSync = (DateTime)reader.GetValue(4);
                        playlist.imageSrc = reader.GetValue(5).ToString();

                        playlists.Add(playlist);
                    }
                }
                db = true;
                reader.Close();
                connection.Close();
            }
            catch
            {
                db = false;
            }
            return (db, playlists);
        }

        public static bool InsertPlaylist(UserPlaylist playlist, bool songs=false)
        {
            return InsertPlaylists(new List<UserPlaylist> { playlist }, songs);
        }

        public static bool InsertPlaylists(List<UserPlaylist> playlists, bool songs = false)
        {
            bool db;
            List<UserPlaylist> exist_playlists = new List<UserPlaylist>(); ;
            (db, exist_playlists) = GetPlaylists();
            if (!db)
                return false;
            var ids = exist_playlists.Select(f => f.Id_playlist).ToList();

            string sqlExpression = "INSERT INTO Playlists (Id_user, Id_playlist, Name, Last_update, Last_sync, Playlist_image_source) VALUES" +
                    " (@Id_user, @Id_playlist, @Name, @Last_update, @Last_sync, @Playlist_image_source)";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    foreach (var playlist in playlists)
                    {
                        if (ids.Contains(playlist.Id_playlist))
                            continue;
                        SqlCommand command = new SqlCommand(sqlExpression, connection);
                        command.Parameters.Add(new SqlParameter("@Id_user", playlist.Id_user));
                        command.Parameters.Add(new SqlParameter("@Id_playlist", playlist.Id_playlist));
                        command.Parameters.Add(new SqlParameter("@Name", playlist.name));
                        command.Parameters.Add(new SqlParameter("@Last_update", playlist.lastUpdate));
                        command.Parameters.Add(new SqlParameter("@Last_sync", playlist.lastSync));
                        command.Parameters.Add(new SqlParameter("@Playlist_image_source", playlist.imageSrc));
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                if (songs)
                    foreach (var playlist in playlists)
                    {
                        if (!InsertSongs(playlist.songs, playlist.Id_playlist, false))
                            db = false;
                    }
            }
            catch
            {
                db = false;
            }
            return db;
        }

        public static bool UpdatePlaylist(UserPlaylist playlist, bool songs = false)
        {
            bool db = true;
            string sqlExpression = "UPDATE Playlists SET Last_sync = @Last_sync, Last_update = @Last_update " + 
                "Where Playlists.Id_playlist = " + playlist.Id_playlist;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.Parameters.Add(new SqlParameter("@Last_sync", playlist.lastSync));
                    command.Parameters.Add(new SqlParameter("@Last_update", playlist.lastUpdate));
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                if(songs)                    
                        if (!InsertSongs(playlist.songs, playlist.Id_playlist, false))
                            db = false;
            }
            catch
            {
                db = false;
            }
            return db;
        }

        public static bool DeletePlaylist(UserPlaylist playlist, bool NULL_name=true)
        {
            string sqlExpression;
            bool deleted = true;
            try
            {
                sqlExpression = "DELETE FROM Playlist_songs WHERE Playlist_songs.Id_playlist = " + playlist.Id_playlist;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                if (!NULL_name || playlist.lastSync == DateTime.MinValue)
                {

                    sqlExpression = "DELETE FROM Playlists WHERE Playlists.Id_playlist = " + playlist.Id_playlist;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlExpression, connection);
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                else
                {
                    sqlExpression = "UPDATE Playlists SET Name = @NULL_NAME Where Playlists.Id_playlist = " + playlist.Id_playlist;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlExpression, connection);
                        command.Parameters.Add(new SqlParameter("@NULL_NAME", UserPlaylist.NULL_NAME));
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch
            {
                deleted = false;
            }
            return deleted;
        }

        public static (bool, List<Song>) GetSongs(Guid Id_playlist)
        {
            List<Song> songs = new List<Song>();
            bool db;
            string sqlExpression = "SELECT * FROM Playlist_songs, Songs WHERE Playlist_songs.Id_song = Songs.Id_song  and Playlist_songs.Id_playlist = @Id_playlist"; // Делаем запрос к исполнителям
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.Add(new SqlParameter("@Id_playlist", Id_playlist));
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read())
                    {
                        Song song = new Song();

                        song.Id_song = (Guid)reader.GetValue(0);
                        song.is_local = (bool)reader.GetValue(1);
                        song.full_name = reader.GetValue(2).ToString();
                        song.name = reader.GetValue(3).ToString();
                        song.artist = reader.GetValue(4).ToString();
                        song.album = reader.GetValue(5).ToString();
                        song.length = (int)reader.GetValue(6);
                        song.path = reader.GetValue(7).ToString();
                        song.n_sequence = (int)reader.GetValue(10);

                        songs.Add(song);
                    }
                }
                db = true;
                reader.Close();
                connection.Close();
            }
            catch
            {
                db = false;
            }
            return (db, songs);
        }

        public static bool InsertSong(Song songs, Guid Id_playlist)
        {
            return InsertSongs(new List<Song> { songs }, Id_playlist);
        }
        public static bool InsertSongs(List<Song> songs, Guid Id_playlist, bool update = true)
        {
            bool db = true;
            List<Song> exist_songs = new List<Song>(); ;
            (db, exist_songs) = GetSongs(Id_playlist);
            if (!db)
                return false;
            int n_exists = exist_songs.Count;

            string sqlExpression;
            try
            {
                sqlExpression = "INSERT INTO Playlist_songs (Id_playlist, Id_song, N_sequence) " +
                    "VALUES (@Id_playlist, @Id_song, @N_sequence)";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    foreach (var song in songs)
                    {
                        SqlCommand command = new SqlCommand(sqlExpression, connection);
                        command.Parameters.Add(new SqlParameter("@Id_playlist", Id_playlist));
                        command.Parameters.Add(new SqlParameter("@Id_song", song.Id_song));
                        command.Parameters.Add(new SqlParameter("@N_sequence", ++n_exists));
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                if (!update)
                    return db;
                sqlExpression = "UPDATE Playlists SET Last_update = @Last_update " +
                "Where Playlists.Id_playlist = " + Id_playlist;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.Parameters.Add(new SqlParameter("@Last_update", DateTime.Now));
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch
            {
                db = false;
            }
            return db;
        }

    }
}
