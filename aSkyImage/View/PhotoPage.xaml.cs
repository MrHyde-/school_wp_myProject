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
    public partial class PhotoPage : PhoneApplicationPage
    {
        private Popup _popup = null;

        public PhotoPage()
        {
            InitializeComponent();
            Loaded += PhotoPage_Loaded;
        }

        private void PhotoPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.ViewModel.SelectedPhoto != null)
            {
                if (App.ViewModel.SelectedPhoto.Title.Length > 30)
                {
                    PhotoTitle.Style = (Style) Resources["PhoneTextTitle3Style"];
                }
                
                if (ApplicationBar.Buttons.Count > 0)
                {
                    var addCommentButton = (ApplicationBar.Buttons[0] as ApplicationBarIconButton);
                    addCommentButton.IsEnabled = App.ViewModel.SelectedPhoto.CommentingEnabled;
                    addCommentButton.Text = AppResources.PhotoPageAppBarAddNewComment;
                }

                if (ApplicationBar.MenuItems.Count > 0)
                {
                    (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.CommonRefresh;
                }

                DataContext = App.ViewModel.SelectedPhoto;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //before loading we have to make sure we have LiveSession
            if (App.LiveSession == null)
            {
                App.ViewModel.SelectedPhoto = (SkyDrivePhoto) State[App.SelectedPhotoKey];
                App.ViewModel.SelectedPhoto.Comments = (ObservableCollection<SkyDriveComment>) State[App.SelectedPhotoCommentsKey];
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // If this is a back navigation, the page will be discarded, so there
            // is no need to save state.
            if (e.NavigationMode != System.Windows.Navigation.NavigationMode.Back)
            {
                // Save the Session variable in the page's State dictionary.
                State[App.SelectedPhotoKey] = App.ViewModel.SelectedPhoto;
                State[App.SelectedPhotoCommentsKey] = App.ViewModel.SelectedPhoto.Comments;
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

        private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            if (_popup != null)
            {
                _popup.IsOpen = false;
                _popup = null;
            }

            _popup = new Popup() { IsOpen = true, Child = new InputPrompt(AppResources.PhotoPageAddCommentTitle, PopupAction.AddCommentToPhoto) };
        }

        private void AppBarRefreshPhoto_OnClick(object sender, EventArgs e)
        {
            if (App.ViewModel.SelectedPhoto != null)
            {
                App.ViewModel.LoadPhotoComments(App.ViewModel.SelectedPhoto);
            }
        }
    }
}