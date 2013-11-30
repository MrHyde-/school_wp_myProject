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
    /// <summary>
    /// Custom control that holds the Microsoft.Live.Controls.SignInButton
    /// </summary>
    public partial class LoginPrompt : UserControl
    {
        //control is used also at the mainpage and we do not want to show the title there..
        private bool _titleShown = true;
        public bool TitleShown { get { return _titleShown; } set { _titleShown = value; } }

        private PopupLogin _promtPage;
        
        #region eventsForSessionHandling
        public event EventHandler<EventArgs> LoginCompleted;

        public event EventHandler<EventArgs> NoActiveSession;

        protected virtual void OnLoginCompleted()
        {
            EventHandler<EventArgs> handler = LoginCompleted;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnNoActiveSession()
        {
            EventHandler<EventArgs> handler = NoActiveSession;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        #endregion eventsForSessionHandling

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

        /// <summary>
        /// When control is loaded ensure things
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            //signInSkyDriveButton.ClientId = ;

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

        /// <summary>
        /// After popup is loaded ensure that it is located correctly
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="thisPopup"></param>
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

        /// <summary>
        /// Event that is called when orientation changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            Popup thisPopup = this.Parent as Popup;
            if (thisPopup != null)
            {
                SetPopupLocationAndProjectionByOrientation(e.Orientation, thisPopup);
            }
        }

        /// <summary>
        /// Sets projections and offsets to popup to keep it in the top of the screen
        /// </summary>
        /// <param name="thisPopup"></param>
        /// <param name="rotationZ"></param>
        /// <param name="verticalOffset"></param>
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

        /// <summary>
        /// When user sign in make sure to start data loading and refresh correct page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            if (e.Status == LiveConnectSessionStatus.Unknown)
            {
                //this hapens because tombstoning, because LiveConnectSession is not serializable we have to ensure that we close the inactive connection
                if (App.LiveSession != null)
                {
                    App.LiveSession = null;
                }
                OnNoActiveSession();
            }
        }
    }
}
