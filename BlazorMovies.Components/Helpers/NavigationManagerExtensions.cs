using Microsoft.AspNetCore.Components;

namespace BlazorMovies.Components.Helpers
{
    public static class NavigationManagerExtensions
    {
        public static Dictionary<string, string> GetQueryStrings(
            this NavigationManager navigationManager, string url)
        {
            if (string.IsNullOrEmpty(url) || !url.Contains("?") || url.Substring(url.Length - 1) == "?")
            {
                return null;
            }

            // https://domain.com?key1=value1&key2=value2
            var queryStrings = url.Split(new [] { "?" }, StringSplitOptions.None)[1];
            var dicQueryString =
                queryStrings.Split('&')
                    .ToDictionary(c => c.Split('=')[0],
                        c => Uri.UnescapeDataString(c.Split('=')[1]));

            return dicQueryString;
        }
    }
}
