using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BlazorLocalizer;

namespace BlazorLocalizerSample.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddTransient<IResourceProvider, ResourceProviderImplementation>();

            builder.Services.AddBlazorLocalization(config => {
                config.ShowKeyAsPlaceholder = true;
                config.LocalStorageOptions.CacheInvalidation = TimeSpan.FromMinutes(10);
            });

            await builder.Build().RunAsync();
        }
    }
}
