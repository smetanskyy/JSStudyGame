using JSStudyGame.Helpers;
using JSStudyGame.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace JSStudyGame
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private ObservableCollection<PlayerFullInfo> _playersFullInfo = new ObservableCollection<PlayerFullInfo>();
        private readonly string _hostUrl;
        public int TotalPlayers { get; set; }
        private int _activePage;
        private int _pages;

        public AdminWindow(string hostUrl)
        {
            _hostUrl = hostUrl;
            InitializeComponent();
            _activePage = 1;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetDataFromWebServer();
            string requestUrl = _hostUrl + $"/api/jsstudygame/amountofplayers?login={MainWindow.playerLogin}&password={MainWindow.playerPassword}";
            TotalPlayers = ServerWorker.GetInfoFromServer<int>(requestUrl);
            if (TotalPlayers == 0)
                TotalPlayers = 1;
        }

        private void GetDataFromWebServer()
        {

            try
            {
                string requestUrl = _hostUrl + $"/api/jsstudygame/fullinfo?login={MainWindow.playerLogin}&password={MainWindow.playerPassword}&page={_activePage}";
                _playersFullInfo = ServerWorker.GetInfoFromServer<ObservableCollection<PlayerFullInfo>>(requestUrl);
                foreach (var item in _playersFullInfo)
                {
                    item.Photo = _hostUrl + $"/photos/{item.Photo}";
                }

                dgShow.ItemsSource = _playersFullInfo;
                MyPadding();

            }
            catch (Exception)
            {
                MessageBox.Show("Cannot connect to server!");
            }

        }


        private void MyPadding()
        {
            wpWithDGV.Children.Clear();
            int numberOfObjectsPerPage = 5;
            _pages = (int)Math.Ceiling((double)TotalPlayers / (double)numberOfObjectsPerPage);

            var sizeButton = new Size { Width = 50, Height = 25 };

            var colorActive = Brushes.Blue;
            var previousButton = new Button()
            {
                Content = $"previous",
                Width = sizeButton.Width,
                Height = sizeButton.Height,
                Background = Brushes.Gray,
                Margin = new Thickness(10, 0, 10, 0)
            };
            previousButton.Click += Handler_Page_Click;

            wpWithDGV.Children.Add(previousButton);

            for (int i = 0; i < _pages; i++)
            {
                if (i == _activePage - 3 && _activePage >= 6 && _pages >= 6)
                {
                    var label = new Label()
                    {
                        Content = " . . . ",
                        Width = sizeButton.Width,
                        HorizontalContentAlignment = HorizontalAlignment.Center
                    };
                    wpWithDGV.Children.Add(label);
                }
                else if (i < 3)
                {
                    var b = new Button()
                    {
                        Content = $"{i + 1}",
                        Width = sizeButton.Width,
                        Height = sizeButton.Height,
                        Background = i == _activePage - 1 ? colorActive : Brushes.WhiteSmoke
                    };
                    b.Click += Handler_Page_Click;
                    wpWithDGV.Children.Add(b);
                }
                else if (i <= _activePage && (_activePage > 2 && _activePage < 7))
                {
                    var b = new Button()
                    {
                        Content = $"{i + 1}",
                        Width = sizeButton.Width,
                        Height = sizeButton.Height,
                        Background = i == _activePage - 1 ? colorActive : Brushes.WhiteSmoke
                    };
                    b.Click += Handler_Page_Click;
                    wpWithDGV.Children.Add(b);
                }

                else if (i == _pages - 2 && _activePage <= _pages - 3)
                {
                    var l = new Label()
                    {
                        Content = " . . . ",
                        Width = sizeButton.Width,
                        HorizontalContentAlignment = HorizontalAlignment.Center
                    };
                    wpWithDGV.Children.Add(l);
                }
                else if (i >= _activePage - 2 && i <= _activePage)
                {
                    var b = new Button()
                    {
                        Content = $"{i + 1}",
                        Width = sizeButton.Width,
                        Height = sizeButton.Height,
                        Background = i == _activePage - 1 ? colorActive : Brushes.WhiteSmoke
                    };
                    b.Click += Handler_Page_Click;
                    wpWithDGV.Children.Add(b);
                }
                else if (i == _pages - 1)
                {
                    var b = new Button()
                    {
                        Content = $"{i + 1}",
                        Width = sizeButton.Width,
                        Height = sizeButton.Height,
                        Background = i == _activePage - 1 ? colorActive : Brushes.WhiteSmoke
                    };
                    b.Click += Handler_Page_Click;
                    wpWithDGV.Children.Add(b);
                }

                if (i == _pages - 1)
                {
                    var nextButton = new Button()
                    {
                        Content = "next",
                        Width = sizeButton.Width,
                        Height = sizeButton.Height,
                        Background = Brushes.Gray,
                        Margin = new Thickness(10, 0, 0, 0)
                    };
                    nextButton.Click += Handler_Page_Click;
                    wpWithDGV.Children.Add(nextButton);
                }
            }
        }

        private void Handler_Page_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;

            if (btn.Content.ToString() == "previous")
                _activePage = _activePage == 1 ? 1 : --_activePage;
            else if (btn.Content.ToString() == "next")
                _activePage = _activePage == _pages ? _pages : ++_activePage;
            else
                _activePage = Convert.ToInt32(btn.Content.ToString());

            GetDataFromWebServer();
        }

        private void CbIsAdmin_Checked(object sender, RoutedEventArgs e)
        {
            if (!(dgShow.SelectedItem is PlayerFullInfo))
            {
                dgShow.SelectedItem = null;
                return;
            }

            PlayerFullInfo playerFull = dgShow.SelectedItem as PlayerFullInfo;
            PlayerVM player = new PlayerVM()
            {
                Id = playerFull.Id,
                Login = playerFull.Login,
                Email = playerFull.Email,
                Password = playerFull.Password,
                IsAdmin = ((CheckBox)sender).IsChecked == true ? true : false
            };

            string url = _hostUrl + "/api/jsstudygame/player";
            int mistake = ServerWorker.ChangePlayerToServer(player, url);

            if (mistake <= 0)
            {
                MessageBox.Show("Adding error! Try again!");
                return;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgShow.SelectedItem is PlayerFullInfo))
            {
                dgShow.SelectedItem = null;
                return;
            }
            string login = MainWindow.playerLogin;
            string password = MainWindow.playerPassword;
            this.Hide();

            RegistrationWindow registration = new RegistrationWindow(_hostUrl);
            registration.Owner = this;
            registration.ShowDialog();

            MainWindow.playerLogin = login;
            MainWindow.playerPassword = password;
            Window_Loaded(sender, e);
            this.ShowDialog();
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgShow.SelectedItem is PlayerFullInfo))
            {
                dgShow.SelectedItem = null;
                return;
            }
            string login = MainWindow.playerLogin;
            string password = MainWindow.playerPassword;

            MainWindow.playerLogin = ((PlayerFullInfo)dgShow.SelectedItem).Login;
            MainWindow.playerPassword = ((PlayerFullInfo)dgShow.SelectedItem).Password;

            this.Hide();
            ProfileWindow profile = new ProfileWindow(_hostUrl);
            profile.Owner = this;
            profile.ShowDialog();

            var currentPlayer = _playersFullInfo.SingleOrDefault(p => p.Login == MainWindow.playerLogin && p.Password == MainWindow.playerPassword);

            var requestUrl = _hostUrl + $"/api/jsstudygame/login?emailOrLogin={MainWindow.playerLogin}&password={MainWindow.playerPassword}";
            var player = ServerWorker.GetInfoFromServer<PlayerVM>(requestUrl);
            requestUrl = _hostUrl + $"/api/jsstudygame/addinfo?login={MainWindow.playerLogin}&password={MainWindow.playerPassword}";
            var playerAddInfo = ServerWorker.GetInfoFromServer<PlayerAdditionalInfoVM>(requestUrl);

            currentPlayer.Id = player.Id;
            currentPlayer.Password = player.Password;
            currentPlayer.Email = player.Email;
            currentPlayer.Login = player.Login;
            currentPlayer.IsAdmin = player.IsAdmin;
            currentPlayer.Name = playerAddInfo.Name;
            currentPlayer.Surname = playerAddInfo.Surname;
            currentPlayer.Photo = _hostUrl + $"/photos/{playerAddInfo.Photo}";
            currentPlayer.BirthDate = playerAddInfo.BirthDate;
            currentPlayer.Gender = playerAddInfo.Gender == true ? "male" : "female";

            MainWindow.playerLogin = login;
            MainWindow.playerPassword = password;

            this.ShowDialog();
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgShow.SelectedItem is PlayerFullInfo))
            {
                dgShow.SelectedItem = null;
                return;
            }
            PlayerFullInfo playerFull = dgShow.SelectedItem as PlayerFullInfo;
            dgShow.SelectedItem = null;

            string url = _hostUrl + $"/api/jsstudygame/player?login={playerFull.Login}&password={playerFull.Password}";
            if (ServerWorker.DeletePlayer(url) == true)
            {
                if (playerFull.Login == MainWindow.playerLogin && playerFull.Password == MainWindow.playerPassword)
                {
                    MainWindow.playerLogin = "none";
                    MainWindow.playerPassword = "none";
                    this.Close();
                    return;
                }
                else
                    Window_Loaded(sender, e);
            }
        }

        private void BtnShowAll_Click(object sender, RoutedEventArgs e)
        {
            Window_Loaded(sender, e);
        }
    }
}
