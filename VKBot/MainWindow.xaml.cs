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
using VKBot.GroupsSettings;
using VKBot.PostSettings;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Wpf.Ui.Controls;
using Wpf.Ui.Appearance;

namespace VKBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            OpenLoginWindow();
            Wpf.Ui.Appearance.Theme.Apply(ThemeType.Dark, BackgroundType.Mica);
            Card1.ClipToBounds = true;
        }

        private void LoginWindow_Login(object? sender, EventArgs e)
        {
            var api = (sender as LoginWindow)!.Api;

            LoginGrid.Children.Clear();

            ScrollViewer postSettings = new ScrollViewer();
            postSettings.Content = new PostSettingsPage(api);

            ScrollViewer textSettings = new ScrollViewer();
            textSettings.Content = new TextSettingsPage(api);

            ScrollViewer groupsSettings = new ScrollViewer();
            groupsSettings.Content = new GroupsSettings.GroupsSettings(api);

            ScrollViewer publish = new ScrollViewer();
            publish.Content = new Publish.PublishPage(api);

            ScrollViewer logout = new ScrollViewer();

            var logoutPage = new Logout.LogoutPage(api);
            logoutPage.Logout += Logout_Logout;
            logout.Content = logoutPage;

            Tabs.Items.Add(new TabItem() { Height = 0, Content = postSettings, Header = "Images" });
            Tabs.Items.Add(new TabItem() { Height = 0, Content = textSettings, Header = "Posts" });
            Tabs.Items.Add(new TabItem() { Height = 0, Content = groupsSettings, Header = "Groups" });
            Tabs.Items.Add(new TabItem() { Height = 0, Content = publish, Header = "Publish" });
            Tabs.Items.Add(new TabItem() { Height = 0, Content = logout, Header = "Logout" });

            MainGrid.Visibility = Visibility.Visible;
            LoginGrid.Visibility = Visibility.Collapsed;
        }

        private void Logout_Logout(object? sender, EventArgs e)
        {
            OpenLoginWindow();
        }

        private void OpenLoginWindow()
        {

            MainGrid.Visibility = Visibility.Collapsed;
            LoginGrid.Visibility = Visibility.Visible;
            Tabs.Items.Clear();
            LoginGrid.Children.Clear();
            var loginWindow = new LoginWindow();
            loginWindow.Login += LoginWindow_Login;
            loginWindow.Loaded += (s, e) => { loginWindow.Start(); };
            LoginGrid.Children.Add(loginWindow);
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void NavigationItem_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as NavigationItem)!.IsActive)
            {
                return;
            }
            foreach (var item in NavBar.Items.OfType<NavigationItem>())
            {
                item.IsActive = false;
            }
            (sender as NavigationItem)!.IsActive = true;
        }

        private void NavigationItem_Activated(object sender, RoutedEventArgs e)
        {
            var item = (sender as NavigationItem)!;
            var tab = Tabs.Items.OfType<TabItem>().Where(x => x.Header.ToString() == item.Tag.ToString()).FirstOrDefault()!;
            if (tab != null)
            {
                tab.IsSelected = true;
            }
        }
    }
}
