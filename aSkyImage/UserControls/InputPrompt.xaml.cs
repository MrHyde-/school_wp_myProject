using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

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
            PromptTitle.Text = _caption;
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
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
        }
    }
}
