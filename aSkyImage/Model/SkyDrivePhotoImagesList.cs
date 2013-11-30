using System.Collections.Generic;
using System.Runtime.Serialization;

namespace aSkyImage.Model
{
    /// <summary>
    /// DataObject for reading skydrive photo properties from json
    /// </summary>
    [DataContract]
    public class SkyDrivePhotoImagesList
    {
        [DataMember(Name = "images")]
        public List<SkyDrivePhotoImage> PhotoImages { get; set; }
    }
}
