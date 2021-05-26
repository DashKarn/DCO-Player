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

        public static bool UpdateUser()
        {
            bool db = true;
            string sqlExpression = "UPDATE Users SET Subscription_date = @Subscription_date, N_GB = @N_GB, GB_date = @GB_date " +
                "WHERE Id_user = @Id_user";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.Parameters.Add(new SqlParameter("@Subscription_date",Profile.subscriptionDate));
                    command.Parameters.Add(new SqlParameter("@N_GB", Profile.gbs));
                    command.Parameters.Add(new SqlParameter("@GB_date", Profile.GBDate));
                    command.Parameters.Add(new SqlParameter("@Id_user", Profile.Id_user));
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
                        station.Id = (int)reader.GetValue(0);
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
                string sqlExpression = "DELETE FROM Radio WHERE Radio.Radio = @name";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.Parameters.Add(new SqlParameter("@name", name));
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

        public static (bool, List<UserPlaylist>) GetPlaylists(bool songs = true, bool deleted = false)
        {
            List<UserPlaylist> playlists = new List<UserPlaylist>();
            bool db;
            string sqlExpression = "SELECT * FROM Playlists WHERE Playlists.Id_user = @Id_user"; // Делаем запрос к плейлистам
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.Add(new SqlParameter("@Id_user", Profile.Id_user));
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read())
                    {
                        UserPlaylist playlist = new UserPlaylist();

                        playlist.Id_user = (Guid)reader.GetValue(0);
                        playlist.Id_playlist = (Guid)reader.GetValue(1);
                        playlist.name = reader.GetValue(2).ToString();
                        if (playlist.name == UserPlaylist.NULL_NAME && !deleted)
                            continue;
                        playlist.lastUpdate = (DateTime)reader.GetValue(3);
                        if (reader.GetValue(4) == DBNull.Value)
                            playlist.lastSync = DateTime.MinValue;
                        else
                            playlist.lastSync = (DateTime)reader.GetValue(4);
                        playlist.imageSrc = reader.GetValue(5).ToString();

                        playlists.Add(playlist);
                    }
                }
                db = true;
                reader.Close();
                connection.Close();
                if (songs)
                {
                    int i = 0;
                    bool pl;
                    while (i < playlists.Count)
                    {
                        (pl, playlists[i].songs) = GetSongsFromPlaylist(playlists[i].Id_playlist);
                        if (!pl)
                            db = false;
                        i++;
                    }
                }
            }
            catch
            {
                db = false;
            }
            return (db, playlists);
        }

        public static bool InsertPlaylist(UserPlaylist playlist)
        {
            return InsertPlaylists(new List<UserPlaylist> { playlist });
        }

        public static bool InsertPlaylists(List<UserPlaylist> playlists)
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
                        if (playlist.lastSync == DateTime.MinValue)
                            command.Parameters.Add(new SqlParameter("@Last_sync", DBNull.Value));
                        else
                            command.Parameters.Add(new SqlParameter("@Last_sync", playlist.lastSync));
                        command.Parameters.Add(new SqlParameter("@Playlist_image_source", playlist.imageSrc));
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                foreach (var playlist in playlists)
                {
                    if (!InsertSongsToPlaylist(playlist.songs, playlist.Id_playlist))
                        db = false;
                }
            }
            catch
            {
                db = false;
            }
            return db;
        }

        public static bool UpdatePlaylist(UserPlaylist playlist, List<Song> songs = null, bool s = false)
        {
            bool db = true;
            string sqlExpression = "UPDATE Playlists SET Last_sync = @Last_sync, Last_update = @Last_update " + 
                "Where Playlists.Id_playlist = @Id_playlist";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.Parameters.Add(new SqlParameter("@Id_playlist", playlist.Id_playlist));
                    if (playlist.lastSync == DateTime.MinValue)
                        command.Parameters.Add(new SqlParameter("@Last_sync", DBNull.Value));
                    else
                        command.Parameters.Add(new SqlParameter("@Last_sync", playlist.lastSync));
                    command.Parameters.Add(new SqlParameter("@Last_update", playlist.lastUpdate));
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                if (s && songs != null)
                    if (!InsertSongsToPlaylist(songs, playlist.Id_playlist))
                        db = false;
                if (s && songs == null)
                    if (!InsertAllSongsToPlaylist(playlist))
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
                sqlExpression = "DELETE FROM Playlist_songs WHERE Playlist_songs.Id_playlist = @Id_playlist";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.Parameters.Add(new SqlParameter("@Id_playlist", playlist.Id_playlist));
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                if (!NULL_name || playlist.lastSync == DateTime.MinValue)
                {

                    sqlExpression = "DELETE FROM Playlists WHERE Playlists.Id_playlist = @Id_playlist";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlExpression, connection);
                        command.Parameters.Add(new SqlParameter("@Id_playlist", playlist.Id_playlist));
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                else
                {
                    sqlExpression = "UPDATE Playlists SET Name = @NULL_NAME Where Playlists.Id_playlist = @Id_playlist";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlExpression, connection);
                        command.Parameters.Add(new SqlParameter("@NULL_NAME", UserPlaylist.NULL_NAME));
                        command.Parameters.Add(new SqlParameter("@Id_playlist", playlist.Id_playlist));
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

        public static (bool, List<Song>) GetSongsFromPlaylist(Guid Id_playlist)
        {
            List<Song> songs = new List<Song>();
            bool db;
            string sqlExpression = "SELECT * FROM Songs, Playlist_songs WHERE Playlist_songs.Id_song = Songs.Id_song  and Playlist_songs.Id_playlist = @Id_playlist"; // Делаем запрос к исполнителям
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
                songs = songs.OrderByDescending(o => o.n_sequence).ToList();
            }
            catch
            {
                db = false;
            }
            return (db, songs);
        }

        public static bool InsertSongToPlaylist(Song song, Guid Id_playlist)
        {
            return InsertSongsToPlaylist(new List<Song> { song }, Id_playlist);
        }
        public static bool InsertSongsToPlaylist(List<Song> songs, Guid Id_playlist)
        {
            bool db = true;
            List<Song> exist_songs;
            (db, exist_songs) = GetSongsFromPlaylist(Id_playlist);
            if (!db)
                return false;
            int max = 0;
            foreach (var s in exist_songs)
                if (s.n_sequence > max)
                    max = s.n_sequence;

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
                        command.Parameters.Add(new SqlParameter("@N_sequence", ++max));
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch
            {
                db = false;
            }
            return db;
        }

        public static bool InsertAllSongsToPlaylist(UserPlaylist playlist)
        {
            bool db = true;

            string sqlExpression;
            try
            {
                sqlExpression = "DELETE FROM Playlist_songs WHERE Playlist_songs.Id_playlist = @Id_playlist";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.Parameters.Add(new SqlParameter("@Id_playlist", playlist.Id_playlist));
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                sqlExpression = "INSERT INTO Playlist_songs (Id_playlist, Id_song, N_sequence) " +
                    "VALUES (@Id_playlist, @Id_song, @N_sequence)";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    foreach (var song in playlist.songs)
                    {
                        SqlCommand command = new SqlCommand(sqlExpression, connection);
                        command.Parameters.Add(new SqlParameter("@Id_playlist", playlist.Id_playlist));
                        command.Parameters.Add(new SqlParameter("@Id_song", song.Id_song));
                        command.Parameters.Add(new SqlParameter("@N_sequence", song.n_sequence));
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch
            {
                db = false;
            }
            return db;
        }

        public static (bool, List<Song>) GetAllSongs()
        {
            List<Song> songs = new List<Song>();
            bool db;
            string sqlExpression = "SELECT * FROM Songs";
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
                        Song song = new Song();
                        song.Id_song = (Guid)reader.GetValue(0);
                        song.is_local = Convert.ToBoolean(reader.GetValue(1));
                        song.full_name = reader.GetValue(2).ToString();
                        song.name = reader.GetValue(3).ToString();
                        song.artist = reader.GetValue(4).ToString();
                        song.album = reader.GetValue(5).ToString();
                        song.length = (int)reader.GetValue(6);
                        song.path = reader.GetValue(7).ToString();

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

        public static (bool, List<Tuple<Guid, string>>) GetSongsPath()
        {
            bool db;
            List<Tuple<Guid, string>> paths = new List<Tuple<Guid, string>>();
            string sqlExpression = "SELECT Id_song, Path FROM Songs";
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                        paths.Add(new Tuple<Guid, string> ((Guid)reader.GetValue(0), reader.GetValue(1).ToString()));
                reader.Close();
                connection.Close();
                db = true;
            }
            catch { db = false; }
            return (db, paths);
        }
        public static bool InsertSongs(List<Song> songs)
        {
            bool db = true;
            string sqlExpression = "INSERT INTO Songs (Id_song, Is_local, Full_name, Name, Artist, Album, Length, Path) VALUES" +
                    " (@Id_song, '1', @Full_name, @Name, @Artist, @Album, @Length, @Path)";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    foreach (var song in songs)
                    {
                        SqlCommand command = new SqlCommand(sqlExpression, connection);
                        command.Parameters.Add(new SqlParameter("@Id_song", song.Id_song));
                        //  command.Parameters.Add(new SqlParameter("@Is_local", song.is_local));
                        command.Parameters.Add(new SqlParameter("@Full_name", song.full_name));
                        command.Parameters.Add(new SqlParameter("@Name", song.name));
                        command.Parameters.Add(new SqlParameter("@Artist", song.artist));
                        command.Parameters.Add(new SqlParameter("@Album", song.album));
                        command.Parameters.Add(new SqlParameter("@Length", song.length));
                        command.Parameters.Add(new SqlParameter("@Path", song.path));
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch { db = false; }
            return db;
        }

    }
}
