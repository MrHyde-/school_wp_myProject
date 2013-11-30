using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using aSkyImage.Model;
using aSkyImage.Resources;
using aSkyImage.UserControls;
using aSkyImage.ViewModel;

namespace aSkyImage.View
{
    /// <summary>
    /// View that displays users single photo and its comments
    /// </summary>
    public partial class PhotoPage : PhoneApplicationPage
    {
        private Popup _popup = null;

        public PhotoPage()
        {
            InitializeComponent();
            Loaded += PhotoPage_Loaded;
        }

        /// <summary>
        /// After page is loaded do some ui tricks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PhotoPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.PhotoViewModel.SelectedPhoto != null)
            {
                //if photo has long name make title smaller so it would fit to the screen..
                if (App.PhotoViewModel.SelectedPhoto.Title.Length > 30)
                {
                    PhotoTitle.Style = (Style) Resources["PhoneTextTitle3Style"];
                }
                
                //localize application bar and disable//enable commenting if skydrive allows it to this photo
                if (ApplicationBar.Buttons.Count > 0)
                {
                    var addCommentButton = (ApplicationBar.Buttons[0] as ApplicationBarIconButton);
                    addCommentButton.IsEnabled = App.PhotoViewModel.SelectedPhoto.CommentingEnabled;
                    addCommentButton.Text = AppResources.PhotoPageAppBarAddNewComment;
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.AlbumPageAppBarDownload;
                }

                //localize application bar menu item
                if (ApplicationBar.MenuItems.Count > 0)
                {
                    (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.CommonRefresh;
                }

                DataContext = App.PhotoViewModel.SelectedPhoto;
            }
        }

        /// <summary>
        /// When navigated to this page ensure some data.. (tombstone)
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //before loading we have to make sure we have LiveSession if not get data from the State
            if (App.LiveSession == null)
            {
                App.PhotoViewModel.SelectedPhoto = (SkyDrivePhoto)State[App.SelectedPhotoKey];
                App.PhotoViewModel.SelectedPhoto.Comments = (ObservableCollection<SkyDriveComment>)State[App.SelectedPhotoCommentsKey];
            }
        }

        /// <summary>
        /// When navigated away from this page ensure data saving (tombstone).. and close popoup if it is open
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // If this is a back navigation, the page will be discarded, so there
            // is no need to save state.
            if (e.NavigationMode != System.Windows.Navigation.NavigationMode.Back)
            {
                // Save the Session variable in the page's State dictionary.
                State[App.SelectedPhotoKey] = App.PhotoViewModel.SelectedPhoto;
                State[App.SelectedPhotoCommentsKey] = App.PhotoViewModel.SelectedPhoto.Comments;
            }
            else
            {
                //on navigate back close popup if it is still open
                if (_popup != null)
                {
                    _popup.IsOpen = false;
                    _popup = null;
                }
            }
        }

        /// <summary>
        /// Show add new comment popup when users presses the add comment button from the application bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            if (_popup != null)
            {
                _popup.IsOpen = false;
                _popup = null;
            }

            _popup = new Popup() { IsOpen = true, Child = new InputPrompt(AppResources.PhotoPageAddCommentTitle, PopupAction.AddCommentToPhoto) };
        }

        /// <summary>
        /// Refresh data when refresh is pressed from the application bar menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarRefreshPhoto_OnClick(object sender, EventArgs e)
        {
            if (App.PhotoViewModel.SelectedPhoto != null)
            {
                App.PhotoViewModel.LoadPhotoComments(App.PhotoViewModel.SelectedPhoto);
            }
        }

        /// <summary>
        /// Download photo to phone when users presses the download icon from the application bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarDownloadButton_OnClick(object sender, EventArgs e)
        {
            App.PhotoViewModel.Download();
        }
    }
}