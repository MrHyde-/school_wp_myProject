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
            //for localizing text in the application bar
            if (ApplicationBar.Buttons.Count > 0)
            {
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.MainPageAppBarAlbums;
            }
        }

        /// <summary>
        /// After data is loaded transfer user to albums view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewModeldata_loaded(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/AlbumsPage.xaml", UriKind.Relative));
            ApplicationBar.IsVisible = true;
            textBlockStatus.Text = AppResources.MainPageStatusLoadedAlbums;
        }
        
        /// <summary>
        /// Go to albums when show albums is pressed from the application bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppIconViewFolders_OnClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/AlbumsPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// if there are no active session tell it to the user (example logout)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewModelData_noactiveSession(object sender, EventArgs e)
        {
            textBlockStatus.Text = AppResources.MainPageStatusPleaseLogin;
            ApplicationBar.IsVisible = false;
        }
    }
}