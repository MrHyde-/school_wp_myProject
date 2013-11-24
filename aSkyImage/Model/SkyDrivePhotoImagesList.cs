using System.Collections.Generic;
using System.Runtime.Serialization;

namespace aSkyImage.Model
{
    [DataContract]
    public class SkyDrivePhotoImagesList
    {
        [DataMember(Name = "images")]
        public List<SkyDrivePhotoImage> PhotoImages { get; set; }
    }
}
