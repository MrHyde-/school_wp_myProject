using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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

            if (thisPopup != null)
            {
                thisPopup.VerticalOffset = 10d;
            }
            PromptTitle.Text = _caption;
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
                    App.ViewModel.CreateAlbum(TextBoxUserInput.Text);
                    break;
                case PopupAction.AddCommentToPhoto:
                    App.ViewModel.AddCommentToPhoto(TextBoxUserInput.Text);
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
