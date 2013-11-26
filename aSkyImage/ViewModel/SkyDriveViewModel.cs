using System;
using System.ComponentModel;

namespace aSkyImage.ViewModel
{
    public class SkyDriveViewModel
    {
        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
