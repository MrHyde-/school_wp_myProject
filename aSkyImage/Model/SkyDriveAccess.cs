using System;
using System.Runtime.Serialization;

namespace aSkyImage.Model
{
    /// <summary>
    /// DataObject to discover is it possible to add comments to photos
    /// </summary>
    [DataContract]
    public class SkyDriveAccess
    {
        private string _sharedWith = string.Empty;
        [DataMember(Name = "access")]
        public String SharedWith
        {
            get { return _sharedWith; }
            set
            {
                if (value != _sharedWith)
                {
                    _sharedWith = value;
                }
            }
        }
    }
}
