using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;

namespace Core
{
    class Image_Storage
    {

        /*public static async Task<bool> Save(string filename, object data)
        {
            if (data is WriteableBitmap)
            {
                WriteableBitmap wb = (WriteableBitmap)data;

                try
                {


                    Guid BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
                    switch (fileFormat)
                    {
                        case FileFormat.Jpeg:
                            BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
                            break;

                        case FileFormat.Png:
                            BitmapEncoderGuid = BitmapEncoder.PngEncoderId;
                            break;
                    }

                    StorageFile file = await folder.CreateFileAsync(Name + "." + fileFormat.ToString(), CreationCollisionOption.ReplaceExisting);

                    using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoderGuid, stream);
                        Stream pixelStream = wb.PixelBuffer.AsStream();
                        byte[] pixels = new byte[pixelStream.Length];
                        await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                        encoder.SetPixelData(
                            BitmapPixelFormat.Bgra8,
                            BitmapAlphaMode.Ignore,
                            (uint)wb.PixelWidth,
                            (uint)wb.PixelHeight,
                            96.0,
                            96.0,
                            pixels
                        );
                        await encoder.FlushAsync();
                    }
                    Result = true;
                }
                catch (Exception e)
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                    Result = false;
                }
                
            }
            return false;
        }*/


    }

    public class FileTypes
    {
        public string FileName { get; set; }
        public string FileExtension { get; set; }
    }

    public static class Saving
    {
        

        public static async Task SaveData(object data, PickerLocationId dir, FileTypes[] ft, string SuggestedName)
        {
            if (dir == null) { dir = PickerLocationId.Unspecified; }
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = dir;

            if (ft == null || ft.Length == 0) { return; }

            for(int i = 0; i < ft.Length; i++)
            {
                savePicker.FileTypeChoices.Add(ft[i].FileName, new List<string>() { ft[i].FileExtension });
            }

            savePicker.SuggestedFileName = SuggestedName;

            StorageFile file = await savePicker.PickSaveFileAsync();
            
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);

                if (data is WriteableBitmap)
                {
                    await WriteImage(file, (WriteableBitmap)data);
                }





                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    System.Diagnostics.Debug.Write("File " + file.Name + " was saved.");
                }
                else
                {
                    System.Diagnostics.Debug.Write("File " + file.Name + " couldn't be saved.");
                }


            }




        }

        public static async Task WriteImage(StorageFile file, WriteableBitmap wb)
        {
            Guid BitmapEncoderGuid = new Guid();

            if (file.FileType.ToString() == ".png")
            {
                BitmapEncoderGuid = BitmapEncoder.PngEncoderId;
            }
            else if (file.FileType.ToString() == ".jpg")
            {
                BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
            }

            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoderGuid, stream);
                Stream pixelStream = wb.PixelBuffer.AsStream();
                byte[] pixels = new byte[pixelStream.Length];
                await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)wb.PixelWidth,
                    (uint)wb.PixelHeight,
                    96.0,
                    96.0,
                    pixels
                );
                await encoder.FlushAsync();
            }





        }



    }

    public static class Open
    {
        public static async Task<StorageFile> OpenData(PickerLocationId dir, FileTypes[] ft)
        {
            if (dir == null) { dir = PickerLocationId.Unspecified; }

            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = dir;

            for (int i = 0; i < ft.Length; i++)
            {
                openPicker.FileTypeFilter.Add(ft[i].FileExtension);
            }

            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                return file;
            }

            return null;
        }


        public static async Task<object> OpenImage(StorageFile file)
        {
            try
            {
                IRandomAccessStream stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

                if (stream.Size != 0)
                {
                    BitmapImage bi = new BitmapImage();
                    await bi.SetSourceAsync(stream);
                    bi = new BitmapImage();
                    stream.Seek(0);



                    return stream;


                }
            } catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + Environment.NewLine + ex.InnerException, ex.HResult.ToString());
                msg.ShowAsync();
            }
           
            return null;
        }


    }
    


}
