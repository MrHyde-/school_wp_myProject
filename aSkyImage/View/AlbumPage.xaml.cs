﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using aSkyImage.Model;
using aSkyImage.Resources;
using aSkyImage.UserControls;
using aSkyImage.ViewModel;

namespace aSkyImage.View
{
    public partial class AlbumPage : PhoneApplicationPage
    {
        private Popup _popup = null;

        public AlbumPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            LocalizeApplicationBar();
            PageTitle.Text = App.ViewModel.SelectedAlbum.Title;
            App.ViewModel.LoadAlbumData();

            if (App.ViewModel.SelectedPhoto == null)
            {
                if (ApplicationBar.Buttons.Count > 2)
                {
                    (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = false;
                }
            }

            DataContext = App.ViewModel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //before loading we have to make sure we have LiveSession
            if (App.LiveSession == null)
            {
                //If was tombstoned check state
                if (State.ContainsKey(App.SelectedAlbumKey))
                {
                    App.ViewModel.SelectedAlbum = (SkyDriveAlbum) State[App.SelectedAlbumKey];
                    if (State.ContainsKey(App.SelectedAlbumPhotosKey))
                    {
                        App.ViewModel.SelectedAlbum.Photos = (ObservableCollection<SkyDrivePhoto>)State[App.SelectedAlbumPhotosKey];
                    }
                    App.ViewModel.AlbumDataLoaded = true;
                }

                ChangeApplicationBarIconStatus(false);
            }
        }

        private void ChangeApplicationBarIconStatus(bool isEnabled)
        {
            if (ApplicationBar.Buttons.Count > 2)
            {
                //disable appbar buttons if no session
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = isEnabled;
                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = isEnabled;
                (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = isEnabled;
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // If this is a back navigation, the page will be discarded, so there
            // is no need to save state.
            if (e.NavigationMode != System.Windows.Navigation.NavigationMode.Back)
            {
                // Save the Session variable in the page's State dictionary.
                if (App.ViewModel != null)
                {
                    if (App.ViewModel.SelectedAlbum != null)
                    {
                        State[App.SelectedAlbumKey] = App.ViewModel.SelectedAlbum;
                        State[App.SelectedAlbumPhotosKey] = App.ViewModel.SelectedAlbum.Photos;
                    }
                }
            }
            else
            {
                //without this we are not able to navigate straight back to same album..
                App.ViewModel.SelectedAlbum.Photos.Clear(); 
                App.ViewModel.SelectedAlbum = null;
            }
        }

        private void AppIconUpload_OnClick(object sender, EventArgs e)
        {
            App.ViewModel.Upload();
        }

        private void PhotoListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewLoginPromptIfSessionEnded() == false)
            {
                if (App.ViewModel.SelectedPhoto != null)
                {
                    if (ApplicationBar.Buttons.Count > 2)
                    {
                        (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;
                    }

                    App.ViewModel.LoadPhotoComments(App.ViewModel.SelectedPhoto);

                    var selectedDataObject = e.AddedItems[0]; // assuming single selection
                    ChangeItemForegroundColor(selectedDataObject, Colors.Red);

                    if (e.RemovedItems.Count > 0)
                    {
                        var removedObject = e.RemovedItems[0]; // assuming single selection
                        ChangeItemForegroundColor(removedObject, (Color)Resources["PhoneForegroundColor"]);
                    }
                }   
            }
        }

        private void ChangeItemForegroundColor(object selectedDataObject, Color newColor)
        {
            if (PhotoListBox.ItemContainerGenerator != null)
            {
                ListBoxItem selectedItem = (ListBoxItem)(PhotoListBox.ItemContainerGenerator.ContainerFromItem(selectedDataObject));
                selectedItem.Foreground = new SolidColorBrush(newColor);    
            }
        }

        private void AppIconDownload_OnClick(object sender, EventArgs e)
        {
            App.ViewModel.Download();
        }

        private void AppIconShowImage_OnClick(object sender, EventArgs e)
        {
            if (ViewLoginPromptIfSessionEnded() == false)
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

        private bool ViewLoginPromptIfSessionEnded()
        {
            if (App.LiveSession == null)
            {
                if (_popup != null)
                {
                    _popup.IsOpen = false;
                    _popup = null;
                }

                var childPopup = new LoginPrompt(PopupLogin.AlbumPage);
                childPopup.LoginCompleted += loginCompleted;

                _popup = new Popup() { IsOpen = true, Child = childPopup };

                return true;
            }
            return false;
        }

        private void AppIconPin_OnClick(object sender, EventArgs e)
        {
            //need to create the tile(?)
            // Application Tile is always the first Tile, even if it is not pinned to Start.
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("AlbumPage.xaml"));

            // Application should always be found
            if (TileToFind == null)
            {
                // if Count was not entered, then assume a value of 0
                int newCount = App.ViewModel.SelectedAlbum.Photos.Count;

                // Set the properties to update for the Application Tile.
                // Empty strings for the text values and URIs will result in the property being cleared.
                StandardTileData NewTileData = new StandardTileData
                {
                    Title = "Your photos",
                    BackgroundImage = new Uri("ApplicationIcon.png", UriKind.Relative),
                    Count = newCount,
                    //BackTitle = "Your albums",
                    //BackBackgroundImage = new Uri("Background.png", UriKind.Relative),
                    //BackContent = "content?"
                };

                // Update the Application Tile
                ShellTile.Create(new Uri("/View/AlbumPage.xaml", UriKind.Relative), NewTileData);
            }
            else
            {
                MessageBox.Show("A tile exists already.");
            }
        }

        private void AppBarRefreshAlbum_OnClick(object sender, EventArgs e)
        {
            if (App.LiveSession == null)
            {
                //open popup please login
                if (_popup != null)
                {
                    _popup.IsOpen = false;
                    _popup = null;
                }

                var childPopup = new LoginPrompt(PopupLogin.AlbumPage);
                childPopup.LoginCompleted += loginCompleted;

                _popup = new Popup() { IsOpen = true, Child = childPopup };
                
            }
            else
            {
                App.ViewModel.AlbumDataLoaded = false;
                App.ViewModel.LoadAlbumData();    
            }
        }

        private void loginCompleted(object sender, EventArgs e)
        {
            //activate application bar icons
            ChangeApplicationBarIconStatus(true);
        }

        private void LocalizeApplicationBar()
        {
            if (ApplicationBar.Buttons.Count > 2)
            {
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.AlbumPageAppBarUpload;
                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.AlbumPageAppBarDownload;
                (ApplicationBar.Buttons[2] as ApplicationBarIconButton).Text = AppResources.AlbumPageAppBarZoom;
            }
            if (ApplicationBar.MenuItems.Count > 0)
            {
                (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.CommonRefresh;
            }
        }
    }
}