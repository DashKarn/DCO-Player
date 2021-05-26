using Microsoft.Win32;
using System;
using System.Linq;
using System.Data.SqlClient;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Threading;


namespace DCO_Player
{
    /// <summary>
    /// Логика взаимодействия для CreatePlaylist.xaml
    /// </summary>
    public partial class CreatePlaylist : Window
    {
        //Максимизация окна без полей
        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }
            return (IntPtr)0;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>x coordinate of point.</summary>
            public int x;
            /// <summary>y coordinate of point.</summary>
            public int y;
            /// <summary>Construct a point of coordinates (x,y).</summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public static readonly RECT Empty = new RECT();
            public int Width { get { return Math.Abs(right - left); } }
            public int Height { get { return bottom - top; } }
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
            public RECT(RECT rcSrc)
            {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }
            public bool IsEmpty { get { return left >= right || top >= bottom; } }
            public override string ToString()
            {
                if (this == Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }
            public override bool Equals(object obj)
            {
                if (!(obj is Rect)) { return false; }
                return (this == (RECT)obj);
            }
            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode() => left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2) { return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom); }
            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2) { return !(rect1 == rect2); }
        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        CroppedBitmap cb;

        private DispatcherTimer timer = null;
        int number = 0;
        Regex regx_name = new Regex(@"^.{1, 45}$");
        Regex regx_empty = new Regex(@"^\s+$");
        List<Song> songs = new List<Song>();
        List<Song> songs_all = new List<Song>();

        public CreatePlaylist()
        {
            InitializeComponent();            
        }

        // Событие, направленное на правильную работу окна
        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr handle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
        }

        private void Image_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|Portable Network Graphic (*.png)|*.png";
                if (openFileDialog.ShowDialog() == true)
                {
                    Uri uri = new Uri(openFileDialog.FileName); // Получаем ссылку на файл (картинку)

                    System.Windows.Controls.Image croppedImage = new System.Windows.Controls.Image();
                    BitmapImage bm = new BitmapImage(uri); // Создаем новый образ битового изображения
                    cb = new CroppedBitmap(
                       bm,
                       new Int32Rect((int)(((int)bm.PixelWidth - 600) / 2), (int)(((int)bm.PixelHeight - 600) / 2), 600, 600));       // Выбираем настройки обрезки

                    Image.Background = new ImageBrush(cb);
                }
            }
            catch
            {
                MessageBox.Show("Изображение должно быть 600х600 пикселей");
            }
            
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            CroppedBitmap Cb = cb;

            new Thread(() =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {

                    string Img;

                    string Text;

                    if (Cb != null)
                    {
                        BitmapEncoder encoder = new PngBitmapEncoder(); // Создаем новый образ кодировщика
                        encoder.Frames.Add(BitmapFrame.Create(Cb)); // кодируем наше обрезанное изображение в png и далее ниже его сохраняем

                        Img = "/Images/Playlists/" + NamePlaylist.Text + ".png";

                        using (var fileStream = new System.IO.FileStream(Environment.CurrentDirectory + Img, System.IO.FileMode.Create))
                        {
                            encoder.Save(fileStream);
                        }
                    }
                    else
                    {
                        Img = "";
                    }

                    if (regx_name.IsMatch(NamePlaylist.Text))
                    {
                        Text = NamePlaylist.Text;
                    }
                    else
                    {
                        Text = "Playlist " + DateTime.Today.ToString("dd.MM.yyyyy");
                    }
                    int i = 1;
                    while (true)
                        if (Vars.allPlaylists.Select(p => p.Item2).ToList().Contains(Text))
                        {
                            MessageBox.Show("Такое имя уже существует");
                            Text = Text + " " + i.ToString();
                            i++;
                        }
                        else
                            break;

                    UserPlaylist playlist = new UserPlaylist();
                    playlist.Id_playlist = Guid.NewGuid();
                    playlist.Id_user = Profile.Id_user;
                    playlist.name = Text;
                    playlist.lastUpdate = DateTime.Now;
                    playlist.lastSync = DateTime.MinValue;
                    playlist.imageSrc = Img;
                    playlist.songs = songs;

                    if (Database.InsertSongs(songs_all))
                    {
                        Firebase.AddSongs(songs_all);
                        if (Database.InsertPlaylist(playlist))
                        {
                            if (Firebase.AddPlaylist(playlist))
                            {
                                playlist.lastSync = DateTime.Now;
                                Database.UpdatePlaylist(playlist);
                                Firebase.AddPlaylist(playlist);
                            }
                        }
                        else
                            MessageBox.Show("Не удалось создать плейлист. \n" +
                                "Нет подключения к БД");
                    }
                    else
                        MessageBox.Show("Не удалось создать плейлист. \n" +
                            "Нет подключения к БД");
                }));                
            }).Start();

            Close();
        }

        private void CloseWindow_Executed(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

            bool db;
            List<Tuple<Guid, string>> paths;
            string Text = "";
            int i;

            new Thread(() =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                    dialog.InitialDirectory = "C:\\Users";
                    dialog.IsFolderPicker = true;
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        Text = dialog.FileName;
                        string[] allfiles = Directory.GetFiles(Text);
                        (db, paths) = Database.GetSongsPath();
                        if (!db)
                            MessageBox.Show("Отсутствует подключение к БД");
                        else
                        {
                            i = 0;
                            foreach (string file in allfiles)
                            {
                                Song song = new Song();
                                song.addProperties(file);
                                if (paths.Any(m => m.Item2 == file))
                                    song.Id_song = paths.First(m => m.Item2 == file).Item1;
                                else
                                {
                                    song.Id_song = Guid.NewGuid();
                                    songs_all.Add(song);
                                }
                                song.n_sequence = i;
                                songs.Add(song);
                                i++;
                            }
                        }
                    }                   
                }));

            }).Start();
        }
    }
}
