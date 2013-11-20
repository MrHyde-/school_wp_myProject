using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Live;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;

namespace aSkyImage.ViewModel
{
    public class LiveServices
    {
        public ObservableCollection<SkydriveAlbum> Albums { get; private set; }

        public LiveServices()
        {
            this.Albums = new ObservableCollection<SkydriveAlbum>(); 
        }
        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            //GetProfileData();
            GetAlbumData();
            this.IsDataLoaded = true;
        }

        private void GetAlbumData()
        {
            LiveConnectClient clientFolder = new LiveConnectClient(App.LiveSession);
            clientFolder.GetCompleted += clientFolder_GetCompleted;
            clientFolder.GetAsync("/me/albums");
        }

        private void clientFolder_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                System.Diagnostics.Debug.WriteLine(e.Error.Message);
                return;
            }

            List<object> data = (List<object>)e.Result["data"];

            Albums.Clear();

            foreach (IDictionary<string, object> album in data)
            {
                SkydriveAlbum albumItem = new SkydriveAlbum();
                albumItem.Title = (string)album["name"];

                albumItem.Description = (string)album["description"];
                albumItem.ID = (string)album["id"];

                Albums.Add(albumItem);
                GetAlbumPicture(albumItem);
                //DownloadPictures(albumItem);
            }

            //set current folder to selectedAlbum
        }

        protected bool IsDataLoaded { get; set; }

        private SkydriveAlbum _selectedAlbum;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public SkydriveAlbum SelectedAlbum
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

        private void GetAlbumPicture(SkydriveAlbum albumItem)
        {
            LiveConnectClient albumPictureClient = new LiveConnectClient(App.LiveSession);
            albumPictureClient.GetCompleted += albumPictureClient_GetCompleted;
            albumPictureClient.GetAsync(albumItem.ID + "/picture", albumItem);
        }

        void albumPictureClient_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                SkydriveAlbum album = (SkydriveAlbum)e.UserState;
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
                //prompt user creating completed
                MessageBox.Show("Album created", "Done", MessageBoxButton.OK);
                
                //TODO
                //from result fetch new album id

                //add it to albums

                //refresh data..
                LoadData();
            }
        }

        internal void Upload()
        {
            PhotoChooserTask task = new PhotoChooserTask();
            task.ShowCamera = true;
            task.Completed += task_Completed;
            task.Show();
        }

        void task_Completed(object sender, PhotoResult e)
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

        private void DownloadPictures(SkydriveAlbum albumItem)
        {
            if (albumItem != null)
            {
                LiveConnectClient folderListClient = new LiveConnectClient(App.LiveSession);
                folderListClient.GetCompleted += folderListClient_GetCompleted;
                folderListClient.GetAsync(albumItem.ID + "/files", albumItem);
            }
        }

        void folderListClient_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                return;
            }

            int i = 0;
            SkydriveAlbum album = (SkydriveAlbum)e.UserState;

            album.Photos.Clear();
            List<object> data = (List<object>)e.Result["data"];

            foreach (IDictionary<string, object> photo in data)
            {
                var item = new SkydrivePhoto();
                item.Title = (string)photo["name"];
                item.Subtitle = (string)photo["name"];

                item.PhotoUrl = (string)photo["source"];
                item.Description = (string)photo["description"];
                item.ID = (string)photo["id"];

                if (album != null)
                {
                    album.Photos.Add(item);
                }
                // Stop after downloaing 10 imates
                if (i++ > 10)
                    break;
            }
        }

        public void LoadAlbumData()
        {
            if (SelectedAlbum != null)
            {
                LiveConnectClient clientAlbum = new LiveConnectClient(App.LiveSession);
                clientAlbum.GetCompleted += clientAlbum_GetCompleted;
                clientAlbum.GetAsync(SelectedAlbum.ID + "/files");
            }
            else
            {
                MessageBox.Show("Problem with selected album..");
            }
        }

        private void clientAlbum_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                return;
            }

            int i = 0;
            SkydriveAlbum album = SelectedAlbum;

            album.Photos.Clear();
            List<object> data = (List<object>)e.Result["data"];

            foreach (IDictionary<string, object> photo in data)
            {
                var item = new SkydrivePhoto();
                item.Title = (string)photo["name"];
                item.Subtitle = (string)photo["name"];

                item.PhotoUrl = (string)photo["source"];
                item.Description = (string)photo["description"];
                item.ID = (string)photo["id"];

                if (album != null)
                {
                    album.Photos.Add(item);
                }
                // Stop after downloaing 10 imates
                if (i++ > 10)
                    break;
            }
        }

        private SkydrivePhoto _selectedPhoto;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public SkydrivePhoto SelectedPhoto
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
    }
}
