using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Core
{
    public class Loader
    {
        public static async Task<WriteableBitmap> LoadImage(string path)
        {
            StorageFile imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(path));

            using (IRandomAccessStream fileStream = await imageFile.OpenAsync(FileAccessMode.Read))
            {
                WriteableBitmap bitmap = new WriteableBitmap(1, 1);
                await bitmap.SetSourceAsync(fileStream);
                return bitmap;
            }

        }
    }
}
