namespace AdminPanelApp.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    using Models;
    using Services.Contracts;

    [Authorize(Roles = "Admin")]
    public class ManagementController : Controller
    {
        private readonly IEmailService emailService;
        private readonly IAdminService adminService;
        private readonly IProductService productService;

        public ManagementController(IEmailService emailService, IAdminService adminService, IProductService productService)
        {
            this.emailService = emailService;
            this.adminService = adminService;
            this.productService = productService;
        }

        public async Task<IActionResult> ChangeProductSupplier()
        {
            return await Task.Run(() => View());
        }

        public async Task<IActionResult> ChangeEmailTimer()
        {
            return await Task.Run(() => View());
        }
        
        public async Task<IActionResult> SupplierMessages()
        {
            return await Task.Run(() => View());
        }

        public async Task<IActionResult> History()
        {
            return await Task.Run(() => View());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeSupplierToProduct(ChangeProductSupplierModel changeProductSupplierModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            await this.productService.ChangeProductSupplier(changeProductSupplierModel);
            return RedirectToAction("ChangeProductSupplier");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeSupplier(ChangeSupplierModel changeSupplierModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            await this.productService.ChangeSupplier(changeSupplierModel);
            return RedirectToAction("ChangeProductSupplier");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmailSchedule(NextScheduleModel nextScheduleModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            await this.adminService.ChangeNextProductSchedule(nextScheduleModel);
            return RedirectToAction("ChangeEmailTimer");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveSupplierMessages()
        {
            await this.emailService.SendProductsToSupplier();
            return await Task.Run(() => RedirectToAction("SupplierMessages"));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DownloadProductTable(int supplierId, int dateIndex)
        {
            if (supplierId == 0)
            {
                return BadRequest();
            }

            var supplier = await this.emailService.GetSupplier(supplierId);
            var latestMonths = await this.adminService.GetLatestMonths();
            ushort status = 2;
            if (dateIndex != 0)
            {
                status = 4;
            }
            var requisitions = await this.emailService.GetRequisitionsForSupplier(latestMonths[dateIndex].Month, latestMonths[dateIndex].Year, status);
            var hotels = requisitions.Select(x => x.Location).Distinct().ToList();

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.Add(supplier.Name);
                int tableIndex = 1;

                sheet.Cells[tableIndex, 1].Value = "Purchase Order";
                sheet.Cells[tableIndex, 1, tableIndex, 6].Merge = true; //Merge columns start and end range
                sheet.Cells[tableIndex, 1, tableIndex, 6].Style.Font.Bold = true; //Font should be bold
                sheet.Cells[tableIndex, 1, tableIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Alignment is center
                sheet.Cells[tableIndex, 1, tableIndex, 6].Style.Font.Size = 20;
                sheet.Cells[tableIndex, 1, tableIndex, 6].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                sheet.Cells[tableIndex, 1, tableIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                sheet.Cells[tableIndex, 1, tableIndex, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                sheet.Cells[tableIndex, 1, tableIndex, 6].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                tableIndex++;

                for (int i = 0; i < hotels.Count; i++)
                {
                    tableIndex++;

                    sheet.Cells[tableIndex, 1].Value = "Hotel";
                    sheet.Cells[tableIndex, 1].Style.Font.Bold = true; //Font should be bold
                    sheet.Cells[tableIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Alignment is center
                    sheet.Cells[tableIndex, 1].Style.Font.Size = 15;
                    sheet.Cells[tableIndex, 1].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    sheet.Cells[tableIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    sheet.Cells[tableIndex, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    sheet.Cells[tableIndex, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    

                    sheet.Cells[tableIndex, 2].Value = hotels[i];
                    sheet.Cells[tableIndex, 2, tableIndex, 6].Merge = true; //Merge columns start and end range
                    sheet.Cells[tableIndex, 2, tableIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Alignment is center
                    sheet.Cells[tableIndex, 2, tableIndex, 6].Style.Font.Size = 15;
                    sheet.Cells[tableIndex, 2, tableIndex, 6].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    sheet.Cells[tableIndex, 2, tableIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    sheet.Cells[tableIndex, 2, tableIndex, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    sheet.Cells[tableIndex, 2, tableIndex, 6].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    tableIndex++;

                    sheet.Cells[tableIndex, 1].Value = "Address";
                    sheet.Cells[tableIndex, 1].Style.Font.Bold = true; //Font should be bold
                    sheet.Cells[tableIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Alignment is center
                    sheet.Cells[tableIndex, 1].Style.Font.Size = 15;
                    sheet.Cells[tableIndex, 1].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    sheet.Cells[tableIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    sheet.Cells[tableIndex, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    sheet.Cells[tableIndex, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    

                    sheet.Cells[tableIndex, 2].Value = this.emailService.GetHotelAddressByName(hotels[i]);
                    sheet.Cells[tableIndex, 2, tableIndex, 6].Merge = true; //Merge columns start and end range
                    sheet.Cells[tableIndex, 2, tableIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Alignment is center
                    sheet.Cells[tableIndex, 2, tableIndex, 6].Style.Font.Size = 15;
                    sheet.Cells[tableIndex, 2, tableIndex, 6].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    sheet.Cells[tableIndex, 2, tableIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    sheet.Cells[tableIndex, 2, tableIndex, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    sheet.Cells[tableIndex, 2, tableIndex, 6].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    tableIndex++;

                    string[] tableHead = { "#", "Name", "Unit", "Category", "Packaging", "Quantity" };

                    for (int index = 0; index < tableHead.Length; index++)
                    {
                        sheet.Cells[tableIndex, index + 1].Value = tableHead[index];
                        sheet.Cells[tableIndex, index + 1].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                        sheet.Cells[tableIndex, index + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        sheet.Cells[tableIndex, index + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 0));
                        sheet.Cells[tableIndex, index + 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                        sheet.Cells[tableIndex, index + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                        sheet.Cells[tableIndex, index + 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    }

                    tableIndex++;
                    var requisitionsForHotel = requisitions.Where(x => x.Location == hotels[i]).ToList();
                    var products = await this.emailService.ProductFilter(requisitionsForHotel, supplier.Name);

                    for (int j = 0; j < products.Count; j++)
                    {
                        sheet.Cells[tableIndex, 1].Value = j + 1;
                        sheet.Cells[tableIndex, 2].Value = products[j].Name;
                        sheet.Cells[tableIndex, 3].Value = products[j].Unit;
                        sheet.Cells[tableIndex, 4].Value = products[j].Category;
                        sheet.Cells[tableIndex, 5].Value = products[j].Packaging;
                        sheet.Cells[tableIndex, 6].Value = products[j].Quantity;
                        tableIndex++;
                    }
                }
                sheet.Cells[$"A1:F{tableIndex}"].AutoFitColumns();
                package.Save();
            }
            stream.Position = 0;
            var fileName = $"{supplier.Name} - Requested Products For - {latestMonths[dateIndex].ToString("dd/MM/yyyy HH:mm:ss")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}