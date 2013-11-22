using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using aSkyImage.ViewModel;

namespace aSkyImage.View
{
    public partial class PhotoPage : PhoneApplicationPage
    {
        public PhotoPage()
        {
            InitializeComponent();
            Loaded += PhotoPage_Loaded;
        }

        private void PhotoPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.ViewModel.SelectedPhoto != null)
            {
                ImageUsersImage.Source = new BitmapImage(new Uri(App.ViewModel.SelectedPhoto.PhotoUrl, UriKind.RelativeOrAbsolute));
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //before loading we have to make sure we have LiveSession
            if (App.LiveSession == null)
            {
                App.ViewModel.SelectedPhoto = (SkyDrivePhoto) State[App.SelectedPhotoKey];
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // If this is a back navigation, the page will be discarded, so there
            // is no need to save state.
            if (e.NavigationMode != System.Windows.Navigation.NavigationMode.Back)
            {
                // Save the Session variable in the page's State dictionary.
                State[App.SelectedPhotoKey] = App.ViewModel.SelectedPhoto;
            }
        }
    }
}