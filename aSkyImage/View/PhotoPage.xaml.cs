using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
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
                ImageUsersImage.Source = new BitmapImage(new Uri(App.ViewModel.SelectedPhoto.PhotoUrl, UriKind.RelativeOrAbsolute));
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = App.ViewModel.SelectedPhoto.CommentingEnabled;

                foreach (var comment in App.ViewModel.SelectedPhoto.Comments)
                {
                    var textBlockComment = new System.Windows.Controls.TextBlock();

                    textBlockComment.Style = (Style) Resources["PhoneTextSmallStyle"];
                    textBlockComment.Text = comment.CommentedBy.UserName + ": " + comment.Message;

                    PhotoComments.Children.Add(textBlockComment);
                }
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //before loading we have to make sure we have LiveSession
            if (App.LiveSession == null)
            {
                App.ViewModel.SelectedPhoto = (SkyDrivePhoto) State[App.SelectedPhotoKey];
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
            }
        }

        private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            if (_popup != null)
            {
                _popup.IsOpen = false;
                _popup = null;
            }

            _popup = new Popup() { IsOpen = true, Child = new InputPrompt("Write your comment", PopupAction.AddCommentToPhoto) };
        }
    }
}