using JSStudyGame.Helpers;
using JSStudyGame.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JSStudyGame
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        //private readonly string hostUrl = "https://myappazurebystepan.azurewebsites.net/";
        //private readonly string hostUrl = "http://localhost:49979";
        private readonly string _hostUrl;
        private MainWindow main;

        public LoginWindow()
        {
            _hostUrl = ConfigurationManager.AppSettings["urlName"];
            _hostUrl = "http://localhost:49979";
            main = null;
            InitializeComponent();

        }
        
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string requestUrl = null;
                requestUrl = _hostUrl + $"/api/jsstudygame/login?emailOrLogin={txtLogin.Text}&password={txtPassword.Text}";
                PlayerVM playerVM = ServerWorker.GetInfoFromServer<PlayerVM>(requestUrl);

                if (playerVM == null)
                    MessageBox.Show("Enter correct data!", "Error");
                else if (playerVM.Login == "none" && playerVM.Password == "none")
                {
                    MessageBox.Show("Incorrect data! Try again!");
                    return;
                }
                else
                {
                    Hide();
                    MainWindow.playerLogin = playerVM.Login;
                    MainWindow.playerPassword = playerVM.Password;
                    main = new MainWindow(_hostUrl);
                    main.Owner = this;
                    main.ShowDialog();
                    txtLogin.Text = $"{MainWindow.playerLogin}";
                    txtPassword.Text = $"{MainWindow.playerPassword}";
                    Show();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot connect to server!");
            }
        }

        private void BtnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            RegistrationWindow registration = new RegistrationWindow(_hostUrl);
            registration.Owner = this;
            registration.ShowDialog();
            if(MainWindow.playerLogin != "none" && MainWindow.playerPassword != "none")
            {
                txtLogin.Text = MainWindow.playerLogin;
                txtPassword.Text = MainWindow.playerPassword;
            }
            Show();
        }

        private async void ImgBox_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                Random random = new Random();
                string imgPathStr = _hostUrl + $"/images/jsImg{random.Next(1, 20)}.jpg";
                Uri resourceUri = new Uri(imgPathStr, UriKind.Absolute);
                this.Dispatcher.Invoke(() =>
                {
                    imgBox.Source = new BitmapImage(resourceUri);
                });
            });
        }
    }
}
