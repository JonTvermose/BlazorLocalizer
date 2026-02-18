# BlazorLocalizer Agent Instructions

This document provides context and guidelines for AI agents working on the BlazorLocalizer project.

## Project Overview

BlazorLocalizer is a library that provides asynchronous localization in Blazor WebAssembly applications. It caches localized values in memory and in `localStorage` for a configurable amount of time.

**Repository**: https://github.com/JonTvermose/BlazorLocalizer
**Package**: Tvermose.BlazorLocalizer on NuGet
**Current Version**: 10.2.0
**Target Framework**: .NET 10

## Project Structure

```
/home/runner/work/BlazorLocalizer/BlazorLocalizer/
├── src/
│   └── BlazorLocalizer/                 # Main library project
│       ├── BlazorLocalizer.cs           # Core implementation
│       ├── IBlazorLocalizer.cs          # Public interface
│       ├── IResourceProvider.cs         # Resource provider interface
│       ├── Extensions.cs                # DI extension methods
│       ├── ResourceCache.cs             # Caching logic
│       ├── Components/
│       │   └── Localized.razor          # Razor component
│       ├── Models/
│       │   └── CultureCategoryResources.cs
│       └── Options/
│           └── BlazorLocalizerOptions.cs  # Configuration options
├── samples/
│   └── BlazorLocalizerSample/
│       ├── Client/                      # Blazor WebAssembly client
│       ├── Server/                      # ASP.NET Core server host
│       └── Shared/                      # Shared models
├── README.md                            # Main documentation
└── README-nuget.md                      # NuGet package documentation
```

## Key Files and Components

### Core Library (`src/BlazorLocalizer/`)
- **BlazorLocalizer.cs**: Main implementation of the IBlazorLocalizer interface
- **IBlazorLocalizer.cs**: Public interface for localization functionality
- **IResourceProvider.cs**: Interface that consumers must implement to provide localized resources
- **Extensions.cs**: Contains `AddBlazorLocalization()` extension method for service registration
- **ResourceCache.cs**: Handles caching of localized strings in memory and localStorage
- **Components/Localized.razor**: Razor component for declarative localization in markup
- **Options/BlazorLocalizerOptions.cs**: Configuration options including LocalStorageOptions

### Dependencies
- **Blazored.LocalStorage** (v4.4.0): For localStorage functionality
- **Microsoft.AspNetCore.Components** (.NET 10): Core Blazor components
- **Microsoft.AspNetCore.Components.Web** (.NET 10): Web-specific Blazor components

## Build and Package Information

- **SDK**: Microsoft.NET.Sdk.Razor for the main library
- **Package Generation**: Enabled on build (`GeneratePackageOnBuild=true`)
- **Package README**: README.md is included in the NuGet package
- **No Test Projects**: This project does not have automated tests

## Development Guidelines

### Making Changes
1. **Maintain Simplicity**: This is a focused library with ~500 lines of code. Keep it simple.
2. **No Breaking Changes**: The public API (IBlazorLocalizer, IResourceProvider) should remain stable
3. **Update Documentation**: Update both README.md and README-nuget.md when making feature changes
4. **Version Numbering**: Follow the existing versioning scheme (currently 10.2.0)

### Code Style
- Follow standard C# conventions
- Use async/await for asynchronous operations
- Minimal comments (code should be self-explanatory)
- Keep dependencies minimal

### Testing
Since there are no automated tests:
1. Build the library to ensure compilation
2. Test with the sample application in `samples/BlazorLocalizerSample/`
3. Verify NuGet package generation completes successfully

### Build Commands
```bash
# Build the library
dotnet build src/BlazorLocalizer/BlazorLocalizer.csproj

# Build the sample application
dotnet build samples/BlazorLocalizerSample/Server/BlazorLocalizerSample.Server.csproj

# Build everything
dotnet build
```

## Key Concepts

### Localization Flow
1. Consumer implements `IResourceProvider` to fetch localized strings from their backend/API
2. Consumer registers their provider and calls `AddBlazorLocalization()` in Program.cs
3. In components, inject `IBlazorLocalizer` and use:
   - Synchronous: `loc["Key"]` (returns last cached value immediately)
   - Asynchronous: `await loc.L("Key", "Category")` (fetches fresh value)
4. Localized strings are cached in memory and localStorage

### Caching Strategy
- **Memory Cache**: Always used for immediate access
- **localStorage Cache**: Optional, configurable via `LocalStorageOptions`
  - Default: 3 days expiration
  - Can be disabled with `CacheDisabled = true`
  - Configurable expiration with `CacheInvalidation` TimeSpan

### Blazor Server Support
**Not Currently Supported** - The library is designed specifically for Blazor WebAssembly. The localStorage dependency and async patterns are optimized for WebAssembly scenarios.

## Common Tasks

### Updating Dependencies
When updating .NET versions or package versions:
1. Update all .csproj files (main library and all sample projects)
2. Update package references to match the .NET version
3. Build and verify everything compiles
4. Test with the sample application

### Adding Features
1. Consider if the feature is in scope (localization-focused)
2. Implement in the appropriate location (core vs. options vs. components)
3. Update both README files with usage examples
4. Ensure backward compatibility or increment major version

### Documentation Updates
- **README.md**: Main documentation for GitHub
- **README-nuget.md**: Shorter version for NuGet package page
- Keep both files synchronized in terms of core features and examples

## Limitations and Known Issues

1. **Blazor Server**: Not supported (localStorage dependency, async patterns)
2. **No Tests**: Manual testing required
3. **No CI/CD**: No automated build/test pipeline configured

## Contact and Resources

- **Author**: Jon Tvermose Nielsen
- **License**: MIT
- **Repository**: https://github.com/JonTvermose/BlazorLocalizer
- **NuGet**: https://www.nuget.org/packages/Tvermose.BlazorLocalizer/
