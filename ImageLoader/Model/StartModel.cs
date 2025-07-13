using System.IO;
using System.Net.Http;
using System.Windows.Media.Imaging;

namespace ImageLoader.Model;

public class StartModel
{
    HttpClient client = new HttpClient();

    public async Task<BitmapImage> GetImage(string uri)
    {
        var response = await client.GetByteArrayAsync(uri);
        var image = new BitmapImage();
        var stream = new MemoryStream(response);
        
        image.BeginInit();
        image.StreamSource = stream;
        image.EndInit();

        return image;
    }
}