using BlazorLocalizer;
using BlazorLocalizerSample.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Globalization;

namespace BlazorLocalizerSample.Client
{
  /// <summary>
  /// A simple implementation example of the IResourceProvider interface where the values are fetched via Json from a backend
  /// </summary>
  public class ResourceProviderImplementation : IResourceProvider
  {
    private readonly HttpClient _httpClient;

    public ResourceProviderImplementation(HttpClient httpClient)
    {
      _httpClient = httpClient;
    }

    public async Task AddMissingKey(string category, string cultureName, string key)
    {
      await _httpClient.PostAsJsonAsync($"Localization/AddMissingKey", new MissingKeyValue { Key = key, Category = category, Culture = cultureName, Value = key });
    }

    public async Task<IDictionary<string, string>> GetCategoryResources(string category, string cultureName)
    {
      var localizedValues = await _httpClient.GetFromJsonAsync<List<KeyValue>>($"Localization/GetLocalizedCategory?category={category}&culture={cultureName}");
      return localizedValues.ToDictionary(x => x.Key, x => x.Value);
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<string> GetCultureName()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
      return CultureInfo.CurrentUICulture.Name;
    }
  }
}
