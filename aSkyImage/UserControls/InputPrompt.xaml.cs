using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace aSkyImage.UserControls
{
    public partial class InputPrompt : UserControl
    {
        private String _caption;
        public InputPrompt()
        {
            InitializeComponent();
        }

        public InputPrompt(string title)
        {
            Loaded += OnLoaded;
            _caption = title;
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
            
            App.ViewModel.CreateAlbum(TextBoxUserInput.Text);
            ClosePopup();
        }
    }
}
