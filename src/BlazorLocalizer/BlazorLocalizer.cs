using BlazorLocalizer.Components;
using Microsoft.AspNetCore.Components;
using System.Globalization;
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
      var currentNamespace = this.GetType().FullName;
      for (var i = 0; i < frames.Length; i++)
      {
        var methodInfo = frames[i].GetMethod();
        var nameSpace = methodInfo.ReflectedType.FullName;
        if (!nameSpace.Equals(currentNamespace, System.StringComparison.OrdinalIgnoreCase))
          return nameSpace;
      }
      return null;
    }

    public Task<string> L(string key, object o)
    {
      return GetLocalizedValue(key, o.GetType().FullName, null);
    }
    #endregion
  }
}
