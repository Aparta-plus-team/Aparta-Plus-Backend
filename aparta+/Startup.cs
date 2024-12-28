using data_aparta_.Context;
using Microsoft.EntityFrameworkCore;

namespace aparta_
{
    public static class Startup
    {
        public static IServiceCollection SetupEntityFrameworkCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextFactory<ApartaPlusContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("apartaPlusDatabase"));
            });

            return services;
        }

        public static IServiceCollection SetupGraphQL(this IServiceCollection services)
        {
            services.AddGraphQLServer()
                .RegisterDbContextFactory<ApartaPlusContext>()
                .AddTypes()
                .AddFiltering()
                .AddSorting()
                .AddProjections();

            return services;
        }

        public static IServiceCollection SetupCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                //Use after deployment ONLY
                /*options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.WithOrigins("https://aparta-plus-frontend.com", "http://aparta-plus-frontend.com", "127.0.0.1:8000")
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });*/

                //Use during development ONLY
                options.AddPolicy("AllowAnyOrigin", policy =>
                {
                    policy.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            return services;
        }
    }
}
