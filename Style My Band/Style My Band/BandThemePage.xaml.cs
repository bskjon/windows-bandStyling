using Core;
using Microsoft.Band;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Style_My_Band
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BandThemePage : Page
    {

        public bool PassedHex { get; set; }

        public bool ResetTheme = false;

        public BandThemePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            App.HexWatcher.ValueChanged += HexWatcher_ValueChanged;
        }

        private void ResetWatcher_ValueChanged(bool value)
        {
            ResetTheme = value;
        }

        private void HexWatcher_ValueChanged(string value)
        {
            if (value is string)
            {
                if (value.Contains("#"))
                    PassedHex = true;
            }
        }

        Color _Base;
        Color _HighContrast;
        Color _Lowlight;
        Color _Highlight;
        Color _Muted;
        Color _Secondary;

        public BandTheme _Theme;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (SingleBinding.ResetThemePage)
            {
                SingleBinding.ResetThemePage = false;
                App.HexWatcher.Color = string.Empty;
                if (App._SelectedBandTheme == -1 && App._CustomBandThemes.Count == 0) { this.Frame.GoBack(); return; }
                _Theme = App._CustomBandThemes[App._SelectedBandTheme].Theme;
                await Load_Theme(_Theme);
            }

            UpdateHex();

        }

        public async void UpdateHex()
        {
            if (PassedHex)
            {
                PassedHex = false;
                try
                {
                    string hex = App.HexWatcher.Color.ToString();

                    if ((hex.Length == 9 || hex.Length == 7) && hex.Substring(0, 1).Contains("#") ||
                        (hex.Length == 6 || hex.Length == 8) && !hex.Substring(0, 1).Contains("#"))
                    {
                        Color _Color = await Parse._ColorFromHEX(hex);


                        _R.Text = _Color.R.ToString();
                        _G.Text = _Color.G.ToString();
                        _B.Text = _Color.B.ToString();
                        _RSlider.Value = double.Parse(_Color.R.ToString());
                        _GSlider.Value = double.Parse(_Color.G.ToString());
                        _BSlider.Value = double.Parse(_Color.B.ToString());
                        Update_Hex(_Color);

                        await Update_Color(_Color);
                        Apply_Color(_Color);
                    }




                }
                catch (Exception ex)
                {

                }

            }
        }

        public async Task Load_Theme(BandTheme theme)
        {
            Base.Background = new SolidColorBrush(_Theme.Base.ToColor());
            HighContrast.Background = new SolidColorBrush(_Theme.HighContrast.ToColor());
            Lowlight.Background = new SolidColorBrush(_Theme.Lowlight.ToColor());
            Highlight.Background = new SolidColorBrush(_Theme.Highlight.ToColor());
            Muted.Background = new SolidColorBrush(_Theme.Muted.ToColor());
            Secondary.Background = new SolidColorBrush(_Theme.SecondaryText.ToColor());

            _Base = _Theme.Base.ToColor();
            _HighContrast = _Theme.HighContrast.ToColor();
            _Lowlight = _Theme.Lowlight.ToColor();
            _Highlight = _Theme.Highlight.ToColor();
            _Muted = _Theme.Muted.ToColor();
            _Secondary = _Theme.SecondaryText.ToColor();

            PreviewControl.Update_ColorOnPreview(-1, await Generate_BandTheme());

            if (ColorTypeSelector.SelectedIndex == -1)
            {
                ColorTypeSelector.SelectedIndex = 0;
            }
            else
            {
                Update_ColorSelectors(ColorTypeSelector.SelectedIndex);
            }
                
        }

        public async void Update_ColorSelectors(int selected)
        {
            Color c = await Get_Color();
            _R.Text = c.R.ToString();
            _G.Text = c.G.ToString();
            _B.Text = c.B.ToString();

            try
            {
                _RSlider.Value = double.Parse(c.R.ToString());
                _GSlider.Value = double.Parse(c.G.ToString());
                _BSlider.Value = double.Parse(c.B.ToString());
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message.ToString(), ex.HResult.ToString());
                await msg.ShowAsync();
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            
            Update_Hex(c);

            //PreviewControl.Update_ColorOnPreview(-1, await Generate_BandTheme());
        }


        public async Task<BandTheme> Generate_BandTheme()
        {
            BandTheme theme = new BandTheme()
            {
                Base = _Base.ToBandColor(),
                HighContrast = _HighContrast.ToBandColor(),
                Highlight = _Highlight.ToBandColor(),
                Lowlight = _Lowlight.ToBandColor(),
                Muted = _Muted.ToBandColor(),
                SecondaryText = _Secondary.ToBandColor()
            };

            return theme;
        }

        private async void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int it = ((ListBox)sender).SelectedIndex;
            PreviewControl.Change_Item(it);
            Color c = await Get_Color();
            _R.Text = c.R.ToString();
            _G.Text = c.G.ToString();
            _B.Text = c.B.ToString();

            try
            {
                _RSlider.Value = double.Parse(c.R.ToString());
                _GSlider.Value = double.Parse(c.G.ToString());
                _BSlider.Value = double.Parse(c.B.ToString());
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message.ToString(), ex.HResult.ToString());
                await msg.ShowAsync();
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            Update_Hex(c);

            PreviewControl.Update_ColorOnPreview(-1, await Generate_BandTheme());

        }

        public async Task<Color> Get_Color()
        {
            int Index = ColorTypeSelector.SelectedIndex;
            switch (Index)
            {
                case 0:
                    return _Base;
                    break;
                case 1:
                    return _HighContrast;
                    break;
                case 2:
                    return _Lowlight;
                    break;
                case 3:
                    return _Highlight;
                    break;
                case 4:
                    return _Muted;
                    break;
                case 5:
                    return _Secondary;
                    break;
            }
            return Color.FromArgb(255, 50, 50, 50);
        }

        public async Task Update_Color(Color _color)
        {
            int Index = ColorTypeSelector.SelectedIndex;
            
            switch (Index)
            {
                case 0:
                    _Base = _color;
                    break;
                case 1:
                    _HighContrast = _color;
                    break;
                case 2:
                    _Lowlight = _color;
                    break;
                case 3:
                    _Highlight = _color;
                    break;
                case 4:
                    _Muted = _color;
                    break;
                case 5:
                    _Secondary = _color;
                    break;
            }
            PreviewControl.Update_ColorOnPreview(Index, await Generate_BandTheme());

        }
        /// <summary>
        /// This updates the colors of the circles in the listbox
        /// </summary>
        /// <param name="_color"></param>
        public void Apply_Color(Color _color)
        {
            SolidColorBrush color = new SolidColorBrush(_color);
            int index = ColorTypeSelector.SelectedIndex;
            switch (index)
            {
                case 0:
                    Base.Background = color;
                    break;
                case 1:
                    HighContrast.Background = color;
                    break;
                case 2:
                    Lowlight.Background = color;
                    break;
                case 3:
                    Highlight.Background = color;
                    break;
                case 4:
                    Muted.Background = color;
                    break;
                case 5:
                    Secondary.Background = color;
                    break;
            }
        }

        private async void _R_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            TextBox tb = ((TextBox)sender);
            string Number = Regex.Replace(tb.Text, "[^0-9]+", string.Empty);
            if (Number.Length == 0)
                Number = "0";
            Color c = await Get_Color();

            byte _byte;

            bool parsed = byte.TryParse(Number, out _byte);
            if (parsed)
            {
                c.A = 255;
                c.R = _byte;
            } else
            {
                c.A = 255;
                c.R = 255;
            }

            await Update_Color(c);
            Update_Hex(c);
            Apply_Color(c);

        }

        private async void _G_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            TextBox tb = ((TextBox)sender);
            string Number = Regex.Replace(tb.Text, "[^0-9]+", string.Empty);
            if (Number.Length == 0)
                Number = "0";

            Color c = await Get_Color();

            byte _byte;
            bool parsed = byte.TryParse(Number, out _byte);
            
            if (parsed)
            {    
                c.A = 255;
                c.G = _byte;
            }
            else
            {
                c.A = 255;
                c.G = 255;
            }

            await Update_Color(c);

            Update_Hex(c);
            Apply_Color(c);

        }

        private  async void _B_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            TextBox tb = ((TextBox)sender);
            string Number = Regex.Replace(tb.Text, "[^0-9]+", string.Empty);
            if (Number.Length == 0)
                Number = "0";

            Color c = await Get_Color();

            byte _byte;
            bool parsed = byte.TryParse(Number, out _byte);
            if (parsed)
            {
                c.A = 255;
                c.B = _byte;
            }
            else
            {
                c.A = 255;
                c.B = 255;
            }

            await Update_Color(c);


            Update_Hex(c);
            Apply_Color(c);
        }

        public void Update_Hex (Color c)
        {
            _Hex.Text = c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //await Core.BandHandler.Set_Theme(await Generate_BandTheme());


            App._CustomBandThemes[App._SelectedBandTheme].Theme = await Generate_BandTheme();

            if (App._SelectedBandTheme != 0)
            {
                //var conv = XmlManager.Create.
                

                if (App._BandThemes != null && App._BandThemes.Descendants().Count() > 0)
                {

                    App._BandThemes = await XmlManager.Create.Update_XDocument(App._BandThemes, App._SelectedBandTheme.ToString(),
                        App._CustomBandThemes[App._SelectedBandTheme].Profile.ToString(),
                            _Base.ToString(),
                            _HighContrast.ToString(),
                            _Highlight.ToString(),
                            _Lowlight.ToString(),
                            _Muted.ToString(),
                            _Secondary.ToString());

                }

                else if (App._BandThemes == null || App._BandThemes.Descendants().Count() == 0)
                {
                    try
                    {
                        XElement newElement = await XmlManager.Create.Create_XElement(
                            App._SelectedBandTheme.ToString(),
                            App._CustomBandThemes[App._SelectedBandTheme].Profile.ToString(),
                            _Base.ToString(), 
                            _HighContrast.ToString(),
                            _Highlight.ToString(),
                            _Lowlight.ToString(),
                            _Muted.ToString(),
                            _Secondary.ToString()
                            );

                        App._BandThemes = new XDocument(new XElement("theme"));
                        App._BandThemes.Element("theme").Add(newElement);
                        //string ret = await XmlManager.Create.Create_New_Xml(items.ToArray());
                        //App._BandThemes = XDocument.Parse(ret);

                    } catch (Exception exxx)
                    {
                        MessageDialog msg = new MessageDialog(exxx.Message + Environment.NewLine + exxx.InnerException + Environment.NewLine);
                        await msg.ShowAsync();
                    }



                }
                await XmlManager.Manager.SaveFile(App._BandThemes, XmlManager.Files.XmlFiles.BandThemes);
            }








            this.Frame.GoBack();
        }

        private async void _Hex_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            Color _Color;
            int _Length = ((TextBox)sender).Text.Length;
            try
            {
                if (_Length == 6)
                {
                    _Color = new SolidColorBrush(ColorHelper.FromArgb(255,
                       byte.Parse(((TextBox)sender).Text.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                       byte.Parse(((TextBox)sender).Text.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                       byte.Parse(((TextBox)sender).Text.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)

                       )).Color;
                    await Update_Color(_Color);
                    Apply_Color(_Color);

                    _R.Text = _Color.R.ToString();
                    _G.Text = _Color.G.ToString();
                    _B.Text = _Color.B.ToString();
                }
                else if (_Length == 8)
                {
                    _Color = new SolidColorBrush(ColorHelper.FromArgb(255,
                        byte.Parse(((TextBox)sender).Text.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                        byte.Parse(((TextBox)sender).Text.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                        byte.Parse(((TextBox)sender).Text.Substring(6, 2), System.Globalization.NumberStyles.HexNumber)

                        )).Color;
                    await Update_Color(_Color);
                    Apply_Color(_Color);

                    _R.Text = _Color.R.ToString();
                    _G.Text = _Color.G.ToString();
                    _B.Text = _Color.B.ToString();
                }
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.Write("<ERROR> " + err + " </ERROR>");
            }
        }

        private void AppBarButton_Colors_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ColorPalettePage));
            if(_Hex.Text.ToString().Contains("#"))
            {
                App.HexWatcher.Color = _Hex.Text.ToString();
            }
            else
            {
                App.HexWatcher.Color = "#" + _Hex.Text.ToString();
            }
            PassedHex = true;
        }

        private void R_Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SetNewColor(e.NewValue.ToString(), null, null);
        }

        private void G_Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SetNewColor(null, e.NewValue.ToString(), null);
        }

        private void B_Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SetNewColor(null, null, e.NewValue.ToString());
        }


        private async void SetNewColor(string r, string g, string b)
        {
            Color c = await Get_Color();

            try
            {
                if (r != null) { c.R = byte.Parse(r.ToString()); _R.Text = r.ToString(); }
                if (g != null) { c.G = byte.Parse(g.ToString()); _G.Text = g.ToString(); }
                if (b != null) { c.B = byte.Parse(b.ToString()); _B.Text = b.ToString(); }


                await Update_Color(c);
                Update_Hex(c);
                Apply_Color(c);
            }
            catch { }


        }


    }
}
