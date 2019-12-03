using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Shapes;
using JSStudyGame.Helpers;
using System.Net;
using JSStudyGame.ViewModels;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace JSStudyGame
{
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        private BitmapImage photoUser;
        private string _hostUrl;
        private PlayerVM playerBasicInfo;
        private PlayerAdditionalInfoVM playerAddInfo;
        private Random random;

        public RegistrationWindow(string hostUrl)
        {
            playerBasicInfo = new PlayerVM();
            playerAddInfo = new PlayerAdditionalInfoVM();
            random = new Random();
            _hostUrl = hostUrl;

            InitializeComponent();
        }

        private void BtnAddPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a picture";
            openFileDialog.Filter = "Image files (*.bmp;*jpg*;.jpeg*;.png)|*.bmp;*jpg*;.jpeg*;.png|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                photoUser = ImageEdit.CreateBitmapImageUserSize(openFileDialog.FileName, 1000, 1000);
                if (photoUser != null)
                    imgPhotoAndPicture.Source = photoUser;
            }
        }

        private void BtnSignUp_Click(object sender, RoutedEventArgs e)
        {
            if (txtPasswordOne.IsVisible)
                txtPasswordOneTxt.Text = txtPasswordOne.Password;
            else
                txtPasswordOne.Password = txtPasswordOneTxt.Text;

            if (txtPasswordTwo.IsVisible)
                txtPasswordTwoTxt.Text = txtPasswordTwo.Password;
            else
                txtPasswordTwo.Password = txtPasswordTwoTxt.Text;

            if (!CheckIfAllCorrect())
                return;

            playerBasicInfo.Id = 0;
            playerBasicInfo.Login = txtLogin.Text;
            playerBasicInfo.Email = txtEmail.Text;
            playerBasicInfo.Password = txtPasswordOne.Password;
            playerBasicInfo.IsAdmin = false;

            string url = _hostUrl + "/api/jsstudygame/player";
            playerBasicInfo.Id = ServerWorker.SendPlayerToServer(playerBasicInfo, url);

            if (playerBasicInfo.Id == 0)
            {
                MessageBox.Show("Adding error! Try again!");
                return;
            }

            if (playerBasicInfo.Id == -1 || playerBasicInfo.Id == -3)
            {
                lblLoginError.Content = "Sorry! This login is already in use!";
                txtLogin.IsReadOnly = false;
            }

            if (playerBasicInfo.Id == -2 || playerBasicInfo.Id == -3)
            {
                lblEmailError.Content = "Sorry! This email is already in use!";
                txtEmail.IsReadOnly = false;
            }

            if (playerBasicInfo.Id < 0)
                return;

            MainWindow.playerLogin = playerBasicInfo.Login;
            MainWindow.playerPassword = playerBasicInfo.Password;

            playerAddInfo.IdPlayerAdditionalInfo = playerBasicInfo.Id;
            playerAddInfo.Name = txtName.Text;
            playerAddInfo.Surname = txtSurname.Text;
            
            playerAddInfo.Photo = photoUser.ToBase64String();
            playerAddInfo.BirthDate = dateChoose.SelectedDate.Value;

            url = _hostUrl + "/api/jsstudygame/playeraddinfo";
            playerAddInfo.IdPlayerAdditionalInfo = ServerWorker.SendPlayerToServer(playerAddInfo, url);

            if (playerAddInfo.IdPlayerAdditionalInfo == 0)
                MessageBox.Show("Adding Additional Information error! Try again!");
            else
            {
                MessageBox.Show("Account was successfully registered!");
            }
        }

        private bool CheckIfAllCorrect()
        {
            bool result = true;

            // check name (can contain only latin characters, space, hyphen and apostrophe)  
            if (Regex.IsMatch(txtName.Text, @"^[a-zA-Z \-\']+$"))
            {
                lblNameError.Content = null;
                txtName.IsReadOnly = true;
            }
            else
            {
                result = false;
                lblNameError.Content = "use only latin characters";
            }

            // check surname (can contain only latin characters, space, hyphen and apostrophe)
            if (Regex.IsMatch(txtSurname.Text, @"^[a-zA-Z \-\']+$"))
            {
                lblSurnameError.Content = null;
                txtSurname.IsReadOnly = true;
            }
            else
            {
                result = false;
                lblSurnameError.Content = "use only latin characters";
            }

            // check login (can contain only latin characters, digits)
            if (Regex.IsMatch(txtLogin.Text, @"^[a-zA-Z0-9]+$"))
            {
                lblLoginError.Content = null;
                txtLogin.IsReadOnly = true;
            }
            else
            {
                result = false;
                lblLoginError.Content = "use only latin characters and digits";
            }

            // check passwordOne
            lblPasswordOneError.Content = null;
            if (!Regex.IsMatch(txtPasswordOne.Password, @"[a-z]+"))
            {
                lblPasswordOneError.Content = "must contain at least one lowercase";
                result = false;
            }
            else if (!Regex.IsMatch(txtPasswordOne.Password, @"[A-Z]+"))
            {
                lblPasswordOneError.Content = "must contain at least one uppercase";
                result = false;
            }
            else if (!Regex.IsMatch(txtPasswordOne.Password, @".{8,15}"))
            {
                lblPasswordOneError.Content = "mustn't be less 8 characters";
                result = false;
            }
            else if (!Regex.IsMatch(txtPasswordOne.Password, @"[0-9]+"))
            {
                lblPasswordOneError.Content = "must contain at least one numeric value";
                result = false;
            }
            else if (Regex.IsMatch(txtPasswordOne.Password, @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]"))
            {
                lblPasswordOneError.Content = @"shouldn't contain !@#$%^&*()_+=\[{\]};:<>|./?,-";
                result = false;
            }

            //check passwordTwo
            lblPasswordTwoError.Content = null;
            if (txtPasswordTwo.Password != txtPasswordOne.Password)
            {
                lblPasswordTwoError.Content = "enter the same password";
                result = false;
            }
            else
            {
                lblPasswordTwoError.Content = lblPasswordOneError.Content;
            }
            // check e-mail
            try
            {
                lblEmailError.Content = null;
                MailAddress email = new MailAddress(txtEmail.Text);
                txtEmail.IsReadOnly = true;
            }
            catch (Exception)
            {
                lblEmailError.Content = "incorect email";
                result = false;
            }

            // check birthdate
            lblBirthdateError.Content = null;
            if (dateChoose.SelectedDate == null)
            {
                lblBirthdateError.Content = "select date";
                result = false;
            }

            return result;
        }

        private void BtnShowPasswordOne_Click(object sender, RoutedEventArgs e)
        {
            if (txtPasswordOne.IsVisible)
            {
                txtPasswordOneTxt.Text = txtPasswordOne.Password;
                txtPasswordOne.Visibility = Visibility.Collapsed;
                txtPasswordOneTxt.Visibility = Visibility.Visible;
                btnShowPasswordOne.Content = "Hide";
            }
            else
            {
                txtPasswordOne.Password = txtPasswordOneTxt.Text;
                txtPasswordOneTxt.Visibility = Visibility.Collapsed;
                txtPasswordOne.Visibility = Visibility.Visible;
                btnShowPasswordOne.Content = "Show";
            }
        }

        private void BtnShowPasswordTwo_Click(object sender, RoutedEventArgs e)
        {
            if (txtPasswordTwo.IsVisible)
            {
                txtPasswordTwoTxt.Text = txtPasswordTwo.Password;
                txtPasswordTwo.Visibility = Visibility.Collapsed;
                txtPasswordTwoTxt.Visibility = Visibility.Visible;
                btnShowPasswordTwo.Content = "Hide";
            }
            else
            {
                txtPasswordTwo.Password = txtPasswordTwoTxt.Text;
                txtPasswordTwoTxt.Visibility = Visibility.Collapsed;
                txtPasswordTwo.Visibility = Visibility.Visible;
                btnShowPasswordTwo.Content = "Show";
            }
        }

        private void ImgQuestionMakr_Loaded(object sender, RoutedEventArgs e)
        {
            //string path = System.IO.Path.Combine(Environment.CurrentDirectory, "my_question_mark_icon.png");
            imgQuestionMakr.Source = ImageEdit.CreateBitmapImage(System.IO.Path.Combine(Helper.GetPathToSolution(), "Images", "my_question_mark_icon.png"));
        }

        private void ImgQuestionMakrTwo_Loaded(object sender, RoutedEventArgs e)
        {
            //string path = System.IO.Path.Combine(Environment.CurrentDirectory, "my_question_mark_icon.png");
            imgQuestionMakrTwo.Source = ImageEdit.CreateBitmapImage(System.IO.Path.Combine(Helper.GetPathToSolution(), "Images", "my_question_mark_icon.png"));
        }

        private void RadioBtnMale_Checked(object sender, RoutedEventArgs e)
        {
            if (playerAddInfo != null)
            {
                playerAddInfo.Gender = true;
            }
        }

        private void RadioBtnFemale_Checked(object sender, RoutedEventArgs e)
        {
            if (playerAddInfo != null)
            {
                playerAddInfo.Gender = false;
            }
        }

        private async void ImgPhotoAndPicture_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
                {
                    string imgPathStr = _hostUrl + $"/images/jsImg{random.Next(1, 20)}.jpg";
                    Uri resourceUri = new Uri(imgPathStr, UriKind.Absolute);
                    Dispatcher.Invoke(() =>
                    {
                        photoUser = new BitmapImage(resourceUri);
                        imgPhotoAndPicture.Source = photoUser;
                    });
                });
        }
    }
}
