using Microsoft.JSInterop;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorLocalizer.Internal
{
    internal interface ILocalStorageAccessor
    {
        Task<T> GetItemAsync<T>(string key);
        Task SetItemAsync<T>(string key, T value);
        Task RemoveItemAsync(string key);
        Task ClearAsync();
    }

    internal class LocalStorageAccessor : ILocalStorageAccessor, IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private IJSObjectReference _module;
        private readonly JsonSerializerOptions _jsonOptions;

        public LocalStorageAccessor(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        private async Task<IJSObjectReference> GetModuleAsync()
        {
            if (_module == null)
            {
                _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/BlazorLocalizer/blazorLocalizer.js");
            }
            return _module;
        }

        public async Task<T> GetItemAsync<T>(string key)
        {
            var module = await GetModuleAsync();
            var json = await module.InvokeAsync<string>("getItem", key);
            if (json == null)
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        public async Task SetItemAsync<T>(string key, T value)
        {
            var module = await GetModuleAsync();
            var json = JsonSerializer.Serialize(value, _jsonOptions);
            await module.InvokeVoidAsync("setItem", key, json);
        }

        public async Task RemoveItemAsync(string key)
        {
            var module = await GetModuleAsync();
            await module.InvokeVoidAsync("removeItem", key);
        }

        public async Task ClearAsync()
        {
            var module = await GetModuleAsync();
            await module.InvokeVoidAsync("clear");
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
            {
                await _module.DisposeAsync();
                _module = null;
            }
        }
    }
}
