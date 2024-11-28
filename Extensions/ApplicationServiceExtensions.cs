using DatingApp.BusinessLayer;
using DatingApp.BusinessLayer.Interface;
using DatingApp.Data;
using DatingApp.Helpers;
using DatingApp.Models;
using DatingApp.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IRegisterUserDetail, RegisterUserDetail>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository,UserRepository>();
            services.AddScoped<LogUserActivity>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            // add services for signal R
            services.AddSignalR();
            services.AddSingleton<PresenceTracker>();
            return services;
        }
    }
}
