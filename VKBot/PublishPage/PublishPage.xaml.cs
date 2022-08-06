using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using VkNet.Model.Attachments;

namespace VKBot.Publish
{
    /// <summary>
    /// Логика взаимодействия для PublishPage.xaml
    /// </summary>
    public partial class PublishPage : UserControl
    {
        public PublishPage()
        {
            InitializeComponent();
        }

        public PublishPage(VkApi api)
        {
            InitializeComponent();
            Api = api;
            _timer = new System.Timers.Timer();
            _timer.Interval = Properties.Settings1.Default.Interval;
            _timer.AutoReset = true;
            _timer.Elapsed += (s, e) =>
            {
                Run();
            };
            IntervalTB.Text = Properties.Settings1.Default.Interval.ToString();
            MaxPosts.Text = Properties.Settings1.Default.MaxPosts.ToString();
            Loaded += PublishPage_Loaded;

        }

        private void PublishPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings1.Default.Posts != null)
            {
                UpdateSavedPosts();
            }
        }

        public VkApi Api { get; }

        private const string Separator = "--";
        private System.Timers.Timer _timer;
        int counter = 0;
        private void Run()
        {

            DeletePreviousPosts();
            DeletePreviousPosts();
            Task.Factory.StartNew(() =>
            {
                foreach (var item in Properties.Settings1.Default.Groups)
                {
                    try
                    {
                        long postId = CreatePost(item);
                        SavePost(postId, item);
                        counter++;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
                Dispatcher.Invoke(() =>
                {
                    Counter.Content = counter;
                });
            });

        }
        Random _random = new Random();

        private long CreatePost(string item)
        {
            List<MediaAttachment> attachments = GetAttachmentVariant();

            var message = GetMessageVariant();
            if (string.IsNullOrEmpty(message) == false &&
               attachments.Count > 0)
            {
                return Api.Wall.Post(new VkNet.Model.RequestParams.WallPostParams()
                {
                    Message = message,
                    OwnerId = -long.Parse(item),
                    Attachments = attachments
                });
            }
            return -1;
        }

        private List<MediaAttachment> GetAttachmentVariant()
        {
            List<MediaAttachment> attachments = new List<MediaAttachment>();
            var index = -1;
            while (index < 0 || index > Properties.Settings1.Default.PhotoIDs.Count - 1)
            {
                index = _random.Next(-10, Properties.Settings1.Default.PhotoIDs.Count + 10);

            }
            var photoID = Properties.Settings1.Default.PhotoIDs[index];
            attachments.Add(new Photo() { OwnerId = Properties.Settings1.Default.UserID, UserId = Properties.Settings1.Default.UserID, Id = long.Parse(photoID!) });
            return attachments;
        }

        private string GetMessageVariant()
        {
            if (File.Exists("variables") == false)
                return "";

            var json = File.ReadAllText("posts");

            List<Tuple<string, string>> tuples = json.ToObject<List<Tuple<string, string>>>() ?? new List<Tuple<string, string>>();

            if (tuples.Count > 0)
            {

                var index = -1;
                while (index < 0 || index > tuples.Count - 1)
                {
                    index = _random.Next(-10, tuples.Count + 10);

                }

                var message = tuples[index].Item2;



                if (File.Exists("variables") == true)
                {
                    var variables = File.ReadAllText("variables").ToObject<List<Tuple<string, string>>>() ?? new List<Tuple<string, string>>();
                    foreach (var item in variables)
                    {
                        message = message.Replace(item.Item1, item.Item2);
                    }
                }
                return message;
            }
            else
            {
                return "";
            }
        }

        private void SavePost(long postId, string groupId)
        {
            if (Properties.Settings1.Default.Posts == null)
            {
                Properties.Settings1.Default.Posts = new System.Collections.Specialized.StringCollection();
            }
            Properties.Settings1.Default.Posts.Add(postId.ToString() + Separator + groupId.ToString());
            Properties.Settings1.Default.Save();

            Dispatcher.Invoke(() =>
            {
                PostsStack.Children.Clear();
                UpdateSavedPosts();
            });
        }

        private void DeletePreviousPosts()
        {
            while (Properties.Settings1.Default.Posts != null && Properties.Settings1.Default.Posts.Count > Properties.Settings1.Default.MaxPosts - 1 * Properties.Settings1.Default.Groups.Count)
            {
                System.Collections.IList list = Properties.Settings1.Default.Posts;
                string? item = list[0]!.ToString();
                DeletePost(item);
            }
        }

        private void DeletePost(string? item)
        {
            if (string.IsNullOrEmpty(item) == false)
            {
                try
                {
                    var post = item.Split(Separator)[0];
                    var group = item.Split(Separator)[1];
                    bool deleted = false;
                    try
                    {
                        deleted = Api.Wall.Delete(-long.Parse(group), long.Parse(post));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    Properties.Settings1.Default.Posts.RemoveAt(0);
                    Properties.Settings1.Default.Save();
                }
            }
            Dispatcher.Invoke(() =>
            {
                UpdateSavedPosts();
            });
        }

        private void UpdateSavedPosts()
        {
            PostsStack.Children.Clear();
            foreach (var item in Properties.Settings1.Default.Posts)
            {
                var btn = new Button() { Margin = new Thickness(8), Content = item!.Split(Separator)[1] };
                btn.Click += (s, e) =>
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = $"https://vk.com/wall-{item!.Split(Separator)[1]}_{item!.Split(Separator)[0]}",
                        UseShellExecute = true
                    });
                };
                PostsStack.Children.Add(btn);
            }
        }

        private void Start()
        {
            Run();

            _timer.Start();
            StartBtn.IsEnabled = false;
            StopBtn.IsEnabled = true;
        }
        private void Stop()
        {
            _timer.Stop();
            StartBtn.IsEnabled = true;
            StopBtn.IsEnabled = false;
        }
        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            Start();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double result;
            if (double.TryParse(IntervalTB.Text, out result))
            {
                Properties.Settings1.Default.Interval = result;
                if (result != 0)
                {
                    _timer.Interval = result;
                    Properties.Settings1.Default.Save();

                    result /= 60000;
                    if (IntervalTBmins.Text != result.ToString() && IntervalTBmins.IsFocused == false)
                    {
                        IntervalTBmins.Text = result.ToString();
                    }
                }

            }

        }

        private void IntervalTBmins_TextChanged(object sender, TextChangedEventArgs e)
        {
            var selection = IntervalTBmins.SelectionStart;
            IntervalTBmins.Text = IntervalTBmins.Text.Replace('.', ',');
            IntervalTBmins.Select(selection, 0);
            double result;
            if (double.TryParse(IntervalTBmins.Text, out result))
            {
                result *= 60000;
                if (result != 0)
                {
                    if (IntervalTB.Text != result.ToString() && IntervalTB.IsFocused == false)
                    {
                        IntervalTB.Text = result.ToString();
                    }
                    Properties.Settings1.Default.Interval = result;
                    _timer.Interval = result;
                    Properties.Settings1.Default.Save();
                }
            }
        }

        private void MaxPosts_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MaxPosts_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int result;
            if (int.TryParse(e.Text, out result))
            {
                if (result > 1)
                {
                    Properties.Settings1.Default.MaxPosts = result;
                    Properties.Settings1.Default.Save();
                }
                else
                {
                    e.Handled = !e.Handled;
                }
            }
            else
            {
                e.Handled = !e.Handled;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();
            foreach (var item in Properties.Settings1.Default.Posts)
            {
                list.Add(item!);
            }
            foreach (var item in list)
            {
                DeletePost(item.ToString());
            }
        }
    }
}
