using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using aSkyImage.Model;
using aSkyImage.Resources;
using aSkyImage.UserControls;
using aSkyImage.ViewModel;

namespace aSkyImage.View
{
    public partial class AlbumsPage : PhoneApplicationPage
    {
        private Popup _popup = null;

        public AlbumsPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (ApplicationBar.Buttons.Count > 0)
            {
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.AlbumsPageAppBarAddAlbum;
            }
            if (ApplicationBar.MenuItems.Count > 0)
            {
                (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.CommonRefresh;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //before loading we have to make sure we have LiveSession
            if (App.LiveSession == null)
            {
                App.ViewModel.Albums = (ObservableCollection<SkyDriveAlbum>) State[App.AlbumsKey];
            }
            else
            {
                AlbumListBox.SelectedItem = null;
                App.ViewModel.SelectedAlbum = null;
                App.ViewModel.LoadData();    
            }
            
            DataContext = App.ViewModel;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // If this is a back navigation, the page will be discarded, so there
            // is no need to save state.
            if (e.NavigationMode != System.Windows.Navigation.NavigationMode.Back)
            {
                // Save the Session variable in the page's State dictionary.
                if (App.ViewModel != null)
                {
                    State[App.AlbumsKey] = App.ViewModel.Albums;
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

        private void AlbumListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewLoginPromptIfSessionEnded(PopupLogin.AlbumsPage) == false)
            {
                if (App.ViewModel.SelectedAlbum != null)
                {
                    App.ViewModel.SelectedPhoto = null;
                    App.ViewModel.AlbumDataLoaded = false;
                    NavigationService.Navigate(new Uri("/View/AlbumPage.xaml", UriKind.Relative));
                }
            }
        }

        private void AppIconNewFolder_OnClick(object sender, EventArgs e)
        {
            if (ViewLoginPromptIfSessionEnded(PopupLogin.AlbumsPage) == false)
            {

                if (_popup != null)
                {
                    _popup.IsOpen = false;
                    _popup = null;
                }

                _popup = new Popup()
                    {
                        IsOpen = true,
                        Child = new InputPrompt(AppResources.AlbumsPageAddNewAlbumsName, PopupAction.CreateAlbum)
                    };
            }
        }

        private void AppBarRefreshAlbums_OnClick(object sender, EventArgs e)
        {
            if (ViewLoginPromptIfSessionEnded(PopupLogin.AlbumsPage) == false)
            {
                App.ViewModel.LoadData(true);    
            }
        }

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
    }
}