using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Phone.Controls;

namespace aSkyImage.UserControls
{
    public partial class InputPrompt : UserControl
    {
        private String _caption;
        private PopupAction _action;
        public InputPrompt()
        {
            InitializeComponent();
        }

        public InputPrompt(string title, PopupAction action)
        {
            Loaded += OnLoaded;
            _caption = title;
            _action = action;
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Popup thisPopup = this.Parent as Popup;

            ((PhoneApplicationFrame)Application.Current.RootVisual).OrientationChanged += OnOrientationChanged;

            if (thisPopup != null)
            {
                var orientation = ((PhoneApplicationFrame) Application.Current.RootVisual).Orientation;
                SetPopupLocationAndProjectionByOrientation(orientation, thisPopup);
            }

            PromptTitle.Text = _caption;
        }

        private void SetPopupLocationAndProjectionByOrientation(PageOrientation orientation, Popup thisPopup)
        {
            if (orientation == PageOrientation.LandscapeLeft || orientation == PageOrientation.LandscapeRight)
            {
                TextBoxUserInput.Width = 660d;

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

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            ClosePopup();
        }

        private void ClosePopup()
        {
            Popup thisPopup = this.Parent as Popup;

            if (thisPopup != null)
            {
                thisPopup.IsOpen = false;
            }
        }

        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(TextBoxUserInput.Text))
            {
                //say no no you bloody API8
                return;
            }

            switch (_action)
            {
                case PopupAction.CreateAlbum:
                    App.AlbumsViewModel.CreateAlbum(TextBoxUserInput.Text);
                    break;
                case PopupAction.AddCommentToPhoto:
                    App.PhotoViewModel.AddCommentToPhoto(TextBoxUserInput.Text);
                    break;
            }
            
            ClosePopup();
        }
    }

    public enum PopupAction
    {
        Undefined = 0,
        CreateAlbum = 1,
        AddCommentToPhoto = 2,
    }
}
