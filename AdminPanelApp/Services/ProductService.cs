namespace AdminPanelApp.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Data;
    using Models;
    using Models.Enums;
    using Services.Contracts;
    
    using ReflectionIT.Mvc.Paging;

    public class ProductService : IProductService
    {
        private AdminPanelDbContext context;

        public ProductService(AdminPanelDbContext context)
        {
            this.context = context;
        }

        public async Task AddProduct(AddProductModel productModel, byte[] picture)
        {
            var newProduct = new Products()
            {
                Name = productModel.Name,
                Packaging = productModel.Packaging,
                Unit = productModel.Unit,
                Price = productModel.Price,
                Category = Enum.GetName(typeof(Category), productModel.Category).ToString(),
                Supplier = productModel.Supplier,
                Picture = picture
            };

            await this.context.Products.AddAsync(newProduct);
            await this.context.SaveChangesAsync();
        }

        public async Task RemoveProduct(int productId)
        {
            var productForRemove = this.context.Products.Where(x => x.Id == productId).FirstOrDefault();
            this.context.Products.Remove(productForRemove);
            await this.context.SaveChangesAsync();
        }

        public async Task<List<Supplier>> GetSupplierNames()
        {
            var suppliers = this.context.Suppliers.ToList();
            return await Task.Run(() => suppliers);
        }

        public async Task ChangeProductSupplier(ChangeProductSupplierModel changeProductSupplierModel)
        {
            var product = this.context.Products.Where(x => x.Id == changeProductSupplierModel.ProductId).FirstOrDefault();
            var suppliers = await GetSupplierNames();
            product.Supplier = suppliers[changeProductSupplierModel.NewSupplierIndex].Name;
            this.context.Products.Update(product);
            await this.context.SaveChangesAsync();
        }

        public async Task ChangeSupplier(ChangeSupplierModel changeSupplierModel)
        {
            var suppliers = await GetSupplierNames();
            var products = this.context.Products.Where(x => x.Supplier == suppliers[changeSupplierModel.CurrentSupplierIndex].Name).ToList();
            for (int i = 0; i < products.Count; i++)
            {
                products[i].Supplier = suppliers[changeSupplierModel.NewSupplierIndex].Name;
                this.context.Update(products[i]);
            }
            await this.context.SaveChangesAsync();
        }

        public async Task<string> GetProductSupplierName(int productId)
        {
            var product = this.context.Products.Where(x => x.Id == productId).FirstOrDefault();
            return await Task.Run(() => product.Supplier);
        }

        public async Task<int> CountAllProducts()
        {
            var count = this.context.Products.ToArray().Length;
            return await Task.Run(() => count);
        }

        public async Task<List<Products>> GetAddedProductsDesc()
        {

            var productsList = this.context.Products.OrderByDescending(x => x.Id).ToList();
            return await Task.Run(() => productsList);
        }

        public async Task<PagingList<Products>> GetAddedProductsDesc(int page = 1)
        {
            
            var productsList = this.context.Products.OrderByDescending(x => x.Id);
            var list = await PagingList.CreateAsync(productsList, 5, page);
            return await Task.Run(() => list);
        }

        public async Task<List<Products>> GetAddedProductsDescByCategory(string category)
        {

            var productsList = this.context.Products.Where(x => x.Category == category).OrderByDescending(x => x.Id).ToList();
            return await Task.Run(() => productsList);
        }

        public async Task<List<Products>> GetAddedProductsDescBySearch(string searchKey)
        {
            var productsList = this.context.Products.Where(x => x.Name.Contains(searchKey)).OrderByDescending(x => x.Id).ToList();
            return await Task.Run(() => productsList);
        }

        public async Task<List<Products>> GetAddedProductsDescBySearchAndCategory(string category, string searchKey)
        {
            var productsList = this.context.Products.Where(x => x.Category == category && x.Name.Contains(searchKey)).OrderByDescending(x => x.Id).ToList();
            return await Task.Run(() => productsList);
        }
    }
}
