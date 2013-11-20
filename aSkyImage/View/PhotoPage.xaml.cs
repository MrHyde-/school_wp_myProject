using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace aSkyImage.View
{
    public partial class PhotoPage : PhoneApplicationPage
    {
        public PhotoPage()
        {
            InitializeComponent();
            Loaded += PhotoPage_Loaded;
        }

        void PhotoPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.ViewModel.SelectedPhoto != null)
            {
                ImageUsersImage.Source = new BitmapImage(new Uri(App.ViewModel.SelectedPhoto.PhotoUrl, UriKind.RelativeOrAbsolute));
                
                //tweak width length to show max at screen..
            }
        }
    }
}