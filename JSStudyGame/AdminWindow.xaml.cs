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


        private async void GetDataFromWebServer()
        {
            await Task.Run(() =>
            {
                try
                {
                    string requestUrl = _hostUrl + $"/api/jsstudygame/fullinfo?login={MainWindow.playerLogin}&password={MainWindow.playerPassword}&page={_activePage}";
                    _playersFullInfo = ServerWorker.GetInfoFromServer<ObservableCollection<PlayerFullInfo>>(requestUrl);
                    foreach (var item in _playersFullInfo)
                    {
                        item.Photo = _hostUrl + $"/photos/{item.Photo}";
                    }
                    Dispatcher.Invoke(new Action(() =>
                    {
                        dgShow.ItemsSource = _playersFullInfo;
                        MyPadding();
                    }));
                }
                catch (Exception)
                {
                    MessageBox.Show("Cannot connect to server!");
                }
            });
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

            //GetDataFromWebServer();
        }
    }
}
