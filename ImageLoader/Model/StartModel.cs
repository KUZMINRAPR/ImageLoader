using System.IO;
using System.Net.Http;
using System.Windows.Media.Imaging;

namespace ImageLoader.Model;

public static class StartModel
{
    private static HttpClient _client = new();

    public static async Task<BitmapImage> GetImage(string? uri, CancellationToken token)
    {
        if (uri == null)
        {
            throw new ArgumentException("uri пустое");
        }

        await Task.Delay(3000, token); // Было добавлено для того, чтобы можно было успеть остановить загрузку
        
        var response = await _client.GetByteArrayAsync(uri, token);
        
        Console.WriteLine("The request has been sent");
        
        var image = new BitmapImage();
        var stream = new MemoryStream(response);
        
        image.BeginInit();
        image.StreamSource = stream;
        image.EndInit();
        
        Console.WriteLine("The image has been uploaded");
        
        return image;
    }
}