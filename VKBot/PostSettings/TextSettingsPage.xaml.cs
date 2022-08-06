using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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

namespace VKBot.PostSettings
{
    /// <summary>
    /// Логика взаимодействия для TextSettingsPage.xaml
    /// </summary>
    public partial class TextSettingsPage : UserControl
    {
        private Timer _variablesTimer;
        private Timer _postTimer;
        private List<Tuple<string, string>> _variables = new List<Tuple<string, string>>();
        private List<Tuple<string, string>> _textVariants = new List<Tuple<string, string>>();
        public VkApi Api { get; }

        public TextSettingsPage(VkApi api)
        {
            Api = api;
            InitializeComponent();
            _variablesTimer = new Timer();
            _variablesTimer.Interval = 1000;
            _variablesTimer.Elapsed += _variablesTimer_Elapsed;

            _postTimer = new Timer();
            _postTimer.Interval = 1000;
            _postTimer.Elapsed += _postTimer_Elapsed;

            OpenVariables();
            OpenPosts();
        }

        private void OpenVariables()
        {
            if (File.Exists("variables") == false)
                return;

            var json = File.ReadAllText("variables");
            _variables = json.ToObject<List<Tuple<string, string>>>() ?? new List<Tuple<string, string>>();
            foreach (var item in _variables)
            {
                VariablesStack.Children.Add(CreateVariable(item));
            }
        }

        private void OpenPosts()
        {
            if (File.Exists("variables") == false)
                return;

            var json = File.ReadAllText("posts");
            _textVariants = json.ToObject<List<Tuple<string, string>>>() ?? new List<Tuple<string, string>>();
            foreach (var item in _textVariants)
            {
                TextVariantsStack.Children.Add(CreateTextVariantItem(item));
            }
        }

        private void _postTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            var file = File.CreateText("posts");
            file.Write(_textVariants.ToJson());
            file.Close();
            _postTimer.Stop();
            Snackbar1.Show("posts saved");
        }

        private void _variablesTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            var file = File.CreateText("variables");
            file.Write(_variables.ToJson());
            file.Close();
            _variablesTimer.Stop();
            Snackbar1.Show("variables saved", "variables saved", Wpf.Ui.Common.SymbolRegular.Save24);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClearEmptyVariableItems();
            VariablesStack.Children.Add(CreateVariable());
        }

        private TextVariant CreateTextVariantItem(Tuple<string, string>? tuple = null)
        {
            var textVariantItem = new TextVariant(tuple);
            textVariantItem.NameTB.Text = $"Variant {TextVariantsStack.Children.Count + 1}";
            textVariantItem.TextChanged += TextVariantItem_TextChanged; ;
            textVariantItem.Close += (s, e) => { TextVariantsStack.Children.Remove(e); };
            return textVariantItem;
        }

        private VariableItem CreateVariable(Tuple<string, string>? tuple = null)
        {
            var variableItem = new VariableItem(tuple);
            variableItem.TextChanged += VariableItem_TextChanged;
            variableItem.Close += (s, e) => { VariablesStack.Children.Remove(e); };
            return variableItem;
        }

        private void ClearEmptyVariableItems()
        {
            foreach (var item in VariablesStack.Children.OfType<VariableItem>().ToList())
            {
                if (item.IsEmpty == true)
                {
                    VariablesStack.Children.Remove(item);
                }
            }
        }

        private void ClearEmptyTexts()
        {
            foreach (var item in TextVariantsStack.Children.OfType<TextVariant>().ToList())
            {
                if (item.IsEmpty == true)
                {
                    TextVariantsStack.Children.Remove(item);
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ClearEmptyTexts();
            TextVariantsStack.Children.Add(CreateTextVariantItem());
        }

        private void VariableItem_TextChanged(object? sender, Tuple<string, string> e)
        {
            _variables.Clear();
            foreach (var item in VariablesStack.Children.OfType<VariableItem>())
            {
                if (item.IsEmpty == false)
                {
                    _variables.Add(item.GetTuple());
                }
            }
            _variablesTimer.Start();
        }

        private void TextVariantItem_TextChanged(object? sender, Tuple<string, string> e)
        {
            _textVariants.Clear();
            foreach (var item in TextVariantsStack.Children.OfType<TextVariant>())
            {
                if (item.IsEmpty == false)
                {
                    _textVariants.Add(item.GetTuple());
                }
            }

            _postTimer.Start();
        }
    }
}
