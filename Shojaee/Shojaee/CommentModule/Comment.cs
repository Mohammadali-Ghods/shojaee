using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;
using Shojaee.ProductModule;
using Shojaee.UserModule;

namespace Shojaee.CommentModule
{
    public class Comment : Entity
    {
        public string CommentContent { get; set; }
        public string UserID { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<CommentLike> Likes { get; set; }
        public string ProductID { get; set; }

        [BsonIgnore]
        public User User { get; set; }

        [BsonIgnore]
        public string ProductName { get; set; }
    }
    public class CommentLike
    {
        public string UserID { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
