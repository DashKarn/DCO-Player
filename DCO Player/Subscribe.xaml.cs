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
    /// Логика взаимодействия для Subscribe.xaml
    /// </summary>
    public partial class Subscribe : Page
    {
        public Subscribe()
        {
            InitializeComponent();

            UpdateSubscription();
        }        
        public void UpdateSubscription()
        {
            if (Profile.subscriptionDate == DateTime.MinValue)
                s_date.Text = "unsubscribed";
            else
                s_date.Text = Profile.subscriptionDate.ToString("dd.MM.yyyy");

            if (Profile.GBDate == DateTime.MinValue)
            {
                to.Content = " ";
                gb_date.Text = " ";
                n_gb.Text = "0 GB";
            }
            else
            {
                gb_date.Text = Profile.GBDate.ToString("dd.MM.yyyy");
                to.Content = "to";
                if (Profile.gbs == Profile.UNLIM_GB)
                    n_gb.Text = "UNLIMITED GB";
                else
                    n_gb.Text = Profile.gbs.ToString() + " GB";
            }
        }


        private void s3_Click(object sender, RoutedEventArgs e)
        {
            Profile.subscriptionDate = DateTime.Now.AddMonths(3);
            Database.UpdateUser();
            Firebase.UpdateUser();
            UpdateSubscription();
        }


        private void s6_Click(object sender, RoutedEventArgs e)
        {
            Profile.subscriptionDate = DateTime.Now.AddMonths(6);
            Database.UpdateUser();
            Firebase.UpdateUser();
            UpdateSubscription();
        }

        private void s12_Click(object sender, RoutedEventArgs e)
        {
            Profile.subscriptionDate = DateTime.Now.AddMonths(12);
            Database.UpdateUser();
            Firebase.UpdateUser();
            UpdateSubscription();
        }

        private void gb5_Click(object sender, RoutedEventArgs e)
        {
            Profile.GBDate = DateTime.Now.AddYears(1);
            Profile.gbs = 5;
            Database.UpdateUser();
            Firebase.UpdateUser();
            UpdateSubscription();
        }

        private void gb10_Click(object sender, RoutedEventArgs e)
        {
            Profile.GBDate = DateTime.Now.AddYears(1);
            Profile.gbs = 10;
            Database.UpdateUser();
            Firebase.UpdateUser();
            UpdateSubscription();
        }


        private void unlim_Click(object sender, RoutedEventArgs e)
        {
            Profile.GBDate = DateTime.Now.AddYears(1);
            Profile.gbs = Profile.UNLIM_GB;
            Database.UpdateUser();
            Firebase.UpdateUser();
            UpdateSubscription();
        }

    }
}
