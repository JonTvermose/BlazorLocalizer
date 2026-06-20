using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorLocalizer;
using BlazorLocalizer.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Xunit;

namespace BlazorLocalizer.Tests
{
    /// <summary>
    /// Tests that the localStorage cache layer degrades to the <see cref="IResourceProvider"/>
    /// (instead of throwing) when JS interop is unavailable, and resumes once JS is available.
    /// </summary>
    public class GracefulDegradationTests
    {
        private static ServiceProvider BuildProvider(
            IJSRuntime jsRuntime,
            IResourceProvider resourceProvider,
            System.Action<BlazorLocalizerOptions>? configure = null)
        {
            var services = new ServiceCollection();
            services.AddBlazorLocalization(o => configure?.Invoke(o));
            services.AddSingleton(jsRuntime);
            services.AddSingleton(resourceProvider);
            return services.BuildServiceProvider();
        }

        [Fact]
        public async Task L_WhenJsUnavailable_ReturnsProviderValue_AndDoesNotThrow()
        {
            var provider = new FakeResourceProvider(new Dictionary<string, string> { ["Hello"] = "World" });
            await using var sp = BuildProvider(new ThrowingJsRuntime(), provider);
            var loc = sp.GetRequiredService<IBlazorLocalizer>();

            var value = await loc.L("Hello");

            Assert.Equal("World", value);
            Assert.True(provider.GetCategoryResourcesCalls >= 1);
        }

        [Fact]
        public async Task L_WhenJsUnavailable_DoesNotLatchFailure_RetriesEveryCall()
        {
            var provider = new FakeResourceProvider(new Dictionary<string, string> { ["Hello"] = "World" });
            var js = new ThrowingJsRuntime();
            await using var sp = BuildProvider(js, provider);
            var loc = sp.GetRequiredService<IBlazorLocalizer>();

            // First lookup populates the in-memory cache; subsequent lookups for the same category
            // are served from memory, so to prove the module import is retried (not latched) we
            // request distinct categories and assert the runtime is hit again each time.
            await loc.L("Hello", "Category.A");
            var countAfterFirst = js.InvokeCount;
            await loc.L("Hello", "Category.B");

            Assert.True(countAfterFirst >= 1, "Module import should have been attempted on the first lookup.");
            Assert.True(js.InvokeCount > countAfterFirst, "Module import should be retried, not latched as failed.");
        }

        [Fact]
        public async Task L_WhenJsAvailable_WritesValueToLocalStorage()
        {
            var store = new Dictionary<string, string>();
            var provider = new FakeResourceProvider(new Dictionary<string, string> { ["Hello"] = "World" });
            var js = new FakeJsRuntime(store) { Available = true };
            await using var sp = BuildProvider(js, provider);
            var loc = sp.GetRequiredService<IBlazorLocalizer>();

            var value = await loc.L("Hello");

            Assert.Equal("World", value);
            Assert.NotEmpty(store); // localStorage write-back happened
            Assert.True(js.SetItemCalls >= 1);
        }

        [Fact]
        public async Task L_WhenJsAvailable_SecondSession_ReadsFromLocalStorage_NotProvider()
        {
            // Two service providers sharing one localStorage store == two browser sessions sharing
            // the same localStorage. The second session has its own (empty) in-memory cache.
            var store = new Dictionary<string, string>();
            var js = new FakeJsRuntime(store) { Available = true };

            var firstProvider = new FakeResourceProvider(new Dictionary<string, string> { ["Hello"] = "World" });
            await using (var sp1 = BuildProvider(js, firstProvider))
            {
                var loc1 = sp1.GetRequiredService<IBlazorLocalizer>();
                Assert.Equal("World", await loc1.L("Hello"));
            }

            // The second session's provider would return a different value if it were ever queried.
            var secondProvider = new FakeResourceProvider(new Dictionary<string, string> { ["Hello"] = "SHOULD-NOT-BE-USED" });
            await using var sp2 = BuildProvider(js, secondProvider);
            var loc2 = sp2.GetRequiredService<IBlazorLocalizer>();

            var value = await loc2.L("Hello");

            Assert.Equal("World", value); // served from localStorage written by the first session
            Assert.Equal(0, secondProvider.GetCategoryResourcesCalls);
        }

        [Fact]
        public async Task L_ResumesLocalStorageCaching_OnceJsBecomesAvailable()
        {
            var store = new Dictionary<string, string>();
            var provider = new FakeResourceProvider(new Dictionary<string, string> { ["Hello"] = "World" });
            var js = new FakeJsRuntime(store) { Available = false }; // before first render
            await using var sp = BuildProvider(js, provider);
            var loc = sp.GetRequiredService<IBlazorLocalizer>();

            // JS unavailable: value comes from the provider, nothing is written to localStorage.
            Assert.Equal("World", await loc.L("Hello", "Category.A"));
            Assert.Empty(store);

            // First render completes -> JS interop becomes available.
            js.Available = true;

            // A lookup for a not-yet-cached category now flows through to localStorage.
            Assert.Equal("World", await loc.L("Hello", "Category.B"));
            Assert.True(store.ContainsKey("Category.B-en-US"));
        }
    }
}
