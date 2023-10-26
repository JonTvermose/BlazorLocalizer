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
