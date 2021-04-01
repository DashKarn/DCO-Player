using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace DCO_Player
{
    /// <summary>
    /// Логика взаимодействия для RadioControl.xaml
    /// </summary>
    public partial class RadioControl : UserControl
    {
        public static RadioControl Instance { get; private set; }

        public RadioControl()
        {
            InitializeComponent();
            Instance = this;
        }

        public string src { get; set;}

        private void StartRadio_Click(object sender, RoutedEventArgs e)
        {
            if(src != "")
            {
                MusicStream.Stop();
                MusicStream.PlayRadio(src, MusicStream.Volume);
                MusicStream.StreamLineStart(CompositionName.Text, ArtistName.Text, sender);
                StopRadio.Visibility = Visibility.Visible;
            }
        }

        private void StopRadio_Click(object sender, RoutedEventArgs e)
        {
            MusicStream.Stop();
            MusicStream.StreamLineStop(sender);
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            BorderRadioControl.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5AE3DB"));
            Line.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5AE3DB"));
            RadiostationName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#F0F0F0"));
            ArtistName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#C2C2C2"));
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderRadioControl.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#303030"));
            Line.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#616161"));
            RadiostationName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            ArtistName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#7E7E7E"));
        }

        private void Share_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(src);
            MessageBox.Show("Скопировано в буфер");
        }
    }
}
