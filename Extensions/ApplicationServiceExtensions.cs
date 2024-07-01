using DatingApp.Data;
using DatingApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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
            
            services.AddScoped<IUserDetails, UserDetails>();
            services.AddScoped<IRegisterUserDetail, RegisterUserDetail>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}
