using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Band;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Band.Personalization;

namespace Core
{
    public class BandHandler
    {
        public static IBandInfo[] PairedBands;
        public static IBandClient BandClient = null;

        public static async Task<IBandInfo> Get_Band()
        {
            PairedBands = await BandClientManager.Instance.GetBandsAsync();

            if (PairedBands.Length == 1)
            {
                return PairedBands[0];
            }
            else if (PairedBands.Length > 1)
            {
                //return a selector
                return PairedBands[0];
            }
            else
            {
                MessageDialog msg = new MessageDialog(Localization.Get_Text(Localization.Tag.MessageDialog_, "BandMissingContent"), Localization.Get_Text(Localization.Tag.MessageDialog_, "BandMissingTitle"));
                await msg.ShowAsync();
                return null;
            }

        }

        public static async Task<IBandClient> Connect_Band(IBandInfo Band)
        {
            try
            {
                return await BandClientManager.Instance.ConnectAsync(Band);
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(Localization.Get_Text(Localization.Tag.MessageDialog_, "BandConnectErrorContent"), Localization.Get_Text(Localization.Tag.MessageDialog_, "BandConnectErrorTitle"));
                await msg.ShowAsync();
                
            }
            return null;
        }

        
        public static async Task Set_Image(object path)
        {
            BandClient = await Connect_Band(await Get_Band());

            if (BandClient == null)
                return;

            using (BandClient)
            {
                if (path is WriteableBitmap)
                {
                    WriteableBitmap wb = (WriteableBitmap)path;
                    BandImage bi = wb.ToBandImage();
                    await BandClient.PersonalizationManager.SetMeTileImageAsync(bi);

                    //set
                }
                else if (path is string)
                {
                    WriteableBitmap writeableBitmap = await Loader.LoadImage((string)path);
                    BandImage bandImage = writeableBitmap.ToBandImage();
                    await BandClient.PersonalizationManager.SetMeTileImageAsync(bandImage);
                }

            }

        }
        /// <summary>
        /// Returns MeTile from Microsoft Band
        /// </summary>
        /// <returns></returns>
        public static async Task<WriteableBitmap> Get_Image()
        {
            BandClient = await Connect_Band(await Get_Band());

            if (BandClient == null)
                return new WriteableBitmap(1, 1);

            using (BandClient)
            {
                BandImage bandImage = await BandClient.PersonalizationManager.GetMeTileImageAsync();
                return bandImage.ToWriteableBitmap();
            }

            
        }

        public static async Task Set_Theme(BandTheme theme)
        {
            BandClient = await Connect_Band(await Get_Band());

            if (BandClient == null)
                return;

            using (BandClient)
            {
                await BandClient.PersonalizationManager.SetThemeAsync(theme);
            }
        }
        /// <summary>
        /// Returns the theme from Microsoft Band
        /// </summary>
        /// <returns></returns>
        public static async Task<BandTheme> Get_Theme()
        {
            BandClient = await Connect_Band(await Get_Band());

            if (BandClient == null)
                return new BandTheme();

            using (BandClient)
            {

                BandTheme bandTheme = await BandClient.PersonalizationManager.GetThemeAsync();
                return bandTheme;
            }
        }

        public static async Task<int> Get_BandGeneration()
        {
            int hwVersion;

            BandClient = await Connect_Band(await Get_Band());

            if (BandClient == null)
                return -1;

            using (BandClient)
            {
                bool res = int.TryParse(await BandClient.GetHardwareVersionAsync(), out hwVersion);

                if (res)
                {
                    if (hwVersion <= 19)
                    {
                        return 1;
                    }
                    else if (hwVersion >= 20)
                    {
                        return 2;
                    }
                }
            }

            

            return 0;

        }



    }

    

}
