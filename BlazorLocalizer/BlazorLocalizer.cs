using BlazorLocalizer.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics;
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
            return await _resourceCache.GetResource(key, category, CultureInfo.CurrentUICulture.Name, _resourceProvider, _localStorageService);
        }

        public async Task<string> L(string key, object o)
        {
            return await _resourceCache.GetResource(key, o.GetType().FullName, CultureInfo.CurrentUICulture.Name, _resourceProvider, _localStorageService);
        }

        public async Task<string> L(string key, string category, CultureInfo culture)
        {
            return await _resourceCache.GetResource(key, category, culture.Name, _resourceProvider, _localStorageService);
        }

        public async Task<string> L(string key, object o, CultureInfo culture)
        {
            return await _resourceCache.GetResource(key, o.GetType().FullName, culture.Name, _resourceProvider, _localStorageService);
        }

        public async Task<string> L(string key, string category, string cultureName)
        {
            return await _resourceCache.GetResource(key, category, cultureName, _resourceProvider, _localStorageService);
        }

        public async Task<string> L(string key, object o, string cultureName)
        {
            return await _resourceCache.GetResource(key, o.GetType().FullName, cultureName, _resourceProvider, _localStorageService);
        }

        public async Task ClearCache()
        {
            await _resourceCache.ClearCache(_localStorageService);
        }

        public async Task ClearCache(string category, string cultureName)
        {
            await _resourceCache.ClearCache(category, cultureName, _localStorageService);
        }

    }
}
