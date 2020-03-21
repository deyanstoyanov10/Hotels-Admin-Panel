namespace AdminPanelApp.Services.Contracts
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Models;
    using ReflectionIT.Mvc.Paging;

    public interface IProductService
    {
        Task AddProduct(AddProductModel productModel, byte[] picture);

        Task RemoveProduct(int productId);

        Task<List<Supplier>> GetSupplierNames();

        Task ChangeProductSupplier(ChangeProductSupplierModel changeProductSupplierModel);

        Task ChangeSupplier(ChangeSupplierModel changeSupplierModel);

        Task<string> GetProductSupplierName(int productId);

        Task<int> CountAllProducts();

        Task<List<Products>> GetAddedProductsDesc();

        Task<PagingList<Products>> GetAddedProductsDesc(int page = 1);

        Task<List<Products>> GetAddedProductsDescByCategory(string category);

        Task<List<Products>> GetAddedProductsDescBySearch(string searchKey);

        Task<List<Products>> GetAddedProductsDescBySearchAndCategory(string category, string searchKey);
    }
}
