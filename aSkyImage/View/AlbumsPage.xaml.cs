using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using aSkyImage.Model;
using aSkyImage.Resources;
using aSkyImage.UserControls;
using aSkyImage.ViewModel;

namespace aSkyImage.View
{
    /// <summary>
    /// View that displays users albums with coverphotos
    /// </summary>
    public partial class AlbumsPage : PhoneApplicationPage
    {
        //pages popup, to add new album or to sign in
        private Popup _popup = null;

        public AlbumsPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        /// <summary>
        /// After page is loaded load album data, localize the application bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (ApplicationBar.Buttons.Count > 0)
            {
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.AlbumsPageAppBarAddAlbum;
            }
            if (ApplicationBar.MenuItems.Count > 0)
            {
                (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.CommonRefresh;
                (ApplicationBar.MenuItems[1] as ApplicationBarMenuItem).Text = AppResources.CommonPinToStart;
            }
        }

        /// <summary>
        /// When user navigates to albums load data or restore it from the state (tombstone)
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //before loading we have to make sure we have LiveSession
            if (App.LiveSession == null)
            {
                App.AlbumsViewModel.Albums = (ObservableCollection<SkyDriveAlbum>) State[App.AlbumsKey];
            }
            else
            {
                AlbumListBox.SelectedItem = null;
                App.AlbumViewModel.SelectedAlbum = null;
                App.AlbumsViewModel.LoadAlbumsData();    
            }
            
            DataContext = App.AlbumsViewModel;

            UpdateLiveTileIfTileIsPresent();
        }

        /// <summary>
        /// Method for updating random cover photo to livetile if it is present and user has more than one album
        /// </summary>
        private void UpdateLiveTileIfTileIsPresent()
        {
            ShellTile TileToFind = FindASkyImageLiveTile();

            if (TileToFind != null)
            {
                if (App.AlbumsViewModel.Albums.Count > 1)
                {
                    WebClient backTileClient = new WebClient();
                    backTileClient.OpenReadCompleted += backTileClient_OnOpenReadCompleted;

                    Random random = new Random();
                    int albumCoverToTileBack = random.Next(1, App.AlbumsViewModel.Albums.Count);

                    backTileClient.OpenReadAsync(new Uri(App.AlbumsViewModel.Albums[albumCoverToTileBack].AlbumPicture));
                }
            }
        }

