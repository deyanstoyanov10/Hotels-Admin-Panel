namespace AdminPanelApp.Services
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Microsoft.EntityFrameworkCore;

    using Data;
    using Models;
    using Services.Contracts;

    public class ApprovalService : IApprovalService
    {
        private AdminPanelDbContext context;
        private IAdminService adminService;

        public ApprovalService(AdminPanelDbContext context, IAdminService adminService)
        {
            this.context = context;
            this.adminService = adminService;
        }

        public async Task ApproveOrRejectRequisition(string requisitionId, ushort status)
        {
            var requisition = this.context.Requisitions.Where(x => x.Id == requisitionId).FirstOrDefault();
            requisition.Status = status;
            if (status == 3)
            {
                var products = await this.context.Product.Where(x => x.RequisitionId == requisitionId).ToListAsync();
                var productsCount = products.Count;
                for (int i = 0; i < productsCount; i++)
                {
                    products[i].Status = 2;
                    this.context.Product.Update(products[i]);
                }
            }

            var approvedProducts = await this.context.Product.Where(x => x.RequisitionId == requisitionId && x.Status == 2).Select(x => new { Price = x.Price, Quanity = x.Quantity}).ToListAsync();
            uint totalCost = 0;
            for (int i = 0; i < approvedProducts.Count; i++)
            {
                totalCost += approvedProducts[i].Price * approvedProducts[i].Quanity;
            }
            requisition.Total = totalCost;

            this.context.Requisitions.Update(requisition);
            await this.context.SaveChangesAsync();
        }

        public async Task ApproveOrRejectProduct(int productId, ushort status)
        {
            var product = this.context.Product.Where(x => x.Id == productId).FirstOrDefault();
            product.Status = status;
            this.context.Update(product);
            await this.context.SaveChangesAsync();
        }

        public async Task<List<Requisitions>> GetRequisitionsForApproval()
        {
            var requisitions = await this.context.Requisitions.Where(x => x.Status == 1 || x.Status == 0).OrderByDescending(x => x.Date).ToListAsync();
            int count = requisitions.Count;

            for (int i = 0; i < count; i++)
            {
                requisitions[i].AddedBy = await adminService.GetUserFirstAndLastName(requisitions[i].AddedBy);
            }

            return requisitions;
        }

        public async Task<List<Product>> GetProductsFromRequisition(string requisitionId)
        {
            var products = await this.context.Product.Where(x => x.RequisitionId == requisitionId).ToListAsync();
            return products;
        }

        //Users Requests

        public async Task<List<Requisitions>> GetRequisitionsApprovedOrRejected(string user)
        {
            var requisitions = await this.context.Requisitions.Where(x => (x.Status == 1 || x.Status == 2 || x.Status == 3) && x.AddedBy == user).OrderByDescending(x => x.Date).ToListAsync();
            int count = requisitions.Count;

            for (int i = 0; i < count; i++)
            {
                requisitions[i].AddedBy = await adminService.GetUserFirstAndLastName(user);
            }

            return requisitions;
        }
    }
}
