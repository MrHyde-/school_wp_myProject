using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls;
using aSkyImage.UserControls;

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
            base.OnNavigatedTo(e);

            //before loading we have to make sure we have LiveSession
            if (App.LiveSession == null)
            {
                //need to go to the mainpage for login event..
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                return;
            }

            App.ViewModel.LoadData();
            DataContext = App.ViewModel;
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