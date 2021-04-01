using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;
using System.Data.SqlClient;
using System.Configuration;
using Un4seen.Bass;
using System.Windows.Threading;
using Un4seen.Bass.AddOn.Fx;

namespace DCO_Player
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }
            Marshal.StructureToPtr(mmi, lParam, true);
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

        public static MainWindow Instance { get; private set; }

        public Radio radioPage;
        public My_playlists MPPage;
        public Shop ShopPage;
        public Albums AlbumsPage;
        public Countries CountriesPage;
        public Settings SettingsPage;

        public MainWindow()
        {
            
            InitializeComponent();
            Instance = this;                                                // Ссылка на окно
            WindowState = WindowState.Maximized;                            // При запуске состояние окна - на полный экран
            SourceInitialized += MainWindow_SourceInitialized; 

            Frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;   // Прячем гребаную панель навигации

            AccountName.Text = Profile.name + " " + Profile.surname;        // Имя и фамилия в профиле
            if(Profile.imageSrc != "")
            {
                AccountImage.ImageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + Profile.imageSrc, UriKind.Absolute)); // Изображение профиля
            }

            radioPage = new Radio();                                        // Новые образы страниц, для загрузки их вместе с окном
            MPPage = new My_playlists();
            ShopPage = new Shop();
            AlbumsPage = new Albums();
            CountriesPage = new Countries();
            SettingsPage = new Settings();

            CountriesBrush.Brush = CountriesTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E46E62")); // Запускаем первую страны
            Frame.Navigate(CountriesPage);

            MusicStream.InitBass(MusicStream.HZ);
            
        }

        // Событие, направленное на правильную работу окна
        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr handle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
        }

        // Событие скрытия окна
        private void MinimizeWindow_Executed(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // Событие "на полный экран"
        private void MaximizeWindow_Executed(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            if (WindowState.Normal == 0)
            {
                this.Width = 1350;
                this.Height = SystemParameters.FullPrimaryScreenHeight + 8;
                this.Top = 8;
                this.Left = 8;
            }

        }

        // Событие закрытия приложения
        private void CloseWindow_Executed(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Событие скрытия меню
        private void CloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonMenuOpen.Visibility = Visibility.Visible;
            ButtonMenuClose.Visibility = Visibility.Collapsed;
            LogOut.Visibility = Visibility.Collapsed;
            Menu.Margin = new Thickness(4,0,0,0);
            search.Visibility = Visibility.Collapsed;
            My_playlists.Margin = new Thickness(10, 140, 0, 0);
            ButtonMenuOpen.Margin = new Thickness(0, 6, 6, 0);
            LogOutGrid.Margin = new Thickness(5, 0, 13, 11);
            Compositions.Visibility = Visibility.Collapsed;
            Albums.Visibility = Visibility.Collapsed;
        }

        // Событие развертывания меню
        private void OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonMenuOpen.Visibility = Visibility.Collapsed;
            ButtonMenuClose.Visibility = Visibility.Visible;
            LogOut.Visibility = Visibility.Visible;
            Menu.Margin = new Thickness(14, 0, 0, 0);
            search.Visibility = Visibility.Visible;
            My_playlists.Margin = new Thickness(10, 47, 0, 0);
            ButtonMenuOpen.Margin = new Thickness(0, 6, 6, 0);
            LogOutGrid.Margin = new Thickness(15, 0, 15, 11);
            Compositions.Visibility = Visibility.Visible;
            Albums.Visibility = Visibility.Visible;
        }

        // Событие перехода к странице радио
        private void Radio_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RadioBrush.Brush = RadioTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E46E62"));
            PlaylistsBrush.Brush = PlaylistsTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            ShopBrush.Brush = ShopTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            AlbumsBrush.Brush = AlbumsTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            CountriesBrush.Brush = CountriesTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            Frame.NavigationService.Navigate(radioPage);
        }

        // Событие перехода к странице плейлистов
        private void Playlists_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PlaylistsBrush.Brush = PlaylistsTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E46E62"));
            RadioBrush.Brush = RadioTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            ShopBrush.Brush = ShopTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            AlbumsBrush.Brush = AlbumsTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            CountriesBrush.Brush = CountriesTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            Frame.Navigate(new My_playlists());
        }

        // Событие перехода к магазину
        private void Shop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ShopBrush.Brush = ShopTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E46E62"));
            PlaylistsBrush.Brush = PlaylistsTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            RadioBrush.Brush = RadioTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            AlbumsBrush.Brush = AlbumsTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            CountriesBrush.Brush = CountriesTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            Frame.Navigate(ShopPage);
        }

        // Событие перехода к альбомам
        private void Albums_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AlbumsBrush.Brush = AlbumsTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E46E62"));
            ShopBrush.Brush = ShopTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            PlaylistsBrush.Brush = PlaylistsTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            RadioBrush.Brush = RadioTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            CountriesBrush.Brush = CountriesTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            Frame.Navigate(new Albums());
        }

        // Событие перехода к странам
        private void Countries_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CountriesBrush.Brush = CountriesTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E46E62"));
            AlbumsBrush.Brush = AlbumsTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            ShopBrush.Brush = ShopTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            PlaylistsBrush.Brush = PlaylistsTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            RadioBrush.Brush = RadioTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            Frame.Navigate(CountriesPage);
        }

        // Событие запуска воспроизведения
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            MusicStream.Stop();
            MusicStream.Play();
            MusicStream.StreamLineStart();
        }

        // События останова воспроизведения
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            MusicStream.Stop();
            MusicStream.StreamLineStop();
        }

        // Событие выхода из аккаунта
        private void LogOut_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Bass.BASS_ChannelStop(MusicStream.Stream);
            Bass.BASS_StreamFree(MusicStream.Stream);

            Start start = new Start();
            start.Show();

            MainWindow.Instance.Close();
        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new Statictics());
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new Search());
        }

        public void NextButton(object sender, RoutedEventArgs e)
        {
            MusicStream.Next();
            MusicStream.NextPoint = true;
        }

        public void EndButton(object sender, RoutedEventArgs e)
        {
            MusicStream.End();
            MusicStream.EndPoint = true;
        }

        private void Settings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(SettingsPage);
        }

        private void TimeLine_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(Math.Abs(TimeLine.Value - MusicStream.CorTime) > 2) {
                Bass.BASS_ChannelSetPosition(MusicStream.Stream, (double)MusicStream.GetTimeOfStream(MusicStream.Stream) * (TimeLine.Value / 646.0));
            }
                   
        }
    }
}
