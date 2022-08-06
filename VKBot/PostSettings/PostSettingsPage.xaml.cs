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
using VKBot.PostSettings;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace VKBot
{
    /// <summary>
    /// Логика взаимодействия для PostSettingsPage.xaml
    /// </summary>
    public partial class PostSettingsPage : UserControl
    {
        public VkApi Api { get; } = new VkApi();

        public PostSettingsPage(VkApi api)
        {
            Api = api;

            InitializeComponent();
            //PhotoID.Text = Properties.Settings1.Default.PhotoIDs.ToString();
            AlbumID.Text = Properties.Settings1.Default.AlbumID.ToString();

            if (string.IsNullOrEmpty(AlbumID.Text) == false)
            {
                long albumid = /*288002998*/long.Parse(AlbumID.Text);
                try
                {
                    List<string> photoIDs = new List<string>();
                    if (Properties.Settings1.Default.PhotoIDs != null)
                    {
                        foreach (var item in Properties.Settings1.Default.PhotoIDs)
                        {
                            if (item != null)
                            {
                                photoIDs.Add(item);
                            }
                        }

                        var photos = Api.Photo.Get(new PhotoGetParams
                        {
                            AlbumId = PhotoAlbumType.Id(albumid),
                            PhotoIds = photoIDs,
                            Extended = true
                        });

                        foreach (var item in photos)
                        {
                            CurrentPhotos.Children.Add(CreatePhotoItem(item));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            GetAlbums();
            GetPhotos();
        }

        private void GetAlbums()
        {
            try
            {
                var albums = Api.Photo.GetAlbums(new PhotoGetAlbumsParams() { OwnerId = Properties.Settings1.Default.UserID, NeedSystem = true, PhotoSizes = true, NeedCovers = true }, true);
                Albums.Children.Clear();
                foreach (var item in albums)
                {
                    var photo = new PhotoAlbumItem(item);
                    photo.Click += PhotoAlbum_Click1;
                    Albums.Children.Add(photo);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void PhotoAlbum_Click1(object? sender, VkNet.Model.PhotoAlbum e)
        {

            CurrentPhotos.Children.Clear();
            AlbumID.Text = e.Id.ToString();
            GetPhotos();

        }

        private void Save()
        {
            try
            {
                if (Properties.Settings1.Default.PhotoIDs != null)
                {
                    Properties.Settings1.Default.PhotoIDs.Clear();
                }
                else
                {
                    Properties.Settings1.Default.PhotoIDs = new System.Collections.Specialized.StringCollection();
                }
                foreach (var item in CurrentPhotos.Children.OfType<PhotoItem>())
                {
                    Properties.Settings1.Default.PhotoIDs.Add(item.Item.Id.ToString()!);
                }
                Properties.Settings1.Default.AlbumID = long.Parse(AlbumID.Text);
                Properties.Settings1.Default.Save();
                GetAlbums();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Save();
            GetPhotos();
        }

        private void GetPhotos()
        {
            long albumid = long.Parse(AlbumID.Text);
            try
            {
                var photos = Api.Photo.Get(new PhotoGetParams
                {
                    AlbumId = PhotoAlbumType.Id(albumid),

                    Extended = true
                });
                List<MediaAttachment> attachments = new List<MediaAttachment>();
                attachments.Add(photos.Last());
                Photos.Children.Clear();
                foreach (var item in photos)
                {
                    var photo = new PhotoItem(item);
                    photo.Click += Photo_Click;
                    Photos.Children.Add(photo);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Photo_Click(object? sender, Photo e)
        {
            if (CurrentPhotos.Children.OfType<PhotoItem>().Where(x => x.Item.Id == e.Id).FirstOrDefault() == null)
            {
                CurrentPhotos.Children.Add(CreatePhotoItem(e));
            }
        }

        private PhotoItem CreatePhotoItem(Photo e)
        {
            var photo = new PhotoItem(e);
            photo.Icon.Symbol = Wpf.Ui.Common.SymbolRegular.Delete24;
            photo.Click += (s, e) => { CurrentPhotos.Children.Remove(photo); };
            return photo;
        }
    }
}
