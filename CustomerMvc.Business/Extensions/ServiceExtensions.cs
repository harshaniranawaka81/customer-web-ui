
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CustomerMvc.Business.Services;
using CustomerMvc.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerMvc.Business.Extensions
{
    /// <summary>
    /// Class to store all service extensions
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Register all custom services
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ICustomerHttpClientService, CustomerHttpClientService>();
        }
    }
}
