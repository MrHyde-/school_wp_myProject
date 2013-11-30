using System;
using System.Runtime.Serialization;

namespace aSkyImage.Model
{
    /// <summary>
    /// DataObject for photo properties (response contains 2 to 4 of these)
    /// </summary>
    [DataContract]
    public class SkyDrivePhotoImage
    {
        [DataMember(Name = "height")]
        public decimal Height { get; set; }
        [DataMember(Name = "width")]
        public decimal Width { get; set; }
        [DataMember(Name = "source")]
        public String Source { get; set; }
        [DataMember(Name = "type")]
        public String Type { get; set; }
    }
}
