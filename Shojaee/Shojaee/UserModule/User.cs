using MongoDB.Entities;

namespace Shojaee.UserModule
{
    public class User : Entity
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string LastToken { get; set; }
        public string Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ImageAddress { get; set; }
        public bool Disabled { get; set; }
    }
}
