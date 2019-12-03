using JSStudyGame.Helpers;
using JSStudyGame.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        private string _hostUrl;
        private BitmapImage photoUser;
        private PlayerVM playerVM;
        private PlayerAdditionalInfoVM playerAddInfoVM;
        public ProfileWindow(string hostUrl)
        {
            _hostUrl = hostUrl;
            playerVM = new PlayerVM();
            playerAddInfoVM = new PlayerAdditionalInfoVM();
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string requestUrl = null;
            requestUrl = _hostUrl + $"/api/jsstudygame/login?emailOrLogin={MainWindow.playerLogin}&password={MainWindow.playerPassword}";
            playerVM = ServerWorker.GetInfoFromServer<PlayerVM>(requestUrl);
            txtLogin.Text = playerVM.Login;
            txtLogin.IsReadOnly = true;
            txtEmail.Text = playerVM.Email;
            txtEmail.IsReadOnly = true;
            txtEmail.Text = playerVM.Email;
            txtPasswordOne.Password = playerVM.Password;
            txtPasswordOne.IsEnabled = false;
            txtPasswordOneTxt.Text = playerVM.Password;
            txtPasswordOneTxt.IsReadOnly = true;

            txtPasswordTwo.Password = playerVM.Password;
            txtPasswordTwoTxt.Text = playerVM.Password;
            txtPasswordTwoTxt.IsReadOnly = true;

            requestUrl = _hostUrl + $"/api/jsstudygame/addinfo?login={MainWindow.playerLogin}&password={MainWindow.playerPassword}";
            playerAddInfoVM = ServerWorker.GetInfoFromServer<PlayerAdditionalInfoVM>(requestUrl);

            txtName.Text = playerAddInfoVM.Name;
            txtName.IsReadOnly = true;
            txtSurname.Text = playerAddInfoVM.Surname;
            txtSurname.IsReadOnly = true;

            dateChoose.SelectedDate = playerAddInfoVM.BirthDate;
            if (playerAddInfoVM.Gender == true)
            {
                radioBtnMale.IsChecked = true;
                radioBtnFemale.Visibility = Visibility.Hidden;
            }
            else
            {
                radioBtnFemale.IsChecked = true;
                radioBtnMale.Visibility = Visibility.Hidden;
            }

            ImgPhotoAndPicture_Loaded(sender, e);
        }

        private void RadioBtnMale_Checked(object sender, RoutedEventArgs e)
        {
            if (playerAddInfoVM != null)
            {
                playerAddInfoVM.Gender = true;
            }
        }

        private void RadioBtnFemale_Checked(object sender, RoutedEventArgs e)
        {
            if (playerAddInfoVM != null)
            {
                playerAddInfoVM.Gender = false;
            }
        }

        private void ImgQuestionMark_Loaded(object sender, RoutedEventArgs e)
        {
            //string path = System.IO.Path.Combine(Environment.CurrentDirectory, "my_question_mark_icon.png");
            imgQuestionMark.Source = ImageEdit.CreateBitmapImage(System.IO.Path.Combine(Helper.GetPathToSolution(), "Images", "my_question_mark_icon.png"));
        }

        private void BtnShowPasswordOne_Click(object sender, RoutedEventArgs e)
        {
            if (txtPasswordOne.IsVisible)
            {
                if (txtPasswordOneTxt.IsReadOnly == false)
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

        private void ImgQuestionMarkTwo_Loaded(object sender, RoutedEventArgs e)
        {
            //string path = System.IO.Path.Combine(Environment.CurrentDirectory, "my_question_mark_icon.png"); 
            imgQuestionMarkTwo.Source = ImageEdit.CreateBitmapImage(System.IO.Path.Combine(Helper.GetPathToSolution(), "Images", "my_question_mark_icon.png"));
        }

        private void BtnShowPasswordTwo_Click(object sender, RoutedEventArgs e)
        {
            if (txtPasswordTwo.IsVisible)
            {
                if (txtPasswordTwoTxt.IsReadOnly == false)
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

        private void ImgPhotoAndPicture_Loaded(object sender, RoutedEventArgs e)
        {
            string imgPathStr = null;

            if (string.IsNullOrEmpty(playerAddInfoVM.Photo) || playerAddInfoVM.Photo == "none")
            {
                Random random = new Random();
                imgPathStr = _hostUrl + $"/images/jsImg{random.Next(1, 20)}.jpg";
                Uri resourceUri = new Uri(imgPathStr, UriKind.Absolute);
                photoUser = new BitmapImage(resourceUri);
                imgPhotoAndPicture.Source = photoUser;
            }
            else
            {
                imgPathStr = _hostUrl + $"/photos/{playerAddInfoVM.Photo}";
                Uri resourceUri = new Uri(imgPathStr, UriKind.Absolute);
                photoUser = new BitmapImage(resourceUri);
                imgPhotoAndPicture.Source = photoUser;
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

        private void BtnChangePhoto_Click(object sender, RoutedEventArgs e)
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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
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

            playerVM.Login = txtLogin.Text;
            playerVM.Email = txtEmail.Text;
            playerVM.Password = txtPasswordOne.Password;
            
            string url = _hostUrl + "/api/jsstudygame/player";
            int mistake = ServerWorker.ChangePlayerToServer(playerVM, url);

            if (mistake == 0)
            {
                MessageBox.Show("Adding error! Try again!");
                this.Close();
            }

            if (mistake == -1 || mistake == -3)
            {
                lblLoginError.Content = "Sorry! This login is already in use!";
                txtLogin.IsReadOnly = false;
            }

            if (mistake == -2 || mistake == -3)
            {
                lblEmailError.Content = "Sorry! This email is already in use!";
                txtEmail.IsReadOnly = false;
            }

            if (mistake < 0)
                return;

            MainWindow.playerLogin = playerVM.Login;
            MainWindow.playerPassword = playerVM.Password;

            playerAddInfoVM.IdPlayerAdditionalInfo = playerVM.Id;
            playerAddInfoVM.Name = txtName.Text;
            playerAddInfoVM.Surname = txtSurname.Text;

            playerAddInfoVM.Photo = photoUser.ToBase64String();
            playerAddInfoVM.BirthDate = dateChoose.SelectedDate.Value;

            url = _hostUrl + "/api/jsstudygame/playeraddinfo";
            mistake = ServerWorker.ChangePlayerToServer(playerAddInfoVM, url);

            if (mistake == 0)
            {
                MessageBox.Show("Changing Additional Information error! Try again!");
                this.Close();
            }
            else
            {
                wpPasswordTwo.Visibility = Visibility.Collapsed;
                wpPasswordTwoTitle.Visibility = Visibility.Collapsed;
                btnChange.Visibility = Visibility.Visible;
                btnChangePhoto.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Hidden;
                Window_Loaded(sender, e);
                MessageBox.Show("Account was successfully changed!");
            }
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            txtLogin.IsReadOnly = false;
            txtEmail.IsReadOnly = false;
            txtPasswordOne.IsEnabled = true;
            txtPasswordOneTxt.IsReadOnly = false;
            txtPasswordTwoTxt.IsReadOnly = false;
            txtName.IsReadOnly = false;
            txtSurname.IsReadOnly = false;
            radioBtnFemale.Visibility = Visibility.Visible;
            radioBtnMale.Visibility = Visibility.Visible;
            wpPasswordTwo.Visibility = Visibility.Visible;
            wpPasswordTwoTitle.Visibility = Visibility.Visible;
            btnChange.Visibility = Visibility.Collapsed;
            btnChangePhoto.Visibility = Visibility.Visible;
            btnSave.Visibility = Visibility.Visible;
        }
    }
}
