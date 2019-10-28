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
    public sealed partial class BandSecondaryControl : UserControl
    {
        public BandSecondaryControl()
        {
            this.InitializeComponent();
        }

        public async void Set_Color(BandTheme theme)
        {
            Border.BorderBrush = new SolidColorBrush(theme.Highlight.ToColor());
            Highlight.Foreground = new SolidColorBrush(theme.Highlight.ToColor());
            Secondary.Foreground = new SolidColorBrush(theme.SecondaryText.ToColor());

                      
        }

    }
}
