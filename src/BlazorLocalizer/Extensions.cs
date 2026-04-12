using BlazorLocalizer.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlazorLocalizer
{
    public static class Extensions
    {
        public static IServiceCollection AddBlazorLocalization(this IServiceCollection services, Action<BlazorLocalizerOptions> config)
        {
            return services
                .AddSingleton<ILocalStorageAccessor, LocalStorageAccessor>()
                .AddTransient<IBlazorLocalizer, BlazorLocalizer>()
                .AddSingleton<ResourceCache>()
                .Configure<BlazorLocalizerOptions>(cfg => config?.Invoke(cfg));
        }

        public static IServiceCollection AddBlazorLocalization(this IServiceCollection services)
        {
            return services
                .AddSingleton<ILocalStorageAccessor, LocalStorageAccessor>()
                .AddTransient<IBlazorLocalizer, BlazorLocalizer>()
                .AddSingleton<ResourceCache>();
        }
    }
}
