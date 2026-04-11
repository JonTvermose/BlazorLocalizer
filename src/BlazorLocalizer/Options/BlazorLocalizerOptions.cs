using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorLocalizer
{
    public class BlazorLocalizerOptions
    {

        /// <summary>
        /// Fetched localized values will be cached in the users localstorage.
        /// </summary>
        public LocalStorageOptions LocalStorageOptions { get; set; } = new LocalStorageOptions();

        /// <summary>
        /// Fetched localized values will be cached for the current session. Set this to true to disable the cache entirely. This also disables localstorage caching.
        /// </summary>
        /// <remarks>Default value is false</remarks>
        public bool MemoryCacheDisabled { get; set; } = false;

        /// <summary>
        /// When localizing text in HTML, show the key as a placeholder untill the localized value has been fetched
        /// </summary>
        /// <remarks>Default value is true</remarks>
        public bool ShowKeyAsPlaceholder { get; set; } = true;

        /// <summary>
        /// When enabled, renders the Category as a hidden span after the localized value for debugging purposes.
        /// </summary>
        /// <remarks>Default value is false</remarks>
        public bool DebugMode { get; set; } = false;

        /// <summary>
        /// List of category names to eager load on app startup when calling PreloadAsync().
        /// Each category will be fetched from the resource provider and cached in memory (and localStorage if enabled).
        /// This is only used as a fallback if the resource provider does not implement GetAllResources().
        /// </summary>
        /// <remarks>Default value is an empty list</remarks>
        public IList<string> EagerLoadCategories { get; set; } = new List<string>();
    }

    public class LocalStorageOptions
    {
        /// <summary>
        /// Time before the localstorage cache is invalidated.
        /// </summary>
        /// <remarks>Default value is 3 days</remarks>
        public TimeSpan CacheInvalidation { get; set; } = TimeSpan.FromDays(3);

        /// <summary>
        /// Fetched localized values will be cached in the users localstorage. Set this to true to disable the localstorage cache.
        /// </summary>
        /// <remarks>Default value is false</remarks>
        public bool CacheDisabled { get; set; } = false;
    }
}
