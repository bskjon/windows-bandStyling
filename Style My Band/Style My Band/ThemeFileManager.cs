using Core;
using Microsoft.Band;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Popups;
using XmlManager;

namespace Style_My_Band
{
    public class ThemeFileManager
    {


        public static class Export
        {

            public static async void ExportTheme(XDocument document)
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("Xml", new List<string>() { ".xml" });
                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = "Band Theme";

                StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                    CachedFileManager.DeferUpdates(file);
                    // write to file
                    //await FileIO.WriteTextAsync(file, file.Name);

                    // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                    // Completing updates may require Windows to ask for user input.
                    try
                    {
                        using (Stream stream = await file.OpenStreamForWriteAsync())
                        {
                            document.Save(stream);
                        }


                        FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                        if (status == FileUpdateStatus.Complete)
                        {
                            MessageDialog msg = new MessageDialog(Localization.Get_Text(Localization.Tag.MessageDialog_, "ExportContent"), Localization.Get_Text(Localization.Tag.MessageDialog_, "ExportTitle"));
                            await msg.ShowAsync();
                            System.Diagnostics.Debug.Write("File " + file.Name + " was saved.");
                        }
                        else
                        {
                            System.Diagnostics.Debug.Write("File " + file.Name + " couldn't be saved.");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                        System.Diagnostics.Debug.Write(ex);
                    }





                }
            }


        }

        public static class Import
        {
            public static async void ImportTheme()
            {
                try
                {
                    FileOpenPicker fop = new FileOpenPicker();
                    fop.ViewMode = PickerViewMode.List;
                    fop.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    fop.FileTypeFilter.Add(".xml");

                    StorageFile sfile = await fop.PickSingleFileAsync();
                    if (sfile != null)
                    {
                        string Data = await FileIO.ReadTextAsync(sfile);

                        #region Importer
                        if (Data.Length != 0)
                        {
                            List<Core.Classes.BandTheme_Profiles> BandProfiles = new List<Classes.BandTheme_Profiles>();
                            Manager._BandTheme_Items[] themeStrings = await XmlManager.Parser.Get_BandThemeFromXml(await Manager.ToXDocument(Data));


                            /// CREATES NEW XDOCUMENT IF IT IS NULL OR EMPTY
                            if (App._BandThemes == null || App._BandThemes.Descendants().Count() == 0)
                            {
                                App._BandThemes = new XDocument(new XElement("theme"));
                            }

                            if (themeStrings != null && themeStrings.Length != 0)
                            {
                                for (int i = 0; i < themeStrings.Length; i++)
                                {
                                    App.ViewModel.Items.Add(new Core.Observable.Items()
                                    {
                                        LineOne = themeStrings[i]._Name,
                                        LineNine = themeStrings[i]._Base,
                                        LineTen = themeStrings[i]._HighContrast
                                    });

                                    int newId = App.ViewModel.Items.IndexOf(App.ViewModel.Items.Where(X => X.LineOne == themeStrings[i]._Name).Last());

                                    BandProfiles.Add(new Classes.BandTheme_Profiles()
                                    {

                                        Id = newId,
                                        Profile = themeStrings[i]._Name,
                                        Theme = new BandTheme()
                                        {
                                            Base = await Parse._BandColor(await Parse._ColorFromHEX(themeStrings[i]._Base)),
                                            HighContrast = await Parse._BandColor(await Parse._ColorFromHEX(themeStrings[i]._HighContrast)),
                                            Highlight = await Parse._BandColor(await Parse._ColorFromHEX(themeStrings[i]._Highlight)),
                                            Lowlight = await Parse._BandColor(await Parse._ColorFromHEX(themeStrings[i]._Lowlight)),
                                            Muted = await Parse._BandColor(await Parse._ColorFromHEX(themeStrings[i]._Muted)),
                                            SecondaryText = await Parse._BandColor(await Parse._ColorFromHEX(themeStrings[i]._Secondary)),
                                        }


                                    });

                                    App._CustomBandThemes.Add(new Classes.BandTheme_Profiles()
                                    {

                                        Id = newId,
                                        Profile = themeStrings[i]._Name,
                                        Theme = new BandTheme()
                                        {
                                            Base = await Parse._BandColor(await Parse._ColorFromHEX(themeStrings[i]._Base)),
                                            HighContrast = await Parse._BandColor(await Parse._ColorFromHEX(themeStrings[i]._HighContrast)),
                                            Highlight = await Parse._BandColor(await Parse._ColorFromHEX(themeStrings[i]._Highlight)),
                                            Lowlight = await Parse._BandColor(await Parse._ColorFromHEX(themeStrings[i]._Lowlight)),
                                            Muted = await Parse._BandColor(await Parse._ColorFromHEX(themeStrings[i]._Muted)),
                                            SecondaryText = await Parse._BandColor(await Parse._ColorFromHEX(themeStrings[i]._Secondary)),
                                        }

                                    });



                                    XElement newElement = await Create.Create_XElement(
                                        newId.ToString(),
                                        themeStrings[i]._Name,
                                        themeStrings[i]._Base,
                                        themeStrings[i]._HighContrast,
                                        themeStrings[i]._Highlight,
                                        themeStrings[i]._Lowlight,
                                        themeStrings[i]._Muted,
                                        themeStrings[i]._Secondary);


                                    App._BandThemes.Element("theme").Add(newElement);

                                }




                                ///This is needed to save the profiles
                                await XmlManager.Manager.SaveFile(App._BandThemes, XmlManager.Files.XmlFiles.BandThemes);

                                MessageDialog msg = new MessageDialog(Localization.Get_Text(Localization.Tag.MessageDialog_, "ImportContent"), Localization.Get_Text(Localization.Tag.MessageDialog_, "ImportTitle"));
                                await msg.ShowAsync();

                            }
                        }
                        #endregion

                    }
                }
                catch (Exception ex)
                {
                    MessageDialog msg = new MessageDialog(ex.Message + Environment.NewLine + ex.InnerException, "Error..");
                    msg.ShowAsync();
                }
            }
        }
    }


}
