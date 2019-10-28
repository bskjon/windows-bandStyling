using Core;
using Core.Observable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class ColorPalettePage : Page
    {



        public ColorPalettePage()
        {
            this.InitializeComponent();
            this.DataContext = ViewColorModel;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await Set_Colors();
        }

        public async Task Set_Colors()
        {
            ViewColorModel.Items.Clear();
            for (int i = 0; i < _Colors.Length; i++)
            {
                string val = _Colors[i];

                Color c = await Parse._ColorFromHEX("#" + val);


                ViewColorModel.Items.Add(new Core.Observable.Items()
                {
                    LineOne = "#" + val
                });


            }

            Color hexa = (Color)this.Resources["SystemAccentColor"];
            string hex = "#" + await Core.Parse._ColorToHEX(hexa);

            viewColorModel.Items.Add(new Items()
            {
                LineOne = hex
            });

        }

        #region  Colors
        public string[] _Colors =
        {
            //Row 1
            "ffb900",
            "e74856",
            "e74856",
            "0099bc",
            "7a7574",
            "767676",
            //Row 2
            "ff8c00",
            "e81123",
            "0063b1",
            "2d7d9a",
            "5d5a58",
            "4c4a48",
            //Row 3
            "f7630c",
            "ea005e",
            "8e8cd8",
            "00b7c3",
            "68768a",
            "69797e",
            //Row 4
            "ca5010",
            "c30052",
            "6b69d6",
            "038387",
            "515c6b",
            "4a5459",
            //Row 5
            "da3b01",
            "e3008c",
            "8764b8",
            "00b294",
            "567c73",
            "647c64",
            //Row 6
            "ef6950",
            "bf0077",
            "744da9",
            "018574",
            "486860",
            "525e54",
            //Row 7
            "d13438",
            "c239b3",
            "b146c2",
            "00cc6a",
            "498205",
            "847545",
            //Row 8
            "ff4343",
            "9a0089",
            "881798",
            "10893e",
            "107c10",
            "7e735f"
        };
        #endregion

        private void Colors_ItemClick(object sender, ItemClickEventArgs e)
        {
            var ClickedItem = (Items)e.ClickedItem;
            App.HexWatcher.Color = ClickedItem.LineOne.ToString();
            this.Frame.GoBack();
        }

        private static List viewColorModel = null;
        public static List ViewColorModel
        {
            get
            {
                if (viewColorModel == null)
                    viewColorModel = new List();
                return viewColorModel;
            }
        }

    }
}
