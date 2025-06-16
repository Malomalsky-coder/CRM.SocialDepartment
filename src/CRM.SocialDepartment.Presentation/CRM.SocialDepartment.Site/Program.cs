using CRM.SocialDepartment.Application.Assignments;
using CRM.SocialDepartment.Application.Patients;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Repositories;
using CRM.SocialDepartment.Site.Filters;
using CRM.SocialDepartment.Site.MappingProfile;
using CRM.SocialDepartment.Site.Middleware;
using CRM.SocialDepartment.Site.Services;
using CRM.SocialDepartment.WebApp.Settings;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace CRM.SocialDepartment.Site;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Подключение базы данных //////////////////////////////////////////////////////////////////////////////////

        var mongoDbConfiguration = builder.Configuration.GetSection(nameof(MongoDbSetting)).Get<MongoDbSetting>()
                ?? throw new InvalidOperationException("Connection string MongoDbSetting not found.");

        builder.Services.AddSingleton<IMongoDatabase>(provider =>
        {
            var mongoClient = new MongoClient(mongoDbConfiguration.ConnectionString);
            return mongoClient.GetDatabase(mongoDbConfiguration.Database);
        });

        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false; // подтверждение email
        })
        .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(
            mongoDbConfiguration.ConnectionString,
            mongoDbConfiguration.Database)
        .AddDefaultTokenProviders()
        .AddDefaultUI();

        //BsonConfiguration.RegisterMappings(); // Настройка маппинга для MongoDB

        // Регистрация репозиториев /////////////////////////////////////////////////////////////////////////////////

        builder.Services.AddScoped(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            return new MongoBasicRepository<Patient, Guid>(database, "Patients");
        });

        builder.Services.AddScoped(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            return new MongoBasicRepository<Assignment, Guid>(database, "Assignments");
        });

        // Регистрация сервисов /////////////////////////////////////////////////////////////////////////////////////

        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<ResultFilter>();
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;   // True говорит, что регистр названий свойств не учитывается.
            options.JsonSerializerOptions.PropertyNamingPolicy = null;          // Отключает camelCase
            options.JsonSerializerOptions.WriteIndented = true;                 // Включает форматирование JSON
        });
        builder.Services.AddRazorPages();
        builder.Services.AddScoped<DataTableNetService>();
        builder.Services.AddAutoMapper(typeof(ProjectMappingProfile));
        builder.Services.AddScoped<PatientAppService>();
        builder.Services.AddScoped<AssignmentService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
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
        app.UseGlobalExceptionHandler();
        app.Run();
    }
}
