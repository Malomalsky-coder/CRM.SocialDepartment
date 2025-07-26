using CRM.SocialDepartment.Application.Assignments;
using CRM.SocialDepartment.Application.Handlers.EventHandlers;
using CRM.SocialDepartment.Application.Patients;
using CRM.SocialDepartment.Application.Users;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Events;
using CRM.SocialDepartment.Domain.Repositories;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Repositories;
using CRM.SocialDepartment.Infrastructure.Identity;
using CRM.SocialDepartment.Site.Filters;
using CRM.SocialDepartment.Site.MappingProfile;
using CRM.SocialDepartment.Site.Middleware;
using CRM.SocialDepartment.Site.Services;
using CRM.SocialDepartment.WebApp.Settings;
using DDD.Events;
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

        // Регистрация MediatR и доменных событий ///////////////////////////////////////////////////////////////////
        
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PatientCreatedEventHandler).Assembly));
        
        // Регистрация диспетчера доменных событий
        builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Регистрация Unit of Work //////////////////////////////////////////////////////////////////////////////////

        builder.Services.AddScoped<IUnitOfWork>(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            var domainEventDispatcher = provider.GetService<IDomainEventDispatcher>();
            return new MongoUnitOfWork(database, domainEventDispatcher);
        });

        // Регистрация отдельных репозиториев для обратной совместимости (опционально)
        builder.Services.AddScoped<IPatientRepository>(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            return new MongoPatientRepository(database);
        });

        builder.Services.AddScoped<IAssignmentRepository>(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            return new MongoAssignmentRepository(database);
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
        builder.Services.AddScoped<UserRepository>();
        builder.Services.AddScoped<UserAppService>();

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
