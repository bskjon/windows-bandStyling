using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace Core
{
    class Lookup
    {
    }

    public class Localization
    {
        public enum Tag
        {
            Global_,
            Band_,
            MessageDialog_,
        }

        public static string Get_Text(Tag tag, string value)
        {
            ResourceLoader rl = new ResourceLoader();
            return rl.GetString(tag + value);
        }


    }


}
