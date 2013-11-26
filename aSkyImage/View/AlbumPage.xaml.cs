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
    public partial class AlbumPage : PhoneApplicationPage
    {
        private Popup _popup = null;

        public AlbumPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

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

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //before loading we have to make sure we have LiveSession
            if (App.LiveSession == null)
            {
                //If was tombstoned check state
                if (State.ContainsKey(App.SelectedAlbumKey))
                {
                    App.AlbumViewModel.SelectedAlbum = (SkyDriveAlbum) State[App.SelectedAlbumKey];
                    if (State.ContainsKey(App.SelectedAlbumPhotosKey))
                    {
                        App.AlbumViewModel.SelectedAlbum.Photos = (ObservableCollection<SkyDrivePhoto>)State[App.SelectedAlbumPhotosKey];
                    }
                    App.AlbumViewModel.AlbumDataLoaded = true;
                }

                ChangeApplicationBarIconStatus(false);
            }
        }

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

        private void AppIconUpload_OnClick(object sender, EventArgs e)
        {
            App.AlbumViewModel.Upload();
        }

        private void PhotoListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewLoginPromptIfSessionEnded() == false)
            {
                if (App.PhotoViewModel.SelectedPhoto != null)
                {
                    if (ApplicationBar.Buttons.Count > 2)
                    {
                        (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                        (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;
                    }

                    App.PhotoViewModel.LoadPhotoComments(App.PhotoViewModel.SelectedPhoto);

                    var selectedDataObject = e.AddedItems[0]; // assuming single selection
                    ChangeItemForegroundColor(selectedDataObject, Colors.Red);

                    if (e.RemovedItems.Count > 0)
                    {
                        var removedObject = e.RemovedItems[0]; // assuming single selection
                        ChangeItemForegroundColor(removedObject, (Color)Resources["PhoneForegroundColor"]);
                    }
                }   
            }
        }

        private void ChangeItemForegroundColor(object selectedDataObject, Color newColor)
        {
            if (PhotoListBox.ItemContainerGenerator != null)
            {
                ListBoxItem selectedItem = (ListBoxItem)(PhotoListBox.ItemContainerGenerator.ContainerFromItem(selectedDataObject));
                selectedItem.Foreground = new SolidColorBrush(newColor);    
            }
        }

        private void AppIconDownload_OnClick(object sender, EventArgs e)
        {
            App.PhotoViewModel.Download();
        }

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

                MessageBox.Show("Please select a photo.");
            }
        }

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

        private void loginCompleted(object sender, EventArgs e)
        {
            //activate application bar icons
            ChangeApplicationBarIconStatus(true);
        }

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