using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace DevExample.Platform.Shared
{
    public static class Clipboard
    {
        public static async Task CopyTextToClipboard(IJSRuntime JSRuntimestring, string text)
        {
            await JSRuntimestring.InvokeVoidAsync("clipboardCopy.copyText", text);

        }
    }
}
