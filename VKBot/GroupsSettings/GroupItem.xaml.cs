using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace VKBot.GroupsSettings
{
    /// <summary>
    /// Логика взаимодействия для GroupItem.xaml
    /// </summary>
    public partial class GroupItem : UserControl
    {
        public GroupItem()
        {
            InitializeComponent();
        }

        public GroupItem(Group item)
        {
            InitializeComponent();
            Item = item;
            Name.Text = item.Name;
            {
                Preview.Source = new BitmapImage(item.Photo200);
            }
            ContextMenu = new ContextMenu();
            var button = new System.Windows.Controls.Button();
            button.Content = $"https://vk.com/club{Item.Id}";
            button.Click += (s, e) =>
            {
                
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"https://vk.com/club{Item.Id}",
                    UseShellExecute = true
                } );
            };
            ContextMenu.Items.Add(button);
        }

        public Group Item { get; }

        public event EventHandler<Group>? Click;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, Item);
        }
    }
}