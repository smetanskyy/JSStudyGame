using JSStudyGame.Helpers;
using JSStudyGame.ViewModels;
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
        private PlayerVM _player;
        private PlayerAdditionalInfoVM _playerAddInfo;
        private PlayerScoreVM _playerScore;
        private bool isGame = false;
        private TestVM _test;
        private SectionVM _sectionVM;
        private int _currentTest = 1;
        public MainWindow(string hostUrl)
        {
            _hostUrl = hostUrl;
            InitializeComponent();
            string requestUrl = _hostUrl + $"/api/jsstudygame/login?emailOrLogin={MainWindow.playerLogin}&password={MainWindow.playerPassword}";
            _player = ServerWorker.GetInfoFromServer<PlayerVM>(requestUrl);
            requestUrl = _hostUrl + $"/api/jsstudygame/addinfo?login={MainWindow.playerLogin}&password={MainWindow.playerPassword}";
            _playerAddInfo = ServerWorker.GetInfoFromServer<PlayerAdditionalInfoVM>(requestUrl);
            requestUrl = _hostUrl + $"/api/jsstudygame/score?login={MainWindow.playerLogin}&password={MainWindow.playerPassword}";
            _playerScore = ServerWorker.GetInfoFromServer<PlayerScoreVM>(requestUrl);

            Task.Run(() =>
            {
                Random random = new Random();
                string imgPathStr = _hostUrl + $"/images/jsImg2.jpg";
                Uri resourceUri = new Uri(imgPathStr, UriKind.Absolute);
                this.Dispatcher.Invoke(() =>
                {
                    backgroundImg.ImageSource = new BitmapImage(resourceUri);
                });
            });
        }

        private void ImgLogout_Loaded(object sender, RoutedEventArgs e)
        {
            //string path = System.IO.Path.Combine(Environment.CurrentDirectory, "logout.png");
            imgLogout.Source = ImageEdit.CreateBitmapImage(System.IO.Path.Combine(Helper.GetPathToSolution(), "Images", "logout.png"));
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.playerLogin = "none";
            MainWindow.playerPassword = "none";
            isGame = false;
            _player = null;
            _playerAddInfo = null;
            _playerScore = null;
            this.Close();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            if(isGame)
            {
                MessageBox.Show("At first you must end the game!");
                return;
            }
            this.Hide();
            ProfileWindow profile = new ProfileWindow(_hostUrl);
            profile.Owner = this;
            profile.ShowDialog();
            string requestUrl = _hostUrl + $"/api/jsstudygame/login?emailOrLogin={MainWindow.playerLogin}&password={MainWindow.playerPassword}";
            _player = ServerWorker.GetInfoFromServer<PlayerVM>(requestUrl);
            requestUrl = _hostUrl + $"/api/jsstudygame/addinfo?login={MainWindow.playerLogin}&password={MainWindow.playerPassword}";
            _playerAddInfo = ServerWorker.GetInfoFromServer<PlayerAdditionalInfoVM>(requestUrl);
            requestUrl = _hostUrl + $"/api/jsstudygame/score?login={MainWindow.playerLogin}&password={MainWindow.playerPassword}";
            _playerScore = ServerWorker.GetInfoFromServer<PlayerScoreVM>(requestUrl);
            this.ShowDialog();
        }

        private void ImgProfile_Loaded(object sender, RoutedEventArgs e)
        {
            //string path = System.IO.Path.Combine(Environment.CurrentDirectory, "profile.png");
            imgProfile.Source = ImageEdit.CreateBitmapImage(System.IO.Path.Combine(Helper.GetPathToSolution(), "Images", "profile.png"));
        }

        private void BtnAnswer_Click(object sender, RoutedEventArgs e)
        {
            if((string)((Button)sender).Content == _test.CorrectAnswer)
            {
                MessageBox.Show("You are right!");
            }
            ++_currentTest;
            if (!UpdateQuestion())
                BtmEndGame_Click(sender, e);
        }

        private void BtmStartGame_Click(object sender, RoutedEventArgs e)
        {
            spStartGame.Visibility = Visibility.Visible;
            isGame = true;
            if (!UpdateQuestion())
                BtmEndGame_Click(sender, e);
        }

        private void BtmEndGame_Click(object sender, RoutedEventArgs e)
        {
            spStartGame.Visibility = Visibility.Hidden;
            isGame = false;
        }

        private bool UpdateQuestion()
        {
            string requestUrl = _hostUrl + $"/api/jsstudygame/test?login={MainWindow.playerLogin}" +
                $"&password={MainWindow.playerPassword}&id={_currentTest}";
            _test = ServerWorker.GetInfoFromServer<TestVM>(requestUrl);
            if (_test == null)
                return false;
            txtQuestion.Text = _test.Question;
            btnAnswerA.Content = _test.AnswerA;
            btnAnswerB.Content = _test.AnswerB;
            btnAnswerC.Content = _test.AnswerC;
            btnAnswerD.Content = _test.CorrectAnswer;
            lblQuestionN.Content = _test.Id;
            return true;
        }
    }
}
