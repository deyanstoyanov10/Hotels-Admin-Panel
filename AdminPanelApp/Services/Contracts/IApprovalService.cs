namespace AdminPanelApp.Services.Contracts
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Models;
    

    public interface IApprovalService
    {
        Task ApproveOrRejectRequisition(string requisitionId, ushort status);

        Task ApproveOrRejectProduct(int productId, ushort status);

        Task<List<Requisitions>> GetRequisitionsForApproval();

        Task<List<Product>> GetProductsFromRequisition(string requisitionId);

        // For Users

        Task<List<Requisitions>> GetRequisitionsApprovedOrRejected(string user);
    }
}
