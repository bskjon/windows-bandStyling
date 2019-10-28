using Microsoft.Band;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Style_My_Band
{
    public sealed partial class MyBandTilePreview : UserControl
    {
        public MyBandTilePreview()
        {
            this.InitializeComponent();
            if (timer == null)
            {
                Run_Clock();
            }
        }

        DispatcherTimer timer = null;


        public void SetImage(WriteableBitmap image)
        {
            BandMainImage.Source = image;
        }

        public void SetTheme(BandTheme theme)
        {
            Style st = new Style();
            st.TargetType = typeof(ListBoxItem);
            Setter Highlight = new Setter();




            foreach (var item in BandLayout.Items)
            {
                if (item is ListBoxItem)
                {
                    ListBoxItem it = (ListBoxItem)item;
                    if (it.Tag != null)
                    {
                        //it.Style.Setters.Add(new Setter(ListBoxItem.))

                        if (it.Tag.ToString() == "BaseColorStatic")
                        {
                            it.Background = new SolidColorBrush(theme.Base.ToColor());
                        }
                        else if (it.Tag.ToString() == "BaseColorHighlighted")
                        {
                            it.Background = new SolidColorBrush(theme.HighContrast.ToColor());
                            
                        }
                    }
           
                    
                }
            }
        }

        public void Run_Clock()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Update_Time;
            timer.Start();
        }

        private void Update_Time(object sender, object e)
        {
            Time.Text = DateTime.Now.ToString("HH:mm");
            DayText.Text = DateTime.Now.DayOfWeek.ToString().Substring(0, 3);
            DayNumber.Text = DateTime.Now.Date.ToString("dd");
        }
    }
}
