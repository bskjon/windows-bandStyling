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
using Core;
using Microsoft.Band;
using System.Threading.Tasks;
using XmlManager;
using Windows.UI.Popups;
using System.Xml.Linq;
using Core.Observable;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.UI;
using Windows.UI.Xaml.Media.Animation;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Style_My_Band
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            DataContext = App.ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.OverlayApp.ValueChanged += OverlayApp_ValueChanged;
            await Load_BandData();
        }

        #region Slide Out StoryBoard

        Storyboard slideOut;
        private void SlideOut()
        {
            slideOut = new Storyboard();

            DoubleAnimation slideY = new DoubleAnimation();
            slideY.Duration = TimeSpan.FromMilliseconds(250);
            slideY.From = 0;
            slideY.To = this.ActualHeight;

            Storyboard.SetTarget(slideOut, transform_Onemoment);
            Storyboard.SetTargetProperty(slideY, "TranslateY");

            slideOut.Children.Clear();
            slideOut.Children.Add(slideY);

            slideOut.Completed += SlideOut_Completed;

            slideOut.Begin();
        }

        private void SlideOut_Completed(object sender, object e)
        {
            ListView_BandUpdate.IsEnabled = true;
            // hide status bar - do this only once (mobile device only)
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var i = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ShowAsync();
            }
            Root.IsHitTestVisible = true;
            OneMoment.Visibility = Visibility.Collapsed;
            CustomProgressControl.State = -1;
            slideOut.Stop();

        }

        #endregion

        private async void OverlayApp_ValueChanged(bool value)
        {
            if (value == true)
            {
                ListView_BandUpdate.IsEnabled = false;
                // hide status bar - do this only once (mobile device only)
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    var i = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
                }
                Root.IsHitTestVisible = false;
                OneMoment.Visibility = Visibility.Visible;

                if (CustomProgressControl.State == 0 || CustomProgressControl.State == 1 || CustomProgressControl.State == -1)
                {
                    CustomProgressControl.State = 2;
                } //else { CustomProgressControl.State = 0; CustomProgressControl.State = 1; }


            }
            else if (value == false)
            {
                if (CustomProgressControl.State == 2)
                {
                    CustomProgressControl.State = 1;
                    await CustomProgressControl.AnimationFinished();
                    await Task.Delay(100);
                    SlideOut();
                }
                else if (CustomProgressControl.State == -1)
                {
                    ListView_BandUpdate.IsEnabled = true;
                    // hide status bar - do this only once (mobile device only)
                    if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                    {
                        var i = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ShowAsync();
                    }
                    Root.IsHitTestVisible = true;
                    OneMoment.Visibility = Visibility.Collapsed;
                    if (slideOut != null)
                        slideOut.Stop();

                    CustomProgressControl.State = 1;

                }

            }
            
        }




        public async void Disable_BandFeatures()
        {
            if (System.Diagnostics.Debugger.IsAttached == true)
            {
                /// Run debugg code here
                /// 
                if (App._CustomBandThemes == null || App._CustomBandThemes.Count == 0)
                {
                    App._CustomBandThemes = new List<Classes.BandTheme_Profiles>();
                    App._CurrentBandTheme = await Demo.DemoTheme();
                    App._CustomBandThemes.Add(new Classes.BandTheme_Profiles()
                    {
                        Id = 0,
                        Profile = Localization.Get_Text(Localization.Tag.Global_, "DefaultProfileName"),
                        Theme = await Demo.DemoTheme()
                    });
                    App.ViewModel.Items.Add(new Core.Observable.Items()
                    {
                        LineOne = Localization.Get_Text(Localization.Tag.Global_, "DefaultProfileName"),
                        LineTwo = "",
                        LineThree = "",
                        LineFour = "",
                        LineFive = "",
                        LineSix = "",
                        LineSeven = "",
                        LineEight = "",
                        LineNine = "#" + await Parse._ColorToHEX(App._CurrentBandTheme.Base.ToColor()),
                        LineTen = "#" + await Parse._ColorToHEX(App._CurrentBandTheme.HighContrast.ToColor())
                    });
                    App._SelectedBandTheme = 0;
                    Profile_Selector.SelectedIndex = 0;
                }
                
            }
            else
            {
                /// If this is a end user
                /// 

                PreviewBackground.IsHitTestVisible = false;
                PreviewColor.IsHitTestVisible = false;
                AppBarButtonUpload.IsEnabled = false;
                AppBarButtonNewProfile.IsEnabled = false;
                ProfileEdit_AppBarButton.IsEnabled = false;
                ProfileImport_AppBarButton.IsEnabled = false;
                ReloadBandData_AppBarButton.IsEnabled = false;
                if (Profile_Selector.Items.Count == 0)
                {
                    ProfileExport_AppBarButton.IsEnabled = false;
                }
                App.OverlayApp.Overlay = false;
                return;
            }
        }

        ///SingleBinding.ReloadMainPage

        public async Task Load_BandData()
        {
            IBandInfo Band_Found = await BandHandler.Get_Band();

            if (Band_Found == null)
            {
                Disable_BandFeatures();
                return;
            }



            #region Setup BandData

            if (App._CustomBandThemes == null || App._CustomBandThemes.Count == 0)
            {
                OneMomentDetails.Text = Localization.Get_Text(Localization.Tag.Band_, "GetDetails1");
                App.OverlayApp.Overlay = true;


                #region Gets and sets the meTile
                App._MeImageTile = await Core.BandHandler.Get_Image(); /// Gets the band metile form the band
                BandPreview.SetImage(App._MeImageTile); /// Sets the image of the previewControl
                PreviewImage.Source = App._MeImageTile; /// Sets the image of the tapp to change control
                #endregion
                #region Gets and sets the Theme
                BandTheme tempBandTheme = await Core.BandHandler.Get_Theme(); /// Gets the current theme on the band
                BandPreview.SetTheme(tempBandTheme); /// Sets the preview theme as the current
                App._CurrentBandTheme = tempBandTheme; /// Sets the retrieved teme as the current
                App._SelectedBandTheme = 0; /// Sets the selected as 0 since the control now contains a theme. If this is not set the applicaiton will not work correctly. Possible issue of .20 -> .25 issue. Caused rewrite of this task
                App._CustomBandThemes = await ReturnProfiles(); /// Gets the custom profiles created by user from storage.
                #endregion

                App.BandGeneration = await BandHandler.Get_BandGeneration(); /// Gets the current band generation possible values -1, 1, 2

                App.OverlayApp.Overlay = false;
            }
            else
            {
                App._CurrentBandTheme = App._CustomBandThemes[App._SelectedBandTheme].Theme;
                BandPreview.SetImage(App._MeImageTile);
                BandPreview.SetTheme(App._CurrentBandTheme);

                PreviewImage.Source = App._MeImageTile;

                Profile_Selector.SelectedIndex = App._SelectedBandTheme;

                App.ViewModel.Items.ElementAt(App._SelectedBandTheme).LineNine = "#" + await Parse._ColorToHEX(App._CurrentBandTheme.Base.ToColor());
                App.ViewModel.Items.ElementAt(App._SelectedBandTheme).LineTen = "#" + await Parse._ColorToHEX(App._CurrentBandTheme.HighContrast.ToColor());
            }

            FillBase.Fill = new SolidColorBrush(App._CurrentBandTheme.Base.ToColor());
            FillHighContrast.Fill = new SolidColorBrush(App._CurrentBandTheme.HighContrast.ToColor());

            #endregion

            

        }


        public async Task<List<Classes.BandTheme_Profiles>> ReturnProfiles()
        {
            List<Core.Classes.BandTheme_Profiles> BandProfiles = new List<Classes.BandTheme_Profiles>();

            BandProfiles.Add(new Classes.BandTheme_Profiles()
            {
                Id = 0,
                Profile = Localization.Get_Text(Localization.Tag.Global_, "DefaultProfileName"),
                Theme = App._CurrentBandTheme
            });

            App.ViewModel.Items.Add(new Core.Observable.Items()
            {
                LineOne = BandProfiles.ElementAt(0).Profile,
                LineTwo = "",
                LineThree = "",
                LineFour = "",
                LineFive = "",
                LineSix = "",
                LineSeven = "",
                LineEight = "",
                LineNine = "#" + await Parse._ColorToHEX(BandProfiles.ElementAt(0).Theme.Base.ToColor()),
                LineTen = "#" + await Parse._ColorToHEX(BandProfiles.ElementAt(0).Theme.HighContrast.ToColor())
            });

            Profile_Selector.SelectedIndex = 0;

            App._BandThemes = await Manager.ToXDocument(await Manager.ReadFile(Files.XmlFiles.BandThemes));
            Manager._BandTheme_Items[] themeStrings = await XmlManager.Parser.Get_BandThemeFromXml(App._BandThemes);

            if (themeStrings != null && themeStrings.Length != 0)
            {
                for (int i = 0; i < themeStrings.Length; i++)
                {
                    BandProfiles.Add(new Classes.BandTheme_Profiles()
                    {
                        Id = Core.Parse._int(themeStrings[i]._Id.ToString()),
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

                    App.ViewModel.Items.Add(new Core.Observable.Items()
                    {
                        LineOne = themeStrings[i]._Name,
                        LineTwo = "",
                        LineThree = "",
                        LineFour = "",
                        LineFive = "",
                        LineSix = "",
                        LineSeven = "",
                        LineEight = "",
                        LineNine = themeStrings[i]._Base,
                        LineTen = themeStrings[i]._HighContrast
                    });
                }
            }



            return BandProfiles;
        }


        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SingleBinding.ResetThemePage = true;
            //CustomProgressControl.State = -1;
            this.Frame.Navigate(typeof(BandThemePage));
        }

        private async void Profile_Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ((ComboBox)sender).SelectedIndex;

            if (index == 0)
            {
                ProfileEdit_AppBarButton.IsEnabled = false;
                ProfileDelete_AppBarButton.IsEnabled = false;
            }
            else if (index == -1)
            {
                return;
            }
            else
            {
                ProfileEdit_AppBarButton.IsEnabled = true;
                ProfileDelete_AppBarButton.IsEnabled = true;
            }

            /*if (index == 0)
            {
            
                /// Normal Execution here, due to current theme on band
                /// 
                App._SelectedBandTheme = index;
            }
            else
            {
                //int ProfileIndex = index - 1; /// This is due to index will always be 1 or more. Array will always start with 0. int = index - 1 is required
                App._SelectedBandTheme = index; //ProfileIndex;
                //_SelectedTheme = App._CustomBandThemes[ProfileIndex].Theme;
                ///Specified execution here, due to timed or manual theme here.
            }*/
            //FillBase.Fill = new SolidColorBrush(_SelectedTheme.Base.ToColor());
            //FillHighContrast.Fill = new SolidColorBrush(_SelectedTheme.HighContrast.ToColor());

            App._SelectedBandTheme = index;

            if (App.OverlayApp.Overlay == false && App._CustomBandThemes != null)
            {

                App._CurrentBandTheme = App._CustomBandThemes[App._SelectedBandTheme].Theme;
                BandPreview.SetImage(App._MeImageTile);
                BandPreview.SetTheme(App._CurrentBandTheme);

                FillBase.Fill = new SolidColorBrush(App._CurrentBandTheme.Base.ToColor());
                FillHighContrast.Fill = new SolidColorBrush(App._CurrentBandTheme.HighContrast.ToColor());

            }

        }

        private async void AddNewProfile_Click(object sender, RoutedEventArgs e)
        {
            //Profile_Selector.Items.Add(NewProfileName.Text);
            if (System.Diagnostics.Debugger.IsAttached)
            {
                MessageDialog msg1 = new MessageDialog("Status? :" + Windows.ApplicationModel.Package.Current.IsDevelopmentMode.ToString(), "Debugger is Attached");
                await msg1.ShowAsync();
            }


            App.ViewModel.Items.Add(new Items()
            {
                LineOne = NewProfileName.Text,
                LineTwo = "",
                LineThree = "",
                LineFour = "",
                LineFive = "",
                LineSix = "",
                LineSeven = "",
                LineEight = "",
                LineNine = "",
                LineTen = ""
            });

            //var it = ViewModel.Items.Where(X => X.LineOne == NewProfileName.Text).Last();
            int newId = App.ViewModel.Items.IndexOf(App.ViewModel.Items.Where(X => X.LineOne == NewProfileName.Text).Last());

            //int newId = Profile_Selector.Items.IndexOf(NewProfileName.Text);
            try
            {
                App._CustomBandThemes.Add(new Classes.BandTheme_Profiles()
                {

                    Id = newId,
                    Profile = NewProfileName.Text,
                    Theme = App._CustomBandThemes.FirstOrDefault().Theme

                });

                NewProfileFlyout.Hide();
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " " + ex.InnerException, ex.Source);
                msg.ShowAsync();
            }
        }

        private async void ProfileDelete_AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            int Selected = App._SelectedBandTheme;

            if (Selected != 0)
            {
                if (App._BandThemes != null && App._BandThemes.Descendants().Count() > 0)
                {
                    try
                    {
                        App._SelectedBandTheme -= 1;
                        Profile_Selector.SelectedIndex = App._SelectedBandTheme;
                        Profile_Selector.SelectionChanged -= Profile_Selector_SelectionChanged; //Prevent Selection Event.



                        //Profile_Selector.SelectedIndex = App._SelectedBandTheme;

                        var item = App.ViewModel.Items.ElementAt(Selected);
                        //App.ViewModel.Items.Clear();
                        App.ViewModel.Items.Remove(item);




                        App._BandThemes = await XmlManager.Create.RemoveEntry_XDocument(App._BandThemes, Selected.ToString());

                        await XmlManager.Manager.SaveFile(App._BandThemes, Files.XmlFiles.BandThemes);





                        Profile_Selector.SelectionChanged += Profile_Selector_SelectionChanged;

                    }
                    catch (Exception ex)
                    {
                        if (Windows.ApplicationModel.Package.Current.IsDevelopmentMode)
                        {
                            MessageDialog msg = new MessageDialog(ex.Message.ToString() + Environment.NewLine + ex.InnerException.ToString() + Environment.NewLine + ex.Source.ToString(), "Error...");
                            msg.ShowAsync();
                        }

                    }



                }
            }
        }

        private async void UpdateNewProfile_Click(object sender, RoutedEventArgs e)
        {
            ///UpdateProfileName.Text /// Text in flyout
            ///
            #region Content from BandThemePage.Xaml.Cs

            var it = App._CustomBandThemes.ElementAt(App._SelectedBandTheme);

            if (it != null && it.Profile != "" && UpdateProfileName.Text != "")
            {
                string id = it.Id.ToString();
                string name = UpdateProfileName.Text;
                string Base = it.Theme.Base.ToColor().ToString();
                string HighContrast = it.Theme.HighContrast.ToColor().ToString();
                string Highlight = it.Theme.Highlight.ToColor().ToString();
                string Lowlight = it.Theme.Lowlight.ToColor().ToString();
                string Muted = it.Theme.Muted.ToColor().ToString();
                string Secondary = it.Theme.SecondaryText.ToColor().ToString();

                App._BandThemes = await XmlManager.Create.Update_XDocument(App._BandThemes,
                    id,
                    name,
                    Base,
                    HighContrast,
                    Highlight,
                    Lowlight,
                    Muted,
                    Secondary);

                await XmlManager.Manager.SaveFile(App._BandThemes, XmlManager.Files.XmlFiles.BandThemes);

                App.ViewModel.Items.ElementAt(App._SelectedBandTheme).LineOne = UpdateProfileName.Text;
            }

            UpdateProfileFlyout.Hide();

            #endregion
        }

        private void UpdateProfileFlyout_Opening(object sender, object e)
        {
            string ProfileName = App.ViewModel.Items.ElementAt(App._SelectedBandTheme).LineOne;

            if (ProfileName != null || ProfileName != "")
            {
                UpdateProfileName.Text = ProfileName;
            }
        }

        private async void ProfileExport_AppBarButton_Click(object sender, RoutedEventArgs e)
        {

            if (Profile_Selector.Items.Count == 1)
            {
                MessageDialog msg = new MessageDialog(Localization.Get_Text(Localization.Tag.MessageDialog_, "ExportContentNo"), Localization.Get_Text(Localization.Tag.MessageDialog_, "ExportTitleNo"));
                await msg.ShowAsync();
                return;
            }

            ThemeFileManager.Export.ExportTheme(App._BandThemes);

        }

        private void ProfileImport_AppBarButton_Click(object sender, RoutedEventArgs e)
        {

            ThemeFileManager.Import.ImportTheme();

        }

        private void Border_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //CustomProgressControl.State = -1;
            this.Frame.Navigate(typeof(BandImagePage));
        }

        private async void ListViewItem_UpdateMeTile_Tapped(object sender, TappedRoutedEventArgs e)
        {

            if (App._MeImageTile != null)
            {
                await UpdateBand(2);
            }
            else
            {
                MessageDialog msg = new MessageDialog(Localization.Get_Text(Localization.Tag.MessageDialog_, "BandUpdateErrorContent1"), Localization.Get_Text(Localization.Tag.MessageDialog_, "BandUpdateErrorOops"));
                await msg.ShowAsync();
            }
        }

        private async void ListViewItem_UpdateTheme_Tapped(object sender, TappedRoutedEventArgs e)
        {

            if (Profile_Selector.Items.Count == App._CustomBandThemes.Count)
            {
                await UpdateBand(1);
            }
            else
            {
                MessageDialog msg = new MessageDialog(Localization.Get_Text(Localization.Tag.MessageDialog_, "BandUpdateErrorContent1"), Localization.Get_Text(Localization.Tag.MessageDialog_, "BandUpdateErrorOops"));
                await msg.ShowAsync();
            }
        }

        private async void ListViewItem_UpdateAll_Tapped(object sender, TappedRoutedEventArgs e)
        {

            if (Profile_Selector.Items.Count == App._CustomBandThemes.Count)
            {
                await UpdateBand(0);
            }
            else
            {
                MessageDialog msg = new MessageDialog(Localization.Get_Text(Localization.Tag.MessageDialog_, "BandUpdateErrorContent1"), Localization.Get_Text(Localization.Tag.MessageDialog_, "BandUpdateErrorOops"));
                await msg.ShowAsync();
            }
        }

        /// <summary>
        /// This calls the update code for the Microsoft Band.
        /// </summary>
        /// <param name="val"> int 0 = all, int 1 = theme, int 2 = image </param>
        /// <returns></returns>
        public async Task UpdateBand(int val)
        {
            if (val == -1 || val > 2) { System.Diagnostics.Debug.Write("WUT! Max Val = 2 !"); return; }
            Flyout_BandUpdate.Hide();
            ListView_BandUpdate.SelectedIndex = -1;


            using (var session = new ExtendedExecutionSession())
            {
                session.Reason = ExtendedExecutionReason.Unspecified;
                session.Description = Localization.Get_Text(Localization.Tag.Global_, "ExtendedExecutionSession_Description01");
                session.Revoked += Session_Revoked;
                var result = await session.RequestExtensionAsync();
                if (result == ExtendedExecutionResult.Denied)
                {
                    MessageDialog msg = new MessageDialog(Localization.Get_Text(Localization.Tag.Global_, "ExtendedExecutionSession_DeniedText"), Localization.Get_Text(Localization.Tag.Global_, "ExtendedExecutionSession_DeniedTitle"));
                    await msg.ShowAsync();
                }
                OneMomentSubTitle.Visibility = Visibility.Collapsed;

                switch (val)
                {
                    case 0:
                        OneMomentDetails.Text = Localization.Get_Text(Localization.Tag.Band_, "DefaultUpdateText1");
                        App.OverlayApp.Overlay = true;
                        await Core.BandHandler.Set_Theme(App._CustomBandThemes[App._SelectedBandTheme].Theme);
                        OneMomentTitle.Text = Localization.Get_Text(Localization.Tag.Band_, "DefaultWaitText1");
                        OneMomentSubTitle.Text = Localization.Get_Text(Localization.Tag.Band_, "OneMomentSubTitle");
                        OneMomentSubTitle.Visibility = Visibility.Visible;
                        OneMomentDetails.Text = Localization.Get_Text(Localization.Tag.Band_, "UpdatingBandImage");
                        await Core.BandHandler.Set_Image(App._MeImageTile);
                        App.OverlayApp.Overlay = false;
                        break;
                    case 1:
                        OneMomentDetails.Text = Localization.Get_Text(Localization.Tag.Band_, "DefaultUpdateText1");
                        OneMomentSubTitle.Text = "";
                        App.OverlayApp.Overlay = true;
                        await Core.BandHandler.Set_Theme(App._CustomBandThemes[App._SelectedBandTheme].Theme);
                        App.OverlayApp.Overlay = false;
                        break;

                    case 2:
                        OneMomentTitle.Text = Localization.Get_Text(Localization.Tag.Band_, "DefaultWaitText1");
                        OneMomentSubTitle.Text = Localization.Get_Text(Localization.Tag.Band_, "OneMomentSubTitle");
                        OneMomentSubTitle.Visibility = Visibility.Visible;
                        OneMomentDetails.Text = Localization.Get_Text(Localization.Tag.Band_, "UpdatingBandImage");

                        App.OverlayApp.Overlay = true;
                        await Core.BandHandler.Set_Image(App._MeImageTile);
                        App.OverlayApp.Overlay = false;
                        await Task.Delay(1000);
                        OneMomentTitle.Text = Localization.Get_Text(Localization.Tag.Band_, "OneMomentDefault1");
                        break;
                }
            }


        }

        private void Session_Revoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            //throw new NotImplementedException();
        }

        private async void AppBarButton_Reload_Click(object sender, RoutedEventArgs e)
        {
            ReloadBandData_AppBarButton.IsEnabled = false;
            App._CustomBandThemes.Clear();
            App.ViewModel.Items.Clear();
            await Load_BandData();
            ReloadBandData_AppBarButton.IsEnabled = true;
        }

        private async void MainPage_ApplicationVersionAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var V = Windows.ApplicationModel.Package.Current.Id.Version;
            string Version = V.Major + "." + V.Minor + "." + V.Build + "." + V.Revision;
            MessageDialog msg = new MessageDialog(Version, Localization.Get_Text(Localization.Tag.MessageDialog_, "VersionTitle"));
            await msg.ShowAsync();

        }

    }

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
