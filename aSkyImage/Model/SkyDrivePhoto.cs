using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace aSkyImage.ViewModel
{
    [DataContract]
    public class SkyDrivePhoto : INotifyPropertyChanged
    {
        private string _id = string.Empty;
        [DataMember(Name = "id")]
        public string ID
        {
            get
            {
                return this._id;
            }

            set
            {
                if (this._id != value)
                {
                    this._id = value;
                    this.NotifyPropertyChanged("ID");
                }
            }
        }

        private string _description = string.Empty;
        [DataMember(Name = "description")]
        public string Description
        {
            get
            {
                return this._description;
            }

            set
            {
                if (this._description != value)
                {
                    this._description = value;
                    this.NotifyPropertyChanged("Description");
                }
            }
        }

        private string _photoUrl;
        [DataMember(Name = "source")]
        public string PhotoUrl
        {
            get
            {
                return _photoUrl;
            }
            set
            {
                if (value != _photoUrl)
                {
                    _photoUrl = value;
                    NotifyPropertyChanged("PhotoUrl");
                }
            }
        }

        private string _photoThumbnailUrl;
        public string PhotoThumbnailUrl
        {
            get
            {
                return _photoThumbnailUrl;
            }
            set
            {
                if (value != _photoThumbnailUrl)
                {
                    _photoThumbnailUrl = value;
                    NotifyPropertyChanged("PhotoThumbnailUrl");
                }
            }
        }

        private string _subtitle;
        public string Subtitle
        {
            get
            {
                return _subtitle;
            }
            set
            {
                if (value != _subtitle)
                {
                    _subtitle = value;
                    NotifyPropertyChanged("Subtitle");
                }
            }
        }


        private string _title;
        [DataMember(Name = "name")]
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }
}