namespace AdminPanelApp.Services.Contracts
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Models;

    public interface IRequisitionService
    {
        Task<string> CreateRequisition(string location, string user);

        Task<Requisitions> GetRequisitionDetails(string requisitionId);

        Task<List<Product>> GetItemsInCart(string requisitionId);

        Task AddProductToRequisition(int productId, string requisitionId, uint quantity);

        Task RemoveProductFromRequisition(int productId);

        Task<uint> CountPrice(string requisitionId);

        Task UpdateRequisitionStatus(string requisitionId, ushort status);

        Task DeleteRequisition(string requisitionId);
    }
}
