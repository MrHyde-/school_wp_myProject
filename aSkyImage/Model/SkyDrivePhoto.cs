using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using aSkyImage.Model;

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

        private string _photoThumbnailUrl = String.Empty;
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

        private bool _commentsEnabled;
        [DataMember(Name = "comments_enabled")]
        public bool CommentingEnabled
        {
            get
            { 
                return _commentsEnabled;
            }
            set
            {
                if (value != _commentsEnabled)
                {
                    _commentsEnabled = value;
                    NotifyPropertyChanged("CommentingEnabled");
                }
            }
        }

        [DataMember(Name = "shared_with")]
        public SkyDriveAccess PhotoAccess { get; set; }

        [DataMember(Name = "images")]
        public List<SkyDrivePhotoImage> PhotoImages { get; set; }

        private ObservableCollection<SkyDriveComment> _comments;
        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<SkyDriveComment> Comments
        {
            get
            {
                if (_comments == null)
                {
                    _comments = new ObservableCollection<SkyDriveComment>();
                }
                return _comments;
            }
            set
            {
                _comments = value;
                NotifyPropertyChanged("Comments");
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