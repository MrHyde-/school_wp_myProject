using System;
using System.Runtime.Serialization;

namespace aSkyImage.Model
{
    /// <summary>
    /// DataObject for photo comments in the skydrive json
    /// </summary>
    [DataContract]
    public class SkyDriveComment
    {
        private string _message = String.Empty;
        [DataMember(Name = "message")]
        public string Message
        {
            get { return _message; }
            set
            {
                if (value != _message)
                {
                    _message = value;
                }
            }
        }

        [DataMember(Name = "from")]
        public SkyDriveCommentUser CommentedBy { get; set; }
        
    }

    [DataContract]
    public class SkyDriveCommentUser
    {
        [DataMember(Name = "name")]
        public string UserName { get; set; }
        [DataMember(Name = "id")]
        public string UserId { get; set; }
    }
}
