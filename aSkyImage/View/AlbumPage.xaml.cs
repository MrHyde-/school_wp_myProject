using System;
using System.Windows;
using System.Windows.Controls;
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
            if (App.ViewModel.SelectedPhoto == null)
            {
                
            }
        }

        private void AppIconDownload_OnClick(object sender, EventArgs e)
        {
            App.ViewModel.Download();
        }
    }
}