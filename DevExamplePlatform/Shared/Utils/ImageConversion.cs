using Microsoft.AspNetCore.Components.Forms;
using System;
using System.IO;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;

namespace DevExample.Platform.Shared.Utils
{
    public static class ImageConversion
    {
        // Returns null if file size is empty or over max

        // 2048000 bytes
        // 2.048 Megabytes
        public readonly static int MaxFileSize = 2048000;

        public async static Task<string> ConvertFileToBase64(IBrowserFile file)
        {
            if (file.Size > 0 || file.Size <= MaxFileSize)
            {
                try
                {
                    var buffer = new byte[file.Size];

                    await file.OpenReadStream(MaxFileSize).ReadAsync(buffer);

                    var base64Url = $"data:{file.ContentType};base64,{Convert.ToBase64String(buffer)}";

                    return base64Url;
                }
                catch (InvalidCastException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return null;
        }

        public async static Task<string> ConvertBufferToBase64(byte[] buffer, string contentType)
        {
            if (buffer.Length > 0 || buffer.Length <= MaxFileSize)
            {
                try
                { 

                    var base64Url = $"data:{contentType};base64,{Convert.ToBase64String(buffer)}";

                    return base64Url;
                }
                catch (InvalidCastException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return null;
        }
    }
}
