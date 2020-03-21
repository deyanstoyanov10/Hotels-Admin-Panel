namespace AdminPanelApp.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Newtonsoft.Json;

    using Models.Enums;
    using Services.Contracts;


    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IProductService productService;
        private IRequisitionService requisitionService;
        private IApprovalService approvalService;
        private IAdminService adminService;

        public ValuesController(IProductService productService,
            IRequisitionService requisitionService,
            IApprovalService approvalService,
            IAdminService adminService)
        {
            this.productService = productService;
            this.requisitionService = requisitionService;
            this.approvalService = approvalService;
            this.adminService = adminService;
        }

        public async Task RemoveProduct(int productId)
        {
            await productService.RemoveProduct(productId);
        }

        public async Task<string> GetAllItemsDesc()
        {
            var list = await this.productService.GetAddedProductsDesc();
            return JsonConvert.SerializeObject(list);
        }

        public async Task<string> GetItemsByCategory(int categoryId)
        {
            string categoryName = Enum.GetName(typeof(Category), categoryId).ToString();
            var list = await this.productService.GetAddedProductsDescByCategory(categoryName);
            return JsonConvert.SerializeObject(list);
        }

        public async Task<string> GetItemsBySearch(string searchKey)
        {
            var list = await this.productService.GetAddedProductsDescBySearch(searchKey);
            return JsonConvert.SerializeObject(list);
        }

        public async Task<string> GetItemsBySearchAndCategory(string searchKey, int categoryId)
        {
            string categoryName = Enum.GetName(typeof(Category), categoryId).ToString();
            var list = await this.productService.GetAddedProductsDescBySearchAndCategory(categoryName, searchKey);
            return JsonConvert.SerializeObject(list);
        }

        //===== RequisitionValues =====//

        public async Task<string> UpdateProductCart(string requisitionId)
        {
            var productsInCart = await this.requisitionService.GetItemsInCart(requisitionId);
            return JsonConvert.SerializeObject(productsInCart);
        }

        public async Task AddProductToRequisition(int productId, string requisitionId, uint quantity)
        {
            await this.requisitionService.AddProductToRequisition(productId, requisitionId, quantity);
        }

        public async Task RemoveProductFromRequistion(int productId)
        {
            await this.requisitionService.RemoveProductFromRequisition(productId);
        }

        public async Task<uint> CountPrice(string requisitionId)
        {
            var sum = await this.requisitionService.CountPrice(requisitionId);
            return sum;
        }

        //===== ApprovalValues =====//

        //===== AdminUrls =====//

        public async Task<string> UpdateRequisitionTable()
        {
            var requisitions = await this.approvalService.GetRequisitionsForApproval();
            return JsonConvert.SerializeObject(requisitions);
        }

        public async Task<string> GetProductsForRequisition(string requisitionId)
        {
            var products = await this.approvalService.GetProductsFromRequisition(requisitionId);
            return JsonConvert.SerializeObject(products);
        }

        public async Task ApproveOrRejectProduct(int productId, ushort status)
        {
            await this.approvalService.ApproveOrRejectProduct(productId, status);
        }
        
        //===== End Admin Urls =====//

        //===== Users Urls =====//

        public async Task<string> UpdateRequisitionTableForUsers(string user)
        {
            var requisitions = await this.approvalService.GetRequisitionsApprovedOrRejected(user);
            return JsonConvert.SerializeObject(requisitions);
        }
        //===== End Users Urls =====//

        public async Task AutoSendProductsToSuppliers()
        {
            await this.adminService.SendProductsToSupplier();
        }

        public async Task RemoveSupplier(int supplierId)
        {
            await this.adminService.RemoveSupplier(supplierId);
        }

        public async Task<string> GetProductSupplierName(int productId)
        {
            return await this.productService.GetProductSupplierName(productId);
        }

        //History Values

        public async Task<string> GetRequisitionsFromMonth(int index, int? hotelIndex)
        {
            var requisitions = await this.adminService.GetRequisitionsFrom(index, hotelIndex);
            return JsonConvert.SerializeObject(requisitions);
        }

        public async Task UpdateProduct(int productId, uint quantity)
        {
            await this.adminService.UpdateProductQuantity(productId, quantity);
        }
    }
}