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

namespace DCO_Player
{
    /// <summary>
    /// Логика взаимодействия для RadioPlaylistControl.xaml
    /// </summary>
    public partial class RadioPlaylistControl : UserControl
    {
        public RadioPlaylistControl()
        {
            InitializeComponent();
        }

        private void Share_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CompositionName.Text + " - " + ArtistName.Text);
            MessageBox.Show("Скопировано в буфер");
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            BorderRPC.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5AE3DB"));
            CompositionName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#F0F0F0"));
            ArtistName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#C2C2C2"));
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderRPC.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#303030"));
            CompositionName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AFAFAF"));
            ArtistName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#7E7E7E"));
        }
    }
}
