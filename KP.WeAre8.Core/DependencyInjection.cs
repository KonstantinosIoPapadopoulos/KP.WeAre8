using KP.WeAre8.Core.Interfaces;
using KP.WeAre8.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KP.WeAre8.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddScoped<IDataMergerService, DataMergerService>();

            return services;
        }
    }
}
