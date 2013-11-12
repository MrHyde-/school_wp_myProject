using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Live;

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
                
                //from result fetch new album id

                //add it to albums

                //refresh data..
                //LoadData();
            }
        }
    }
}
