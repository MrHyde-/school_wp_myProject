using System.Collections.Generic;
using System.Runtime.Serialization;
using aSkyImage.ViewModel;

namespace aSkyImage.Model
{
    /// <summary>
    /// DataObject for reading skydrive albums from json
    /// </summary>
    [DataContract]
    public class SkyDriveAlbumList
    {
        [DataMember(Name = "data")]
        public List<SkyDriveAlbum> SkyDriveAlbums { get; set; }
    }
}
