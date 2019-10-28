using Microsoft.Band;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Style_My_Band
{
    public sealed partial class BandLowlightControl : UserControl
    {
        public BandLowlightControl()
        {
            this.InitializeComponent();
        }

        public void Set_Color(BandTheme theme)
        {

            try
            {
                foreach (var item in BandLayout.Items)
                {
                    if (item is ListBoxItem)
                    {
                        ListBoxItem it = (ListBoxItem)item;
                        if (it.Tag != null)
                        {
                            if (it.Tag.ToString() == "Base")
                            {
                                it.Background = new SolidColorBrush(theme.Base.ToColor());
                            }
                            else if (it.Tag.ToString() == "Lowlight")
                            {
                                it.Background = new SolidColorBrush(theme.Lowlight.ToColor());
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e);
            }

            
        }

    }
}
