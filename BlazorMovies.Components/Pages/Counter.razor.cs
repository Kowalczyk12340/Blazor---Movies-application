using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorMovies.Components.Pages
{
    public partial class Counter
    {
        [Inject] private IJSRuntime js { get; set; }

        private int currentCount = 0;
        private static int currentCountStatic = 0;
        IJSObjectReference module;

        [JSInvokable]
        public async Task IncrementCount()
        {
            currentCount++;
            currentCountStatic++;
            await js.InvokeVoidAsync("dotnetStaticInvocation");
        }

        private async Task IncrementCountJavaScript()
        {
            await js.InvokeVoidAsync("dotnetInstanceInvocation",
                DotNetObjectReference.Create(this));
        }

        [JSInvokable]
        public static Task<int> GetCurrentCount()
        {
            return Task.FromResult(currentCountStatic);
        }
    }
}