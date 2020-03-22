namespace AdminPanelApp.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Microsoft.EntityFrameworkCore;

    using Data;
    using Models;
    using Services.Contracts;

    public class RequisitionService : IRequisitionService
    {
        private AdminPanelDbContext context;

        public RequisitionService(AdminPanelDbContext context)
        {
            this.context = context;
        }

        public async Task<string> CreateRequisition(string location, string user)
        {
            var newId = Guid.NewGuid().ToString();
            var scheduleFor = this.context.NextProductsSchedule.FirstOrDefault();
            var requisition = new Requisitions()
            {
                Id = newId,
                Date = DateTime.UtcNow,
                Location = location,
                Status = 0,
                ScheduleFor = scheduleFor.NextSchedule,
                AddedBy = user,
                Total = 0
            };
            await this.context.Requisitions.AddAsync(requisition);
            await this.context.SaveChangesAsync();
            return newId;
        }

        public async Task<Requisitions> GetRequisitionDetails(string requisitionId)
        {
            var requisition = this.context.Requisitions.Where(x => x.Id == requisitionId).FirstOrDefault();
            return await Task.Run(() => requisition);
        }

        public async Task<List<Product>> GetItemsInCart(string requisitionId)
        {
            var itemsList = await this.context.Product.Where(x => x.RequisitionId == requisitionId).ToListAsync();
            return itemsList;
        }

        public async Task AddProductToRequisition(int productId, string requisitionId, uint quantity)
        {
            var item = this.context.Products.Where(x => x.Id == productId).FirstOrDefault();
            var product = new Product()
            {
                Name = item.Name,
                Unit = item.Unit,
                Price = item.Price,
                Category = item.Category,
                Supplier = item.Supplier,
                Quantity = quantity,
                Packaging = item.Packaging,
                Status = 1,
                RequisitionId = requisitionId
            };
            this.context.Product.Add(product);
            await this.context.SaveChangesAsync();
        }

        public async Task RemoveProductFromRequisition(int productId)
        {
            var product = this.context.Product.Where(x => x.Id == productId).FirstOrDefault();
            this.context.Product.Remove(product);
            await this.context.SaveChangesAsync();
        }

        public async Task<uint> CountPrice(string requisitionId)
        {
            var products = await this.context.Product.Where(x => x.RequisitionId == requisitionId)
                .Select(x => new { Price = x.Price, Quantity = x.Quantity })
                .ToListAsync();
            uint sum = 0;

            for (int i = 0; i < products.Count; i++)
            {
                sum += products[i].Price * products[i].Quantity;
            }

            return sum;
        }

        public async Task UpdateRequisitionStatus(string requisitionId, ushort status)
        {
            var requisition = this.context.Requisitions.Where(x => x.Id == requisitionId).FirstOrDefault();
            if (requisition != null)
            {
                requisition.Status = status;
                requisition.Total = await CountPrice(requisitionId);
                this.context.Requisitions.Update(requisition);
                await this.context.SaveChangesAsync();
            }
        }

        public async Task DeleteRequisition(string requisitionId)
        {
            var requisition = this.context.Requisitions.Where(x => x.Id == requisitionId).FirstOrDefault();

            if (requisition != null)
            {
                this.context.Requisitions.Remove(requisition);
                await this.context.SaveChangesAsync();
            }
        }
    }
}
