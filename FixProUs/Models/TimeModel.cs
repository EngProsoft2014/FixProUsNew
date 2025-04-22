using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixProUs.Models
{
    public class TimeModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string? Time { get; set; }

        string _BackgroundColor = "#ffffff";
        public string BackgroundColor
        {
            get
            {
                return _BackgroundColor;
            }
            set
            {
                _BackgroundColor = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("BackgroundColor"));
                }
            }
        }

        string _TextColor = "#0098BC";
        public string TextColor
        {
            get
            {
                return _TextColor;
            }
            set
            {
                _TextColor = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TextColor"));
                }
            }
        }

        public bool IsChecked { get; set; }
    }
}
