using Microsoft.Band;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Core
{
    public static class Parse
    {

        public static async Task<BandColor> _BandColor(object value)
        {
            BandColor bc = Colors.Purple.ToBandColor();
            if (value is Color)
            {
                Color c = (Color)value;
                bc = c.ToBandColor();
                
            }
            return bc;
        }

        public static async Task<Color> _Color (string value)
        {
            Color color = Color.FromArgb(0, 0, 0, 0);

            string[] values = value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (value.Length == 3)
            {
                color = Color.FromArgb(255, _byte(values[0])[0], _byte(values[1])[0], _byte(values[2])[0]);
            }
            else if (values.Length == 4)
            {
                color = Color.FromArgb(_byte(values[0])[0], _byte(values[1])[0], _byte(values[2])[0], _byte(values[3])[0]);
            }


            return color;
        }

        public static async Task<string> _ColorToHEX(Color value)
        {

            return value.R.ToString("X2") + value.G.ToString("X2") + value.B.ToString("X2");
        }

        public static async Task<Color> _ColorFromHEX(string Value)
        {
            Color _Color;
            string value = "";
            if (Value.Contains("#")) { value = Value.Substring(1); }
            int _Length = value.Length;
            try
            {
                if (_Length == 6)
                {
                    _Color = ColorHelper.FromArgb(255,
                       byte.Parse(value.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                       byte.Parse(value.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                       byte.Parse(value.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)

                       );

                }
                else if (_Length == 8)
                {
                    _Color = ColorHelper.FromArgb(255,
                        byte.Parse(value.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                        byte.Parse(value.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                        byte.Parse(value.Substring(6, 2), System.Globalization.NumberStyles.HexNumber)

                        );
                }
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.Write("<ERROR> " + err + " </ERROR>");
            }
            return _Color;
        }



        public static async Task<SolidColorBrush> _SolidColorBrush(string value)
        {
            SolidColorBrush color = new SolidColorBrush(Colors.Black);

            string[] values = value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            if (values.Length == 3 )
            {
                color = new SolidColorBrush(Color.FromArgb(255, _byte(values[0])[0], _byte(values[1])[0], _byte(values[2])[0]));
            }
            else if (values.Length == 4)
            {
                color = new SolidColorBrush(Color.FromArgb(_byte(values[0])[0], _byte(values[1])[0], _byte(values[2])[0], _byte(values[3])[0]));
            }


            return color;
        }

        public static int _int(string value)
        {
            int i;

            bool Result = int.TryParse(value, out i);

            if (Result)
            {
                return i;
            }
            return i;
        }

        public static byte[] _byte(string value)
        {
            return System.Text.Encoding.UTF8.GetBytes(value);
        }

    }
}
