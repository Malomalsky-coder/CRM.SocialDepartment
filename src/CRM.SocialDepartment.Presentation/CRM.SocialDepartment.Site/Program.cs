using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CRM.SocialDepartment.Infrastructure.Interfaces.Inrastructures;
using CRM.SocialDepartment.WebApp.Settings;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data;

namespace CRM.SocialDepartment.Site;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");;

        var mongoDbConfiguration = builder.Configuration.GetSection(nameof(MongoDbSetting)).Get<MongoDbSetting>()
                ?? throw new InvalidOperationException("Connection string MongoDbSetting not found."); ;

        builder.Services.AddDbContext<IDbContext, ApplicationDbContext>(options =>
                    options.UseMongoDB(mongoDbConfiguration.ConnectionString, mongoDbConfiguration.Name)
                        .EnableSensitiveDataLogging());

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

        //���������:
        //1. https://dev.to/ashirafumiiro/using-the-new-ef-core-provider-for-mongodb-with-aspnet-core-identity-31
        //2. https://www.freecodecamp.org/news/using-entity-framework-core-with-mongodb/

        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options => {
            options.SignIn.RequireConfirmedAccount = true; // ��������� ������������� email
        })
        .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(
            mongoDbConfiguration.ConnectionString,
            mongoDbConfiguration.Name)
        .AddSignInManager()
        .AddDefaultTokenProviders();

        //var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        //builder.Services.AddDbContext<ApplicationDbContext>(options =>
        //    options.UseSqlServer(connectionString));
        //builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        //    .AddEntityFrameworkStores<ApplicationDbContext>();
        //builder.Services.AddControllersWithViews();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.MapStaticAssets();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();
        app.MapRazorPages()
           .WithStaticAssets();
        app.Run();
    }
}
