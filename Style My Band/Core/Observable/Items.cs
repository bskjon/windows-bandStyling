using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Observable
{
    public class Items : INotifyPropertyChanged
    {
        private string _lineOne; //Normally Sign Letter
        private string _lineTwo; //Normally Sign Frame Color
        private string _lineThree; //Normally Sign Background Color
        private string _lineFour; //Normally Region
        private string _lineFive; //Normally Rest of Title
        private string _lineSix; //Normally Description
        private string _lineSeven; //Normally Link
        private string _lineEight;
        private string _lineNine; //Normally DateTime
        private string _lineTen; //Normally Unique ID


        public string LineOne
        {
            get { return _lineOne; }
            set
            {
                if (value != _lineOne)
                {
                    _lineOne = value;
                    NotifyPropertyChanged("LineOne");
                }
            }
        }

        public string LineTwo
        {
            get { return _lineTwo; }
            set
            {
                if (value != _lineTwo)
                {
                    _lineTwo = value;
                    NotifyPropertyChanged("LineTwo");
                }
            }
        }

        public string LineThree
        {
            get { return _lineThree; }
            set
            {
                if (value != _lineThree)
                {
                    _lineThree = value;
                    NotifyPropertyChanged("LineThree");
                }
            }
        }

        public string LineFour
        {
            get { return _lineFour; }
            set
            {
                if (value != _lineFour)
                {
                    _lineFour = value;
                    NotifyPropertyChanged("LineFour");
                }
            }
        }

        public string LineFive
        {
            get { return _lineFive; }
            set
            {
                if (value != _lineFive)
                {
                    _lineFive = value;
                    NotifyPropertyChanged("LineFive");
                }
            }
        }

        public string LineSix
        {
            get { return _lineSix; }
            set
            {
                if (value != _lineSix)
                {
                    _lineSix = value;
                    NotifyPropertyChanged("LineSix");
                }
            }
        }

        public string LineSeven
        {
            get { return _lineSeven; }
            set
            {
                if (value != _lineSeven)
                {
                    _lineSeven = value;
                    NotifyPropertyChanged("LineSeven");
                }
            }
        }

        public string LineEight
        {
            get { return _lineEight; }
            set
            {
                if (value != _lineEight)
                {
                    _lineEight = value;
                    NotifyPropertyChanged("LineEight");
                }
            }
        }

        public string LineNine
        {
            get { return _lineNine; }
            set
            {
                if (value != _lineNine)
                {
                    _lineNine = value;
                    NotifyPropertyChanged("LineNine");
                }
            }
        }

        public string LineTen
        {
            get { return _lineTen; }
            set
            {
                if (value != _lineTen)
                {
                    _lineTen = value;
                    NotifyPropertyChanged("LineTen");
                }
            }
        }






        /// <summary>
        /// No touchy!!!
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class ImageItems : INotifyPropertyChanged
    {
        private string _lineOne; //Normally Sign Letter
        private string _lineTwo; //Normally Sign Frame Color
        private string _lineThree; //Normally Sign Background Color
        private string _lineFour; //Normally Region
        private string _lineFive; //Normally Rest of Title
        private string _lineSix; //Normally Description
        private string _lineSeven; //Normally Link
        private string _lineEight;
        private string _lineNine; //Normally DateTime
        private string _lineTen; //Normally Unique ID


        public string LineOne
        {
            get { return _lineOne; }
            set
            {
                if (value != _lineOne)
                {
                    _lineOne = value;
                    NotifyPropertyChanged("LineOne");
                }
            }
        }

        public string LineTwo
        {
            get { return _lineTwo; }
            set
            {
                if (value != _lineTwo)
                {
                    _lineTwo = value;
                    NotifyPropertyChanged("LineTwo");
                }
            }
        }

        public string LineThree
        {
            get { return _lineThree; }
            set
            {
                if (value != _lineThree)
                {
                    _lineThree = value;
                    NotifyPropertyChanged("LineThree");
                }
            }
        }

        public string LineFour
        {
            get { return _lineFour; }
            set
            {
                if (value != _lineFour)
                {
                    _lineFour = value;
                    NotifyPropertyChanged("LineFour");
                }
            }
        }

        public string LineFive
        {
            get { return _lineFive; }
            set
            {
                if (value != _lineFive)
                {
                    _lineFive = value;
                    NotifyPropertyChanged("LineFive");
                }
            }
        }

        public string LineSix
        {
            get { return _lineSix; }
            set
            {
                if (value != _lineSix)
                {
                    _lineSix = value;
                    NotifyPropertyChanged("LineSix");
                }
            }
        }

        public string LineSeven
        {
            get { return _lineSeven; }
            set
            {
                if (value != _lineSeven)
                {
                    _lineSeven = value;
                    NotifyPropertyChanged("LineSeven");
                }
            }
        }

        public string LineEight
        {
            get { return _lineEight; }
            set
            {
                if (value != _lineEight)
                {
                    _lineEight = value;
                    NotifyPropertyChanged("LineEight");
                }
            }
        }

        public string LineNine
        {
            get { return _lineNine; }
            set
            {
                if (value != _lineNine)
                {
                    _lineNine = value;
                    NotifyPropertyChanged("LineNine");
                }
            }
        }

        public string LineTen
        {
            get { return _lineTen; }
            set
            {
                if (value != _lineTen)
                {
                    _lineTen = value;
                    NotifyPropertyChanged("LineTen");
                }
            }
        }






        /// <summary>
        /// No touchy!!!
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

}
