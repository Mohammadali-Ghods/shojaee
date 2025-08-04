using MongoDB.Entities;

namespace Shojaee.ProductModule
{
    public class ProductCategory : Entity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageAddress { get; set; }
        public string ParentCategory { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class ProductCategoryViewModel
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public List<ProductCategoryLayer1> SubCategory { get; set; }
    }
    public class ProductCategoryLayer1
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public List<ProductCategoryLayer2> SubCategory { get; set; }
    }
    public class ProductCategoryLayer2
    {
        public string ID { get; set; }
        public string Title { get; set; }
    }
}
