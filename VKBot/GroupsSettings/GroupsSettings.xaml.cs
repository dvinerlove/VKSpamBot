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
using VkNet;

namespace VKBot.GroupsSettings
{
    /// <summary>
    /// Логика взаимодействия для GroupsSettings.xaml
    /// </summary>
    public partial class GroupsSettings : UserControl
    {
        public GroupsSettings(VkApi api)
        {
            InitializeComponent();
            Api = api;
            GetGroups();
        }

        public VkApi Api { get; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Query.Text))
            {
                return;
            }
            var searchResult = Api.Groups.Search(new VkNet.Model.RequestParams.GroupsSearchParams()
            {
                Query = Query.Text,
            });
            SearchStack.Children.Clear();
            foreach (var item in searchResult.Where(x => x.IsClosed == VkNet.Enums.GroupPublicity.Public))
            {
                var group = new GroupItem(item);
                group.Click += Group_Click;
                SearchStack.Children.Add(group);
            }
        }

        private void Group_Click(object? sender, VkNet.Model.Group e)
        {
            if (Properties.Settings1.Default.Groups == null)
            {
                Properties.Settings1.Default.Groups = new System.Collections.Specialized.StringCollection();
            }
            if (Properties.Settings1.Default.Groups.Contains(e.Id.ToString()) == false)
            {
                Properties.Settings1.Default.Groups.Add(e.Id.ToString()); ;
            }
            GetGroups();
        }

        void GetGroups()
        {
            SavedStack.Children.Clear();
            if (Properties.Settings1.Default.Groups == null)
            {
                return;
            }

            List<string> strings = new List<string>();
            foreach (var item in Properties.Settings1.Default.Groups)
            {
                strings.Add(item);
            }
            if (strings.Count > 0)
            {
                try
                {
                    var gr = Api.Groups.GetById(strings, null, new VkNet.Enums.Filters.GroupsFields());

                    foreach (var item in gr)
                    {
                        var group = new GroupItem(item);
                        group.Icon.Symbol = Wpf.Ui.Common.SymbolRegular.Delete24;
                        group.Click += Group_Remove;
                        SavedStack.Children.Add(group);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

            }
            Properties.Settings1.Default.Save();
        }

        private void Group_Remove(object? sender, VkNet.Model.Group e)
        {
            Properties.Settings1.Default.Groups.Remove(e.Id.ToString());
            GetGroups();
        }
    }
}