        /// <summary>
        /// when user navigates away from this page save some information to the appstate (tombstone)
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // If this is a back navigation, the page will be discarded, so there
            // is no need to save state.
            if (e.NavigationMode != System.Windows.Navigation.NavigationMode.Back)
            {
                // Save the Session variable in the page's State dictionary.
                if (App.AlbumsViewModel != null)
                {
                    State[App.AlbumsKey] = App.AlbumsViewModel.Albums;
                }
            }
            else
            {
                //on navigate back close popup if it is still open
                if (_popup != null)
                {
                    _popup.IsOpen = false;
                    _popup = null;
                }
            }
        }

        /// <summary>
        /// Event that occurs when users selects album from the albums listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlbumListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewLoginPromptIfSessionEnded(PopupLogin.AlbumsPage) == false)
            {
                if (App.AlbumViewModel.SelectedAlbum != null)
                {
                    App.PhotoViewModel.SelectedPhoto = null;
                    App.AlbumViewModel.AlbumDataLoaded = false;
                    NavigationService.Navigate(new Uri("/View/AlbumPage.xaml", UriKind.Relative));
                }
            }
        }

        /// <summary>
        /// When user pushes add button on the application bar show popup to ask the name to that new popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppIconNewFolder_OnClick(object sender, EventArgs e)
        {
            //ensure that we have a live sessoin so this trick is possible
            if (ViewLoginPromptIfSessionEnded(PopupLogin.AlbumsPage) == false)
            {
                //if there is already a popup close it
                if (_popup != null)
                {
                    _popup.IsOpen = false;
                    _popup = null;
                }

                //open new popup
                _popup = new Popup()
                    {
                        IsOpen = true,
                        Child = new InputPrompt(AppResources.AlbumsPageAddNewAlbumsName, PopupAction.CreateAlbum)
                    };
            }
        }

        /// <summary>
        /// Event for user pressing the refresh from the application bar menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarRefreshAlbums_OnClick(object sender, EventArgs e)
        {
            if (ViewLoginPromptIfSessionEnded(PopupLogin.AlbumsPage) == false)
            {
                App.AlbumsViewModel.LoadAlbumsData(true);    
            }
        }

        /// <summary>
        /// Show login prompt if session is ended
        /// </summary>
        /// <param name="popupLogin"></param>
        private bool ViewLoginPromptIfSessionEnded(PopupLogin popupLogin)
        {
            if (App.LiveSession == null)
            {
                if (_popup != null)
                {
                    _popup.IsOpen = false;
                    _popup = null;
                }

                _popup = new Popup() {IsOpen = true, Child = new LoginPrompt(popupLogin)};

                return true;
            }
            return false;
        }

        /// <summary>
        /// Event for user pinning the program to the start page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eargs"></param>
        private void AppBarPinProgram_OnClick(object sender, EventArgs eargs)
        {
            //need to create the tile(?)
            ShellTile TileToFind = FindASkyImageLiveTile();

            // if there are no tile we can create new
            if (TileToFind == null)
            {
                //tile is created only if user has albums.. ehm?
                if (App.AlbumsViewModel.Albums.Any())
                {
                    //try to make image from the first albumpicture..
                    WebClient client = new WebClient();
                    client.OpenReadCompleted += ClientOnOpenReadCompleted;

                    client.OpenReadAsync(new Uri(App.AlbumsViewModel.Albums.First().AlbumPicture));
                }
            }
            else
            {
                MessageBox.Show(AppResources.TileExistsAlready);
            }
        }

        /// <summary>
        /// Find applications LiveTile method because of the string MainPage.xaml..
        /// </summary>
        private static ShellTile FindASkyImageLiveTile()
        {
            return ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("MainPage.xaml"));
        }

        /// <summary>
        /// Event for really creating the tile after album cover image has been fetched
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientOnOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            //for accessing isolated storage
            IsolatedStorageFile userStore = IsolatedStorageFile.GetUserStoreForApplication();

            //make up a random id for demo purposes
            string id = Guid.NewGuid().ToString("n");

            //"shared\shellcontent" is the only directory that the WP7 shell can read from
            const string shellContentDirectory = "Shared\\ShellContent";
            userStore.CreateDirectory(shellContentDirectory);

            //render the UIElement into a writeable bitmap, with actual tile size
            WriteableBitmap bmp = new WriteableBitmap(173, 173);
            bmp.SetSource(e.Result);

            //save the bitmap to a file. It's very important to close this stream!
            string imagePath = shellContentDirectory + "\\Tile" + id + ".jpg";
            using (IsolatedStorageFileStream stream = userStore.OpenFile(imagePath, FileMode.Create))
            {
                bmp.SaveJpeg(stream, 173, 173, 0, 100);
            }

            //the uri that will locate us the image at imagePath 
            Uri imageUri = new Uri("isostore:/" + imagePath.Replace('\\', '/'));

            //create a new shelltile
            Uri navigationUri = new Uri("/MainPage.xaml?id=" + id, UriKind.Relative);
            StandardTileData data = new StandardTileData
                {
                    Title = AppResources.ApplicationTitle,
                    BackgroundImage = imageUri
                };
            ShellTile.Create(navigationUri, data);
        }

        /// <summary>
        /// Event for creating the tiles backbackground.. backback for serious MS?
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backTileClient_OnOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            //first make sure that we have the tile still there
            ShellTile TileToFind = FindASkyImageLiveTile();

            // Application should always be found
            if (TileToFind != null)
            {
                //for accessing isolated storage
                IsolatedStorageFile userStore = IsolatedStorageFile.GetUserStoreForApplication();

                //make up a random id for demo purposes
                string id = Guid.NewGuid().ToString("n");

                //"shared\shellcontent" is the only directory that the WP7 shell can read from
                const string shellContentDirectory = "Shared\\ShellContent";
                userStore.CreateDirectory(shellContentDirectory);

                //render the UIElement into a writeable bitmap 
                WriteableBitmap bmp = new WriteableBitmap(173, 173);
                bmp.SetSource(e.Result);

                //save the bitmap to a file. It's very important to close this stream!
                string imagePath = shellContentDirectory + "\\Tile" + id + ".jpg";
                using (IsolatedStorageFileStream stream = userStore.OpenFile(imagePath, FileMode.Create))
                {
                    bmp.SaveJpeg(stream, 173, 173, 0, 100);
                }

                //the uri that will locate us the image at imagePath 
                Uri imageUri = new Uri("isostore:/" + imagePath.Replace('\\', '/'));

                //set background to the tile
                StandardTileData tileBackData = new StandardTileData
                {
                    BackTitle = AppResources.ApplicationTitle,
                    BackBackgroundImage = imageUri
                };
                
                TileToFind.Update(tileBackData);
            }
        }
    }
}