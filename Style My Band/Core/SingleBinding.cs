using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SingleBinding
    {
        public static bool ResetThemePage { get; set; }
    }

    public class OneMoment
    {
        private bool _overlay;

        public bool Overlay
        {
            get { return _overlay; }
            set
            {
                _overlay = value;
                if (ValueChanged != null)
                    ValueChanged(value);
            }
        }

        public event ValueChangedEventHandler ValueChanged;
    }


    public class ResetPage
    {
        private bool _reset;
        public bool Reset
        {
            get { return _reset; }
            set
            {
                _reset = value;
                if (ValueChanged != null)
                    ValueChanged(value);
            }
        }
        public event ValueChangedEventHandler ValueChanged;
    }

    public class Cancel_Button
    {
        private bool _cancel;

        public bool Cancel
        {
            get { return _cancel; }
            set
            {
                _cancel = value;
                if (ValueChanged != null)
                    ValueChanged(value);
            }
        }
        public event ValueChangedEventHandler ValueChanged;
    }



    public delegate void ValueChangedEventHandler(bool value);

}
