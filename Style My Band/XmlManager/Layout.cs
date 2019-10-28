using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlManager
{
    public class Layout
    {

        public class ContainerItem
        {
            public string container { get; set; }
            public SingleItem items { get; set; }
        }


        public class SingleItem
        {
            public string tag { get; set; }
            public string value { get; set; }
        }

        public static async Task<string> Return_String(string tag, string value)
        {
            return "<" + tag + ">" + value + "</" + tag + ">";
        }


    }
}
