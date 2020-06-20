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
    }
}
