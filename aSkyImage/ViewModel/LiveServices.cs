using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Windows;
using Microsoft.Live;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using aSkyImage.Model;

namespace aSkyImage.ViewModel
{
    public class LiveServices
    {
        public ObservableCollection<SkyDriveAlbum> Albums { get; set; }

        public LiveServices()
        {
            this.Albums = new ObservableCollection<SkyDriveAlbum>(); 
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            GetAlbumData();
            this.IsDataLoaded = true;
        }

        private void GetAlbumData()
        {
            if (IsDataLoaded == false)
            {
                LiveConnectClient clientFolder = new LiveConnectClient(App.LiveSession);
                clientFolder.GetCompleted += clientFolder_GetCompleted;
                clientFolder.GetAsync("/me/albums");
            }
        }

        private void clientFolder_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                System.Diagnostics.Debug.WriteLine(e.Error.Message);
                return;
            }

            Albums.Clear();

            string jsonString = e.RawResult;

            //load into memory stream
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                //parse into jsonser
                // note that to using System.Runtime.Serialization.Json
                // need to add reference System.Servicemodel.Web
                var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(SkyDriveAlbumList));
                try
                {
                    var list = (SkyDriveAlbumList)ser.ReadObject(ms);
                    //Albums = list.SkyDriveAlbums;

                    foreach (var album in list.SkyDriveAlbums)
                    {
                        Albums.Add(album);
                        GetAlbumPicture(album);
                    }
                }
                catch (Exception je)
                {
                    System.Diagnostics.Debug.WriteLine("--- " + je.Message);
                }
            }
            
            OnDataLoaded();
        }

        protected bool IsDataLoaded { get; set; }

        private SkyDriveAlbum _selectedAlbum;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public SkyDriveAlbum SelectedAlbum
        {
            get
            {
                return _selectedAlbum;
            }
            set
            {
                _selectedAlbum = value;
                NotifyPropertyChanged("SelectedAlbum");
            }
        }

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private void GetAlbumPicture(SkyDriveAlbum albumItem)
        {
            if (albumItem != null)
            {
                if(String.IsNullOrEmpty(albumItem.ID) == false)
                {
                    LiveConnectClient albumPictureClient = new LiveConnectClient(App.LiveSession);
                    albumPictureClient.GetCompleted += albumPictureClient_GetCompleted;
                    albumPictureClient.GetAsync(albumItem.ID + "/picture?type=thumbnail", albumItem);
                }
            }
        }

        private void albumPictureClient_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                SkyDriveAlbum album = (SkyDriveAlbum)e.UserState;
                album.AlbumPicture = (string)e.Result["location"];
            }
        }

        public void CreateAlbum(string albumName)
        {
            LiveConnectClient albumClient = new LiveConnectClient(App.LiveSession);
            albumClient.PostCompleted += albumClient_PostCompleted;
            var albumData = new Dictionary<string, object>();
            albumData.Add("name", albumName);
            albumClient.PostAsync("/me/albums", albumData);
        }

        private void albumClient_PostCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                string newAlbumJson = e.RawResult;
                InsertNewAlbumToAlbumsCollection(newAlbumJson);
                MessageBox.Show("Album created", "Done", MessageBoxButton.OK);
            }
        }

        private void InsertNewAlbumToAlbumsCollection(string newAlbumJson)
        {
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(newAlbumJson)))
            {
                //parse into jsonser
                // note that to using System.Runtime.Serialization.Json
                // need to add reference System.Servicemodel.Web
                var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(SkyDriveAlbum));
                try
                {
                    var newAlbum = (SkyDriveAlbum)ser.ReadObject(ms);

                    if (String.IsNullOrEmpty(newAlbum.ID) == false)
                    {
                        newAlbum.Photos = new ObservableCollection<SkyDrivePhoto>();
                        Albums.Insert(0, newAlbum);
                    }
                }
                catch (Exception je)
                {
                    System.Diagnostics.Debug.WriteLine("--- " + je.Message);
                }
            }
        }

        internal void Upload()
        {
            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.ShowCamera = true;
            photoChooserTask.Completed += photoChooserTask_Completed;
            photoChooserTask.Show();
        }

        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.ChosenPhoto == null)
            {
                return;
            }

            string uploadLocation = "/shared/transfers/Image" + DateTime.Now.Millisecond + ".jpg";
            
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, args) =>
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = store.CreateFile(uploadLocation))
                    {
                        byte[] buffer = new byte[1 << 10];
                        int bytesRead;
                        while ((bytesRead = e.ChosenPhoto.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            stream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            };

            worker.RunWorkerCompleted += (o, args) =>
            {
                LiveConnectClient uploadClient = new LiveConnectClient(App.LiveSession);
                uploadClient.BackgroundUploadCompleted += uploadClient_BackgroundUploadCompleted;

                string userState = "myUserState";  // arbitrary string to identify the request.
                
                if(SelectedAlbum == null)
                {
                    if (Albums.Count > 0)
                    {
                        SelectedAlbum = Albums[0];
                    }
                }

                MessageBox.Show(String.Format("Uploading image to album: {0}", SelectedAlbum.Title));

                uploadClient.BackgroundUploadAsync(SelectedAlbum.ID, new Uri(uploadLocation, UriKind.RelativeOrAbsolute), OverwriteOption.Rename, userState);
            };

            worker.RunWorkerAsync();
        }

        private void uploadClient_BackgroundUploadCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                MessageBox.Show("Image uploaded");
                Deployment.Current.Dispatcher.BeginInvoke(() => DownloadPictures(SelectedAlbum));
            }
        }

        private void DownloadPictures(SkyDriveAlbum albumItem)
        {
            if (albumItem != null)
            {
                SelectedAlbum = albumItem;
                AlbumDataLoaded = false;
                LoadAlbumData();
            }
        }

        internal void LoadAlbumData()
        {
            if (SelectedAlbum != null)
            {
                if (AlbumDataLoaded == false)
                {
                    LiveConnectClient clientAlbum = new LiveConnectClient(App.LiveSession);
                    clientAlbum.GetCompleted += clientAlbum_GetCompleted;
                    clientAlbum.GetAsync(SelectedAlbum.ID + "/photos?type=album");
                }
            }
            else
            {
                MessageBox.Show("Problem with selected album..");
            }
        }

        internal bool AlbumDataLoaded { get; set; }

        private void clientAlbum_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                return;
            }

            SkyDriveAlbum album = SelectedAlbum;

            album.Photos.Clear();
            var photosJson = e.RawResult;

            //load into memory stream
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(photosJson)))
            {
                //parse into jsonser
                // note that to using System.Runtime.Serialization.Json
                // need to add reference System.Servicemodel.Web
                var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof (SkyDrivePhotoList));
                try
                {
                    var list = (SkyDrivePhotoList) ser.ReadObject(ms);
                    //Albums = list.SkyDriveAlbums;

                    foreach (var photo in list.SkyDrivePhotos)
                    {
                        album.Photos.Add(photo);
                        GetPhotoThumbnailPicture(photo);
                    }
                }
                catch (Exception je)
                {
                    System.Diagnostics.Debug.WriteLine("--- " + je.Message);
                }
            }

            AlbumDataLoaded = true;
        }

        private void GetPhotoThumbnailPicture(SkyDrivePhoto photoItem)
        {
            if (photoItem != null)
            {
                if(String.IsNullOrEmpty(photoItem.ID) == false)
                {
                    LiveConnectClient photoThumbnailClient = new LiveConnectClient(App.LiveSession);
                    photoThumbnailClient.GetCompleted += photoThumbnailClient_GetCompleted;
                    photoThumbnailClient.GetAsync(photoItem.ID + "/picture?type=small", photoItem);
                }
            }
        }

        private void photoThumbnailClient_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                SkyDrivePhoto photo = (SkyDrivePhoto)e.UserState;
                photo.PhotoThumbnailUrl = (string)e.Result["location"];
            }
        }

        private SkyDrivePhoto _selectedPhoto;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public SkyDrivePhoto SelectedPhoto
        {
            get
            {
                return _selectedPhoto;
            }
            set
            {
                _selectedPhoto = value;
                NotifyPropertyChanged("SelectedPhoto");
            }
        }

        public void Download()
        {
            if (SelectedPhoto == null)
            {
                MessageBox.Show("Select photo first, please.");
                return;
            }
            
            LiveConnectClient downloadClient = new LiveConnectClient(App.LiveSession);
            downloadClient.DownloadCompleted += DownloadClientOnDownloadCompleted;
            downloadClient.DownloadAsync(SelectedPhoto.ID + "/content");
        }

        private void DownloadClientOnDownloadCompleted(object sender, LiveDownloadCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MediaLibrary mediaLibrary = new MediaLibrary();
                mediaLibrary.SavePicture(SelectedPhoto.Title, e.Result);
                MessageBox.Show(String.Format("Photo {0} downloaded to your phone.", SelectedPhoto.Title));
            }
        }

        public event EventHandler<EventArgs> DataLoaded;

        protected virtual void OnDataLoaded()
        {
            EventHandler<EventArgs> handler = DataLoaded;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
