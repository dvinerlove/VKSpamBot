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
using VkNet;

namespace VKBot.Logout
{
    /// <summary>
    /// Логика взаимодействия для LogoutPage.xaml
    /// </summary>
    public partial class LogoutPage : UserControl
    {
        public event EventHandler? Logout;
        public LogoutPage(VkApi api)
        {
            InitializeComponent();
            Api = api;
        }

        public VkApi Api { get; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Properties.Settings1.Default.Groups.Clear();
            Properties.Settings1.Default.Posts.Clear();
            Properties.Settings1.Default.PhotoIDs.Clear();
            Properties.Settings1.Default.AlbumID = 0;
            Properties.Settings1.Default.AccessToken = "";
            Properties.Settings1.Default.UserID = 0;
            Properties.Settings1.Default.Image = "";
            Properties.Settings1.Default.Save();
            Logout?.Invoke(this, EventArgs.Empty);
        }
    }
}
