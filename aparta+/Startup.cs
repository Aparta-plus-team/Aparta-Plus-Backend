using data_aparta_.Context;
using Microsoft.EntityFrameworkCore;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using data_aparta_.Repos.Contracts;
using data_aparta_.Repos.Auth;
using Amazon.Runtime;
using Amazon;
using data_aparta_.Repos.Propiedades;
using Amazon.S3;
using data_aparta_.Repos.Utils;
using data_aparta_.Models;
using data_aparta_.Repos;
using data_aparta_.Repos.Imuebles;
using data_aparta_.DTOs;
using aparta_.Services;

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
                .AddProjections()
                .AddType<UploadType>();

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


        public static IServiceCollection ConfugureAWS(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<AmazonCognitoIdentityProviderClient>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var awsCredentials = new BasicAWSCredentials(
                    configuration["AWS:AccessKey"],
                    configuration["AWS:SecretKey"]
                );

                return new AmazonCognitoIdentityProviderClient(
                    awsCredentials,
                    RegionEndpoint.GetBySystemName(configuration["AWS:Region"])
                );
            });

            services.AddSingleton<IAmazonS3>(provider =>
            {
                var awsCredentials = new BasicAWSCredentials(
                    configuration["AWS:AccessKey"],
                    configuration["AWS:SecretKey"]
                );

                return new AmazonS3Client(
                    awsCredentials,
                    RegionEndpoint.GetBySystemName(configuration["AWS:Region"])
                );
            });

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPropertyRepository, PropertyRepository>();
            services.AddScoped<IInquilinoRepository, InquilinoRepository>();
            services.AddScoped<IContratoRepository, ContratoRepository>();
            services.AddScoped<IInmuebleRepository, InmuebleRepository>();
            services.AddScoped<IFacturaRepository, FacturaRepository>();
            services.AddScoped<DashboardStatisticsService>();
            services.AddScoped<S3Uploader>();

            return services;
        }
    }
}
