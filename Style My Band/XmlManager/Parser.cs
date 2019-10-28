using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XmlManager
{
    public class Parser
    {

        public static async Task<Manager._BandTheme_Items[]> Get_BandThemeFromXml(XDocument Document)
        {
            
            
            if (Document != null)
            {
                Manager._BandTheme_Items[] items = null;

                items = (from item in Document.Descendants("item")
                         select new Manager._BandTheme_Items
                         {
                             _Id = int.Parse(item.Attribute("Id") != null ? item.Attribute("Id").Value : "-1"),
                             _Name = item.Element("Name") != null ? item.Element("Name").Value : "",
                             _Base = item.Element("Base") != null ? item.Element("Base").Value : "",
                             _HighContrast = item.Element("HighContrast") != null ? item.Element("HighContrast").Value : "",
                             _Lowlight = item.Element("Lowlight") != null ? item.Element("Lowlight").Value : "",
                             _Highlight = item.Element("Highlight") != null ? item.Element("Highlight").Value : "",
                             _Muted = item.Element("Muted") != null ? item.Element("Muted").Value : "",
                             _Secondary = item.Element("Secondary") != null ? item.Element("Secondary").Value : ""
                         }).ToArray();


                return items;
            }

            return null;

        }

  
    }
}
