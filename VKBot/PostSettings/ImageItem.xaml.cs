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
using VkNet.Model.Attachments;

namespace VKBot.PostSettings
{
    /// <summary>
    /// Логика взаимодействия для ImageItem.xaml
    /// </summary>
    public partial class PhotoItem : UserControl
    {
        public event EventHandler<Photo>? Click;
        public PhotoItem()
        {
            InitializeComponent();
        }

        public PhotoItem(Photo item)
        {
            InitializeComponent();
            Item = item;
            Preview.Source = new BitmapImage(Item.Sizes.LastOrDefault()!.Url);

        }

        public PhotoItem(BitmapImage bitmapImage)
        {

        }

        public Photo Item { get; } = new Photo();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, Item);
        }
    }
}
