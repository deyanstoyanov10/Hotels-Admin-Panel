namespace AdminPanelApp.Services
{
    using AdminPanelApp.Data;
    using AdminPanelApp.Models;
    using AdminPanelApp.Services.Contracts;
    using MailKit.Net.Smtp;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using MimeKit;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class EmailService : IEmailService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly AdminPanelDbContext context;
        private readonly IConfiguration configuration;

        public EmailService(UserManager<IdentityUser> userManager,
            AdminPanelDbContext context,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.context = context;
            this.configuration = configuration;
        }

        private async Task<string> GetUserFirstAndLastName(string userName)
        {
            var user = await this.userManager.FindByNameAsync(userName);
            var sb = new StringBuilder();
            var info = this.context.UsersInfo.Where(x => x.UserName == userName).FirstOrDefault();
            sb.Append(info.FirstName).Append(" ").Append(info.LastName);
            return sb.ToString();
        }

        public async Task SendApprovalEmail(string requisitionId)
        {
            var requisition = this.context.Requisitions.Where(x => x.Id == requisitionId).FirstOrDefault();
            var user = await this.userManager.FindByNameAsync(requisition.AddedBy);
            var userFirsAndLastName = await GetUserFirstAndLastName(requisition.AddedBy);
            var messageBody = await BuildMessageBody(requisitionId);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress($"{configuration.GetSection("AdminSettings")["FirstName"]} {configuration.GetSection("AdminSettings")["LastName"]}", $"{configuration.GetSection("AdminSettings")["Email"]}"));
            message.To.Add(new MailboxAddress(userFirsAndLastName, user.Email));
            message.Subject = "Approval message";
            message.Body = new TextPart("html")
            {
                Text = messageBody
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync(configuration.GetSection("AdminSettings")["Email"], configuration.GetSection("AdminSettings")["EmailPassword"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        private async Task<string> BuildMessageBody(string requisitionId)
        {
            var requisition = this.context.Requisitions.Where(x => x.Id == requisitionId).FirstOrDefault();
            var userFirsAndLastName = await GetUserFirstAndLastName(requisition.AddedBy);
            var hotelAddress = this.context.Hotels.Where(x => x.Name == requisition.Location).FirstOrDefault();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames;
            var adminName = configuration.GetSection("AdminSettings")["FirstName"] + " " + configuration.GetSection("AdminSettings")["LastName"];
            var products = this.context.Product.Where(x => x.RequisitionId == requisitionId && x.Status == 1).ToList();
            var sb = new StringBuilder();
            sb.Append("<html> <head><style>.header{margin-top: 3%; font-family: Georgia, 'Times New Roman', Times, serif;}h1{font-size: 20px;}.doc{margin-left: 30px;}#mainText{text-align: justify;}.footer{margin-top: 30px;}</style> </head> <body class=\"doc\">")
                .Append("<div class=\"bodyText\"> <div id=\"headline\"> <h1>OBJECT: ACCEPTANCE OF ORDER WITH DELIVERY IN LOTS</h1> </div><div style=\"width: 100 %; height: 60px; \"></div><div id=\"mainText\"> Dear <div id=\"contactHotelPerson\" style=\"display: inline - block; margin - bottom: 5px; \">")
                .Append(userFirsAndLastName)
                .Append(",</div><br>We appreciate your business and we will do whatever possible to satisfy your need – please let us know if there’s anything we can do.<br>This letter is sent to confirm acceptance of your order dated <div id=\"orderDate\" style=\"display: inline - block; \">")
                .Append(requisition.Date.Day).Append(" ").Append(monthNames[requisition.Date.Month - 1]).Append(" ").Append(requisition.Date.Year)
                .Append("</div>regarding the delivery of <div id=\"products\" style=\"display: inline - block; \">")
                .Append(await MakeProductsTable(products, hotelAddress.Name))
                .Append("</div>The goods will be shipped to you in the following lots <div id=\"hotelAddress\" style=\"display: inline - block; \">")
                .Append(hotelAddress.Address)
                .Append(".</div></div></div><div class=\"footer\"> <div> <div>")
                .Append(adminName)
                .Append("</div><div style=\"font-family:'Verdana';font-style:italic;\">PA / Support & Logistics Manager</div>")
                .Append("<div><img src=\"https://media-exp1.licdn.com/dms/image/C4D0BAQHLgWwBSA78Gg/company-logo_200_200/0?e=2159024400&v=beta&t=Z_yMYkTbVHxGWVViG6LetouOWXvIdt7GJkPuHKyHBas\"></div>")
                .Append("<div style=\"font-weight:bold;\">De 5 Stjerner A/S</div>")
                .Append("<div>Jernholmen 39-41</div>")
                .Append("<div>2650  Hvidovre</div>")
                .Append("<div>t.  +45 7021 6006</div>")
                .Append("<div>f.  +45 7025 2836</div>")
                .Append("<div>m. +45 3043 3156</div>")
                .Append("<div>").Append(configuration.GetSection("AdminSettings")["Email"]).Append("</div>")
                .Append("<div>www.de5stjerner.dk</div>")
                .Append("<div style=\"color:green;font-size:14px;\">De 5 Stjerner A/S is an independent Danish-owned cleaning company, employing approximately 1300 people. We specialise in hotel services, focusing primarily on cleaning. On a day-to-day basis as your active sparring partner, we strive for top quality at competitive prices</div>")
                .Append("<div style=\"font-size:15px;font-weight:bold;\">Confidentiality and terms of business:</div>")
                .Append("<div style=\"font-size:13px;\">This e-mail and any attachments are confidential and maybe privileged. If you are not a named recipient, please notify the sender immediately and do not disclose the contents to another person, use it for any purpose or store or copy the information in any medium. The contents of any e-mail addressed to our clients and work performed by De 5 Stjerner A/S – Vision Service ApS.</div>")
                .Append("</div></div></body></html>");

            return sb.ToString();
        }

        //End Approval message

        //===== Start Supplier Messages =====//
        public async Task SendProductsToSupplier()
        {
            var date = this.context.NextProductsSchedule.FirstOrDefault();
            var dateNow = DateTime.UtcNow;
            if (dateNow >= date.NextSchedule)
            {
                
                var suppliers = this.context.Suppliers.ToArray();
                for (int i = 0; i < suppliers.Length; i++)
                {
                    var supplier = suppliers[i];
                    var hotels = this.context.Hotels.ToList();
                    var messageBody = "";
                    for (int j = 0; j < hotels.Count; j++)
                    {
                        var products = await TakeItemsFromDataBase(date.NextSchedule, supplier.Name, hotels[j].Name);
                        if (products.Count == 0)
                        {
                            continue;
                        }
                        messageBody += await MakeProductsTable(products, hotels[j].Name);
                    }

                    
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress($"{configuration.GetSection("AdminSettings")["FirstName"]} {configuration.GetSection("AdminSettings")["LastName"]}", $"{configuration.GetSection("AdminSettings")["Email"]}"));
                    message.To.Add(new MailboxAddress(supplier.Name, supplier.Email));
                    message.Subject = "Requested products";
                    message.Body = new TextPart("html")
                    {
                        Text = messageBody
                    };

                    if (messageBody != "")
                    {
                        using (var client = new SmtpClient())
                        {
                            await client.ConnectAsync("smtp.gmail.com", 587, false);
                            await client.AuthenticateAsync(configuration.GetSection("AdminSettings")["Email"], configuration.GetSection("AdminSettings")["EmailPassword"]);
                            await client.SendAsync(message);
                            await client.DisconnectAsync(true);
                        }
                    }
                }
                UpdateNextProductsSchedule();
                UpdateRequisitions(dateNow);
            }
        }

        private void UpdateNextProductsSchedule()
        {
            var currentSchedule = this.context.NextProductsSchedule.FirstOrDefault();
            this.context.PreviousSchedules.Add(new PreviousSchedule() { LastSchedules = currentSchedule.NextSchedule});
            var newSchedule = currentSchedule.NextSchedule.AddMonths(1);
            currentSchedule.NextSchedule = newSchedule;
            this.context.NextProductsSchedule.Update(currentSchedule);
            this.context.SaveChanges();
        }

        private void UpdateRequisitions(DateTime date)
        {
            var requisitions = this.context.Requisitions.Where(x => x.Date <= date && x.Status != 4).ToList();
            var requisitionsCount = requisitions.Count;

            for (int i = 0; i < requisitionsCount; i++)
            {
                requisitions[i].Status = 4;
                this.context.Requisitions.Update(requisitions[i]);
            }
            this.context.SaveChanges();
        }

        private async Task<List<Product>> TakeItemsFromDataBase(DateTime schedule, string supplier, string hotelName)
        {
            var requisitions = await this.context.Requisitions.Where(x => x.ScheduleFor.Day == schedule.Day && x.ScheduleFor.Month == schedule.Month && x.ScheduleFor.Year == schedule.Year && x.Status == 2 && x.Location == hotelName).Select(x => new { Id = x.Id }).ToListAsync();
            var requisitionsCount = requisitions.Count;
            var productsList = new List<Product>();
            for (int i = 0; i < requisitionsCount; i++)
            {
                var products = await this.context.Product.Where(x => x.RequisitionId == requisitions[i].Id && x.Status == 1 && x.Supplier == supplier).ToListAsync();
                var productsCount = products.Count;
                for (int j = 0; j < productsCount; j++)
                {
                    productsList.Add(products[j]);
                }
            }

            return productsList;
        }

        private async Task<string> MakeProductsTable(List<Product> products, string hotelName)
        {
            var sb = new StringBuilder();
            sb.Append("<html><body><table style=\"border: 3px solid black;\"><thead><tr><th colspan=\"7\">").Append(hotelName).Append("</th></tr><th style=\"border: 3px solid black;\">&#8470;</th><th style=\"border: 3px solid black;\">Name</th><th style=\"border: 3px solid black;\">Unit</th><th style=\"border: 3px solid black;\">Category</th><th style=\"border: 3px solid black;\">Packaging</th><th style=\"border: 3px solid black;\">Quantity</th><th style=\"border: 3px solid black;\">Location</th><th style=\"border: 3px solid black;\">Address</th></thead>")
                .Append("<tbody>");

            var productsCount = products.Count;
            for (int i = 0; i < productsCount; i++)
            {
                sb.Append("<tr><td style=\"border: 3px solid black;\">").Append(i + 1).Append("</td>")
                    .Append("<td style=\"border: 3px solid black;\">").Append(products[i].Name).Append("</td>")
                    .Append("<td style=\"border: 3px solid black;\">").Append(products[i].Unit).Append("</td>")
                    .Append("<td style=\"border: 3px solid black;\">").Append(products[i].Category).Append("</td>")
                    .Append("<td style=\"border: 3px solid black;\">").Append(products[i].Packaging).Append("</td>")
                    .Append("<td style=\"border: 3px solid black;\">").Append(products[i].Quantity).Append("</td>")
                    .Append("<td style=\"border: 3px solid black;\">").Append(await GetLocation(products[i].RequisitionId)).Append("</td>")
                    .Append("<td style=\"border: 3px solid black;\">").Append(await GetHotelAddress(products[i].RequisitionId)).Append("</td>")
                    .Append("</tr>");
            }
            sb.Append("</tbody></table></body></html>");
            return sb.ToString();
        }

        private async Task<string> GetHotelAddress(string requisitionId)
        {
            var location = await GetLocation(requisitionId);
            var hotelAddress = this.context.Hotels.Where(x => x.Name == location).Select(x => x.Address).FirstOrDefault();
            
            return hotelAddress;
        }

        private async Task<string> GetLocation(string requisitionId)
        {
            var location = await Task.Run(() => this.context.Requisitions.Where(x => x.Id == requisitionId).Select(x => x.Location).FirstOrDefault());
            return location;
        }

        // Excel Tables

        public async Task<List<Requisitions>> GetRequisitionsForSupplier(int month, int year, ushort status)
        {
            var requisitions = await this.context.Requisitions.Where(x => x.ScheduleFor.Month == month && x.ScheduleFor.Year == year && x.Status == status).ToListAsync();
            return requisitions;
        }

        public async Task<Supplier> GetSupplier(int supplierId)
        {
            var supplier = await Task.Run(() => this.context.Suppliers.Where(x => x.Id == supplierId).FirstOrDefault());
            return supplier;
        }

        public async Task<List<Product>> ProductFilter(List<Requisitions> requisitionsForHotel, string supplierName)
        {
            var products = new List<Product>();
            for (int i = 0; i < requisitionsForHotel.Count; i++)
            {
                var subProduct = await this.context.Product.Where(x => x.RequisitionId == requisitionsForHotel[i].Id && x.Supplier == supplierName && x.Status == 1).ToListAsync();
                for (int j = 0; j < subProduct.Count; j++)
                {
                    products.Add(subProduct[j]);
                }
            }

            return products;
        }

        public string GetHotelAddressByName(string hotelName)
        {
            var hotel = this.context.Hotels.Where(x => x.Name == hotelName).Select(x => x.Address).FirstOrDefault();
            return hotel;
        }
    }
}
