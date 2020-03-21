namespace AdminPanelApp.Controllers
{
    using System.Threading.Tasks;
    
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Models;
    using Services.Contracts;

    [Authorize(Roles = "Admin")]
    public class RegisterUsersController : Controller
    {
        private IAdminService adminService;

        public RegisterUsersController(IAdminService adminService)
        {
            this.adminService = adminService;
        }

        public async Task<IActionResult> RegisterManager()
        {
            return await Task.Run(() => View());
        }

        public async Task<IActionResult> AddSuppliers()
        {
            return await Task.Run(() => View());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return await Task.Run(() => RedirectToAction("RegisterManager"));
            }

            await this.adminService.RegisterManager(registerModel);
            return RedirectToAction("RegisterManager");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterSupplier(AddSupplierModel supplierModel)
        {
            if (!ModelState.IsValid)
            {
                return await Task.Run(() => RedirectToAction("AddSuppliers"));
            }
            await this.adminService.AddSupplier(supplierModel);
            return RedirectToAction("AddSuppliers");
        }
    }
}