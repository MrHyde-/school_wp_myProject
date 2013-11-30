using System;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Live;
using Microsoft.Phone.Tasks;
using aSkyImage.Model;
using aSkyImage.Resources;

namespace aSkyImage.ViewModel
{
    /// <summary>
    /// ViewModel to handle Album data transferring
    /// </summary>
    public class AlbumViewModel : SkyDriveViewModel
    {
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

        //only a helper access for the listbox
        public SkyDrivePhoto SelectedPhoto
        {
            get
            {
                if (App.PhotoViewModel.SelectedPhoto != null)
                {
                    return App.PhotoViewModel.SelectedPhoto;
                }
                return null;
            }
            set { App.PhotoViewModel.SelectedPhoto = value; }
        }

        /// <summary>
        /// When user wants to add image to his album
        /// </summary>
        internal void Upload()
        {
            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.ShowCamera = true;
            photoChooserTask.Completed += photoChooserTask_Completed;
            photoChooserTask.Show();
        }

        /// <summary>
        /// After user has selected the photo it will be uploaded to the skydrive
        /// </summary>
        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.ChosenPhoto == null)
            {
                return;
            }

            string uploadLocation = "/shared/transfers/Image" + DateTime.Now.Day + DateTime.Now.Millisecond + ".jpg";

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

                if (SelectedAlbum == null)
                {
                    if (App.AlbumsViewModel.Albums.Count > 0)
                    {
                        SelectedAlbum = App.AlbumsViewModel.Albums[0];
                    }
                }

                //inform user that photo will be uploaded
                MessageBox.Show(String.Format(AppResources.MessageToUserUploadingImageToAlbum, SelectedAlbum.Title));

                uploadClient.BackgroundUploadAsync(SelectedAlbum.ID, new Uri(uploadLocation, UriKind.RelativeOrAbsolute), OverwriteOption.Rename, userState);
            };

            worker.RunWorkerAsync();
        }

        /// <summary>
        /// event for uploading completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uploadClient_BackgroundUploadCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                //if all went fine inform user
                MessageBox.Show(AppResources.MessageToUserImageUploaded);
                App.AlbumsViewModel.GetAlbumPicture(App.AlbumsViewModel.SelectedAlbum);

                //refresh images so user is able see the new photo
                Deployment.Current.Dispatcher.BeginInvoke(() => DownloadPictures(SelectedAlbum));
            }
        }

        /// <summary>
        /// Get albums photos from the cloud
        /// </summary>
        /// <param name="albumItem"></param>
        private void DownloadPictures(SkyDriveAlbum albumItem)
        {
            if (albumItem != null)
            {
                SelectedAlbum = albumItem;
                AlbumDataLoaded = false;
                LoadSingleAlbumData();
            }
        }

        /// <summary>
        /// Get albums photos from the cloud (the actual cloud call)
        /// </summary>
        internal void LoadSingleAlbumData()
        {
            if (SelectedAlbum != null)
            {
                if (AlbumDataLoaded == false)
                {
                    LiveConnectClient clientAlbum = new LiveConnectClient(App.LiveSession);
                    clientAlbum.GetCompleted += clientAlbum_GetCompleted;
                    clientAlbum.GetAsync(SelectedAlbum.ID + "/photos");
                }
            }
            else
            {
                MessageBox.Show(AppResources.MessageToUserProblemsWithAlbum);
            }
        }

        //helper if data is already loaded etc..
        internal bool AlbumDataLoaded { get; set; }

        /// <summary>
        /// When data is loaded from the cloud make objects from the JSON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clientAlbum_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                return;
            }

            SkyDriveAlbum album = SelectedAlbum;

            if (album != null)
            {
                album.Photos.Clear();
                var photosJson = e.RawResult;

                //load into memory stream
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(photosJson)))
                {
                    //parse into jsonser
                    // note that to using System.Runtime.Serialization.Json
                    // need to add reference System.Servicemodel.Web
                    var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(SkyDrivePhotoList));
                    try
                    {
                        var list = (SkyDrivePhotoList)ser.ReadObject(ms);

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
        }

        /// <summary>
        /// Get thumbnail picture for the photo
        /// </summary>
        /// <param name="photoItem"></param>
        private void GetPhotoThumbnailPicture(SkyDrivePhoto photoItem)
        {
            if (photoItem != null)
            {
                if (String.IsNullOrEmpty(photoItem.ID) == false)
                {
                    //if photo has thumbnail url use it
                    var tnurl = photoItem.PhotoImages.FirstOrDefault(x => x.Type == "thumbnail");
                    if (tnurl != null)
                    {
                        photoItem.PhotoThumbnailUrl = tnurl.Source;
                        return;
                    }
                    
                    //if photo has album url use it
                    tnurl = photoItem.PhotoImages.FirstOrDefault(x => x.Type == "album");
                    if (tnurl != null)
                    {
                        photoItem.PhotoThumbnailUrl = tnurl.Source;
                        return;
                    }

                    //if not anything above get small size image for the photo
                    LiveConnectClient photoThumbnailClient = new LiveConnectClient(App.LiveSession);
                    photoThumbnailClient.GetCompleted += photoThumbnailClient_GetCompleted;
                    photoThumbnailClient.GetAsync(photoItem.ID + "/picture?type=small", photoItem);
                }
            }
        }

        /// <summary>
        /// Get thumbnail picture for the photo completed event from the cloud
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void photoThumbnailClient_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                //if there are no error retrieve location param from the result
                SkyDrivePhoto photo = (SkyDrivePhoto)e.UserState;
                photo.PhotoThumbnailUrl = (string)e.Result["location"];
            }
        }
    }
}
