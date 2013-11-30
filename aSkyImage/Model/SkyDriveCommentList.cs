using System.Collections.Generic;
using System.Runtime.Serialization;

namespace aSkyImage.Model
{
    /// <summary>
    /// DataObject for reading skydrive comments from json
    /// </summary>
    [DataContract]
    public class SkyDriveCommentList
    {
        [DataMember(Name = "data")]
        public List<SkyDriveComment> Comments { get; set; }
    }
}
