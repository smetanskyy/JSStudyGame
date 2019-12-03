using JSStudyGame.Helpers;
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

namespace JSStudyGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _hostUrl;
        public static string playerLogin = "none";
        public static string playerPassword = "none";
        public MainWindow(string hostUrl)
        {
            _hostUrl = hostUrl;
            InitializeComponent();
        }

        private void ImgLogout_Loaded(object sender, RoutedEventArgs e)
        {
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "logout.png");
            imgLogout.Source = ImageEdit.CreateBitmapImage(path);
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.playerLogin = "none";
            MainWindow.playerPassword = "none";
            this.Close();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            ProfileWindow profile = new ProfileWindow(_hostUrl);
            profile.Owner = this;
            profile.ShowDialog();
            this.ShowDialog();
        }

        private void ImgProfile_Loaded(object sender, RoutedEventArgs e)
        {
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "profile.png");
            imgProfile.Source = ImageEdit.CreateBitmapImage(path);
        }
    }
}
