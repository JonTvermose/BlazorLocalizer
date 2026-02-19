using BlazorLocalizer.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using BlazorLocalizer.Internal;
using System.Diagnostics;

namespace BlazorLocalizer
{
  internal class BlazorLocalizer : IBlazorLocalizer
  {
    private readonly ResourceCache _resourceCache;
    private readonly IResourceProvider _resourceProvider;
    private readonly ILocalStorageService _localStorageService;
    private string _fallBackCultureName;

    public BlazorLocalizer(ResourceCache resourceCache, IResourceProvider resourceProvider, ILocalStorageService localStorageService)
    {
      _resourceCache = resourceCache;
      _resourceProvider = resourceProvider;
      _localStorageService = localStorageService;
    }

    #region interface implementation
    public RenderFragment this[string key, CultureInfo culture]
    {
      get
      {
        return GetLocalizedComponent(key, GetCategoryName(), culture.Name);
      }
    }

    public RenderFragment this[string key]
    {
      get
      {
        return GetLocalizedComponent(key, GetCategoryName(), null);
      }
    }

    public RenderFragment this[string key, object o]
    {
      get
      {
        return GetLocalizedComponent(key, o.GetType().FullName, null);
      }
    }

    public RenderFragment this[string key, string category]
    {
      get
      {
        return GetLocalizedComponent(key, category, null);
      }
    }

    public RenderFragment this[string key, string category, CultureInfo culture]
    {
      get
      {
        return GetLocalizedComponent(key, category, culture.Name);
      }
    }

    public RenderFragment this[string key, string category, string cultureName]
    {
      get
      {
        return GetLocalizedComponent(key, category, cultureName);
      }
    }

    public async Task<string> L(string key)
    {
      return await GetLocalizedValue(key, GetCategoryName(), null);
    }

    public async Task<string> L(string key, string category)
    {
      return await GetLocalizedValue(key, category, null);
    }

    public async Task<string> L(string key, string category, CultureInfo culture)
    {
      return await GetLocalizedValue(key, category, culture.Name);
    }

    public async Task<string> L(string key, string category, string cultureName)
    {
      return await GetLocalizedValue(key, category, cultureName);
    }

    public async Task ClearCache()
    {
      await _resourceCache.ClearCache(_localStorageService);
    }

    public async Task ClearCache(string category, string cultureName)
    {
      await _resourceCache.ClearCache(category, cultureName, _localStorageService);
    }

    public async Task<string> L(string key, CultureInfo culture)
    {
      return await L(key, GetCategoryName(), culture);
    }
    #endregion

    #region private methods
    private async Task<string> GetLocalizedValue(string key, string category, string culture)
    {
      if (string.IsNullOrWhiteSpace(culture))
      {
        if (string.IsNullOrWhiteSpace(_fallBackCultureName))
        {
          _fallBackCultureName = (await _resourceProvider.GetCultureName()) ?? CultureInfo.CurrentUICulture.Name;
        }
        culture = _fallBackCultureName;
      }
      return await _resourceCache.GetResource(key, culture, category, _resourceProvider, _localStorageService);
    }

    private RenderFragment GetLocalizedComponent(string key, string category, string culture)
    {
      RenderFragment result = l =>
      {
        l.OpenComponent(1, typeof(Localized));
        l.AddAttribute(2, "Key", key);
        l.AddAttribute(3, "Category", category);
        l.AddAttribute(4, "CultureName", culture);
        l.CloseComponent();
      };
      return result;
    }

    private string GetCategoryName()
    {
      var frames = new StackTrace().GetFrames();
      var currentType = this.GetType();

      for (var i = 0; i < frames.Length; i++)
      {
        var methodInfo = frames[i].GetMethod();
        if (methodInfo == null)
          continue;

        var reflectedType = methodInfo.ReflectedType;
        if (reflectedType == null)
          continue;

        // Skip our own type
        if (reflectedType == currentType)
          continue;

        // Resolve compiler-generated types (closures, state machines, display classes)
        // to their declaring (outer) type
        var resolvedType = ResolveDeclaringType(reflectedType);

        // Skip well-known framework namespaces that aren't user components
        var fullName = resolvedType.FullName;
        if (fullName == null)
          continue;

        if (fullName.StartsWith("Microsoft.AspNetCore.Components", StringComparison.Ordinal)
            || fullName.StartsWith("Microsoft.Extensions.", StringComparison.Ordinal)
            || fullName.StartsWith("System.", StringComparison.Ordinal)
            || fullName.StartsWith("BlazorLocalizer.", StringComparison.Ordinal))
          continue;

        return fullName;
      }
      return null;
    }

    /// <summary>
    /// Walks up the DeclaringType chain to resolve compiler-generated nested types
    /// (closures, display classes, async state machines) to the actual user type.
    /// </summary>
    private static Type ResolveDeclaringType(Type type)
    {
      // Compiler-generated types are always nested and typically have [CompilerGenerated]
      // or contain '<>' in their name (e.g. <>c__DisplayClass0_0, <BuildRenderTree>d__2)
      while (type.DeclaringType != null
             && (Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute))
                 || type.Name.Contains("<>")))
      {
        type = type.DeclaringType;
      }
      return type;
    }

    public Task<string> L(string key, object o)
    {
      return GetLocalizedValue(key, o.GetType().FullName, null);
    }
    #endregion
  }
}
