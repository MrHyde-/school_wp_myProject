using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using aSkyImage.Resources;

namespace aSkyImage
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (ApplicationBar.Buttons.Count > 0)
            {
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.MainPageAppBarAlbums;
            }
        }

        private void viewModeldata_loaded(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/AlbumsPage.xaml", UriKind.Relative));
            ApplicationBar.IsVisible = true;
            textBlockStatus.Text = AppResources.MainPageStatusLoadedAlbums;
        }

        private void AppIconViewFolders_OnClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/AlbumsPage.xaml", UriKind.Relative));
        }
    }
}