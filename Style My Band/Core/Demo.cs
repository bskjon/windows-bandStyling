using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Band.Personalization;
using Microsoft.Band;
using Windows.UI;

namespace Core
{
    public class Demo
    {
        public static async Task<BandTheme> DemoTheme()
        {
            BandTheme bandtheme = new BandTheme();
            bandtheme.Base = Color.FromArgb(255, 0, 184, 241).ToBandColor();
            bandtheme.HighContrast = Color.FromArgb(255, 0, 188, 227).ToBandColor();
            bandtheme.Highlight = Color.FromArgb(255, 0, 199, 239).ToBandColor();
            bandtheme.Lowlight = Color.FromArgb(255, 91, 194, 231).ToBandColor();
            bandtheme.Muted = Color.FromArgb(255, 98, 142, 156).ToBandColor();
            bandtheme.SecondaryText = Color.FromArgb(255, 0, 137, 182).ToBandColor();
            return bandtheme;
        }
    }
}
