using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Threading.Tasks;

namespace BlazorLocalizer
{
    public interface IBlazorLocalizer
    {

        /// <summary>
        /// Localize a key using the fallback category (if any)
        /// </summary>
        /// <returns>The localized value</returns>
        RenderFragment this[string key] { get; }

        /// <summary>
        /// Localize a key using the fallback category (if any)
        /// </summary>
        /// <returns>The localized value</returns>
        RenderFragment this[string key, CultureInfo culture] { get; }

        /// <summary>
        /// Localize a key. The category will be set as o.GetType().FullName
        /// </summary>
        /// <returns>The localized value</returns>
        RenderFragment this[string key, object o] { get; }

        /// <summary>
        /// Localize a key. The category will be set as o.GetType().FullName
        /// </summary>
        /// <returns>The localized value</returns>
        RenderFragment this[string key, object o, CultureInfo culture] { get; }

        /// <summary>
        /// Localize a key. The category will be set as o.GetType().FullName
        /// </summary>
        /// <returns>The localized value</returns>
        RenderFragment this[string key, object o, string cultureName] { get; }

        /// <summary>
        /// Localize a key.
        /// </summary>
        /// <returns>The localized value</returns>
        RenderFragment this[string key, string category] { get; }

        /// <summary>
        /// Localize a key.
        /// </summary>
        /// <returns>The localized value</returns>
        RenderFragment this[string key, string category, CultureInfo culture] { get; }

        /// <summary>
        /// Localize a key.
        /// </summary>
        /// <returns>The localized value</returns>
        RenderFragment this[string key, string category, string cultureName] { get; }

        /// <summary>
        /// Localize a key. The category will be set as o.GetType().FullName
        /// </summary>
        /// <returns>The localized value</returns>
        Task<string> L(string key, object o);

        /// <summary>
        /// Localize a key. The category will be set as o.GetType().FullName
        /// </summary>
        /// <returns>The localized value</returns>
        Task<string> L(string key, object o, CultureInfo culture);

        /// <summary>
        /// Localize a key. The category will be set as o.GetType().FullName
        /// </summary>
        /// <returns>The localized value</returns>
        Task<string> L(string key, object o, string cultureName);

        /// <summary>
        /// Localize a key using the fallback category (if any)
        /// </summary>
        /// <returns>The localized value</returns>
        Task<string> L(string key);

        /// <summary>
        /// Localize a key using the fallback category (if any)
        /// </summary>
        /// <returns>The localized value</returns>
        Task<string> L(string key, CultureInfo culture);

        /// <summary>
        /// Localize a key.
        /// </summary>
        /// <returns>The localized value</returns>
        Task<string> L(string key, string category);

        /// <summary>
        /// Localize a key.
        /// </summary>
        /// <returns>The localized value</returns>
        Task<string> L(string key, string category, CultureInfo culture);

        /// <summary>
        /// Localize a key.
        /// </summary>
        /// <returns>The localized value</returns>
        Task<string> L(string key, string category, string cultureName);

        /// <summary>
        /// Clear all cached values - both in memory and in localStorage
        /// </summary>
        Task ClearCache();

        /// <summary>
        /// Clear cached values in memory and in localstorage for the given category and cultureName
        /// </summary>
        Task ClearCache(string category, string cultureName);
    }
}