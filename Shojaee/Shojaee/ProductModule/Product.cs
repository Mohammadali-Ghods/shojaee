using MongoDB.Entities;

namespace Shojaee.ProductModule
{
    public class Product : Entity
    {
        public string ProductName { get; set; }
        public string ProductBrief { get; set; }
        public string ProductDescription { get; set; }
        public string CategoryID { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<string> ImageAddresses { get; set; }
        public List<ContentPart> ContentParts { get; set; }
        public bool IsPopular { get; set; }
        public bool IsNew { get; set; }
        public List<ProductSpecificationGroup> ProductSpecificationGroup { get; set; }
        public bool Active { get; set; }
        public double Point { get; set; }
        public double CommentsCount { get; set; }
        public List<PriceList> PriceLists { get; set; }

    }
    public class PriceList
    {
        public List<string> ProductSpecificationIds { get; set; }
        public List<PriceObject> Price { get; set; }
    }
    public class PriceObject
    {
        public DateTime PriceDate { get; set; }
        public double Price { get; set; }
    }
    public class ProductSpecificationGroup
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageAddress { get; set; }
        public List<ProductSpecification> ProductSpecifications { get; set; }
    }
    public class ProductSpecification
    {
        public string ID { get; set; }
        public string Specification { get; set; }
        public string ImageAddress { get; set; }
    }
    public class ContentPart
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
