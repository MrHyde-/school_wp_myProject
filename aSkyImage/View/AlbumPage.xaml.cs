using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;

namespace aSkyImage.View
{
    public partial class AlbumPage : PhoneApplicationPage
    {
        public AlbumPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            PageTitle.Text = App.ViewModel.SelectedAlbum.Title;
            App.ViewModel.LoadAlbumData();
            DataContext = App.ViewModel;
        }

        private void AppIconUpload_OnClick(object sender, EventArgs e)
        {
            App.ViewModel.Upload();
        }

        private void PhotoListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (App.ViewModel.SelectedPhoto != null)
            {
                var selectedDataObject = e.AddedItems[0]; // assuming single selection
                ChangeItemForegroundColor(selectedDataObject, Colors.Red);

                if (e.RemovedItems.Count > 0)
                {
                    var removedObject = e.RemovedItems[0]; // assuming single selection
                    ChangeItemForegroundColor(removedObject, (Color)Resources["PhoneForegroundColor"]);    
                }
            }
        }

        private void ChangeItemForegroundColor(object selectedDataObject, Color newColor)
        {
            ListBoxItem selectedItem = (ListBoxItem) (PhotoListBox.ItemContainerGenerator.ContainerFromItem(selectedDataObject));
            selectedItem.Foreground = new SolidColorBrush(newColor);
        }

        private void AppIconDownload_OnClick(object sender, EventArgs e)
        {
            App.ViewModel.Download();
        }

        private void AppIconShowImage_OnClick(object sender, EventArgs e)
        {
            if (App.ViewModel.SelectedPhoto != null)
            {
                //move to photo page
                NavigationService.Navigate(new Uri("/View/PhotoPage.xaml", UriKind.Relative));
                return;
            }
            
            MessageBox.Show("Please select a photo.");
        }
    }
}