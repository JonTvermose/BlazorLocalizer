using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace BlazorLocalizer
{
    public interface IResourceProvider
    {
        /// <summary>
        /// Get all resources for a given category and cultureName.
        /// The category requested, e.g. "MyProjectName.Home.Index"
        /// Name of the culture, e.g. "en-US" or "en"
        /// </summary>
        /// <returns>Dictionary of all resources associated with the given category and culture name</returns>
        Task<IDictionary<string, string>> GetCategoryResources(string category, string cultureName);

        /// <summary>
        /// Called when you have attempted to retrieve a key that was not part of the collection provided by GetCategoryResources(string category, string cultureName).
        /// </summary>
        /// <param name="category"></param>
        /// <param name="cultureName"></param>
        /// <param name="key">The requested key that was not found</param>
        /// <returns></returns>
        Task AddMissingKey(string category, string cultureName, string key);

        /// <summary>
        /// Retrieve the name of the culture to used as a fallback in case a specific culture is not given
        /// </summary>
        /// <returns></returns>
        Task<string> GetCultureName();

        /// <summary>
        /// Get all resources for all categories for a given cultureName.
        /// Override this method to allow eager loading of all resources in a single call.
        /// If not implemented, PreloadAsync() will fall back to loading each category individually from EagerLoadCategories.
        /// </summary>
        /// <param name="cultureName">Name of the culture, e.g. "en-US" or "en"</param>
        /// <returns>Dictionary where the key is the category name and the value is a dictionary of resources for that category, or null if not implemented</returns>
        Task<IDictionary<string, IDictionary<string, string>>> GetAllResources(string cultureName)
        {
            return Task.FromResult<IDictionary<string, IDictionary<string, string>>>(null);
        }
    }
}
