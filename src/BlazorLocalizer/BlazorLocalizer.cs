using BlazorLocalizer.Components;
using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using BlazorLocalizer.Internal;

namespace BlazorLocalizer
{
    internal class BlazorLocalizer : IBlazorLocalizer
    {
        private readonly ResourceCache _resourceCache;
        private readonly IResourceProvider _resourceProvider;
        private readonly ILocalStorageService _localStorageService;

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
                return GetLocalizedComponent(key, null, culture.Name);
            }
        }

        public RenderFragment this[string key]
        {
            get
            {
                return GetLocalizedComponent(key, null, CultureInfo.CurrentUICulture.Name);
            }
        }

        public RenderFragment this[string key, object o]
        {
            get
            {
                return GetLocalizedComponent(key, o.GetType().FullName, CultureInfo.CurrentUICulture.Name);
            }
        }

        public RenderFragment this[string key, string category]
        {
            get
            {
                return GetLocalizedComponent(key, category, CultureInfo.CurrentUICulture.Name);
            }
        }

        public RenderFragment this[string key, object o, CultureInfo culture]
        {
            get
            {
                return GetLocalizedComponent(key, o.GetType().FullName, culture.Name);
            }
        }

        public RenderFragment this[string key, string category, CultureInfo culture]
        {
            get
            {
                return GetLocalizedComponent(key, category, culture.Name);
            }
        }

        public RenderFragment this[string key, object o, string cultureName]
        {
            get
            {
                return GetLocalizedComponent(key, o.GetType().FullName, cultureName);
            }
        }

        public RenderFragment this[string key, string category, string cultureName]
        {
            get
            {
                return GetLocalizedComponent(key, category, cultureName);
            }
        }

        public async Task<string> L(string key, string category)
        {
            return await GetLocalizedValue(key, category, CultureInfo.CurrentUICulture.Name);
        }

        public async Task<string> L(string key, object o)
        {
            return await GetLocalizedValue(key, o.GetType().FullName, CultureInfo.CurrentUICulture.Name);
        }

        public async Task<string> L(string key, string category, CultureInfo culture)
        {
            return await GetLocalizedValue(key, category, culture.Name);
        }

        public async Task<string> L(string key, object o, CultureInfo culture)
        {
            return await GetLocalizedValue(key, o.GetType().FullName, culture.Name);
        }

        public async Task<string> L(string key, string category, string cultureName)
        {
            return await GetLocalizedValue(key, category, cultureName);
        }

        public async Task<string> L(string key, object o, string cultureName)
        {
            return await GetLocalizedValue(key, o.GetType().FullName, cultureName);
        }

        public async Task ClearCache()
        {
            await _resourceCache.ClearCache(_localStorageService);
        }

        public async Task ClearCache(string category, string cultureName)
        {
            await _resourceCache.ClearCache(category, cultureName, _localStorageService);
        }

        public async Task<string> L(string key)
        {
            return await L(key, CultureInfo.CurrentUICulture);
        }

        public async Task<string> L(string key, CultureInfo culture)
        {
            return await L(key, null, culture);
        }
        #endregion

        #region private methods
        private async Task<string> GetLocalizedValue(string key, string category, string culture)
        {
            return await _resourceCache.GetResource(key, category, culture, _resourceProvider, _localStorageService);
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
        #endregion
    }
}
