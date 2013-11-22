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
                
                //now we should inform user that we are downloading he's albums..
                textBlockStatus.Text = "Loading your albums..";
                App.ViewModel.LoadData();

                App.ViewModel.DataLoaded += viewModeldata_loaded;    
            }
        }

        private void viewModeldata_loaded(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/AlbumsPage.xaml", UriKind.Relative));
        }
    }
}