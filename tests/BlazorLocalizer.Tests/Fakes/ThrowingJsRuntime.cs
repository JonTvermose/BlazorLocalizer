using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace BlazorLocalizer.Tests.Fakes
{
    /// <summary>
    /// An <see cref="IJSRuntime"/> that always fails the way a BlazorWebView does before its
    /// first render (.NET MAUI/WPF/WinForms) and the way Blazor Server does during static
    /// prerendering: every interop call throws <see cref="InvalidOperationException"/> with the
    /// "Cannot invoke JavaScript outside of a WebView context." message.
    /// </summary>
    public class ThrowingJsRuntime : IJSRuntime
    {
        public int InvokeCount { get; private set; }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            InvokeCount++;
            throw new InvalidOperationException("Cannot invoke JavaScript outside of a WebView context.");
        }
    }
}
