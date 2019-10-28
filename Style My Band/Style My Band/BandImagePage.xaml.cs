using Bitmapping;
using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Style_My_Band
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BandImagePage : Page
    {
        public BandImagePage()
        {
            this.InitializeComponent();
        }

        public WriteableBitmap wb = new WriteableBitmap(1,1);

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            MeTile.Source = App._MeImageTile;
            Details_BandGenerationTextBlock.Text = App.BandGeneration.ToString();
            if (App.BandGeneration == -1)
            {
                MessageDialog msg = new MessageDialog("Could'n get your band generation", "Error");
                msg.ShowAsync();
            }
            if (App.BandGeneration == 1)
            {
                Details_HeightTextBlock.Text = "102";
                Details_WidthTextBlock.Text = "310";
            } else if (App.BandGeneration == 2)
            {
                Details_HeightTextBlock.Text = "128";
                Details_WidthTextBlock.Text = "310";
            }
        }


        private async void AppBarButton_Save_Tapped(object sender, TappedRoutedEventArgs e)
        {
            List<Core.FileTypes> items = new List<Core.FileTypes>();
            items.Add(new Core.FileTypes()
            {
                 FileName = "PNG",
                 FileExtension = ".png"
            });

            await Core.Saving.SaveData(App._MeImageTile, Windows.Storage.Pickers.PickerLocationId.PicturesLibrary, items.ToArray(), "MeTile");
            
        }

        private async void AppBarButton_Open_Tapped(object sender, TappedRoutedEventArgs e)
        {
            List<Core.FileTypes> items = new List<Core.FileTypes>();
            items.Add(new Core.FileTypes()
            {
                FileName = "PNG",
                FileExtension = ".png"
            });
            items.Add(new Core.FileTypes()
            {
                FileName = "JPG",
                FileExtension = ".jpg"
            });

            try
            {
                StorageFile file = await Core.Open.OpenData(Windows.Storage.Pickers.PickerLocationId.PicturesLibrary, items.ToArray());
                BitmapImage bi = new BitmapImage();
                using (IRandomAccessStream accessStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    await bi.SetSourceAsync(accessStream);
                    MeTile.Source = bi;
                    accessStream.Seek(0);
                    await wb.SetSourceAsync(accessStream);

                }

                if (bi.PixelHeight == 128 || bi.PixelHeight == 102 && bi.PixelWidth == 310)
                {

                }
                else
                {
#if DEBUG
                this.Frame.Navigate(typeof(CroppingPage), file);
#endif
                }
            }
            catch (Exception ex)
            {

            }
           

            
            /*IRandomAccessStream data = await Core.Open.OpenData(Windows.Storage.Pickers.PickerLocationId.PicturesLibrary, items.ToArray());
            data.Seek(0);*/
            //BitmapDecoder decoder = await BitmapDecoder.CreateAsync(data);
            /*this.Frame.Navigate(typeof(CroppingPage), decoder);
            data.Seek(0);
            BitmapImage bi = new BitmapImage();
            await bi.SetSourceAsync(data);
            MeTile.Source = bi;
            data.Seek(0);
            await wb.SetSourceAsync(data);*/


        }

        private async void AppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {

            if ((wb.PixelHeight == 128 || wb.PixelHeight == 102) && wb.PixelWidth == 310)
            {
                App._MeImageTile = wb;
                this.Frame.GoBack();
            }
            else if (wb.PixelHeight == 1 && wb.PixelWidth == 1)
            {
                this.Frame.GoBack();
            }
            else
            {
                MessageDialog msg = new MessageDialog(Localization.Get_Text(Localization.Tag.MessageDialog_, "SelectingImageErrorContent"), Localization.Get_Text(Localization.Tag.MessageDialog_, "SelectingImageErrorTitle"));

                await msg.ShowAsync();
            }



        }
    }
}
