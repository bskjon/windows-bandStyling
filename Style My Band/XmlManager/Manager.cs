using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media;

namespace XmlManager
{
    public class Manager
    {


        public class _BandTheme_Items
        {
            public int _Id { get; set; }
            public string _Name { get; set; }
            public string _Base { get; set; }
            public string _HighContrast { get; set; }
            public string _Lowlight { get; set; }
            public string _Highlight { get; set; }
            public string _Muted { get; set; }
            public string _Secondary { get; set; }
        }





        public static StorageFolder localfolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        /// <summary>
        /// Parsing only xml files
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static async Task<string> ReadFile(Files.XmlFiles filename)
        {
            StorageFile file;
            string Data = "";
            try
            {
                file = await localfolder.GetFileAsync(filename.ToString() + ".xml");
                Data = await FileIO.ReadTextAsync(file);


                /*file = await localfolder.GetFileAsync(FileName + "." + Type.ToString());

                using (Stream readFile = await file.OpenStreamForReadAsync())
                {
                    Data = readFile.;
                }*/

            }
            catch
            {

            }
            return Data;
        }



        public static async Task SaveFile(object obj, Files.XmlFiles filename)
        {
            if (obj is XDocument)
            {
                XDocument doc = (XDocument)obj;
                try
                {
                    StorageFile file = await localfolder.CreateFileAsync(filename.ToString() + ".xml", CreationCollisionOption.ReplaceExisting);
                    using (Stream stream = await file.OpenStreamForWriteAsync())
                    {
                        doc.Save(stream);
                    }
                }
                catch (Exception e)
                {
                    ResourceLoader rl = new ResourceLoader();
                    MessageDialog msg = new MessageDialog(rl.GetString("MessageDialog_ErrorXDocumentContent1"), rl.GetString("MessageDialog_ErrorOopsTitle"));
                    await msg.ShowAsync();
                }
            }
        }





        public static async Task<XDocument> ToXDocument(string Value)
        {
            if (Value == "") { return null; }
            XDocument Document = XDocument.Parse(Value);


            return Document;
        }



    }
}
