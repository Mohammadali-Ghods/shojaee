using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using System;
using Shojaee.CommentModule;
using Shojaee.PointingModule;

namespace Shojaee.ProductModule
{

    public class ProductController : ControllerBase
    {

        [HttpGet("GetProductCategories")]
        public async Task<List<ProductCategoryViewModel>> GetProductCategories()
        {
            List<ProductCategoryViewModel> response = new List<ProductCategoryViewModel>();

            var result = await DB.Find<ProductCategory>().ExecuteAsync();
            var layer0 = result.Where(x => x.ParentCategory == "0").ToList();

            for (int i = 0; i <= layer0.Count - 1; i++)
            {
                var layer1 = result.Where(x => x.ParentCategory == layer0[i].ID).ToList();

                ProductCategoryViewModel layer_0 = new ProductCategoryViewModel();
                layer_0.Title = layer0[i].Title;
                layer_0.ID = layer0[i].ID;
                layer_0.SubCategory = new List<ProductCategoryLayer1>();

                for (int j = 0; j <= layer1.Count - 1; j++)
                {
                    var layer2 = result.Where(x => x.ParentCategory == layer1[j].ID).ToList();

                    ProductCategoryLayer1 layer_1 = new ProductCategoryLayer1();
                    layer_1.Title = layer1[j].Title;
                    layer_1.ID = layer1[j].ID;
                    layer_1.SubCategory = new List<ProductCategoryLayer2>();

                    for (int z = 0; z <= layer2.Count - 1; z++)
                    {
                        ProductCategoryLayer2 layer_2 = new ProductCategoryLayer2();
                        layer_2.Title = layer2[z].Title;
                        layer_2.ID = layer2[z].ID;

                        layer_1.SubCategory.Add(layer_2);
                    }

                    layer_0.SubCategory.Add(layer_1);
                }

                response.Add(layer_0);
            }

            return response;
        }

        [HttpGet("GetCategories")]
        public async Task<List<ProductCategory>> GetCategories([FromQuery] string catId)
        {
            return await DB.Find<ProductCategory>().Match(x => x.ParentCategory == catId).ExecuteAsync();
        }

        [HttpGet("GetCategoryProducts")]
        public async Task<List<Product>> GetCategoryProducts([FromQuery] string categoryId,
            int pageSize, int pageNumber)
        {
            var result = new List<Product>();

            if (categoryId != null)
                result = await DB.Find<Product>().Match(x => x.CategoryID == categoryId)
                     .Skip((pageNumber - 1) * pageSize).Limit(pageSize).ExecuteAsync();

            else
                result = await DB.Find<Product>()
                     .Skip((pageNumber - 1) * pageSize).Limit(pageSize).ExecuteAsync();

            for (int i = 0; i <= result.Count - 1; i++)
            {
                var points = await DB.Find<Pointing>().Match(x => x.ProductID == result[i].ID).ExecuteAsync();
                result[i].CommentsCount = await DB.CountAsync<Comment>(x => x.ProductID == result[i].ID);

                if (points.Count != 0)
                    result[i].Point = points.Average(x => x.Point);
            }

            return result;
        }

        [HttpGet("GetCategoryPopularProducts")]
        public async Task<List<Product>> GetCategoryPopularProducts([FromQuery] string categoryId,
           int pageSize, int pageNumber)
        {
            var result = new List<Product>();

            if (categoryId != null)
                result = await DB.Find<Product>().Match(x => x.CategoryID == categoryId
            && x.IsPopular)
                  .Skip((pageNumber - 1) * pageSize).Limit(pageSize).ExecuteAsync();

            else
                result = await DB.Find<Product>().Match(x => x.IsPopular)
                  .Skip((pageNumber - 1) * pageSize).Limit(pageSize).ExecuteAsync();

            for (int i = 0; i <= result.Count - 1; i++)
            {
                var points = await DB.Find<Pointing>().Match(x => x.ProductID == result[i].ID).ExecuteAsync();
                result[i].CommentsCount = await DB.CountAsync<Comment>(x => x.ProductID == result[i].ID);

                if (points.Count != 0)
                    result[i].Point = points.Average(x => x.Point);
            }

            return result;
        }

        [HttpGet("GetCategoryNewProducts")]
        public async Task<List<Product>> GetCategoryNewProducts([FromQuery] string categoryId,
          int pageSize, int pageNumber)
        {
            var result = new List<Product>();

            if (categoryId != null)
                result = await DB.Find<Product>().Match(x => x.CategoryID == categoryId
           && x.IsNew)
                 .Skip((pageNumber - 1) * pageSize).Limit(pageSize).ExecuteAsync();

            else
                result = await DB.Find<Product>().Match(x => x.IsNew)
                .Skip((pageNumber - 1) * pageSize).Limit(pageSize).ExecuteAsync();

            for (int i = 0; i <= result.Count - 1; i++)
            {
                var points = await DB.Find<Pointing>().Match(x => x.ProductID == result[i].ID).ExecuteAsync();
                result[i].CommentsCount = await DB.CountAsync<Comment>(x => x.ProductID == result[i].ID);

                if (points.Count != 0)
                    result[i].Point = points.Average(x => x.Point);
            }

            return result;
        }

        [HttpGet("GetProduct")]
        public async Task<Product> GetProduct([FromQuery] string productId)
        {
            var result = await DB.Find<Product>().Match(x => x.ID == productId).ExecuteFirstAsync();
            var points = await DB.Find<Pointing>().Match(x => x.ProductID == productId).ExecuteAsync();
            result.CommentsCount = await DB.CountAsync<Comment>(x => x.ProductID == productId);
            result.Point = points.Average(x => x.Point);

            return result;
        }

        [HttpPost("AddProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<Product> AddtProduct([FromBody] Product product)
        {
            product.ID = Guid.NewGuid().ToString();
            product.CreatedDate = DateTime.Now;
            product.CommentsCount = 0;
            product.Point = 0;

            await DB.InsertAsync<Product>(product);

            return product;
        }

        [HttpPost("EditProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<Product> EditProduct([FromBody] Product product)
        {
            await DB.Update<Product>()
           .MatchID(product.ID)
           .ModifyExcept(x => new { x.ID }, product)
           .ExecuteAsync();

            return product;
        }

        [HttpPost("RemoveProduct")]
        [Authorize(Roles = "Admin")]
        public async Task RemoveProduct([FromBody] Product product)
        {
            await DB.DeleteAsync<Product>(product.ID);
        }

        [HttpPost("AddCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<ProductCategory> AddCategory([FromBody] ProductCategory productCategory)
        {
            productCategory.ID = Guid.NewGuid().ToString();
            productCategory.CreatedDate = DateTime.Now;

            await DB.InsertAsync<ProductCategory>(productCategory);

            return productCategory;
        }

        [HttpPost("EditCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<ProductCategory> EditCategory([FromBody] ProductCategory productCategory)
        {
            await DB.Update<ProductCategory>()
           .MatchID(productCategory.ID)
           .ModifyExcept(x => new { x.ID }, productCategory)
           .ExecuteAsync();

            return productCategory;
        }

        [HttpPost("RemoveCategory")]
        [Authorize(Roles = "Admin")]
        public async Task RemoveCategory([FromBody] ProductCategory productCategory)
        {
            await DB.DeleteAsync<ProductCategory>(productCategory.ID);
        }
    }
}
