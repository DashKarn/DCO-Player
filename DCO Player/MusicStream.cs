using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using System.Windows.Threading;
using Un4seen.Bass.AddOn.Fx;
using System.Windows;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Controls;

namespace DCO_Player
{
    class MusicStream
    {
        public static DispatcherTimer timer = new DispatcherTimer();    // Таймер

        private static readonly List<int> BassPluginsHandles = new List<int>();     // Список плагинов

        public static int HZ = 44100;           // Дефолтная частота дискретизации

        public static bool InitDefaultDevice;   // Переменная - инициализатор

        public static bool NextPoint = false;   // Логическая переменная ручного переключения

        public static bool EndPoint = false;    // Логическая переменная ручного переключения

        public static int Stream;               // Аудиоканал

        public static int Volume = 50;          // Громкость звука в процентном отношении

        public static bool HandlerAttached = false; // Проверка подписки событий

        public static int SSR = 0;             // Частота дискретизации в процентном отношении

        public static int Echo = 0;             // Эхо

        private static BASS_DX8_CHORUS _chorus = new BASS_DX8_CHORUS(0f, 25f, 90f, 5f, 1, 0f, BASSFXPhase.BASS_FX_PHASE_ZERO);

        private static BASS_DX8_ECHO _echo = new BASS_DX8_ECHO(90f, 50f, 500f, 500f, false);

        public static int Chorus = 0;           // Задержка

        public static double CorTime;

        private static int[] _fxEQ = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static bool isStopped = true;    // Логическая переменная ручной остановки

        public static bool EndPlaylist;         // Логическая переменная окончания плейлиста

        static object senderStart;              // Ссылка на кнопку старт в ЭУ
        static object senderStop;               // Ссылка на кнопку стоп в ЭУ

        public static bool InitBass(int hz)
        {
            if (!InitDefaultDevice)
            {
                InitDefaultDevice = Bass.BASS_Init(-1, hz, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                if (InitDefaultDevice)
                {
                    BassPluginsHandles.Add(Bass.BASS_PluginLoad(Vars.AppPath + @"Plugins\bass_aac.dll"));
                    BassPluginsHandles.Add(Bass.BASS_PluginLoad(Vars.AppPath + @"Plugins\bass_ac3.dll"));
                    BassPluginsHandles.Add(Bass.BASS_PluginLoad(Vars.AppPath + @"Plugins\bass_ape.dll"));
                    BassPluginsHandles.Add(Bass.BASS_PluginLoad(Vars.AppPath + @"Plugins\bass_mpc.dll"));
                    BassPluginsHandles.Add(Bass.BASS_PluginLoad(Vars.AppPath + @"Plugins\bass_tta.dll"));
                    BassPluginsHandles.Add(Bass.BASS_PluginLoad(Vars.AppPath + @"Plugins\bassalac.dll"));
                    BassPluginsHandles.Add(Bass.BASS_PluginLoad(Vars.AppPath + @"Plugins\bassflac.dll"));
                    BassPluginsHandles.Add(Bass.BASS_PluginLoad(Vars.AppPath + @"Plugins\bassopus.dll"));
                    BassPluginsHandles.Add(Bass.BASS_PluginLoad(Vars.AppPath + @"Plugins\basswebm.dll"));
                    BassPluginsHandles.Add(Bass.BASS_PluginLoad(Vars.AppPath + @"Plugins\basswma.dll"));
                    BassPluginsHandles.Add(Bass.BASS_PluginLoad(Vars.AppPath + @"Plugins\basswv.dll"));
                }

                int ErrorCount = 0;
                for (int i = 0; i < BassPluginsHandles.Count; i++)
                {
                    if (BassPluginsHandles[i] == 0)
                        ErrorCount++;
                    if (ErrorCount != 0)
                    {
                        MessageBox.Show(ErrorCount + "плагинов не было загружено", "Ошибка", MessageBoxButton.OK);
                    }
                    ErrorCount = 0;
                }

                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Tick;
            }


            return InitDefaultDevice;
        }   // Метод - инициализатор

        public static void PlayPlayer(string filename, int vol)
        {
            Bass.BASS_StreamFree(Stream);
            StreamLineStop();
            if (InitBass(HZ))
            {
                Stream = Bass.BASS_StreamCreateFile(filename, 0, 0, BASSFlag.BASS_DEFAULT);
                if (Stream != 0)
                {
                    Volume = vol;
                    SetFXParameters();
                    Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, Volume / 100F);
                    Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_FREQ, SSR / 0.001F);
                    Bass.BASS_ChannelPlay(Stream, false);

                    MainWindow.Instance.end.Content = FormatTimeSpan(TimeSpan.FromSeconds(MusicStream.GetTimeOfStream(MusicStream.Stream)));

                    if (HandlerAttached == false)
                    {
                        HandlerAttached = true;
                        MainWindow.Instance.Next.Click += MainWindow.Instance.NextButton;
                        MainWindow.Instance.End.Click += MainWindow.Instance.EndButton;
                    }


                    timer.Start();
                }
            }
            else
            {
                Bass.BASS_ChannelPlay(Stream, false);
            }
            isStopped = false;
        }   // Метод воспроизведения потока для плеера

