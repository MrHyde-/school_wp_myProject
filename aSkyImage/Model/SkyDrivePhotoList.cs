using System.Collections.Generic;
using System.Runtime.Serialization;
using aSkyImage.ViewModel;

namespace aSkyImage.Model
{
    /// <summary>
    /// DataObject for reading skydrive photos from json
    /// </summary>
    [DataContract]
    public class SkyDrivePhotoList
    {
        [DataMember(Name = "data")]
        public List<SkyDrivePhoto> SkyDrivePhotos { get; set; }
    }
}
