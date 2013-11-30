using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Phone.Controls;

namespace aSkyImage.UserControls
{
    /// <summary>
    /// Popup class to display input field with custom title
    /// </summary>
    public partial class InputPrompt : UserControl
    {
        private String _caption;
        private PopupAction _action;
        public InputPrompt()
        {
            InitializeComponent();
        }

        /// <summary>
        /// constructor for the popup
        /// </summary>
        /// <param name="title"></param>
        /// <param name="action"></param>
        public InputPrompt(string title, PopupAction action)
        {
            Loaded += OnLoaded;
            _caption = title;
            _action = action;
            InitializeComponent();
        }

        /// <summary>
        /// After popup is loaded attach event for orientation change support and set title
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
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

        /// <summary>
        /// After popup is loaded ensure that it is located correctly
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="thisPopup"></param>
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
        /// <param name="horisontalOffset"></param>
        private void SetPopupOffsetsToLandscape(Popup thisPopup, double rotationZ, double verticalOffset, double horisontalOffset)
        {
            var planeProjection = new PlaneProjection();
            planeProjection.RotationZ = rotationZ;
            LayoutRoot.Projection = planeProjection;
            thisPopup.VerticalOffset = verticalOffset;
            thisPopup.HorizontalOffset = horisontalOffset;
        }

        /// <summary>
        /// Event that occurs when users cancels his popup action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            ClosePopup();
        }

        /// <summary>
        /// Method for closing the popup
        /// </summary>
        private void ClosePopup()
        {
            Popup thisPopup = this.Parent as Popup;

            if (thisPopup != null)
            {
                thisPopup.IsOpen = false;
            }
        }

        /// <summary>
        /// Event that occurs when users confirms his popup action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

    /// <summary>
    /// Enumeration to define which method has opened the popup to define what action is called when users presses ok button
    /// </summary>
    public enum PopupAction
    {
        Undefined = 0,
        CreateAlbum = 1,
        AddCommentToPhoto = 2,
    }
}
