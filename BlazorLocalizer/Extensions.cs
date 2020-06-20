using Blazored.LocalStorage;
using BlazorLocalizer.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BlazorLocalizer
{
    public static class Extensions
    {
        public static IServiceCollection AddBlazorLocalization(this IServiceCollection services, Action<BlazorLocalizerOptions> config)
        {
            return services
                .AddBlazoredLocalStorage()
                .AddTransient<IBlazorLocalizer, BlazorLocalizer>()
                .AddSingleton<ResourceCache>()
                .Configure<BlazorLocalizerOptions>(cfg => config?.Invoke(cfg));
        }

        public static IServiceCollection AddBlazorLocalization(this IServiceCollection services)
        {
            return services
                .AddBlazoredLocalStorage()
                .AddTransient<IBlazorLocalizer, BlazorLocalizer>()
                .AddSingleton<ResourceCache>();
        }
    }
}
