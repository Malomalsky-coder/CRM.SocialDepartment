using CRM.SocialDepartment.Application.Patients;
using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Repositories;
using CRM.SocialDepartment.WebApp.Settings;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace CRM.SocialDepartment.Site;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Подключение базы данных ///////////////////////////////////////////////////////////////////////////////

        var mongoDbConfiguration = builder.Configuration.GetSection(nameof(MongoDbSetting)).Get<MongoDbSetting>()
                ?? throw new InvalidOperationException("Connection string MongoDbSetting not found.");

        //Подключение MongoDB.EntityFrameworkCore
        //builder.Services.AddDbContext<IDbContext, ApplicationDbContext>(options =>
        //            options.UseMongoDB(mongoDbConfiguration.ConnectionString, mongoDbConfiguration.Name)
        //                .EnableSensitiveDataLogging());

        //Подключение MongoDB.Driver
        builder.Services.AddSingleton<IMongoDatabase>(provider =>
        {
            var mongoClient = new MongoClient(mongoDbConfiguration.ConnectionString);
            return mongoClient.GetDatabase(mongoDbConfiguration.Database);
        });

        //TODO: Разобраться как подключать при MongoDB.Driver
        //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

        //Подробнее:
        //1. https://dev.to/ashirafumiiro/using-the-new-ef-core-provider-for-mongodb-with-aspnet-core-identity-311
        //2. https://www.freecodecamp.org/news/using-entity-framework-core-with-mongodb/

        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true; // подтверждение email
        })
        .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(
            mongoDbConfiguration.ConnectionString,
            mongoDbConfiguration.Database)
        .AddDefaultTokenProviders();

        //Реализация подключения базы данных по умолчанию
        //var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        //builder.Services.AddDbContext<ApplicationDbContext>(options =>
        //    options.UseSqlServer(connectionString));
        //builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        //    .AddEntityFrameworkStores<ApplicationDbContext>();

        // Регистрация репозиториев /////////////////////////////////////////////////////////////////////////////////
        builder.Services.AddScoped(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            return new MongoBasicRepository<Patient, Guid>(database, "Patients");
        });

        // Регистрация сервисов /////////////////////////////////////////////////////////////////////////////////////
        builder.Services.AddControllersWithViews();
        builder.Services.AddControllers();
        builder.Services.AddRazorPages();
        builder.Services.AddScoped<PatientAppService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
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
