# BlazorLocalizer

A library to provide asynchronous localization in Blazor WebAssembly applications. Caches localized values in memory and i `localStorage` for a configurable amount of time.

[![Nuget](https://img.shields.io/nuget/v/tvermose.blazorlocalizer.svg)](https://www.nuget.org/packages/Tvermose.BlazorLocalizer/)

## Installing
You can install from NuGet using the following command:

`Install-Package Tvermose.BlazorLocalizer`

Or via the Visual Studio package manager.

## Setup
You will need to implement the interface `BlazorLocalizer.IResourceProvider` and register the implementation with the service collection in your <i>Program.cs</i> in Blazor WebAssembly.

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
      config.FallbackCategory = "Your.Fallback.Category";
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

If you do not provide a category for the localized text (null, empty or white space), `FallbackCategory` from your options will be used if that has been set.

## Usage (Blazor Server)
Not currently supported.
