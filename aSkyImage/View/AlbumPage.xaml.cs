using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using aSkyImage.Model;
using aSkyImage.Resources;
using aSkyImage.UserControls;
using aSkyImage.ViewModel;

namespace aSkyImage.View
{
    /// <summary>
    /// View that displays users single albums photos
    /// </summary>
    public partial class AlbumPage : PhoneApplicationPage
    {
        //if session ends at this page we can display popup for login
        private Popup _popup = null;

        public AlbumPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        /// <summary>
        /// After page is loaded load album data, set title and localization for application bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            LocalizeApplicationBar();
            PageTitle.Text = App.AlbumViewModel.SelectedAlbum.Title;
            App.AlbumViewModel.LoadSingleAlbumData();

            if (App.PhotoViewModel.SelectedPhoto == null)
            {
                if (ApplicationBar.Buttons.Count > 2)
                {
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
                    (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = false;
                }
            }

            DataContext = App.AlbumViewModel;
        }

        /// <summary>
        /// when user is navigated to single album page.. contains mainly code for handling tombstoning
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //before loading we have to make sure we have LiveSession
            if (App.LiveSession == null)
            {
                //If was tombstoned check state
                if (State.ContainsKey(App.SelectedAlbumKey))
                {
                    //restore album data from state
                    App.AlbumViewModel.SelectedAlbum = (SkyDriveAlbum) State[App.SelectedAlbumKey];
                    if (State.ContainsKey(App.SelectedAlbumPhotosKey))
                    {
                        //restore album photos from the state
                        App.AlbumViewModel.SelectedAlbum.Photos = (ObservableCollection<SkyDrivePhoto>)State[App.SelectedAlbumPhotosKey];
                    }
                    App.AlbumViewModel.AlbumDataLoaded = true;
                }

                ChangeApplicationBarIconStatus(false);
            }
        }

        /// <summary>
        /// enabling and disabling app bar actions they should not work if there are no live session
        /// </summary>
        /// <param name="isEnabled"></param>
        private void ChangeApplicationBarIconStatus(bool isEnabled)
        {
            if (ApplicationBar.Buttons.Count > 2)
            {
                //disable appbar buttons if no session
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = isEnabled;
                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = isEnabled;
                (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = isEnabled;
            }
        }

        /// <summary>
        /// When navigating away from this page make some saves to handle possible tombstoning
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
                    if (App.AlbumViewModel.SelectedAlbum != null)
                    {
                        State[App.SelectedAlbumKey] = App.AlbumViewModel.SelectedAlbum;
                        State[App.SelectedAlbumPhotosKey] = App.AlbumViewModel.SelectedAlbum.Photos;
                    }
                }
            }
            else
            {
                //without this we are not able to navigate straight back to same album..
                App.AlbumViewModel.SelectedAlbum.Photos.Clear(); 
                App.AlbumViewModel.SelectedAlbum = null;
            }
        }

        /// <summary>
        /// event when users presses upload icon from the application bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppIconUpload_OnClick(object sender, EventArgs e)
        {
            App.AlbumViewModel.Upload();
        }

        /// <summary>
        /// event when users selects different photo from the listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PhotoListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewLoginPromptIfSessionEnded() == false)
            {
                if (App.PhotoViewModel.SelectedPhoto != null)
                {
                    if (ApplicationBar.Buttons.Count > 2)
                    {
                        //enable zoom and download when user has selected the photo
                        (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                        (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;
                    }

                    //start loading comments when selection has been made
                    App.PhotoViewModel.LoadPhotoComments(App.PhotoViewModel.SelectedPhoto);

                    var selectedDataObject = e.AddedItems[0]; // assuming single selection
                    
                    //change items color so user knows what has been selected
                    ChangeItemForegroundColor(selectedDataObject, Colors.Red);

                    if (e.RemovedItems.Count > 0)
                    {
                        var removedObject = e.RemovedItems[0]; // assuming single selection

                        //remove red from the previous selection
                        ChangeItemForegroundColor(removedObject, (Color)Resources["PhoneForegroundColor"]);
                    }
                }   
            }
        }

        /// <summary>
        /// Method to change listbox foregroud color
        /// </summary>
        /// <param name="selectedDataObject"></param>
        /// <param name="newColor"></param>
        private void ChangeItemForegroundColor(object selectedDataObject, Color newColor)
        {
            if (PhotoListBox.ItemContainerGenerator != null)
            {
                ListBoxItem selectedItem = (ListBoxItem)(PhotoListBox.ItemContainerGenerator.ContainerFromItem(selectedDataObject));
                selectedItem.Foreground = new SolidColorBrush(newColor);    
            }
        }

        /// <summary>
        /// event when users presses download icon from the application bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppIconDownload_OnClick(object sender, EventArgs e)
        {
            App.PhotoViewModel.Download();
        }

        /// <summary>
        /// event when users presses Zoom icon from the application bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppIconShowImage_OnClick(object sender, EventArgs e)
        {
            if (ViewLoginPromptIfSessionEnded() == false)
            {
                if (App.PhotoViewModel.SelectedPhoto != null)
                {
                    //move to photo page
                    NavigationService.Navigate(new Uri("/View/PhotoPage.xaml", UriKind.Relative));
                    return;
                }

                //ehm, not localized text, well this should be redundant because icon should not be enabled without selected photo..
                MessageBox.Show("Please select a photo.");
            }
        }

        /// <summary>
        /// Method to display the login popup if there is no active session
        /// </summary>
        private bool ViewLoginPromptIfSessionEnded()
        {
            if (App.LiveSession == null)
            {
                if (_popup != null)
                {
                    _popup.IsOpen = false;
                    _popup = null;
                }

                var childPopup = new LoginPrompt(PopupLogin.AlbumPage);
                childPopup.LoginCompleted += loginCompleted;

                _popup = new Popup() { IsOpen = true, Child = childPopup };

                return true;
            }
            return false;
        }

        /// <summary>
        /// event when users presses Refresh from the application bar menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarRefreshAlbum_OnClick(object sender, EventArgs e)
        {
            if (App.LiveSession == null)
            {
                //open popup please login
                if (_popup != null)
                {
                    _popup.IsOpen = false;
                    _popup = null;
                }

                var childPopup = new LoginPrompt(PopupLogin.AlbumPage);
                childPopup.LoginCompleted += loginCompleted;

                _popup = new Popup() { IsOpen = true, Child = childPopup };
                
            }
            else
            {
                App.AlbumViewModel.AlbumDataLoaded = false;
                App.AlbumViewModel.LoadSingleAlbumData();    
            }
        }

        /// <summary>
        /// after login is completed active buttons on application bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginCompleted(object sender, EventArgs e)
        {
            //activate application bar icons
            ChangeApplicationBarIconStatus(true);
        }

        /// <summary>
        /// method to localize application bar
        /// </summary>
        private void LocalizeApplicationBar()
        {
            if (ApplicationBar.Buttons.Count > 2)
            {
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.AlbumPageAppBarUpload;
                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.AlbumPageAppBarDownload;
                (ApplicationBar.Buttons[2] as ApplicationBarIconButton).Text = AppResources.AlbumPageAppBarZoom;
            }
            if (ApplicationBar.MenuItems.Count > 0)
            {
                (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.CommonRefresh;
            }
        }
    }
}