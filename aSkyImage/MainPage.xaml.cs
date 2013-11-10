using System;
using Microsoft.Live;
using Microsoft.Live.Controls;
using Microsoft.Phone.Controls;

namespace aSkyImage
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void SignInSkyDriveButton_OnSessionChanged(object sender, LiveConnectSessionChangedEventArgs e)
        {
            if (e.Status == LiveConnectSessionStatus.Connected)
            {
                App.LiveSession = e.Session;
                NavigationService.Navigate(new Uri("/View/AlbumPage.xaml", UriKind.Relative));
            }
        }
    }
}