namespace AdminPanelApp.Services.Contracts
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Models;

    public interface IAdminService
    {
        Task RegisterManager(RegisterModel registerModel);

        Task AddSupplier(AddSupplierModel supplierModel);

        Task RemoveSupplier(int id);

        Task<List<Supplier>> GetAllSuppliers();

        Task<List<Hotel>> GetAllHotels();

        Task<DateTime> GetNextProductSchedule();

        Task ChangeNextProductSchedule(NextScheduleModel nextScheduleModel);

        Task<UserInfo> ManagerInfo(string userName);

        Task<string> GetUserFirstAndLastName(string userName);

        Task SendApprovalEmail(string requisitionId);

        Task SendProductsToSupplier();

        Task<List<DateTime>> GetLatestMonths();

        Task<List<Requisitions>> GetRequisitionsFrom(int index, int? hotelIndex);

        Task UpdateProductQuantity(int productId, uint quantity);
    }
}
