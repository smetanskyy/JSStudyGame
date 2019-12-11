using JSStudyGame.Helpers;
using JSStudyGame.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
        private TestVM _test;
        private SectionVM _sectionVM;
        private string requestUrl;
        private bool _isAnswer = false;
        private bool _isGame = false;
        private List<string> _skippedAnswer;
        private List<string> _wrongAnswer;
        private int _amountoftests;
        private Stopwatch _stopwatch;
        public MainWindow(string hostUrl)
        {
            _hostUrl = hostUrl;
            InitializeComponent();

            requestUrl = _hostUrl + $"/api/jsstudygame/login?emailOrLogin={MainWindow.playerLogin}&password={MainWindow.playerPassword}";
            _player = ServerWorker.GetInfoFromServer<PlayerVM>(requestUrl);
            requestUrl = _hostUrl + $"/api/jsstudygame/addinfo?login={MainWindow.playerLogin}&password={MainWindow.playerPassword}";
            _playerAddInfo = ServerWorker.GetInfoFromServer<PlayerAdditionalInfoVM>(requestUrl);
            requestUrl = _hostUrl + $"/api/jsstudygame/score?login={MainWindow.playerLogin}&password={MainWindow.playerPassword}";
            _playerScore = ServerWorker.GetInfoFromServer<PlayerScoreVM>(requestUrl);
            _playerScore.IdPlayerScore = _player.Id;

            requestUrl = _hostUrl + $"/api/jsstudygame/amountoftests?login={MainWindow.playerLogin}&password={MainWindow.playerPassword}";
            _amountoftests = ServerWorker.GetInfoFromServer<int>(requestUrl);
            if (_amountoftests == 0)
                _amountoftests = 1;

            if (_player.IsAdmin)
                btnAdmin.Visibility = Visibility.Visible;

            spStartGame.Visibility = Visibility.Hidden;
            btnReference.Visibility = Visibility.Hidden;

            _skippedAnswer = new List<string>();
            _wrongAnswer = new List<string>();
            if (!string.IsNullOrWhiteSpace(_playerScore.AnswersSkipped))
            {
                _skippedAnswer = _playerScore.AnswersSkipped.Split(',').ToList();
                if (_skippedAnswer != null)
                    _skippedAnswer.Sort();
                foreach (var item in _skippedAnswer)
                {
                    cbSkipped.Items.Add($"<<  {item}  >>");
                }
            }
            if (!string.IsNullOrWhiteSpace(_playerScore.AnswersWrong))
            {
                _wrongAnswer = _playerScore.AnswersWrong.Split(',').ToList();
                if (_wrongAnswer != null)
                    _wrongAnswer.Sort();

                foreach (var item in _wrongAnswer)
                {
                    cbWrong.Items.Add($"<<  {item}  >>");
                }
            }
            _stopwatch = new Stopwatch();
            Task.Run(() =>
            {
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
            if (_isGame)
            {
                MessageBox.Show("At first you must end the game!");
                return;
            }
            MainWindow.playerLogin = "none";
            MainWindow.playerPassword = "none";
            _isGame = false;
            _player = null;
            _playerAddInfo = null;
            _playerScore = null;
            _isAnswer = false;
            requestUrl = _hostUrl + $"/api/jsstudygame/score";
            ServerWorker.ChangePlayerToServer(_playerScore, requestUrl);
            this.Close();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            if (_isGame)
            {
                MessageBox.Show("At first you must end the game!");
                return;
            }
            this.Hide();
            ProfileWindow profile = new ProfileWindow(_hostUrl);
            profile.Owner = this;
            profile.ShowDialog();
            requestUrl = _hostUrl + $"/api/jsstudygame/login?emailOrLogin={MainWindow.playerLogin}&password={MainWindow.playerPassword}";
            _player = ServerWorker.GetInfoFromServer<PlayerVM>(requestUrl);
            requestUrl = _hostUrl + $"/api/jsstudygame/addinfo?login={MainWindow.playerLogin}&password={MainWindow.playerPassword}";
            _playerAddInfo = ServerWorker.GetInfoFromServer<PlayerAdditionalInfoVM>(requestUrl);
            this.ShowDialog();
        }

        private void ImgProfile_Loaded(object sender, RoutedEventArgs e)
        {
            //string path = System.IO.Path.Combine(Environment.CurrentDirectory, "profile.png");
            imgProfile.Source = ImageEdit.CreateBitmapImage(System.IO.Path.Combine(Helper.GetPathToSolution(), "Images", "profile.png"));
        }

        private void BtnAnswer_Click(object sender, RoutedEventArgs e)
        {
            if(_isAnswer == true)
            {
                MessageBox.Show("You've answered this question!");
                return;
            }
            _isAnswer = true;
            string answer;
            Button button;
            if (sender is Button)
            {
                button = sender as Button;
                object content = button.Content;
                if (content is TextBlock)
                {
                    answer = (content as TextBlock).Text;
                }
                else
                    return;
            }
            else
                return;

            if (answer == _test.CorrectAnswer)
            {
                button.Background = Brushes.LightGreen;
                lblResult.Content = "You are right!";
                lblResult.Background = Brushes.LightSeaGreen;
                lblResult.Foreground = Brushes.DarkGreen;

                _playerScore.CorrectAnswers++;
                _playerScore.TotalScore += 10;

                if (_wrongAnswer.Contains(_test.Id.ToString()))
                {
                    cbWrong.SelectedIndex = 0;
                    cbWrong.Items.Remove($"<<  {_test.Id}  >>");
                    _wrongAnswer.Remove(_test.Id.ToString());
                    _playerScore.IncorrectAnswers--;
                }
                else if (_skippedAnswer.Contains(_test.Id.ToString()))
                {
                    cbSkipped.SelectedIndex = 0;
                    cbSkipped.Items.Remove($"<<  {_test.Id}  >>");
                    _skippedAnswer.Remove(_test.Id.ToString());
                    _playerScore.SkippedAnswers--;
                }
                else
                    _playerScore.CurrentQuestionNoAnswer++;
            }
            else
            {
                button.Background = Brushes.LightCoral;
                lblResult.Content = "You are wrong!";
                lblResult.Background = Brushes.LightCoral;
                lblResult.Foreground = Brushes.DarkRed;

                if (_wrongAnswer.Contains(_test.Id.ToString()))
                {
                    lblResult.Content = "You are a loser!";
                    cbWrong.SelectedIndex = 0;
                }
                else if (_skippedAnswer.Contains(_test.Id.ToString()))
                {
                    lblResult.Content = "You are a loser!";

                    cbSkipped.SelectedIndex = 0;
                    cbSkipped.Items.Remove($"<<  {_test.Id}  >>");
                    _skippedAnswer.Remove(_test.Id.ToString());
                    _playerScore.SkippedAnswers--;

                    _wrongAnswer.Add(_test.Id.ToString());
                    _wrongAnswer.Sort();
                    cbWrong.Items.Add($"<<  {_test.Id}  >>");

                    _playerScore.TotalScore -= 5;
                    _playerScore.IncorrectAnswers++;
                    _playerScore.CurrentQuestionNoAnswer++;
                }
                else
                {
                    _wrongAnswer.Add(_test.Id.ToString());
                    cbWrong.Items.Add($"<<  {_test.Id}  >>");
                    _playerScore.TotalScore -= 5;
                    _playerScore.IncorrectAnswers++;
                    _playerScore.CurrentQuestionNoAnswer++;
                }
            }
            SaveScore();
        }

        private void BtmStartGame_Click(object sender, RoutedEventArgs e)
        {
            cbWrong.SelectedItem = cbWrong.Items[0];
            cbSkipped.SelectedItem = cbSkipped.Items[0];

            int index = cbWrong.Items.Count;
            for (int i = index - 1; i > 0; i--)
            {
                if (i != 0)
                    cbWrong.Items.RemoveAt(i);
            }
            index = cbSkipped.Items.Count;
            for (int i = index - 1; i > 0; i--)
            {
                if (i != 0)
                    cbSkipped.Items.RemoveAt(i);
            }
            
            _playerScore.IdPlayerScore = _player.Id;
            _playerScore.TotalScore = 0;
            _playerScore.TimeGameInSeconds = 0;
            _playerScore.SkippedAnswers = 0;
            _playerScore.ProgressInGame = 0;
            _playerScore.IncorrectAnswers = 0;
            _playerScore.CurrentQuestionNoAnswer = 1;
            _playerScore.CorrectAnswers = 0;
            _playerScore.AnswersWrong = "";
            _playerScore.AnswersSkipped = "";

            requestUrl = _hostUrl + $"/api/jsstudygame/score";
            if (ServerWorker.ChangePlayerToServer(_playerScore, requestUrl) <= 0)
                BtmEndGame_Click(sender, e);

            if (_stopwatch.IsRunning)
                _stopwatch.Restart();
            else
                _stopwatch.Start();

            spStartGame.Visibility = Visibility.Visible;
            btnReference.Visibility = Visibility.Visible;
            btnNextQuestion.Visibility = Visibility.Visible;

            _isGame = true;
            if (!UpdateQuestion(_playerScore.CurrentQuestionNoAnswer))
                BtmEndGame_Click(sender, e);
        }

        private void BtmEndGame_Click(object sender, RoutedEventArgs e)
        {
            cbSkipped.SelectedIndex = 0;
            cbWrong.SelectedIndex = 0;
            if(_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = _stopwatch.Elapsed;
                int timeGame = (int)ts.TotalSeconds;
                _playerScore.TimeGameInSeconds += timeGame;
                SaveScore();
            }
            _isGame = false;
            spStartGame.Visibility = Visibility.Hidden;
            btnReference.Visibility = Visibility.Hidden;
        }

        private bool UpdateQuestion(int idTest)
        {
            _isAnswer = false;
            if (CheckIsWinOrEndQuestions(idTest))
                return false;
            spStartGame.Visibility = Visibility.Visible;
            btnReference.Visibility = Visibility.Visible;
            btnNextQuestion.Visibility = Visibility.Visible;

            btnAnswerA.Background = Brushes.LightGray;
            btnAnswerB.Background = Brushes.LightGray;
            btnAnswerC.Background = Brushes.LightGray;
            btnAnswerD.Background = Brushes.LightGray;

            lblResult.Content = null;
            lblResult.Background = null;
            lblResult.Foreground = null;

            requestUrl = _hostUrl + $"/api/jsstudygame/test?login={MainWindow.playerLogin}" +
                $"&password={MainWindow.playerPassword}&id={idTest}";
            _test = ServerWorker.GetInfoFromServer<TestVM>(requestUrl);
            if (_test == null)
                return false;

            txtbtnAnswerA.Text = null;
            txtbtnAnswerB.Text = null;
            txtbtnAnswerC.Text = null;
            txtbtnAnswerD.Text = null;

            Random random = new Random();
            switch (random.Next(4))
            {
                case 0:
                    txtQuestion.Text = _test.Question;
                    txtbtnAnswerA.Text = _test.CorrectAnswer;
                    txtbtnAnswerB.Text = _test.AnswerA;
                    txtbtnAnswerC.Text = _test.AnswerB;
                    txtbtnAnswerD.Text = _test.AnswerC;
                    break;
                case 1:
                    txtbtnAnswerA.Text = _test.AnswerA;
                    txtbtnAnswerB.Text = _test.CorrectAnswer;
                    txtbtnAnswerC.Text = _test.AnswerB;
                    txtbtnAnswerD.Text = _test.AnswerC;
                    break;
                case 2:
                    txtbtnAnswerA.Text = _test.AnswerA;
                    txtbtnAnswerB.Text = _test.AnswerB;
                    txtbtnAnswerC.Text = _test.CorrectAnswer;
                    txtbtnAnswerD.Text = _test.AnswerC;
                    break;
                case 3:
                    txtbtnAnswerA.Text = _test.AnswerA;
                    txtbtnAnswerB.Text = _test.AnswerB;
                    txtbtnAnswerC.Text = _test.AnswerC;
                    txtbtnAnswerD.Text = _test.CorrectAnswer;
                    break;
                default:
                    txtbtnAnswerA.Text = _test.AnswerA;
                    txtbtnAnswerB.Text = _test.AnswerB;
                    txtbtnAnswerC.Text = _test.AnswerC;
                    txtbtnAnswerD.Text = _test.CorrectAnswer;
                    break;
            }
            txtQuestion.Text = _test.Question;
            lblQuestionN.Content = _test.Id;

            try
            {
                requestUrl = _hostUrl + $"/api/jsstudygame/section?login={MainWindow.playerLogin}" +
                    $"&password={MainWindow.playerPassword}&id={_test.IdSection}";
                _sectionVM = ServerWorker.GetInfoFromServer<SectionVM>(requestUrl);

                if (_sectionVM != null)
                    lblSection.Content = _sectionVM.NameOFSection;
            }
            catch (Exception) { Debug.WriteLine("some mistake"); }
            return true;
        }

        private bool CheckIsWinOrEndQuestions(int idTest)
        {
            lblResultEnd.Content = null;
            lblResultEnd.Background = null;
            lblResultEnd.Foreground = null;

            if (_playerScore.CorrectAnswers == _amountoftests)
            {
                lblResultEnd.Content = "Congratulation! You are win!";
                lblResultEnd.Background = Brushes.LightSkyBlue;
                lblResultEnd.Foreground = Brushes.Yellow;

                spStartGame.Visibility = Visibility.Hidden;
                btnReference.Visibility = Visibility.Hidden;

                return true;
            }
            else if (idTest - 1 == _amountoftests)
            {
                lblResultEnd.Content = "We sorry! It was the last question!";
                lblResultEnd.Background = Brushes.LightGray;
                lblResultEnd.Foreground = Brushes.DarkGray;

                spStartGame.Visibility = Visibility.Hidden;
                btnReference.Visibility = Visibility.Hidden;

                return true;
            }
            else return false;
        }

        private void BtnReference_Click(object sender, RoutedEventArgs e)
        {
            if (_isGame)
            {
                ProcessStartInfo procInfo = new ProcessStartInfo();
                // исполняемый файл программы - браузер хром
                //procInfo.FileName = "C://Program Files (x86)//Google//Chrome//Application//chrome.exe";
                procInfo.FileName = "chrome.exe";
                // аргументы запуска - адрес интернет-ресурса
                procInfo.Arguments = _test.Reference;
                Process.Start(procInfo);
            }
        }

        private void BtnNextQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (_isAnswer == false)
            {
                _skippedAnswer.Add(_playerScore.CurrentQuestionNoAnswer.ToString());
                cbSkipped.Items.Add($"<<  {_playerScore.CurrentQuestionNoAnswer}  >>");
                _playerScore.CurrentQuestionNoAnswer++;
                _playerScore.SkippedAnswers++;
                SaveScore();
            }

            if (!UpdateQuestion(_playerScore.CurrentQuestionNoAnswer))
                BtmEndGame_Click(sender, e);
        }

        private void SaveScore()
        {
            _playerScore.AnswersSkipped = "";
            _playerScore.AnswersWrong = "";

            _playerScore.ProgressInGame = _playerScore.CorrectAnswers * 100 / _amountoftests;

            foreach (var item in _skippedAnswer)
            {
                if (string.IsNullOrWhiteSpace(_playerScore.AnswersSkipped))
                    _playerScore.AnswersSkipped += item;
                else
                    _playerScore.AnswersSkipped += $",{item}";
            }
            foreach (var item in _wrongAnswer)
            {
                if (string.IsNullOrWhiteSpace(_playerScore.AnswersWrong))
                    _playerScore.AnswersWrong += item;
                else
                    _playerScore.AnswersWrong += $",{item}";
            }

            requestUrl = _hostUrl + $"/api/jsstudygame/score";
            int result = ServerWorker.ChangePlayerToServer(_playerScore, requestUrl);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = _stopwatch.Elapsed;
                int timeGame = (int)ts.TotalSeconds;
                _playerScore.TimeGameInSeconds += timeGame;
                SaveScore();
            }
            spStartGame.Visibility = Visibility.Hidden;
            btnReference.Visibility = Visibility.Hidden;
            _isGame = false;
        }

        private void CbSkipped_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string idStr;
            try
            {
                idStr = (string)cbSkipped.SelectedItem;
            }
            catch (Exception)
            {
                return;
            }

            _isGame = true;
            spStartGame.Visibility = Visibility.Visible;
            btnReference.Visibility = Visibility.Visible;
            int idTest = 1;
            try
            {
                idStr = idStr.TrimStart(new char[] { '<', '<', ' ', ' ' });
                idStr = idStr.TrimEnd(new char[] { ' ', ' ', '>', '>' });
                idTest = int.Parse(idStr);
            }
            catch (Exception) { Debug.WriteLine("Something wrong"); }

            if (_stopwatch.IsRunning)
                _stopwatch.Restart();
            else
                _stopwatch.Start();

            UpdateQuestion(idTest);
            btnNextQuestion.Visibility = Visibility.Hidden;
        }

        private void CbWrong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string idStr;
            try
            {
                idStr = (string)cbWrong.SelectedItem;
            }
            catch (Exception)
            {
                return;
            }

            _isGame = true;
            spStartGame.Visibility = Visibility.Visible;
            btnReference.Visibility = Visibility.Visible;
            int idTest = 1;
            try
            {
                idStr = idStr.TrimStart(new char[] { '<', '<', ' ', ' ' });
                idStr = idStr.TrimEnd(new char[] { ' ', ' ', '>', '>' });
                idTest = int.Parse(idStr);
            }
            catch (Exception) { Debug.WriteLine("Something wrong"); }

            if (_stopwatch.IsRunning)
                _stopwatch.Restart();
            else
                _stopwatch.Start();

            UpdateQuestion(idTest);
            btnNextQuestion.Visibility = Visibility.Hidden;
        }

        private void BtnContinueGame_Click(object sender, RoutedEventArgs e)
        {
            cbWrong.SelectedItem = cbWrong.Items[0];
            cbSkipped.SelectedItem = cbSkipped.Items[0];

            int idTest = _playerScore.CurrentQuestionNoAnswer;
            if (idTest <= 0)
            {
                idTest = 1;
                _playerScore.CurrentQuestionNoAnswer = 1;
            }

            if (_stopwatch.IsRunning)
                _stopwatch.Restart();
            else
                _stopwatch.Start();

            _isGame = true;
            if (!UpdateQuestion(idTest))
                BtmEndGame_Click(sender, e);
        }

        private void BtnAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (_isGame)
            {
                MessageBox.Show("At first you must end the game!");
                return;
            }
            this.Hide();
            AdminWindow admin = new AdminWindow(_hostUrl);
            admin.Owner = this;
            admin.ShowDialog();
            requestUrl = _hostUrl + $"/api/jsstudygame/login?emailOrLogin={MainWindow.playerLogin}&password={MainWindow.playerPassword}";
            _player = ServerWorker.GetInfoFromServer<PlayerVM>(requestUrl);
            requestUrl = _hostUrl + $"/api/jsstudygame/addinfo?login={MainWindow.playerLogin}&password={MainWindow.playerPassword}";
            _playerAddInfo = ServerWorker.GetInfoFromServer<PlayerAdditionalInfoVM>(requestUrl);
            this.ShowDialog();
        }

        private void ImgAdmib_Loaded(object sender, RoutedEventArgs e)
        {
            imgAdmin.Source = ImageEdit.CreateBitmapImage(System.IO.Path.Combine(Helper.GetPathToSolution(), "Images", "admin.png"));
        }
    }
}