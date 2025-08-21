using CRM.SocialDepartment.Application.Assignments;
using CRM.SocialDepartment.Application.Handlers.EventHandlers;
using CRM.SocialDepartment.Application.Patients;
using CRM.SocialDepartment.Application.Users;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Events;
using CRM.SocialDepartment.Domain.Repositories;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Repositories;
//using CRM.SocialDepartment.Infrastructure.Identity;
using CRM.SocialDepartment.Site.Extensions;
using CRM.SocialDepartment.Site.Filters;
using CRM.SocialDepartment.Site.Localization;
using CRM.SocialDepartment.Site.MappingProfile;
using CRM.SocialDepartment.Site.Middleware;
using CRM.SocialDepartment.Site.Services;
using CRM.SocialDepartment.WebApp.Settings;
using DDD.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
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
            options.SignIn.RequireConfirmedEmail = false; // подтверждение email
            options.SignIn.RequireConfirmedPhoneNumber = false; // подтверждение телефона
            
            // Отключаем регистрацию
            options.User.RequireUniqueEmail = false;
            
            // Настройки паролей
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;
            
            // Отключаем блокировку аккаунта
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = false;
        })
        .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(
            mongoDbConfiguration.ConnectionString,
            mongoDbConfiguration.Database)
        .AddDefaultTokenProviders()
        .AddDefaultUI()
        .AddErrorDescriber<RussianIdentityErrorDescriber>(); // Подключаем русский ErrorDescriber

        BsonConfiguration.RegisterMappings(); // Настройка маппинга для MongoDB

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

        // Регистрация сервисов /////////////////////////////////////////////////////////////////////////////////////

        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<ResultFilter>();
            // Регистрируем кастомные конвертеры
            options.ModelBinderProviders.Insert(0, new DocumentTypeConverterProvider());
            options.ModelBinderProviders.Insert(0, new CitizenshipTypeConverterProvider());
            options.ModelBinderProviders.Insert(0, new HospitalizationTypeConverterProvider());
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;   // True говорит, что регистр названий свойств не учитывается.
            options.JsonSerializerOptions.PropertyNamingPolicy = null;          // Отключаем camelCase
            options.JsonSerializerOptions.WriteIndented = true;                 // Включаем форматирование JSON
        })
        .AddControllersAsServices();

        builder.Services.AddRazorPages();
        builder.Services.AddScoped<DataTableNetService>();
        builder.Services.AddAutoMapper(typeof(ProjectMappingProfile));
        builder.Services.AddScoped<PatientAppService>();
        builder.Services.AddScoped<AssignmentService>();
        builder.Services.AddScoped<UserAppService>();

        // Добавляем поддержку CORS
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        // Настройка культуры для правильного отображения дат
        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[] { "ru-RU" };
            options.SetDefaultCulture(supportedCultures[0])
                   .AddSupportedCultures(supportedCultures)
                   .AddSupportedUICultures(supportedCultures);
        });

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
        app.UseStaticFiles();

        app.UseCors();
        app.UseRouting();
        app.UseRequestLocalization();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<GlobalExceptionHandler>();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapControllers();
        app.MapRazorPages();

        app.Run();
    }
}
