using System.Collections.Generic;
using System.Runtime.Serialization;
using aSkyImage.ViewModel;

namespace aSkyImage.Model
{
    [DataContract]
    public class SkyDrivePhotoList
    {
        [DataMember(Name = "data")]
        public List<SkyDrivePhoto> SkyDrivePhotos { get; set; }
    }
}
