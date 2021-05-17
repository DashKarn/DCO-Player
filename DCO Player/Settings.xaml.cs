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
using System.Windows.Threading;
using Un4seen.Bass;

namespace DCO_Player
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public static DispatcherTimer timer = new DispatcherTimer();

        BASS_DX8_PARAMEQ par = new BASS_DX8_PARAMEQ();

        public Settings()
        {
            InitializeComponent();
        }  

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MusicStream.Volume = (int)e.NewValue;
            Bass.BASS_ChannelSetAttribute(MusicStream.Stream, BASSAttribute.BASS_ATTRIB_VOL, (float)(e.NewValue / 100F));
            if (VolumeContent != null)
            VolumeContent.Content = Math.Round(e.NewValue, 0);

        }

        private void SSR_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MusicStream.SSR = (int)e.NewValue;
            Bass.BASS_ChannelSetAttribute(MusicStream.Stream, BASSAttribute.BASS_ATTRIB_FREQ, (float)(e.NewValue / 0.001F));
            if (SSRContent != null)
                SSRContent.Content = Math.Round(e.NewValue, 0);
        }

        private void updateEqualizer()
        {
            Dispatcher.Invoke(new Action(delegate ()
            {
                foreach (object c in LogicalTreeHelper.GetChildren(gridequalizer))
                {

                    if (c is Slider && ((Slider)c).Tag.ToString() != "echochorus")
                    {
                        int index = int.Parse(((Slider)c).Tag.ToString());
                        MusicStream.UpdateEQ(index, (float)((Slider)c).Value / 10f);
                    }
                }
            }));
        }

        private void First_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MusicStream.UpdateEQ(0, ((float)first.Value / 10f)); // 30
            if(firstContent != null)
                firstContent.Content = Math.Round(e.NewValue / 10, 0) + " dB";
        }

        private void Second_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MusicStream.UpdateEQ(1, ((float)second.Value / 10f));  // 85
            if (secondContent != null)
                secondContent.Content = Math.Round(e.NewValue / 10, 0) + " dB";
        }

        private void Third_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MusicStream.UpdateEQ(2, ((float)third.Value / 10f)); // 155
            if (thirdContent != null)
                thirdContent.Content = Math.Round(e.NewValue / 10, 0) + " dB";
        }

        private void Fourth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MusicStream.UpdateEQ(3, ((float)fourth.Value / 10f)); // 300
            if (fourthContent != null)
                fourthContent.Content = Math.Round(e.NewValue / 10, 0) + " dB";
        }

        private void Fifth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MusicStream.UpdateEQ(4, ((float)fifth.Value / 10f)); // 500
            if (fifthContent != null)
                fifthContent.Content = Math.Round(e.NewValue / 10, 0) + " dB";
        }

        private void Sixth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MusicStream.UpdateEQ(5, ((float)sixth.Value / 10f)); // 1500
            if (sixthContent != null)
                sixthContent.Content = Math.Round(e.NewValue / 10, 0) + " dB";
        }

        private void Seventh_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MusicStream.UpdateEQ(6, ((float)seventh.Value / 10f)); // 3000
            if (seventhContent != null)
                seventhContent.Content = Math.Round(e.NewValue / 10, 0) + " dB";
        }

        private void Eighth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MusicStream.UpdateEQ(7, ((float)eighth.Value / 10f)); // 6000
            if (eighthContent != null)
                eighthContent.Content = Math.Round(e.NewValue / 10, 0) + " dB";
        }

        private void Nineth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MusicStream.UpdateEQ(8, ((float)nineth.Value / 10f)); // 7000
            if (ninethContent != null)
                ninethContent.Content = Math.Round(e.NewValue / 10, 0) + " dB";
        }

        private void Tenth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MusicStream.UpdateEQ(9, ((float)tenth.Value / 10f)); // 8000
            if (tenthContent != null)
                tenthContent.Content = Math.Round(e.NewValue / 10, 0) + " dB";
        }

        private void Chorus_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MusicStream.SetChorus((float)Chorus.Value / 10f);
            if (ChorusContent != null)
                ChorusContent.Content = Math.Round(e.NewValue / 10, 0) + " dB";
        }

        private void Echo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MusicStream.SetEcho((float)Echo.Value);
            if (EchoContent != null)
                EchoContent.Content = Math.Round(e.NewValue / 10, 0) + " dB";
        }
    }
}
