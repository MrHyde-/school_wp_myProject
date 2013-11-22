using System.Collections.Generic;
using System.Runtime.Serialization;
using aSkyImage.ViewModel;

namespace aSkyImage.Model
{
    [DataContract]
    public class SkyDriveAlbumList
    {
        [DataMember(Name = "data")]
        public List<SkyDriveAlbum> SkyDriveAlbums { get; set; }
    }
}
