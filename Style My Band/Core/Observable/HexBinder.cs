using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Observable
{
    public class HexBinder
    {
        private string _color;

        public string Color
        {
            get { return _color; }
            set
            {
                _color = value;
                if (ValueChanged != null)
                    ValueChanged(value);
            }
        }

        public event ValueChangedEventHandler ValueChanged;

    }

    public delegate void ValueChangedEventHandler(string value);

}
