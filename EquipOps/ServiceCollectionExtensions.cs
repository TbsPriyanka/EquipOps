<<<<<<< Updated upstream
﻿using EquipOps.Common.DbConnection;
=======
﻿using EquipOps.API.Services.Implementation;
using EquipOps.API.Services.Interface;
using EquipOps.BAL.Interfaces;
using EquipOps.BAL.Services;
using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.DAL.Repository;
>>>>>>> Stashed changes

namespace EquipOps.API
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection WithRegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
<<<<<<< Updated upstream
=======
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
			// Equipment
			services.AddScoped<IEquipmentRepository, EquipmentRepository>();
			services.AddScoped<IEquipmentService, EquipmentService>();

            // Vendor
            services.AddScoped<IVendorRepository, VendorRepository>();
            services.AddScoped<IVendorService, VendorService>();
>>>>>>> Stashed changes

            return services;
        }
    }
}
