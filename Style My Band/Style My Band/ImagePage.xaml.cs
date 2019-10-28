using Core.Observable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class ImagePage : Page
    {
        private TranslateTransform dragTranslation;

        public ImagePage()
        {
            this.InitializeComponent();
            Select.PointerPressed += Select_PointerPressed;
            Select.PointerReleased += Select_PointerReleased;
            Select.PointerExited += Select_PointerExited;

            Select.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            Select.ManipulationDelta += Select_ManipulationDelta;

            dragTranslation = new TranslateTransform();

            Select.RenderTransform = this.dragTranslation;




        }

        private void Select_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("X :: " + e.Delta.Translation.X);
            //System.Diagnostics.Debug.WriteLine("Y :: " + e.Delta.Translation.Y);

            //dragTranslation.X += e.Delta.Translation.X;
            // dragTranslation.Y += e.Delta.Translation.Y;


            var translate = (TranslateTransform)Select.RenderTransform;

            var newPosX = Canvas.GetLeft(Select) + translate.X + e.Delta.Translation.X;
            var newPosY = Canvas.GetTop(Select) + translate.Y + e.Delta.Translation.Y;

            if (!isBoundary(newPosX, CanvasRoot.ActualWidth - Select.ActualWidth, 0))
                translate.X += e.Delta.Translation.X;
            if (!isBoundary(newPosY, CanvasRoot.ActualHeight - Select.ActualHeight, 0))
                translate.Y += e.Delta.Translation.Y;



        }

        bool isBoundary(double value, double max, double min)
        {
            return value > max ? true : value < min ? true : false;
        }


        public int height = 102;
        public int width = 310;



        private void Select_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Canvas can = sender as Canvas;

            if (null != can)
            {
                can.Width = width;
                can.Height = height;
            }
        }

        private void Select_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Canvas can = sender as Canvas;

            if (null != can)
            {
                can.Width = width;
                can.Height = height;
            }
        }

        private void Select_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Canvas can = sender as Canvas;

            if (null != can)
            {
                can.Width = width;
                can.Height = height;
            }
        }






        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (App.BandGeneration == 1) { width = 310; height = 102; }
            if (App.BandGeneration == 2) { width = 310; height = 128; }

            await Get_Image();

            CanvasRoot.Clip = new RectangleGeometry();
            CanvasRoot.Clip.Rect = new Rect(0, 0, CanvasRoot.ActualWidth, CanvasRoot.ActualHeight);
            SourceImage.MaxHeight = CanvasRoot.ActualHeight;
            SourceImage.MaxWidth = CanvasRoot.ActualWidth;

        }


        
        public async Task UpdateMeTile()
        {
            await Core.BandHandler.Set_Image("ms-appx:///Resources/MeTileBackground.png");
            //BandPreview.SetImage(await Core.BandHandler.Get_Image());
        }

        public static WriteableBitmap _Image;

        public async Task Get_Image()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {

                Uri uri = new Uri(file.Path, UriKind.Relative);

                IRandomAccessStream stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

                if (stream.Size != 0)
                {
                    BitmapImage bi = new BitmapImage();
                    await bi.SetSourceAsync(stream);
                    _Image = new WriteableBitmap(bi.PixelWidth, bi.PixelHeight);
                    bi = new BitmapImage();
                    stream.Seek(0);
                    try
                    {


                        await _Image.SetSourceAsync(stream);

                        if (_Image.PixelHeight < 102 || _Image.PixelWidth < 310)
                        {
                            MessageDialog msg = new MessageDialog("The image you have selected is not going to work on your Band", "Wrong resolution");
                            await msg.ShowAsync();
                            await Get_Image();
                            return;

                        }
                        

                        SourceImage.MaxHeight = _Image.PixelHeight;
                        SourceImage.MaxWidth = _Image.PixelWidth;
                        SourceImage.Source = _Image;

                        if (_Image.PixelHeight == 128 && _Image.PixelWidth == 310 && App.BandGeneration == 2)
                        {
                            Select.Visibility = Visibility.Collapsed;
                        }
                        else if (_Image.PixelHeight == 102 && _Image.PixelWidth == 310 && App.BandGeneration == 1)
                        {
                            Select.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            Select.Visibility = Visibility.Visible;
                        }


                        //SourceImage.


                    }
                    catch (Exception x)
                    {
                        System.Diagnostics.Debug.Write(x);
                    }
                }



            }
        }




        private async void AppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (App.BandGeneration == 1)
            {
                if (_Image.PixelWidth == 310 && _Image.PixelHeight == 102)
                {
                    App._MeImageTile = _Image;
                    this.Frame.GoBack();
                }
            }
            else if (App.BandGeneration == 2)
            {
                if (_Image.PixelWidth == 310 && (_Image.PixelHeight == 102 || _Image.PixelHeight == 128))
                {
                    App._MeImageTile = _Image;
                    this.Frame.GoBack();
                }
            }



            var translate = (TranslateTransform)Select.RenderTransform;
            Point p = new Point(translate.X / Select.Width, translate.Y / Select.Height);

            Size s = new Size(width, height);

            WriteableBitmap wb = new WriteableBitmap(width, height);





           // wb.SetSource();


        }

        private void Select_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void SourceImage_ImageOpened(object sender, RoutedEventArgs e)
        {

        }

        private void AppBarButton_Save_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private async void AppBarButton_Open_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Get_Image();
        }
    }
}
