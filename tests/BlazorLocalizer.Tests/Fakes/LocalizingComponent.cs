using System.Threading.Tasks;
using BlazorLocalizer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorLocalizer.Tests.Fakes
{
    /// <summary>
    /// A minimal component that localizes a string in <see cref="OnInitializedAsync"/> - the exact
    /// pattern (e.g. a layout) that crashed when JS interop was unavailable at initialization time.
    /// </summary>
    public class LocalizingComponent : ComponentBase
    {
        [Inject] public IBlazorLocalizer Loc { get; set; } = default!;

        public string Text { get; private set; } = "";

        protected override async Task OnInitializedAsync()
        {
            Text = await Loc.L("Hello");
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(0, Text);
        }
    }
}
