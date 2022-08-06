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

namespace VKBot.PostSettings
{

    /// <summary>
    /// Логика взаимодействия для VariableItem.xaml
    /// </summary>
    public partial class VariableItem : UserControl
    {
        public event EventHandler<Tuple<string, string>>? TextChanged;
        public VariableItem(Tuple<string, string>? tuple)
        {
            InitializeComponent();
            if (tuple != null)
            {
                Variable.Text = tuple.Item1;
                Text.Text = tuple.Item2;
            }
        }


        public event EventHandler<VariableItem>? Close;
        public bool IsEmpty { get { return string.IsNullOrEmpty(Text.Text) && string.IsNullOrEmpty(Variable.Text); } }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (Variable.Text.Length > 0)
            {
                if (Variable.Text[0] != '{')
                {
                    Variable.Text = Variable.Text.Replace("{", "");
                    Variable.Text = "{" + Variable.Text;
                    Variable.Select(2, 0);
                }

                if (Variable.Text[Variable.Text.Length - 1] != '}')
                {
                    Variable.Text = Variable.Text.Replace("}", "");
                    Variable.Text = Variable.Text + "}";
                    Variable.Select(Variable.Text.Length - 1, 0);
                }


                if (Variable.Text.Contains(" "))
                {
                    Variable.Text = Variable.Text.Replace(" ", "");
                }

                try
                {
                    if (Variable.Text.Substring(1, Variable.Text.Length - 2).Contains("{") ||
                        Variable.Text.Substring(1, Variable.Text.Length - 2).Contains("}"))
                    {
                        var newString = Variable.Text.Replace("{", "");
                        newString = newString.Replace("}", "");
                        Variable.Text = "{" + newString + "}";
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            TextChanged?.Invoke(this, new Tuple<string, string>(Variable.Text, Text.Text));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close?.Invoke(this, this);
        }

        private void Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged?.Invoke(this, GetTuple());
        }

        public Tuple<string, string> GetTuple()
        {
            return new Tuple<string, string>(Variable.Text, Text.Text);
        }

    }
}
