using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Live;
using Microsoft.Phone.Controls;

namespace aSkyImage.View
{
    public partial class AlbumPage : PhoneApplicationPage
    {
        public AlbumPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var client = new LiveConnectClient(App.LiveSession);
            client.GetCompleted += ClientOnGetCompleted;
            client.GetAsync("/me/albums/");
        }

        private void ClientOnGetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                List<object> data = (List<object>)e.Result["data"];
                foreach (IDictionary<string, object> content in data)
                {
                    Debug.WriteLine("Name: " + (string)content["name"]);
                }
            }
        }
    }
}