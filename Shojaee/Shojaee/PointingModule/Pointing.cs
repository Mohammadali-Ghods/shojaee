using MongoDB.Entities;

namespace Shojaee.PointingModule
{
    public class Pointing : Entity
    {
        public string UserID { get; set; }
        public string ProductID { get; set; }
        public DateTime CreatedDate { get; set; }
        public double Point { get; set; }
    }
}
