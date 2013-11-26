using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Live;
using Microsoft.Live.Controls;
using Microsoft.Phone.Controls;
using aSkyImage.Model;

namespace aSkyImage.UserControls
{
    public partial class LoginPrompt : UserControl
    {
        private bool _titleShown = true;
        public bool TitleShown { get { return _titleShown; } set { _titleShown = value; } }

        private PopupLogin _promtPage;
        public event EventHandler<EventArgs> LoginCompleted;

        protected virtual void OnLoginCompleted()
        {
            EventHandler<EventArgs> handler = LoginCompleted;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public LoginPrompt()
        {
            _promtPage = PopupLogin.Undefined;
            Loaded += OnLoaded;
            InitializeComponent();
        }

        public LoginPrompt(PopupLogin promptPage)
        {
            _promtPage = promptPage;
            Loaded += OnLoaded;
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (TitleShown == false)
            {
                PromptTitle.Visibility = Visibility.Collapsed;
            }
            Popup thisPopup = this.Parent as Popup;

            ((PhoneApplicationFrame)Application.Current.RootVisual).OrientationChanged += OnOrientationChanged;

            if (thisPopup != null)
            {
                var orientation = ((PhoneApplicationFrame) Application.Current.RootVisual).Orientation;
                SetPopupLocationAndProjectionByOrientation(orientation, thisPopup);
            }
        }

        private void SetPopupLocationAndProjectionByOrientation(PageOrientation orientation, Popup thisPopup)
        {
            if (orientation == PageOrientation.LandscapeLeft || orientation == PageOrientation.LandscapeRight)
            {
                if (orientation == PageOrientation.LandscapeRight)
                {
                    SetPopupOffsetsToLandscape(thisPopup, 90d, 300d, -240d);
                }
                else
                {
                    SetPopupOffsetsToLandscape(thisPopup, -90d, 300d, 50d);
                }
            }
            else
            {
                SetPopupOffsetsToLandscape(thisPopup, 0d, 10d, 0d);
            }
        }

        private void OnOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            Popup thisPopup = this.Parent as Popup;
            if (thisPopup != null)
            {
                SetPopupLocationAndProjectionByOrientation(e.Orientation, thisPopup);
            }
        }

        private void SetPopupOffsetsToLandscape(Popup thisPopup, double rotationZ, double verticalOffset, double horisontalOffset)
        {
            var planeProjection = new PlaneProjection();
            planeProjection.RotationZ = rotationZ;
            LayoutRoot.Projection = planeProjection;
            thisPopup.VerticalOffset = verticalOffset;
            thisPopup.HorizontalOffset = horisontalOffset;
        }

        private void ClosePopup()
        {
            Popup thisPopup = this.Parent as Popup;

            if (thisPopup != null)
            {
                thisPopup.IsOpen = false;
            }
        }

        private void SignInSkyDriveButton_OnSessionChanged(object sender, LiveConnectSessionChangedEventArgs e)
        {
            if (e.Status == LiveConnectSessionStatus.Connected)
            {
                App.LiveSession = e.Session;

                //depending where user is refresh that data..
                if (_promtPage == PopupLogin.PhotoPage)
                {
                    //refresh selectedphoto data
                    App.PhotoViewModel.LoadPhotoComments(App.PhotoViewModel.SelectedPhoto);
                }
                else if (_promtPage == PopupLogin.AlbumPage)
                {
                    App.AlbumViewModel.AlbumDataLoaded = false;
                    App.AlbumViewModel.LoadSingleAlbumData();
                }
                else
                {
                    App.AlbumsViewModel.LoadAlbumsData(true);
                }

                OnLoginCompleted();
                ClosePopup();
            }
        }
    }
}
