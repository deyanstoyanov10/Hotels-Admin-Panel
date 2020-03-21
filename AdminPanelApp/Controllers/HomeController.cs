namespace AdminPanelApp.Controllers
{
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Models;
    using Services.Contracts;

    public class HomeController : Controller
    {
        private IApprovalService approvalService;
        private IAdminService adminService;

        public HomeController(IApprovalService approvalService, IAdminService adminService)
        {
            this.approvalService = approvalService;
            this.adminService = adminService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            return await Task.Run(() => View());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            return await Task.Run(() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveOrRejectRequisition(string requisitionId, string button)
        {
            if (requisitionId == null || button == null)
            {
                return RedirectToAction("Index");
            }

            if (button == "Yes")
            {
                await this.approvalService.ApproveOrRejectRequisition(requisitionId, 2);
                await this.adminService.SendApprovalEmail(requisitionId);
                return RedirectToAction("Index");
            }
            else
            {
                await this.approvalService.ApproveOrRejectRequisition(requisitionId, 3);
                return RedirectToAction("Index");
            }
        }
    }
}
