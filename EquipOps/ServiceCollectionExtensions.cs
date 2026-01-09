using EquipOps.BAL.Interfaces;
using EquipOps.BAL.Services;
using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.DAL.Repository;

namespace EquipOps.API
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection WithRegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            services.AddScoped<PgHelper>();

            // Auth
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IAuthService, AuthService>();

            // User
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            // Role
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IUserRoleService, UserRoleService>();

            // Email
            services.AddScoped<IEmailService, EmailService>();

            // Vendor
            services.AddScoped<IVendorRepository, VendorRepository>();
            services.AddScoped<IVendorService, VendorService>();

            return services;
        }
    }
}
