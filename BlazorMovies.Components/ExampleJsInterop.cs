using Microsoft.JSInterop;

namespace BlazorMovies.Components
{
    public class ExampleJsInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

        public ExampleJsInterop(IJSRuntime jsRuntime)
        {
            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/BlazorMovies.Components/exampleJsInterop.js"
                ).AsTask());
        }

        public async ValueTask<string> Prompt(string message)
        {
            var module = await _moduleTask.Value;

            return await module.InvokeAsync<string>("showPrompt", message);
        }

        public async ValueTask DisposeAsync()
        {
            if (_moduleTask.IsValueCreated)
            {
                var module = await _moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}