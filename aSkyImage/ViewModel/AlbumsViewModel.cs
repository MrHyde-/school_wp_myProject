using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Live;
using aSkyImage.Model;
using aSkyImage.Resources;

namespace aSkyImage.ViewModel
{
    /// <summary>
    /// ViewModel to handle Albums data transferring
    /// </summary>
    public class AlbumsViewModel : SkyDriveViewModel
    {
        public ObservableCollection<SkyDriveAlbum> Albums { get; set; }

        //only a helper access for the listbox
        public SkyDriveAlbum SelectedAlbum
        {
            get
            {
                if (App.AlbumViewModel.SelectedAlbum != null)
                {
                    return App.AlbumViewModel.SelectedAlbum;
                }
                return null;
            }
            set { App.AlbumViewModel.SelectedAlbum = value; }
        }

        public AlbumsViewModel()
        {
            Albums = new ObservableCollection<SkyDriveAlbum>(); 
        }

        /// <summary>
        /// Calls method that reads users albums from the cloud if necessary
        /// </summary>
        /// <param name="refreshData"></param>
        public void LoadAlbumsData(bool refreshData = false)
        {
            if (refreshData)
            {
                IsDataLoaded = false;
            }

            GetUserAlbumsData();
            IsDataLoaded = true;
        }

        /// <summary>
        /// Actual get caller
        /// </summary>
        private void GetUserAlbumsData()
        {
            if (IsDataLoaded == false)
            {
                LiveConnectClient clientAlbums = new LiveConnectClient(App.LiveSession);
                clientAlbums.GetCompleted += clientAlbums_GetCompleted;
                clientAlbums.GetAsync("/me/albums");
            }
        }

        /// <summary>
        /// Event which handles returning albums json
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clientAlbums_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
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
            
            //rise event so mainpage knows when data is actually loaded
            OnDataLoaded();
        }

        protected bool IsDataLoaded { get; set; }

        /// <summary>
        /// Calls cloud to return the url to albums picture and because skydrive it is the latest added photo
        /// </summary>
        /// <param name="albumItem"></param>
        public void GetAlbumPicture(SkyDriveAlbum albumItem)
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

        /// <summary>
        /// event for getting albums album picture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void albumPictureClient_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                SkyDriveAlbum album = (SkyDriveAlbum)e.UserState;
                
                if (e.Result.ContainsKey("location"))
                {
                    album.AlbumPicture = (string)e.Result["location"];
                }
            }
        }

        /// <summary>
        /// Method to add album to the skydrive cloud
        /// </summary>
        /// <param name="albumName"></param>
        public void CreateAlbum(string albumName)
        {
            LiveConnectClient albumClient = new LiveConnectClient(App.LiveSession);
            albumClient.PostCompleted += albumClient_PostCompleted;
            var albumData = new Dictionary<string, object>();
            albumData.Add("name", albumName);
            albumClient.PostAsync("/me/albums", albumData);
        }

        /// <summary>
        /// event for completing the post call
        /// </summary>
        /// <param name="comment"></param>
        private void albumClient_PostCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                string newAlbumJson = e.RawResult;

                //when album is created add it to the collection (do not load every album again)
                InsertNewAlbumToAlbumsCollection(newAlbumJson);

                //inform user that album has been created
                MessageBox.Show(AppResources.MessageToUserAlbumCreated);
            }
        }

        /// <summary>
        /// Method to read the added album from the json and adds it to the collection of albums
        /// </summary>
        /// <param name="comment"></param>
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

        //skeleton for the OnDataLoaded event
        public event EventHandler<EventArgs> DataLoaded;

        protected virtual void OnDataLoaded()
        {
            EventHandler<EventArgs> handler = DataLoaded;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
