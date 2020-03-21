namespace AdminPanelApp.Services
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;

    using Data;
    using Models;
    using Services.Contracts;

    using MimeKit;
    using MailKit.Net.Smtp;

    public class AdminService : IAdminService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly AdminPanelDbContext context;
        private readonly IEmailService emailService;

        public AdminService(UserManager<IdentityUser> userManager,
            AdminPanelDbContext context,
            IEmailService emailService)
        {
            this.userManager = userManager;
            this.context = context;
            this.emailService = emailService;
        }

        public async Task RegisterManager(RegisterModel registerModel)
        {
            var user = new IdentityUser
            {
                UserName = registerModel.Username,
                Email = registerModel.Email
            };
            var result = await userManager.CreateAsync(user, registerModel.Password);
            if (result.Succeeded)
            {
                var userInfo = new UserInfo()
                {
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName,
                    Hotel = registerModel.Hotel,
                    UserName = registerModel.Username
                };
                this.context.UsersInfo.Add(userInfo);
                await this.context.SaveChangesAsync();
            }
        }

        public async Task AddSupplier(AddSupplierModel supplierModel)
        {
            var newSupplier = new Supplier()
            {
                Name = supplierModel.Name,
                Email = supplierModel.Email
            };
            this.context.Suppliers.Add(newSupplier);
            await this.context.SaveChangesAsync();
        }

        public async Task RemoveSupplier(int id)
        {
            var supplier = this.context.Suppliers.Where(x => x.Id == id).FirstOrDefault();
            this.context.Suppliers.Remove(supplier);
            await this.context.SaveChangesAsync();
        }

        public async Task<List<Supplier>> GetAllSuppliers()
        {
            var suppliers = this.context.Suppliers.ToList();
            return await Task.Run(() => suppliers);
        }

        public async Task<List<Hotel>> GetAllHotels()
        {
            var hotels = this.context.Hotels.ToList();
            return await Task.Run(() => hotels);
        }

        public async Task<DateTime> GetNextProductSchedule()
        {
            var nextSchedule = this.context.NextProductsSchedule.FirstOrDefault().NextSchedule;
            return await Task.Run(() => nextSchedule);
        }    

        public async Task ChangeNextProductSchedule(NextScheduleModel nextScheduleModel)
        {
            var currentSchedule = this.context.NextProductsSchedule.FirstOrDefault();

            var requisitions = this.context.Requisitions.Where(x => x.ScheduleFor == currentSchedule.NextSchedule).ToList();

            for (int i = 0; i < requisitions.Count; i++)
            {
                requisitions[i].ScheduleFor = nextScheduleModel.NextSchedule;
                this.context.Requisitions.Update(requisitions[i]);
            }

            currentSchedule.NextSchedule = nextScheduleModel.NextSchedule;
            this.context.NextProductsSchedule.Update(currentSchedule);
            await this.context.SaveChangesAsync();
        }

        public async Task<UserInfo> ManagerInfo(string userName)
        {
            var user = this.context.UsersInfo.Where(x => x.UserName == userName).FirstOrDefault();
            return await Task.Run(() => user);
        }

        public async Task<string> GetUserFirstAndLastName(string userName)
        {
            var user = await this.userManager.FindByNameAsync(userName);
            var sb = new StringBuilder();
            var info = this.context.UsersInfo.Where(x => x.UserName == userName).FirstOrDefault();
            sb.Append(info.FirstName).Append(" ").Append(info.LastName);
            return sb.ToString();
        }

        public async Task SendApprovalEmail(string requisitionId)
        {
            await this.emailService.SendApprovalEmail(requisitionId);
        }
        
        public async Task SendProductsToSupplier()
        {
            await this.emailService.SendProductsToSupplier();
        }
        
        public async Task<List<DateTime>> GetLatestMonths()
        {
            var latestMonths = this.context.PreviousSchedules.Select(x => x.LastSchedules).Take(20).ToList();
            var thisMonth = this.context.NextProductsSchedule.FirstOrDefault().NextSchedule;
            latestMonths.Add(thisMonth);
            var orderedMonth = latestMonths.OrderByDescending(x => x).ToList();
            return await Task.Run(() => orderedMonth);
        }

        // For History

        public async Task<List<Requisitions>> GetRequisitionsFrom(int index, int? hotelIndex)
        {
            if (hotelIndex == null)
            {
                var latestMonths = await GetLatestMonths();
                var requisitions = this.context.Requisitions.Where(x => x.ScheduleFor.Month == latestMonths[index].Month && x.ScheduleFor.Year == latestMonths[index].Year).ToList();
                return await Task.Run(() => requisitions);
            }
            else
            {
                var latestMonths = await GetLatestMonths();
                var allHotels = await GetAllHotels();
                var requisitions = this.context.Requisitions.Where(x => x.ScheduleFor.Month == latestMonths[index].Month && x.ScheduleFor.Year == latestMonths[index].Year && x.Location == allHotels[(int)hotelIndex].Name).ToList();
                return await Task.Run(() => requisitions);
            }
        }

        public async Task UpdateProductQuantity(int productId, uint quantity)
        {
            var product = this.context.Product.Where(x => x.Id == productId).FirstOrDefault();
            product.Quantity = quantity;
            this.context.Product.Update(product);
            await this.context.SaveChangesAsync();
        }
    }
}
