# BlazorLocalizer

A library to provide asynchronous localization in Blazor WebAssembly applications. Caches localized values in memory and i `localStorage` for a configurable amount of time.

[![Nuget](https://img.shields.io/nuget/v/tvermose.blazorlocalizer.svg)](https://www.nuget.org/packages/Tvermose.BlazorLocalizer/)

## Installing
You can install from NuGet using the following command:

`Install-Package Tvermose.BlazorLocalizer`

Or via the Visual Studio package manager.

## Setup
You will need to implement the interface `BlazorLocalizer.IResourceProvider` and register the implementation with the service collection in your <i>Program.cs</i> in Blazor WebAssembly.

Likewise you will need to register the IBlazorLocalizer by using the extension method `AddBlazorLocalization()`.

```c#
public static async Task Main(string[] args)
{
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    builder.RootComponents.Add<App>("app");
    
    builder.Services.AddTransient<BlazorLocalizer.IResourceProvider, YourResourceProviderImplementation>();
    builder.Services.AddBlazorLocalization();

    await builder.Build().RunAsync();
}
```

## Configuration
BlazorLocalizer provides options that can be modified by you at registration in your <i>Program.cs</i> file in Blazor WebAssembly.
Localized values will per default be cached for 3 days using `localStorage`. This can be modified or disabled using the LocalStorageOptions as shown below.
```c#
public static async Task Main(string[] args)
{
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    builder.RootComponents.Add<App>("app");
    
    builder.Services.AddTransient<BlazorLocalizer.IResourceProvider, YourResourceProviderImplementation>();
    builder.Services.AddBlazorLocalization(config => {
      config.LocalStorageOptions.CacheInvalidation = TimeSpan.FromDays(1);
      config.LocalStorageOptions.CacheDisabled = false;
    });

    await builder.Build().RunAsync();
}
```

## Usage (Blazor WebAssembly)
To use BlazorLocalization in Blazor WebAssembly, inject the `IBlazorLocalizer`as in the example below.
```c#
@inject BlazorLocalizer.IBlazorLocalizer loc

<p title=@_title>
  @loc["Localize this text using the calling class fullname as category"]
  @loc["Localize this text", "Example.Category"]
</p>

@code {

    private string _title = "Title for localize this text";
  
    protected override async Task OnInitializedAsync()
    {
        _title = await loc.L(_title, "Example.Category");
    }

}
```

If you do not provide a category for the localized text, the calling class fullname will be used as category.

## Usage (Blazor Server)
Not currently supported.

## Eager Loading
You can preload resources at app startup to avoid loading delays when the user navigates to a page. There are two approaches:

### Option 1: Implement `GetAllResources` on your resource provider
Override the `GetAllResources` method on your `IResourceProvider` implementation to return all resources for all categories in a single call. This is the most efficient approach as it requires only a single HTTP request.

```c#
public class YourResourceProvider : IResourceProvider
{
    // ... other methods ...

    public async Task<IDictionary<string, IDictionary<string, string>>> GetAllResources(string cultureName)
    {
        // Return all resources grouped by category
        return await _httpClient.GetFromJsonAsync<Dictionary<string, Dictionary<string, string>>>(
            $"api/localization/all?culture={cultureName}");
    }
}
```

Then call `PreloadAsync()` at startup:

```c#
public static async Task Main(string[] args)
{
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    builder.RootComponents.Add<App>("app");

    builder.Services.AddTransient<IResourceProvider, YourResourceProvider>();
    builder.Services.AddBlazorLocalization();

    var host = builder.Build();
    await host.Services.GetRequiredService<IBlazorLocalizer>().PreloadAsync();
    await host.RunAsync();
}
```

### Option 2: Specify categories to eager load
If you don't implement `GetAllResources`, you can specify individual categories to preload using the `EagerLoadCategories` option. Each category will be loaded individually using the existing `GetCategoryResources` method.

```c#
public static async Task Main(string[] args)
{
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    builder.RootComponents.Add<App>("app");

    builder.Services.AddTransient<IResourceProvider, YourResourceProvider>();
    builder.Services.AddBlazorLocalization(config => {
        config.EagerLoadCategories = new List<string>
        {
            "MyApp.Shared.Layout",
            "MyApp.Pages.Home",
            "MyApp.Pages.About"
        };
    });

    var host = builder.Build();
    await host.Services.GetRequiredService<IBlazorLocalizer>().PreloadAsync();
    await host.RunAsync();
}
```
