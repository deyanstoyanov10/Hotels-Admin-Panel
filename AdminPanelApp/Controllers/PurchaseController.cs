namespace AdminPanelApp.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Models;
    using Services.Contracts;

    [Authorize]
    public class PurchaseController : Controller
    {
        private IRequisitionService requisitionService;

        public PurchaseController(IRequisitionService requisitionService)
        {
            this.requisitionService = requisitionService;
        }

        public async Task<IActionResult> Index()
        {
            return await Task.Run(() => View());
        }

        public async Task<IActionResult> Create(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            return await Task.Run(() => View());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequisition(string location, string user)
        {
            if (location == null || user == null)
            {
                return BadRequest();
            }

            var requisitionId = await this.requisitionService.CreateRequisition(location, user);
            return RedirectToAction("Create", new { id = requisitionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmRequisition(RequisitionModel requisition, string button)
        {
            if (!ModelState.IsValid || button == null)
            {
                return BadRequest();
            }

            if (button == "Yes")
            {
                await this.requisitionService.UpdateRequisitionStatus(requisition.RequisitionId, 1);
                return RedirectToAction("Index", "Home");
            }
            else if (button == "No")
            {
                await this.requisitionService.DeleteRequisition(requisition.RequisitionId);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}