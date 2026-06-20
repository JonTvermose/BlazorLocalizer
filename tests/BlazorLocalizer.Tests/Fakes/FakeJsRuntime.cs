using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace BlazorLocalizer.Tests.Fakes
{
    /// <summary>
    /// A working <see cref="IJSRuntime"/> backed by an in-memory dictionary that emulates the
    /// browser <c>localStorage</c> exposed by <c>blazorLocalizer.js</c>. Set <see cref="Available"/>
    /// to <c>false</c> to simulate the pre-first-render / prerender window where interop is not yet
    /// possible; flip it back to <c>true</c> to simulate JS becoming available after the first render.
    /// </summary>
    public class FakeJsRuntime : IJSRuntime
    {
        private readonly FakeLocalStorageModule _module;

        public bool Available { get; set; } = true;

        public FakeJsRuntime(IDictionary<string, string> store)
        {
            _module = new FakeLocalStorageModule(store);
        }

        public int GetItemCalls => _module.GetItemCalls;
        public int SetItemCalls => _module.SetItemCalls;

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            if (!Available)
            {
                throw new InvalidOperationException("Cannot invoke JavaScript outside of a WebView context.");
            }

            if (identifier == "import")
            {
                return new ValueTask<TValue>((TValue)(object)_module);
            }

            throw new InvalidOperationException($"Unexpected JS call on the runtime: {identifier}");
        }
    }

    /// <summary>
    /// Emulates the imported <c>blazorLocalizer.js</c> module (getItem/setItem/removeItem/clear)
    /// over a shared dictionary, so that two test scopes sharing the same store behave like two
    /// browser sessions sharing the same localStorage.
    /// </summary>
    internal class FakeLocalStorageModule : IJSObjectReference
    {
        private readonly IDictionary<string, string> _store;

        public int GetItemCalls { get; private set; }
        public int SetItemCalls { get; private set; }

        public FakeLocalStorageModule(IDictionary<string, string> store)
        {
            _store = store;
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            switch (identifier)
            {
                case "getItem":
                {
                    GetItemCalls++;
                    var key = (string)args![0]!;
                    _store.TryGetValue(key, out var value);
                    return new ValueTask<TValue>((TValue)(object?)value!);
                }
                case "setItem":
                {
                    SetItemCalls++;
                    _store[(string)args![0]!] = (string)args![1]!;
                    return default; // IJSVoidResult
                }
                case "removeItem":
                    _store.Remove((string)args![0]!);
                    return default;
                case "clear":
                    _store.Clear();
                    return default;
                default:
                    throw new InvalidOperationException($"Unexpected JS call on the module: {identifier}");
            }
        }

        public ValueTask DisposeAsync() => default;
    }
}
