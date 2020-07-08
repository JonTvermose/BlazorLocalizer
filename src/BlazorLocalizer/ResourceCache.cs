using Blazored.LocalStorage;
using BlazorLocalizer.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorLocalizer.Internal
{
    internal class ResourceCache
    {
        private List<CultureCategoryResources> _cache;
        private BlazorLocalizerOptions _options;
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public ResourceCache(IOptions<BlazorLocalizerOptions> options)
        {
            _cache = new List<CultureCategoryResources>();
            _options = options.Value;
        }

        public async Task<string> GetResource(string key, string culture, string category, IResourceProvider provider, ILocalStorageService localStorageService)
        {
            var comparer = StringComparer.OrdinalIgnoreCase;

            if (string.IsNullOrWhiteSpace(category) && _options.FallbackCategory != null)
            {
                category = _options.FallbackCategory;
            }

            if (_options.MemoryCacheDisabled)
            {
                var categoryResult = await provider.GetCategoryResources(category, culture);
                categoryResult = new Dictionary<string, string>(categoryResult, comparer);
                if (categoryResult.TryGetValue(key, out var res))
                {
                    return res;
                }
                else
                {
                    await provider.AddMissingKey(category, culture, key);
                    return key;
                }
            }

            var categoryKey = $"{category}-{culture}";

            var cachedCultureCategory = _cache.SingleOrDefault(x => x.Category == category && x.Culture == culture);
            if (cachedCultureCategory == null)
            {
                await _semaphore.WaitAsync();

                if (!_options.LocalStorageOptions.CacheDisabled)
                {
                    var cachedCategory = await localStorageService.GetItemAsync<CultureCategoryResources>(categoryKey);
                    if (cachedCategory != null) 
                    {
                        if(cachedCategory.UpdatedTime.Add(_options.LocalStorageOptions.CacheInvalidation) > DateTime.UtcNow)
                        {
                            cachedCultureCategory = cachedCategory;
                        }
                    }
                }

                if (cachedCultureCategory == null)
                {
                    cachedCultureCategory = _cache.SingleOrDefault(x => x.Category == category && x.Culture == culture);
                    if (cachedCultureCategory == null)
                    {
                        var resources = await provider.GetCategoryResources(category, culture);
                        resources = new Dictionary<string, string>(resources, comparer);
                        cachedCultureCategory = new CultureCategoryResources
                        {
                            Category = category,
                            Culture = culture,
                            Resources = resources,
                            UpdatedTime = DateTime.UtcNow
                        };
                        _cache.Add(cachedCultureCategory);
                        if (!_options.LocalStorageOptions.CacheDisabled)
                        {
                            await localStorageService.SetItemAsync(categoryKey, cachedCultureCategory);
                        }
                    }
                }
                _semaphore.Release();
            }

            await _semaphore.WaitAsync();
            cachedCultureCategory.Resources = new Dictionary<string, string>(cachedCultureCategory.Resources, comparer);
            if (cachedCultureCategory.Resources.TryGetValue(key, out var result))
            {
                _semaphore.Release();
                return result;
            }
            if (cachedCultureCategory.Resources.TryAdd(key, key))
            {
                if (!_options.LocalStorageOptions.CacheDisabled)
                {
                    await localStorageService.SetItemAsync(categoryKey, cachedCultureCategory);
                }
            }
            _semaphore.Release();
            await provider.AddMissingKey(category, culture, key);

            return key;
        }

        internal async Task ClearCache(string category, string cultureName, ILocalStorageService localStorageService)
        {
            await localStorageService.SetItemAsync<CultureCategoryResources>($"{category}-{cultureName}", null);
        }

        internal async Task ClearCache(ILocalStorageService localStorageService)
        {
            await localStorageService.ClearAsync();
        }
    }
}
