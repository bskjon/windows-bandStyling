using Microsoft.Band;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class BandPreviewControl : UserControl
    {
        public BandPreviewControl()
        {
            this.InitializeComponent();
        }

        public void Change_Item(int index)
        {
            BandLayoutPreview.SelectedIndex = index;
        }

        public async Task<bool> ContentIsLoaded()
        {
            bool Loaded = true;

            if (Base == null)
                Loaded = true;
            if (HighContrast == null)
                Loaded = true;
            if (LowLight == null)
                Loaded = true;
            if (Hightlight == null)
                Loaded = true;
            if (Muted == null)
                Loaded = true;
            if (Secondary == null)
                Loaded = false;

            return Loaded;
        }


        public async void Update_ColorOnPreview(int index, BandTheme _BandTheme)
        {
            switch (index)
            {
                case -1:
                    try
                    {
                        while ( await ContentIsLoaded() == false)
                        {
                            await Task.Delay(100);
                            System.Diagnostics.Debug.WriteLine("Task Delayed");
                        }
                        

                        Base.Set_Color(_BandTheme);
                        HighContrast.Set_Color(_BandTheme);
                        LowLight.Set_Color(_BandTheme);
                        Hightlight.Set_Color(_BandTheme);
                        Muted.Set_Color(_BandTheme);
                        Secondary.Set_Color(_BandTheme);
                    } catch (Exception e)
                    {
                        System.Diagnostics.Debug.Write(e);
                    }
                    
                    break;
                case 0:
                    Base.Set_Color(_BandTheme);
                    break;
                case 1:
                    HighContrast.Set_Color(_BandTheme);
                    break;
                case 2:
                    LowLight.Set_Color(_BandTheme);
                    break;
                case 3:
                    Hightlight.Set_Color(_BandTheme);
                    break;
                case 4:
                    Muted.Set_Color(_BandTheme);
                    break;
                case 5:
                    Secondary.Set_Color(_BandTheme);
                    break;
            }
        }


    }
}
