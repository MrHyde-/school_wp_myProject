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

                MessageBox.Show(String.Format(AppResources.MessageToUserUploadingImageToAlbum, SelectedAlbum.Title));

                uploadClient.BackgroundUploadAsync(SelectedAlbum.ID, new Uri(uploadLocation, UriKind.RelativeOrAbsolute), OverwriteOption.Rename, userState);
            };

            worker.RunWorkerAsync();
        }

        private void uploadClient_BackgroundUploadCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                MessageBox.Show(AppResources.MessageToUserImageUploaded);
                Deployment.Current.Dispatcher.BeginInvoke(() => DownloadPictures(SelectedAlbum));
            }
        }

        private void DownloadPictures(SkyDriveAlbum albumItem)
        {
            if (albumItem != null)
            {
                SelectedAlbum = albumItem;
                AlbumDataLoaded = false;
                LoadSingleAlbumData();
            }
        }

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

        internal bool AlbumDataLoaded { get; set; }

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

        private void GetPhotoThumbnailPicture(SkyDrivePhoto photoItem)
        {
            if (photoItem != null)
            {
                if (String.IsNullOrEmpty(photoItem.ID) == false)
                {
                    var tnurl = photoItem.PhotoImages.FirstOrDefault(x => x.Type == "thumbnail");
                    if (tnurl != null)
                    {
                        photoItem.PhotoThumbnailUrl = tnurl.Source;
                        return;
                    }

                    tnurl = photoItem.PhotoImages.FirstOrDefault(x => x.Type == "album");
                    if (tnurl != null)
                    {
                        photoItem.PhotoThumbnailUrl = tnurl.Source;
                        return;
                    }

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
    }
}
