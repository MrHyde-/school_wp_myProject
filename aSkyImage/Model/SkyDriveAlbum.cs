using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace aSkyImage.ViewModel
{
    [DataContract]
    public class SkyDriveAlbum : INotifyPropertyChanged
    {
        public SkyDriveAlbum()
        {
            this.Photos = new ObservableCollection<SkyDrivePhoto>();
        }

        private ObservableCollection<SkyDrivePhoto> _photos; 
        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<SkyDrivePhoto> Photos 
        { 
            get 
            {
                if (_photos == null)
                {
                    _photos = new ObservableCollection<SkyDrivePhoto>();
                }
                return _photos;
            } 
            set
            {
                _photos = value;
            } 
        }

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

        private string _title = string.Empty;
        [DataMember(Name = "name")]
        public string Title
        {
            get
            {
                return this._title;
            }

            set
            {
                if (this._title != value)
                {
                    this._title = value;
                    this.NotifyPropertyChanged("Title");
                }
            }
        }

        private string _albumPicture = String.Empty;
        public string AlbumPicture
        {
            get
            {
                return this._albumPicture;
            }

            set
            {
                if (this._albumPicture != value)
                {
                    this._albumPicture = value;
                    this.NotifyPropertyChanged("AlbumPicture");
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