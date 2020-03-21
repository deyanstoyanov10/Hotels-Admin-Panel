namespace AdminPanelApp
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpsPolicy;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    
    using Data;

    using Models;
    using Services;
    using Services.Contracts;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

                options.Lockout.DefaultLockoutTimeSpan = new TimeSpan(0, 15, 0);
            });

            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IRequisitionService, RequisitionService>();
            services.AddTransient<IApprovalService, ApprovalService>();
            services.AddTransient<IAdminService, AdminService>();
            services.AddTransient<IEmailService, EmailService>();

            services.AddDbContext<AdminPanelDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AdminPanelDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddAntiforgery();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services, AdminPanelDbContext dataContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            dataContext.Database.Migrate();
            CreateUserRoles(services).Wait();
            SetupDatabase(services).Wait();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "api",
                    template: "api/{controller=Values}/{action=Get}");
            });
        }

        private async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetService(typeof(AdminPanelDbContext)) as AdminPanelDbContext;
            //Adding Admin Roles

            await this.CreateRole(roleManager, "Admin");

            //TODO : Assign Admin role to the main User here we have given our newly loregistered login id for Admin management
            var poweruser = new IdentityUser
            {
                UserName = Configuration.GetSection("AdminSettings")["UserName"],
                Email = Configuration.GetSection("AdminSettings")["Email"]
            };

            var userInfo = new UserInfo()
            {
                FirstName = Configuration.GetSection("AdminSettings")["FirstName"],
                LastName = Configuration.GetSection("AdminSettings")["LastName"],
                Hotel = "All",
                UserName = Configuration.GetSection("AdminSettings")["UserName"]
            };

            string userPassword = Configuration.GetSection("AdminSettings")["UserPassword"];
            var _user = await userManager.FindByEmailAsync(Configuration.GetSection("AdminSettings")["Email"]);

            if (_user == null)
            {
                var createPowerUser = await userManager.CreateAsync(poweruser, userPassword);
                
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the "Admin" role 
                    await userManager.AddToRoleAsync(poweruser, "Admin");
                }
                await context.UsersInfo.AddAsync(userInfo);
                await context.SaveChangesAsync();
            }
        }

        private async Task CreateRole(RoleManager<IdentityRole> roleManager, string role)
        {
            var roleCheck = await roleManager.RoleExistsAsync(role);
            if (!roleCheck)
            {
                //create the roles and seed them to the database   
                await roleManager.CreateAsync(new IdentityRole() { Name = role });
            }

        }

        private async Task SetupDatabase(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService(typeof(AdminPanelDbContext)) as AdminPanelDbContext;

            // Setup New Schedule

            var nextSchedule = await context.NextProductsSchedule.FirstOrDefaultAsync();
            if (nextSchedule == null)
            {
                var newDate = new DateTime(2020, 3, 6);
                var newSchedule = new NextProductsSchedule()
                {
                    NextSchedule = newDate
                };
                context.NextProductsSchedule.Add(newSchedule);
                await context.SaveChangesAsync();
            }

            // Add Hotels (Location)

            var hotels = await context.Hotels.ToListAsync();
            if (hotels.Count == 0)
            {
                var hotelsList = new List<List<string>>()
                {
                    new List<string> () { "Raddison Collection Royal Hotel", "Hammerichsgade 1", "1611", "København"},
                    new List<string> () { "Hotel Niels Juel", "Toldbodvej 20", "4600", "Køge"},
                    new List<string> () { "Absalon Hotel", "Helgolandsgade 15", "1653", "København"},
                    new List<string> () { "Skt.Petri", "Krystalgade 22", "1172", "København"},
                    new List<string> () { "Hôtel First Mayfair", "Helgolandsgade 3", "1653", "København"},
                    new List<string> () { "First Hotel Twentyseven", "Løngangstræde 27", "1468", "København"},
                    new List<string> () { "Hellerup Parkhotel A / S", "Strandvejen 203", "2900", "Hellerup"},
                    new List<string> () { "Clarion Hotel Copenhagen Airport", "Ellehammersvej 20", "2770", "Kastrup"},
                    new List<string> () { "Hotel D'Angleterre", "Kongens Nytorv 34", "1050", "København K"},
                    new List<string> () { "Wakeup Copenhagen Borgergade", "Borgergade 9", "1300", "København"},
                    new List<string> () { "Wakeup Copenhagen Bernstorffsgade", "Bernstorffsgade 35", "1577", "København"},
                    new List<string> () { "Wakeup Copenhagen", "Carsten Niebuhrs Gade 11", "1577", "København"},
                    new List<string> () { "Tivoli Hotel", "Arni Magnussons Gade 2", "1577", "København"},
                    new List<string> () { "Imperial Hotel", "Vester Farimagsgade 9", "1606", "København"},
                    new List<string> () { "Hôtel de Ibsen", "Vendersgade 23", "1363", "København K"},
                    new List<string> () { "Andersen Hotel", "Helgolandsgade 12", "1653", "København"},
                    new List<string> () { "Danhostel Copenhagen City", "H. C. Andersens Blvd. 50", "1553", "København"},
                    new List<string> () { "First Hotel Kong Frederik", "Vester Voldgade 25", "1552", "København"},
                    new List<string> () { "Hotel Kong Arthur", "Nørre Søgade 11", "1370", "København K"},
                    new List<string> () { "CABINN Copenhagen", "Arni Magnussons Gade 1", "1577", "København K"},
                    new List<string> () { "Admiral Hotel", "Toldbodgade 24-28", "2531", "København K"},
                    new List<string> () { "Kokkedal Slot Copenhagen", "Kokkedal Alle 6", "2970", "Hørsholm"},
                    new List<string> () { "Comwell Køge Strand", "Strandvejen 111", "4600", "Køge"},
                    new List<string> () { "CABINN Vejle", "Dæmningen 6", "7100", "Vejle"},
                    new List<string> () { "First Hotel Kolding", "Banegårdspladsen 7", "6000", "Kolding"},
                    new List<string> () { "Comwell Kolding", "Skovbrynet 1", "6000", "Kolding"},
                    new List<string> () { "Kolding Hotel Apartments", "Kedelsmedgangen 2", "6000", "Kolding"},
                    new List<string> () { "Kongensbro Kro", "Gl Kongevej 70", "8643", "Silkeborg"},
                    new List<string> () { "Comwell Kellers Park", "H. O. Wildenskovsvej 28", "7080", "Børkop"},
                    new List<string> () { "Comwell Korsør", "Ørnumvej 6", "4220", "Korsør"},
                    new List<string> () { "Comwell Klarskovgaard", "Korsør Lystskov 30", "4220", "Korsør"},
                    new List<string> () { "Hotel Kirstine", "Købmagergade 20", "4700", "Næstved"},
                    new List<string> () { "Comwell Sorø", "Abildvej 100", "4180", "Sorø"},
                    new List<string> () { "Comwell Borupgaard", "Nørrevej 80", "3070", "Snekkersten"},
                    new List<string> () { "Comwell Holte", "Kongevejen 495A", "2840", "Holte"},
                    new List<string> () { "First Hotel Atlantic", "Europaplads 10", "8000", "Aarhus"}
                };

                for (int i = 0; i < hotelsList.Count; i++)
                {
                    var newHotel = new Hotel()
                    {
                        Name = hotelsList[i][0],
                        Address = hotelsList[i][1],
                        City = hotelsList[i][3],
                        PostralCode = hotelsList[i][2],
                    };
                    await context.Hotels.AddAsync(newHotel);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
