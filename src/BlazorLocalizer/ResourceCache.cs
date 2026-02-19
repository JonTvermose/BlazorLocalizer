using Blazored.LocalStorage;
using BlazorLocalizer.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorLocalizer.Internal
{
    internal class ResourceCache
    {
        private ConcurrentDictionary<string, CultureCategoryResources> _memCache;
        private BlazorLocalizerOptions _options;
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public ResourceCache(IOptions<BlazorLocalizerOptions> options)
        {
            _memCache = new(StringComparer.OrdinalIgnoreCase);
            _options = options.Value;
        }

        public async Task<string> GetResource(string key, string culture, string category, IResourceProvider provider, ILocalStorageService localStorageService)
        {
            var comparer = StringComparer.OrdinalIgnoreCase;

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

            // Memory cache hit (fastest)
            if (_memCache.TryGetValue(categoryKey, out var cachedCultureCategory))
            {
                if (cachedCultureCategory.Resources.TryGetValue(key, out var memResult))
                {
                    return memResult;
                }
            }

            await _semaphore.WaitAsync();

            // Memory cache was populated while waiting on semaphore (still fast)
            if (_memCache.TryGetValue(categoryKey, out cachedCultureCategory))
            {
                if (cachedCultureCategory.Resources.TryGetValue(key, out var memResult))
                {
                    _semaphore.Release();
                    return memResult;
                }
            }

            if (cachedCultureCategory == null)
            {
                if (!_options.LocalStorageOptions.CacheDisabled)
                {
                    var cachedCategory = await localStorageService.GetItemAsync<CultureCategoryResources>(categoryKey);
                    if (cachedCategory != null)
                    {
                        if (cachedCategory.UpdatedTime.Add(_options.LocalStorageOptions.CacheInvalidation) > DateTime.UtcNow)
                        {
                            cachedCultureCategory = cachedCategory;
                            cachedCultureCategory.Resources = new Dictionary<string, string>(cachedCultureCategory.Resources, comparer);
                            _memCache.TryAdd(categoryKey, cachedCultureCategory);
                        }
                    }
                }

                if (cachedCultureCategory == null)
                {
                    try
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

                    _memCache.TryAdd(categoryKey, cachedCultureCategory);
          }
          catch
          {

          }
                    if (!_options.LocalStorageOptions.CacheDisabled)
                    {
                        await localStorageService.SetItemAsync(categoryKey, cachedCultureCategory);
                    }
                }
            }

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

        /// <summary>
        /// Synchronously attempts to get a resource from the in-memory cache only.
        /// Returns true if found, false if a full async fetch is needed.
        /// </summary>
        internal bool TryGetResourceSync(string key, string culture, string category, out string value)
        {
            value = null;
            if (_options.MemoryCacheDisabled || string.IsNullOrWhiteSpace(culture))
            {
                return false;
            }

            var categoryKey = $"{category}-{culture}";
            if (_memCache.TryGetValue(categoryKey, out var cachedCultureCategory)
                && cachedCultureCategory.Resources.TryGetValue(key, out var memResult))
            {
                value = memResult;
                return true;
            }

            return false;
        }

        private string _cachedCultureName;

        /// <summary>
        /// Returns the cached culture name if available, or null if not yet resolved.
        /// </summary>
        internal string CachedCultureName => _cachedCultureName;

        /// <summary>
        /// Resolves and caches the culture name from the resource provider.
        /// </summary>
        internal async Task<string> GetOrResolveCultureName(IResourceProvider resourceProvider)
        {
            if (!string.IsNullOrWhiteSpace(_cachedCultureName))
            {
                return _cachedCultureName;
            }
            var name = await resourceProvider.GetCultureName();
            _cachedCultureName = name;
            return name;
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
