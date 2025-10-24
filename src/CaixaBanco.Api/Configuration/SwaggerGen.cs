namespace CaixaBanco.Api.Configuration
{
    /// <summary>
    /// Classe para configurar o SwaggerGen
    /// </summary>
    public static class SwaggerGen
    {
        /// <summary>
        /// Método de extensão para adicionar o SwaggerGen customizado
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwaggerGenCustomizado(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Caixa Banco API",
                    Version = "v1",
                    Description = "API para gerenciamento de contas bancárias da Caixa Banco."
                });

                var assemblies = typeof(Program).Assembly;

                var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{assemblies.GetName().Name}.xml");

                if(File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath, true);
            });
        }
    }
}
