using CaixaBanco.Data;
using CaixaBanco.Domain.Notification;
using CaixaBanco.Domain.Repositories;
using CaixaBanco.Infrastructure.Repositories;

namespace CaixaBanco.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencias(this IServiceCollection services)
        {
            services.AddScoped<ApplicationDbContext>();

            //Repositories
            services.AddScoped<IContaRepository, ContaRepository>();

            //Notificador
            services.AddScoped<INotificador, Notificador>();

            return services;
        }
    }
}
