using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Live;
using Microsoft.Phone.Controls;
using aSkyImage.UserControls;

namespace aSkyImage.View
{
    public partial class AlbumPage : PhoneApplicationPage
    {
        private Popup _popup = null;

        public AlbumPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.ViewModel.LoadData();
            DataContext = App.ViewModel;

            //var client = new LiveConnectClient(App.LiveSession);
            //client.GetCompleted += ClientOnGetCompleted;
            //client.GetAsync("/me/albums/");
        }

        //private void ClientOnGetCompleted(object sender, LiveOperationCompletedEventArgs e)
        //{
        //    if (e.Error == null)
        //    {
        //        List<object> data = (List<object>)e.Result["data"];
        //        foreach (IDictionary<string, object> content in data)
        //        {
        //            Debug.WriteLine("Name: " + (string)content["name"]);
        //        }
        //    }
        //}

        private void AlbumListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (App.ViewModel.SelectedAlbum != null)
            {
                NavigationService.Navigate(new Uri("/AlbumDetailPage.xaml", UriKind.Relative));
            }
        }

        private void AppIconNewFolder_OnClick(object sender, EventArgs e)
        {
            //ask user a album name
            if (_popup != null)
            {
                _popup.IsOpen = false;
                _popup = null;
            }
            _popup = new Popup() { IsOpen = true, Child = new InputPrompt("Please give your album a name") };           
            //create it to skydrive.albums
        }
    }
}