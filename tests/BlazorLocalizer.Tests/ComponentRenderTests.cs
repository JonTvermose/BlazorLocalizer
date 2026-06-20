using System.Collections.Generic;
using BlazorLocalizer;
using BlazorLocalizer.Tests.Fakes;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Xunit;

namespace BlazorLocalizer.Tests
{
    /// <summary>
    /// bUnit tests that render a component which localizes text in OnInitializedAsync - reproducing
    /// the scenario that previously tripped the consuming app's error boundary (BlazorWebView before
    /// first render / Blazor Server prerender).
    /// </summary>
    public class ComponentRenderTests : TestContext
    {
        [Fact]
        public void Component_LocalizingInOnInitializedAsync_RendersWithoutThrowing_WhenJsUnavailable()
        {
            // Override bUnit's default IJSRuntime with one that throws like a pre-render WebView.
            Services.AddSingleton<IJSRuntime>(new ThrowingJsRuntime());
            Services.AddBlazorLocalization(o => { });
            Services.AddSingleton<IResourceProvider>(
                new FakeResourceProvider(new Dictionary<string, string> { ["Hello"] = "World" }));

            var cut = RenderComponent<LocalizingComponent>();

            cut.WaitForAssertion(() => Assert.Contains("World", cut.Markup));
        }

        [Fact]
        public void Component_UsesLocalStorage_WhenJsAvailable()
        {
            var store = new Dictionary<string, string>();
            var js = new FakeJsRuntime(store) { Available = true };
            Services.AddSingleton<IJSRuntime>(js);
            Services.AddBlazorLocalization(o => { });
            Services.AddSingleton<IResourceProvider>(
                new FakeResourceProvider(new Dictionary<string, string> { ["Hello"] = "World" }));

            var cut = RenderComponent<LocalizingComponent>();

            cut.WaitForAssertion(() =>
            {
                Assert.Contains("World", cut.Markup);
                Assert.NotEmpty(store); // localStorage was written once JS was available
            });
        }
    }
}
