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
using Windows.UI.Input;
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

namespace Bitmapping
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CroppingPage : Page
    {
        SelectedRegion selectedRegion;
        StorageFile sourceImageFile = null;

        uint sourceImagePixelWidth;
        uint sourceImagePixelHeight;

        /// <summary>
        /// Sizing of the corners
        /// </summary>
        double cornerSize;
        double CornerSize
        {
            get
            {
                if (cornerSize <= 0)
                {
                    //cornerSize = (double)Application.Current.Resources["Size"];
                    cornerSize = (double)this.Resources["Size"];
                }

                return cornerSize;
            }
        }

        /// <summary>
        /// The previous points of all the pointers.
        /// </summary>
        Dictionary<uint, Point?> pointerPositionHistory = new Dictionary<uint, Point?>();

        public CroppingPage()
        {
            this.InitializeComponent();

            selectRegion.ManipulationMode = ManipulationModes.Scale |
                ManipulationModes.TranslateX | ManipulationModes.TranslateY;

            selectedRegion = new SelectedRegion { MinSelectedRegionWidth = 310 / 128 * 310 / 8, MinSelectedRegionHeight = 310 / 128 * 128 / 8 };
            this.DataContext = selectedRegion;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            selectedRegion.PropertyChanged += SelectedRegion_PropertyChanged;

            ///Adding Corners
            ///
            AddCornerEvents(topLeftCorner);
            AddCornerEvents(topRightCorner);
            AddCornerEvents(bottomLeftCorner);
            AddCornerEvents(bottomRightCorner);

            // Handle the manipulation events of the selectRegion
            selectRegion.ManipulationDelta += SelectRegion_ManipulationDelta;
            selectRegion.ManipulationCompleted += SelectRegion_ManipulationCompleted;

            this.sourceImage.SizeChanged += SourceImage_SizeChanged;


            if (e.Parameter is StorageFile)
            {
                LoadImageFromSource((StorageFile)e.Parameter);
            }

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            selectedRegion.PropertyChanged -= SelectedRegion_PropertyChanged;

            ///Adding Corners
            ///
            RemoveCornerEvents(topLeftCorner);
            RemoveCornerEvents(topRightCorner);
            RemoveCornerEvents(bottomLeftCorner);
            RemoveCornerEvents(bottomRightCorner);

            selectRegion.ManipulationDelta -= SelectRegion_ManipulationDelta;
            selectRegion.ManipulationCompleted -= SelectRegion_ManipulationCompleted;
            this.sourceImage.SizeChanged -= SourceImage_SizeChanged;

        }

        private async void LoadImageFromSource(StorageFile imgFile)
        {

            if (imgFile != null)
            {
                this.previewImage.Source = null;
                this.sourceImage.Source = null;

                this.canvas.Visibility = Visibility.Collapsed;

                using (IRandomAccessStream fileStream = await imgFile.OpenAsync(FileAccessMode.Read))
                {
                    this.sourceImageFile = imgFile;
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);

                    this.sourceImagePixelHeight = decoder.PixelHeight;
                    this.sourceImagePixelWidth = decoder.PixelWidth;

                    if (this.sourceImagePixelHeight < 2 * this.CornerSize || this.sourceImagePixelWidth < 2 * this.CornerSize)
                    {
                        return;
                    }
                    else
                    {
                        double sourceImageScale = 1;

                        if (this.sourceImagePixelHeight < this.sourceImageGrid.ActualHeight && this.sourceImagePixelWidth < this.sourceImageGrid.ActualWidth)
                        {
                            this.sourceImage.Stretch = Stretch.None;
                        }
                        else
                        {
                            sourceImageScale = Math.Min(this.sourceImageGrid.ActualWidth / this.sourceImagePixelWidth, this.sourceImageGrid.ActualHeight / this.sourceImagePixelHeight);
                            this.sourceImage.Stretch = Windows.UI.Xaml.Media.Stretch.Uniform;
                        }

                        this.sourceImage.Source = await CropBitmap.GetCroppedBitmap(
                            this.sourceImageFile,
                            new Point(0, 0),
                            new Size(this.sourceImagePixelWidth, this.sourceImagePixelHeight),
                            sourceImageScale);

                    }


                }

            }

        }









        private void SourceImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.IsEmpty || double.IsNaN(e.NewSize.Height) || e.NewSize.Height <= 0)
            {
                this.canvas.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.selectedRegion.OuterRect = Rect.Empty;
                this.selectedRegion.ResetCorner(0, 0, 0, 0);
            }
            else
            {
                this.canvas.Visibility = Windows.UI.Xaml.Visibility.Visible;

                this.canvas.Height = e.NewSize.Height;
                this.canvas.Width = e.NewSize.Width;
                this.selectedRegion.OuterRect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);

                if (e.PreviousSize.IsEmpty || double.IsNaN(e.PreviousSize.Height) || e.PreviousSize.Height <= 0)
                {
                    this.selectedRegion.ResetCorner(0, 0, e.NewSize.Width, e.NewSize.Height);
                }
                else
                {
                    double scale = e.NewSize.Height / e.PreviousSize.Height;
                    this.selectedRegion.ResizeSelectedRect(scale);
                    UpdatePreviewImage();
                }

            }
        }


        private void AddCornerEvents(Control corner)
        {
            corner.PointerPressed += Corner_PointerPressed;
            corner.PointerMoved += Corner_PointerMoved;
            corner.PointerReleased += Corner_PointerReleased;
        }

        private void RemoveCornerEvents(Control corner)
        {
            corner.PointerPressed -= Corner_PointerPressed;
            corner.PointerMoved -= Corner_PointerMoved;
            corner.PointerReleased -= Corner_PointerReleased;
        }


        private void Corner_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            (sender as UIElement).CapturePointer(e.Pointer);

            Windows.UI.Input.PointerPoint pt = e.GetCurrentPoint(this);

            // Record the start point of the pointer.
            pointerPositionHistory[pt.PointerId] = pt.Position;
        }


        private void Corner_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Input.PointerPoint pt = e.GetCurrentPoint(this);
            uint ptrId = pt.PointerId;

            if (pointerPositionHistory.ContainsKey(ptrId) && pointerPositionHistory[ptrId].HasValue)
            {
                Point currentPosition = pt.Position;
                Point previousPosition = pointerPositionHistory[ptrId].Value;

                double xUpdate = currentPosition.X - previousPosition.X;
                double yUpdate = currentPosition.Y - previousPosition.Y;

                this.selectedRegion.UpdateCorner((sender as ContentControl).Tag as string, xUpdate, yUpdate);

                pointerPositionHistory[ptrId] = currentPosition;
            }

            e.Handled = true;
        }

        private void Corner_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            uint ptrId = e.GetCurrentPoint(this).PointerId;
            if (this.pointerPositionHistory.ContainsKey(ptrId))
            {
                this.pointerPositionHistory.Remove(ptrId);
            }

            (sender as UIElement).ReleasePointerCapture(e.Pointer);

            UpdatePreviewImage();
            e.Handled = true;
        }



        private void SelectedRegion_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            double widthScale = canvas.Width / this.sourceImagePixelWidth;
            double heightScale = canvas.Height / this.sourceImagePixelHeight;

        }

        private void SelectRegion_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            this.selectedRegion.UpdateSelectedRect(e.Delta.Scale, e.Delta.Translation.X, e.Delta.Translation.Y);
            e.Handled = true;
        }



        private void SelectRegion_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            UpdatePreviewImage();
        }


        private async void UpdatePreviewImage()
        {

            try
            {
                double sourceImageWidthScale = canvas.Width / this.sourceImagePixelWidth;
                double sourceImageHeightScale = canvas.Height / this.sourceImagePixelHeight;


                Size previewImageSize = new Size(
                    this.selectedRegion.SelectedRect.Width / sourceImageWidthScale,
                    this.selectedRegion.SelectedRect.Height / sourceImageHeightScale);

                double previewImageScale = 1;

                if (previewImageSize.Width <= canvas.Width &&
                    previewImageSize.Height <= canvas.Height)
                {
                    this.previewImage.Stretch = Windows.UI.Xaml.Media.Stretch.None;
                }
                else
                {
                    this.previewImage.Stretch = Windows.UI.Xaml.Media.Stretch.Uniform;

                    previewImageScale = Math.Min(canvas.Width / previewImageSize.Width,
                        canvas.Height / previewImageSize.Height);
                }



                this.previewImage.Source = await CropBitmap.GetCroppedBitmap(
                       this.sourceImageFile,
                       new Point(this.selectedRegion.SelectedRect.X / sourceImageWidthScale, this.selectedRegion.SelectedRect.Y / sourceImageHeightScale),
                       previewImageSize,
                       previewImageScale);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }



        }

        public static double MaxHeight = 128;

        private async void Accept_AppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            #region Tyr

            double sourceImageWidthScale = canvas.Width / this.sourceImagePixelWidth;
            double sourceImageHeightScale = canvas.Height / this.sourceImagePixelHeight;


            Size previewImageSize = new Size(
                this.selectedRegion.SelectedRect.Width / sourceImageWidthScale,
                this.selectedRegion.SelectedRect.Height / sourceImageHeightScale);

            double previewImageScale = 1;

            if (previewImageSize.Width <= canvas.Width &&
                previewImageSize.Height <= canvas.Height)
            {
                this.previewImage.Stretch = Windows.UI.Xaml.Media.Stretch.None;
            }
            else
            {
                this.previewImage.Stretch = Windows.UI.Xaml.Media.Stretch.Uniform;

                previewImageScale = Math.Min(canvas.Width / previewImageSize.Width,
                    canvas.Height / previewImageSize.Height);
            }



            ImageSource source = await CropBitmap.GetCroppedBitmap(
                   this.sourceImageFile,
                   new Point(this.selectedRegion.SelectedRect.X / sourceImageWidthScale, this.selectedRegion.SelectedRect.Y / sourceImageHeightScale),
                   previewImageSize,
                   previewImageScale);


            #endregion
            /*


            await CropBitmap.SaveCroppedBitmapAsync(
                sourceImageFile,
                croppedImageFile,
                new Point(this.selectedRegion.SelectedRect.X / widthScale, this.selectedRegion.SelectedRect.Y / heightScale),
                new Size(this.selectedRegion.SelectedRect.Width / widthScale, this.selectedRegion.SelectedRect.Height / heightScale));*/
        }
    }

}
