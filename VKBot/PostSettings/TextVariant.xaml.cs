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

namespace VKBot.PostSettings
{
    /// <summary>
    /// Логика взаимодействия для TextVariant.xaml
    /// </summary>
    public partial class TextVariant : UserControl
    {
        public event EventHandler<Tuple<string, string>>? TextChanged;

        public TextVariant(Tuple<string, string>? tuple)
        {
            InitializeComponent();
            if (tuple != null)
            {
                NameTB.Text = tuple.Item1;
                PostText.Text = tuple.Item2;
            }
        }

        public event EventHandler<TextVariant>? Close;
        public bool IsEmpty { get { return string.IsNullOrEmpty(PostText.Text); } }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close?.Invoke(this, this);
        }

        private void PostText_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged?.Invoke(this, GetTuple());
        }

        internal Tuple<string, string> GetTuple()
        {
            return new Tuple<string, string>(NameTB.Text, PostText.Text);
        }
    }
}
