using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;
using Shojaee.ProductModule;

namespace Shojaee.OrderModule
{
    public class Order : Entity
    {
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public double NetTotal { get; set; }
        public double Vat { get; set; }
        public double GrossTotal { get; set; }
        public string UserID { get; set; }
        public bool IsBasket { get; set; }
        public string ShippingID { get; set; }
        public string OrderID { get; set; }
    }
    public class OrderItem
    {
        public string ProductID { get; set; }

        [BsonIgnore]
        public Product Product { get; set; }
        public List<string> SpecificationIDs { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }

}
