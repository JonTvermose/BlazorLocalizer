using Microsoft.JSInterop;
using System;
using System.Text.Json;
using System.Threading;
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
        private readonly SemaphoreSlim _moduleLock = new SemaphoreSlim(1, 1);
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

        /// <summary>
        /// Detects exceptions thrown when JavaScript interop cannot be issued.
        /// This happens before the first render in BlazorWebView (.NET MAUI/WPF/WinForms),
        /// during Blazor Server static prerendering, and on circuit teardown.
        /// These are treated as a transient "no JS available yet" state, not a hard failure:
        /// reads degrade to a cache miss and writes become no-ops, so the caller can fall
        /// back to the resource provider. JS becomes available after the first render, so the
        /// failure must NOT be latched - the next call retries the module import.
        /// </summary>
        private static bool IsJsUnavailable(Exception ex) =>
            ex is InvalidOperationException        // BlazorWebView pre-render + Blazor Server prerender
               or JSDisconnectedException          // Microsoft.JSInterop - circuit/teardown
               or OperationCanceledException;

        private async Task<IJSObjectReference> GetModuleAsync()
        {
            if (_module != null)
            {
                return _module;
            }

            await _moduleLock.WaitAsync();
            try
            {
                if (_module == null)
                {
                    // The failure is not cached: if the import fails because JS is not yet
                    // available, _module stays null and the next call retries the import.
                    _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                        "import", "./_content/Tvermose.BlazorLocalizer/blazorLocalizer.js");
                }
                return _module;
            }
            catch (Exception ex) when (IsJsUnavailable(ex))
            {
                // JS interop not available yet (e.g. before first render / prerender). Retried later.
                return null;
            }
            finally
            {
                _moduleLock.Release();
            }
        }

        public async Task<T> GetItemAsync<T>(string key)
        {
            var module = await GetModuleAsync();
            if (module == null)
            {
                return default; // JS unavailable: treat as a cache miss.
            }
            try
            {
                var json = await module.InvokeAsync<string>("getItem", key);
                if (json == null)
                {
                    return default;
                }
                return JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }
            catch (Exception ex) when (IsJsUnavailable(ex))
            {
                return default; // JS unavailable: treat as a cache miss.
            }
        }

        public async Task SetItemAsync<T>(string key, T value)
        {
            var module = await GetModuleAsync();
            if (module == null)
            {
                return; // JS unavailable: no-op.
            }
            try
            {
                var json = JsonSerializer.Serialize(value, _jsonOptions);
                await module.InvokeVoidAsync("setItem", key, json);
            }
            catch (Exception ex) when (IsJsUnavailable(ex))
            {
                // JS unavailable: no-op. The value stays in the in-memory cache and is
                // written to localStorage on a later call once JS is available.
            }
        }

        public async Task RemoveItemAsync(string key)
        {
            var module = await GetModuleAsync();
            if (module == null)
            {
                return; // JS unavailable: no-op.
            }
            try
            {
                await module.InvokeVoidAsync("removeItem", key);
            }
            catch (Exception ex) when (IsJsUnavailable(ex))
            {
                // JS unavailable: no-op.
            }
        }

        public async Task ClearAsync()
        {
            var module = await GetModuleAsync();
            if (module == null)
            {
                return; // JS unavailable: no-op.
            }
            try
            {
                await module.InvokeVoidAsync("clear");
            }
            catch (Exception ex) when (IsJsUnavailable(ex))
            {
                // JS unavailable: no-op.
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
            {
                try
                {
                    await _module.DisposeAsync();
                }
                catch (Exception ex) when (IsJsUnavailable(ex))
                {
                    // JS already gone (e.g. circuit disconnected): nothing to dispose.
                }
                _module = null;
            }
        }
    }
}
