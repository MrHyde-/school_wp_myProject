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
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadAlbumsData(bool refreshData = false)
        {
            if (refreshData)
            {
                IsDataLoaded = false;
            }

            GetUserAlbumsData();
            IsDataLoaded = true;
        }

        private void GetUserAlbumsData()
        {
            if (IsDataLoaded == false)
            {
                LiveConnectClient clientAlbums = new LiveConnectClient(App.LiveSession);
                clientAlbums.GetCompleted += clientAlbums_GetCompleted;
                clientAlbums.GetAsync("/me/albums");
            }
        }

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
                
                if (e.Result.ContainsKey("location"))
                {
                    album.AlbumPicture = (string)e.Result["location"];
                }
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
                MessageBox.Show(AppResources.MessageToUserAlbumCreated);
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

        public event EventHandler<EventArgs> DataLoaded;

        protected virtual void OnDataLoaded()
        {
            EventHandler<EventArgs> handler = DataLoaded;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
