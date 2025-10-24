using CaixaBanco.Data;
using CaixaBanco.Domain.Notification;
using CaixaBanco.Domain.Repositories;
using CaixaBanco.Infrastructure.Repositories;

namespace CaixaBanco.Api.Configuration
{
    /// <summary>
    /// Classe para gerenciar a injeção de dependências
    /// </summary>
    public static class DependencyInjectionConfig
    {
        /// <summary>
        /// Método de extensão para resolver as dependências da aplicação
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection ResolveDependencias(this IServiceCollection services)
        {
            services.AddScoped<ApplicationDbContext>();

            //Repositories
            services.AddScoped<IContaRepository, ContaRepository>();
            services.AddScoped<ITransacaoRepository, TransacaoRepository>();

            //Notificador
            services.AddScoped<INotificador, Notificador>();

            return services;
        }
    }
}
