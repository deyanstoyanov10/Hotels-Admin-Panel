namespace AdminPanelApp.Controllers
{
    using System.IO;
    using System.Threading.Tasks;
    
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Authorization;

    using Models;
    using Services.Contracts;

    [Authorize(Roles = "Admin")]
    public class AddProductsController : Controller
    {
        private IProductService productService;

        public AddProductsController(IProductService productService)
        {
            this.productService = productService;
        }

        public async Task<IActionResult> AddProduct()
        {
            return await Task.Run(() => View());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProductPost(AddProductModel productModel, IFormFile file)
        {
            if (ModelState.IsValid && file != null)
            {
                byte[] picture = null;

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    picture = stream.ToArray();
                }

                await this.productService.AddProduct(productModel, picture);

                return RedirectToAction("AddProduct", new { id = 1});
            }
            else
            {
                return await Task.Run(() => RedirectToAction("AddProduct", new { id = 1 }));
            }
        }
    }
}