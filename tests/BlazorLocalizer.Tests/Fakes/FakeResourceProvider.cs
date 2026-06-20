using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorLocalizer;

namespace BlazorLocalizer.Tests.Fakes
{
    /// <summary>
    /// In-memory <see cref="IResourceProvider"/> double. Stands in for the consumer's
    /// resource source (e.g. a backend API). Counts how many times it is queried so tests
    /// can assert whether a value came from the provider or from a cache layer.
    /// </summary>
    public class FakeResourceProvider : IResourceProvider
    {
        public Dictionary<string, string> Resources { get; }
        public int GetCategoryResourcesCalls { get; private set; }
        public List<string> MissingKeys { get; } = new List<string>();
        public string CultureName { get; set; } = "en-US";

        public FakeResourceProvider(Dictionary<string, string>? resources = null)
        {
            Resources = resources ?? new Dictionary<string, string> { ["Hello"] = "World" };
        }

        public Task<IDictionary<string, string>> GetCategoryResources(string category, string cultureName)
        {
            GetCategoryResourcesCalls++;
            // Return a copy so the cache can re-wrap it without mutating our backing store.
            return Task.FromResult<IDictionary<string, string>>(new Dictionary<string, string>(Resources));
        }

        public Task AddMissingKey(string category, string cultureName, string key)
        {
            MissingKeys.Add(key);
            return Task.CompletedTask;
        }

        public Task<string> GetCultureName() => Task.FromResult(CultureName);
    }
}