        private static void Timer_Tick(object sender, EventArgs e)
        {
            MainWindow.Instance.start.Content = FormatTimeSpan(TimeSpan.FromSeconds(MusicStream.GetPosOfStream(MusicStream.Stream)));

            double Procent = (double)MusicStream.GetPosOfStream(MusicStream.Stream) / (double)MusicStream.GetTimeOfStream(MusicStream.Stream);

            MainWindow.Instance.TimeLine.Value = CorTime = Procent * 646.0; // наш таймлайн

            if (ToNextTrack() || Next() || Back())
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
               /* string sqlExpression = "SELECT Id_albums, Id_composition, Composition_source, Composition, Artist FROM Album, Artists, Albums where Artists.Id_artists = Albums.Id_artist and Albums.Id_albums = Album.Id_album and Album.Id_album = " + Vars.Id_album;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        if (Vars.Tracklist[Vars.CurrentTrackNumber].Item1 == (int)reader.GetValue(1)) // Проверка на совпадение ключей альбома
                        {
                            MainWindow.Instance.CompositionName.Text = reader.GetValue(3).ToString();
                            MainWindow.Instance.ArtistName.Text = reader.GetValue(4).ToString();
                            MainWindow.Instance.end.Content = FormatTimeSpan(TimeSpan.FromSeconds(MusicStream.GetTimeOfStream(MusicStream.Stream)));
                        }
                    }
                }*/

                string sqlExpression = "SELECT Id_composition, Name, Artist FROM Songs WHERE Songs.Id_playlist = " + Vars.Id_playlist.ToString();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        if (Vars.Tracklist[Vars.CurrentTrackNumber].Item1 == (Guid)reader.GetValue(0)) 
                        {
                            MainWindow.Instance.CompositionName.Text = reader.GetValue(1).ToString();
                            MainWindow.Instance.ArtistName.Text = reader.GetValue(2).ToString();
                            MainWindow.Instance.end.Content = FormatTimeSpan(TimeSpan.FromSeconds(MusicStream.GetTimeOfStream(MusicStream.Stream)));
                        }
                    }
                }
            }

            if (EndPlaylist)
            {
                Stop();
                StreamLineStop();

                MainWindow.Instance.start.Content = FormatTimeSpan(TimeSpan.FromSeconds(GetTimeOfStream(Stream))); // Ставлю время на конец проигрыша
                Bass.BASS_ChannelSetPosition(Stream, (double)GetTimeOfStream(Stream) - 0.5);    // ставлю slider на 100%

                Vars.CurrentTrackNumber = -1;

                EndPlaylist = false;
            }


        }

        private static string FormatTimeSpan(TimeSpan time)
        {
            return ((time < TimeSpan.Zero) ? "-" : "") + time.ToString(@"mm\:ss");
        }

        public static void PlayRadio(string URLName, int vol)
        {
            Bass.BASS_StreamFree(Stream);

            if (InitBass(HZ))
            {
                Stream = Bass.BASS_StreamCreateURL(URLName, 0, BASSFlag.BASS_DEFAULT, null, IntPtr.Zero);
                if (Stream != 0)
                {
                    timer.Stop();

                    Volume = vol;
                    SetFXParameters();
                    Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, Volume / 100F);
                    Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_FREQ, SSR / 0.001F);
                    Bass.BASS_ChannelPlay(Stream, false);

                    MainWindow.Instance.end.Content = "00:00";
                    MainWindow.Instance.start.Content = "00:00";

                    MainWindow.Instance.TimeLine.Value = 0;

                    if (HandlerAttached == true)
                    {
                        HandlerAttached = false;
                        MainWindow.Instance.Next.Click -= MainWindow.Instance.NextButton;
                        MainWindow.Instance.End.Click -= MainWindow.Instance.EndButton;
                    }
                }
            }

        }   // Метод воспроизведения потока для радио

        public static int GetTimeOfStream(int stream)
        {
            long TimeBytes = Bass.BASS_ChannelGetLength(stream);
            double Time = Bass.BASS_ChannelBytes2Seconds(stream, TimeBytes);
            return (int)Time;
        }   // Метод описания полного времени

        public static int GetPosOfStream(int stream)
        {
            long Pos = Bass.BASS_ChannelGetPosition(stream);
            double PosSec = Bass.BASS_ChannelBytes2Seconds(stream, Pos);
            return (int)PosSec;
        }    // Метод описания текущего времени

        public static void StreamLineStart()
        {
            if (MainWindow.Instance != null)
            {
                MainWindow.Instance.Start.Visibility = Visibility.Collapsed;
                MainWindow.Instance.Stop.Visibility = Visibility.Visible;
            }

            if (senderStart != null && senderStop != null)
            {
                Button selectedButton = (Button)senderStop;
                selectedButton.Visibility = Visibility.Visible;

                selectedButton = (Button)senderStart;
                selectedButton.Visibility = Visibility.Collapsed;
            }

        }   // Метод описания поведения кнопки START для панели воспроизведения

        public static void StreamLineStart(string Composition, string Artist, object Sender)
        {
            if (senderStart != null && senderStart != Sender)
            {
                if (senderStart != null)
                {
                    Button selectedButton = (Button)senderStart;
                    selectedButton.Visibility = Visibility.Visible;
                }
            }

            senderStart = Sender;

            if (MainWindow.Instance != null)
            {
                MainWindow.Instance.CompositionName.Text = Composition;
                MainWindow.Instance.ArtistName.Text = Artist;

                MainWindow.Instance.Start.Visibility = Visibility.Collapsed;
                MainWindow.Instance.Stop.Visibility = Visibility.Visible;

                Button selectedButton = (Button)Sender;
                selectedButton.Visibility = Visibility.Collapsed;

                if (senderStop != null)
                {
                    selectedButton = (Button)senderStop;
                    selectedButton.Visibility = Visibility.Visible;
                }
            }
        }   // Метод для описания поведения кнопки START в UserControl

        public static void Play()
        {
            Bass.BASS_ChannelPlay(Stream, false);
            isStopped = false;
        }   // Общий метод воспроизведения потока

        public static void Stop()
        {
            isStopped = true;
            Bass.BASS_ChannelStop(Stream);
        }   // Общий метод остановки потока

        public static void StreamLineStop()
        {
            MainWindow.Instance.Start.Visibility = Visibility.Visible;
            MainWindow.Instance.Stop.Visibility = Visibility.Collapsed;

            if (senderStart != null && senderStop != null)
            {
                Button selectedButton = (Button)senderStart;
                selectedButton.Visibility = Visibility.Visible;

                selectedButton = (Button)senderStop;
                selectedButton.Visibility = Visibility.Collapsed;
            }

        }       // Метод описания поведения кнопки STOP для панели воспроизведения

        public static void StreamLineStop(object Sender)
        {
            senderStop = Sender;

            MainWindow.Instance.Start.Visibility = Visibility.Visible;
            MainWindow.Instance.Stop.Visibility = Visibility.Collapsed;

            Button selectedButton = (Button)Sender;
            selectedButton.Visibility = Visibility.Collapsed;

            if (senderStart != null)
            {
                selectedButton = (Button)senderStart;
                selectedButton.Visibility = Visibility.Visible;
            }
        }   // Метод для описания поведения кнопки STOP в UserControl

        public static bool ToNextTrack()
        {
            if ((Bass.BASS_ChannelIsActive(Stream) == BASSActive.BASS_ACTIVE_STOPPED) && !isStopped)
            {
                if (Vars.Tracklist.Count > Vars.CurrentTrackNumber + 1)
                {
                    PlayPlayer(Vars.Tracklist[++Vars.CurrentTrackNumber].Item2, Volume);
                    StreamLineStart();
                    EndPlaylist = false;
                    return true;
                }
                else
                    EndPlaylist = true;


            }
            return false;
        }   // Метод для описания автоматического переключения

        public static bool Next()
        {
            if (NextPoint)
            {
                NextPoint = false;
                if (Vars.Tracklist.Count > Vars.CurrentTrackNumber + 1)
                {
                    PlayPlayer(Vars.Tracklist[++Vars.CurrentTrackNumber].Item2, Volume);
                    StreamLineStart();

                    return true;
                }
                else
                    EndPlaylist = true;

            }
            return false;
        }   // Метод для описания пренудительного переключения вперед

        public static bool Back()
        {
            if (EndPoint)
            {
                EndPoint = false;
                if (Vars.CurrentTrackNumber > 0)
                {
                    PlayPlayer(Vars.Tracklist[--Vars.CurrentTrackNumber].Item2, Volume);
                    Play();
                    StreamLineStart();
                    return true;
                }
            }
            return false;
        }   // Метод для описания принудительного переключения назад

        public static void UpdateEQ(int band, float gain)
        {
            BASS_DX8_PARAMEQ eq = new BASS_DX8_PARAMEQ();
            if (Bass.BASS_FXGetParameters(_fxEQ[band], eq))
            {
                eq.fGain = gain;
                Bass.BASS_FXSetParameters(_fxEQ[band], eq);
            }
        }

        public static void SetFXParameters()
        {
            Chorus = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_CHORUS, 1);
            _chorus.fWetDryMix = 0f;

            Bass.BASS_FXSetParameters(Chorus, _chorus);

            Echo = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_ECHO, 2);
            _echo.fWetDryMix = 0f;

            Bass.BASS_FXSetParameters(Echo, _echo);

            BASS_DX8_PARAMEQ eq = new BASS_DX8_PARAMEQ();
            _fxEQ[0] = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[1] = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[2] = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[3] = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[4] = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[5] = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[6] = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[7] = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[8] = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[9] = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);

            eq.fBandwidth = 18f;

            eq.fCenter = 30f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[0], eq);
            eq.fBandwidth = 18f;
            eq.fCenter = 85f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[1], eq);
            eq.fBandwidth = 18f;
            eq.fCenter = 155f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[2], eq);
            eq.fBandwidth = 18f;
            eq.fCenter = 300f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[3], eq);
            eq.fBandwidth = 18f;
            eq.fCenter = 500f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[4], eq);
            eq.fBandwidth = 18f;
            eq.fCenter = 1500f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[5], eq);
            eq.fBandwidth = 18f;
            eq.fCenter = 3000f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[6], eq);
            eq.fBandwidth = 18f;
            eq.fCenter = 6000f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[7], eq);
            eq.fBandwidth = 18f;
            eq.fCenter = 7000f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[8], eq);
            eq.fBandwidth = 18f;
            eq.fCenter = 8000f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[9], eq);
        }

        public static void SetEcho(float value)
        {
            _echo.fWetDryMix = value;
            Bass.BASS_FXSetParameters(Echo, _echo);
        }
        public static void SetChorus(float value)
        {
            _chorus.fWetDryMix = value;
            Bass.BASS_FXSetParameters(Chorus, _chorus);
        }
    }
}
