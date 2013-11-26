using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Live;
using Microsoft.Xna.Framework.Media;
using aSkyImage.Model;
using aSkyImage.Resources;

namespace aSkyImage.ViewModel
{
    public class PhotoViewModel : SkyDriveViewModel
    {
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

        public void AddCommentToPhoto(string comment)
        {
            var commentData = new Dictionary<string, object>();
            commentData.Add("message", comment);

            LiveConnectClient addCommentClient = new LiveConnectClient(App.LiveSession);
            addCommentClient.PostCompleted += addCommentClient_OnPostComleted;
            addCommentClient.PostAsync(App.PhotoViewModel.SelectedPhoto.ID + "/comments", commentData);
        }

        private void addCommentClient_OnPostComleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (SelectedPhoto != null)
                {
                    LoadPhotoComments(SelectedPhoto);
                }
                MessageBox.Show(AppResources.MessageToUserCommentAdded);
            }
        }

        public void LoadPhotoComments(SkyDrivePhoto selectedPhoto)
        {
            LiveConnectClient readPhotoComments = new LiveConnectClient(App.LiveSession);
            readPhotoComments.GetCompleted += readPhotoComments_OnCompleted;
            readPhotoComments.GetAsync(selectedPhoto.ID + "/comments");
        }

        private void readPhotoComments_OnCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                var photosJson = e.RawResult;
                SelectedPhoto.Comments = new ObservableCollection<SkyDriveComment>();

                //load into memory stream
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(photosJson)))
                {
                    //parse into jsonser
                    // note that to using System.Runtime.Serialization.Json
                    // need to add reference System.Servicemodel.Web
                    var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(SkyDriveCommentList));
                    try
                    {
                        var list = (SkyDriveCommentList)ser.ReadObject(ms);
                        var photoComments = new ObservableCollection<SkyDriveComment>();

                        foreach (var comment in list.Comments)
                        {
                            photoComments.Add(comment);
                        }

                        SelectedPhoto.Comments = photoComments;
                    }
                    catch (Exception je)
                    {
                        System.Diagnostics.Debug.WriteLine("--- " + je.Message);
                    }
                }

                if (SelectedPhoto.Comments.Any() == false)
                {
                    //add a hint to user so noone has not yet commented
                    SelectedPhoto.Comments.Add(new SkyDriveComment
                    {
                        CommentedBy = new SkyDriveCommentUser
                        {
                            UserName = SelectedPhoto.CommentingEnabled ? AppResources.PhotoPageImageHasNoComments : AppResources.PhotoPageImageCommentingDisabled
                        }
                    });
                }
            }
        }

        public void Download()
        {
            if (App.PhotoViewModel.SelectedPhoto == null)
            {
                MessageBox.Show(AppResources.MessageToUserPleaseSelectPhotoFirst);
                return;
            }

            LiveConnectClient downloadClient = new LiveConnectClient(App.LiveSession);
            downloadClient.DownloadCompleted += DownloadClientOnDownloadCompleted;
            downloadClient.DownloadAsync(App.PhotoViewModel.SelectedPhoto.ID + "/content");
        }

        private void DownloadClientOnDownloadCompleted(object sender, LiveDownloadCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MediaLibrary mediaLibrary = new MediaLibrary();
                mediaLibrary.SavePicture(App.PhotoViewModel.SelectedPhoto.Title, e.Result);
                MessageBox.Show(String.Format(AppResources.MessageToUserDownloadingCompleted, App.PhotoViewModel.SelectedPhoto.Title));
            }
        }
    }
}
