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
using VkNet.Model;

namespace VKBot.PostSettings
{
    /// <summary>
    /// Логика взаимодействия для PhotoAlbumItem.xaml
    /// </summary>
    public partial class PhotoAlbumItem : UserControl
    {
        public PhotoAlbumItem()
        {
            InitializeComponent();
        }

        public PhotoAlbumItem(PhotoAlbum item)
        {
            InitializeComponent();
            Item = item;
            Preview.Source = new BitmapImage(item.Sizes.FirstOrDefault()!.Url);
            Name.Text = item.Title;
        }

        public event EventHandler<PhotoAlbum>? Click;
        public PhotoAlbum Item { get; } = new PhotoAlbum();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, Item);
        }
    }
}
