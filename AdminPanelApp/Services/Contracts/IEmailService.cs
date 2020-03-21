namespace AdminPanelApp.Services.Contracts
{
    using AdminPanelApp.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEmailService
    {
        Task SendApprovalEmail(string requisitionId);

        Task SendProductsToSupplier();

        Task<List<Requisitions>> GetRequisitionsForSupplier(int month, int year, ushort status);

        Task<Supplier> GetSupplier(int supplierId);

        Task<List<Product>> ProductFilter(List<Requisitions> requisitionsForHotel, string supplierName);

       string GetHotelAddressByName(string hotelName);
    }
}
