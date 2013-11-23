using System.Collections.Generic;
using System.Runtime.Serialization;

namespace aSkyImage.Model
{
    [DataContract]
    public class SkyDriveCommentList
    {
        [DataMember(Name = "data")]
        public List<SkyDriveComment> Comments { get; set; }
    }
}
