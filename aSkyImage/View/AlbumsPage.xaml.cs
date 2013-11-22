using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls;
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
                    if (App.ViewModel.SelectedAlbum != null)
                    {
                        State[App.AlbumsKey] = App.ViewModel.Albums;
                    }
                }
            }
        }

        private void AlbumListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (App.ViewModel.SelectedAlbum != null)
            {
                App.ViewModel.AlbumDataLoaded = false;
                NavigationService.Navigate(new Uri("/View/AlbumPage.xaml", UriKind.Relative));
            }
        }

        private void AppIconNewFolder_OnClick(object sender, EventArgs e)
        {
            if (_popup != null)
            {
                _popup.IsOpen = false;
                _popup = null;
            }

            _popup = new Popup() { IsOpen = true, Child = new InputPrompt("Please give your album a name") };           
        }
    }
}