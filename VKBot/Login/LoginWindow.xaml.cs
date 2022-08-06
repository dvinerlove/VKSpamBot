using NHttp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace VKBot
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : UserControl
    {
        public VkApi Api { get; set; } = new VkApi();
        public event EventHandler? Login;

        ulong _clientID = Properties.Settings1.Default.ClientID;
        string _redirectUri = "blank.html";
        string _scopes =
        "photos,wall,groups,pages,status,friends,offline,notifications,stats,email,market,notify";
        private HttpServer? _webServer;
        private string _accessToken = "";

        public LoginWindow()
        {
            InitializeComponent();
            App.Current.MainWindow.Closing += (s, e) =>
            {
                if (_webServer != null)
                {
                    _webServer.Stop();
                }
            };
        }

        public void Start()
        {
            if (string.IsNullOrEmpty(Properties.Settings1.Default.AccessToken) || Properties.Settings1.Default.UserID == 0)
            {
                Run();
            }
            else
            {
                if (Auth(Properties.Settings1.Default.AccessToken) == false)
                {
                    Run();
                }
                else
                {
                    Login?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void Run()
        {
            Stack.Visibility = Visibility.Visible;
            InitializeWebServer();
            var authUrl = $"https://oauth.vk.com/authorize?client_id={_clientID}&display=page&redirect_uri={_redirectUri}&scope={_scopes}&response_type=token&v=5.131&state=123456";
            Debug.WriteLine(authUrl);
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = authUrl,
                UseShellExecute = true
            });
        }

        private void InitializeWebServer()
        {
            if (_webServer != null)
            {
                return;
            }

            //_webServer = new HttpServer();
            //_webServer.EndPoint = new IPEndPoint(IPAddress.Loopback, 8386);

            //_webServer.RequestReceived += (s, e) =>
            //{
            //    using (var writer = new StreamWriter(e.Response.OutputStream))
            //    {
            //        Debug.WriteLine(e.Response.StatusCode);
            //        Debug.WriteLine(e.Response.StatusDescription);

            //        foreach (var item in e.Request.Headers)
            //        {
            //            //Debug.WriteLine(item);
            //        }
            //        foreach (var item in e.Request.Params)
            //        {
            //            //Debug.WriteLine(item);
            //        }
            //        foreach (var item in e.Request.RawUrl)
            //        {
            //            Debug.WriteLine(item);
            //        }
            //        Debug.WriteLine(e.Request.Url);
            //        Debug.WriteLine(e.Request.UrlReferer);
            //        Debug.WriteLine(e.Context.Request.Url);
            //        Debug.WriteLine(e.Context.Request.UrlReferer);
            //        Debug.WriteLine(e.Request.ContentLength);

            //        foreach (var item in e.Request.Cookies.AllKeys)
            //        {
            //            Debug.WriteLine(item);
            //        }
            //        foreach (var item in e.Context.Request.Cookies.AllKeys)
            //        {
            //            Debug.WriteLine(item);
            //        }
            //        if (e.Request.QueryString.AllKeys.Any("code".Contains))
            //        {
            //            Dispatcher.Invoke(() =>
            //            {

            //                App.Current.MainWindow.WindowState = WindowState.Minimized;
            //                App.Current.MainWindow.WindowState = WindowState.Normal;
            //                App.Current.MainWindow.Topmost = true;
            //                this.Focus();
            //                App.Current.MainWindow.Topmost = false;
            //            });
            //            _accessToken = e.Request.QueryString["access_token"];
            //            if (string.IsNullOrEmpty(_accessToken) == false)
            //            {
            //                Auth(_accessToken);
            //            }
            //        }
            //    }

            //};
            //_webServer.Start();
        }

        public bool Auth(string _accessToken)
        {
            Api = new VkApi();

            try
            {
                Api.Authorize(new ApiAuthParams
                {
                    ApplicationId = _clientID,
                    Settings = VkNet.Enums.Filters.Settings.All,
                    AccessToken = _accessToken
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return Api.IsAuthorized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(AccessToken.Text))
            {
                return;
            }

            Properties.Settings1.Default.AccessToken = AccessToken.Text;
            Properties.Settings1.Default.UserID = long.Parse(UserID.Text);
            Properties.Settings1.Default.Save();

            if (Auth(Properties.Settings1.Default.AccessToken))
            {
                Login?.Invoke(this, EventArgs.Empty);
            }


        }
    }
}
