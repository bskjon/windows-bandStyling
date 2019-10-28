using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;

namespace XmlManager
{


    public class Create
    {

        public static async Task<XElement> Create_XElement(string id, string name, string Base, string HighContrast, string Highlight, string Lowlight, string Muted, string Secondary)
        {
            XElement element = new XElement("item", new XAttribute("Id", id),
                new XElement("Name", name),
                new XElement("Base", Base),
                new XElement("HighContrast", HighContrast),
                new XElement("Highlight", Highlight),
                new XElement("Lowlight", Lowlight),
                new XElement("Muted", Muted),
                new XElement("Secondary", Secondary)

                );

            return element;
        }

        public static async Task<XDocument> Update_XDocument(XDocument Document, string id, string name, string Base, string HighContrast, string Highlight, string Lowlight, string Muted, string Secondary)
        {
            var list = from XElement el in Document.Descendants("item")
                       where (string)el.Attribute("Id").Value == id
                       select el;

            if (list == null) {
                ResourceLoader rl = new ResourceLoader();
                MessageDialog msg = new MessageDialog(rl.GetString("MessageDialog_ErrorXElementContetn1"), rl.GetString("MessageDialog_ErrorOopsTitle")); await msg.ShowAsync(); return Document; }

            try
            {
                if (list.Count() > 1) {
                    ResourceLoader rl = new ResourceLoader();

                    MessageDialog msg = new MessageDialog(rl.GetString("MessageDialog_ErrorXElementContetn2"), rl.GetString("MessageDialog_ErrorOopsTitle")); await msg.ShowAsync(); return Document;
                }

                if (list.Count() == 1)
                {
                    XElement item = (XElement)list.FirstOrDefault();
                    item.Element("Name").ReplaceNodes(name);
                    item.Element("Base").ReplaceNodes(Base);
                    item.Element("HighContrast").ReplaceNodes(HighContrast);
                    item.Element("Highlight").ReplaceNodes(Highlight);
                    item.Element("Lowlight").ReplaceNodes(Lowlight);
                    item.Element("Muted").ReplaceNodes(Muted);
                    item.Element("Secondary").ReplaceNodes(Secondary);
                }
                else if (list.Count() == 0)
                {
                    XElement newElement = await Create_XElement(id, name, Base, HighContrast, Highlight, Lowlight, Muted, Secondary);
                    Document.Element("theme").Add(newElement);
                }

            }
            catch (Exception exx)
            {
                System.Diagnostics.Debug.Write(exx);
            }

            return Document;
        }

        public static async Task<XDocument> RemoveEntry_XDocument(XDocument Document, string id)
        {
            var list = from XElement el in Document.Descendants("item")
                       where (string)el.Attribute("Id").Value == id
                       select el;

            if (list == null) { MessageDialog msg = new MessageDialog("Kunne ikke slette tematet..", "Oops.."); await msg.ShowAsync(); return Document; }

            try
            {
                if (list.Count() > 1) { MessageDialog msg = new MessageDialog("Fler verdier inneholder samme id... Data vil tømmes..", "Oops.."); await msg.ShowAsync(); return Document; }

                if (list.Count() == 1)
                {
                    XElement item = (XElement)list.FirstOrDefault();
                    item.Remove();
                }

            }
            catch (Exception ex)
            {

            }


            /// Reset Id for item
            /// 

            for (int i = 0; i < Document.Descendants("item").Count(); i++)
            {
                XElement el = (XElement)Document.Descendants("item").ElementAt(i);
                System.Diagnostics.Debug.Write(el);
                el.SetAttributeValue("Id", i + 1);
                System.Diagnostics.Debug.Write(el);
            }

            System.Diagnostics.Debug.Write(Document);

            return Document;

        }





    }
}
