using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeBarProcAudit.Model
{
    public class Item : INotifyPropertyChanged
    {
        private string _inv;
        public string Inv
        {
            get { return _inv; }
            set { _inv = value; OnPropertyChanged(); }
        }

        private string _info1;
        public string Info1
        {
            get { return _info1; }
            set { _info1 = value; OnPropertyChanged(); }
        }

        private string _info2;
        public string Info2
        {
            get { return _info2; }
            set { _info2 = value; OnPropertyChanged(); }
        }

        //private string _info3;
        //public string Info3
        //{
        //    get { return _info3; }
        //    set { _info3 = value; OnPropertyChanged(); }
        //}

        //private string _info4;
        //public string Info4
        //{
        //    get { return _info4; }
        //    set { _info4 = value; OnPropertyChanged(); }
        //}

        //private string _info5;
        //public string Info5
        //{
        //    get { return _info5; }
        //    set { _info5 = value; OnPropertyChanged(); }
        //}

        //private string _info6;
        //public string Info6
        //{
        //    get { return _info6; }
        //    set { _info6 = value; OnPropertyChanged(); }
        //}

        //private string _info7;
        //public string Info7
        //{
        //    get { return _info7; }
        //    set { _info7 = value; OnPropertyChanged(); }
        //}

        //private string _info8;
        //public string Info8
        //{
        //    get { return _info8; }
        //    set { _info8 = value; OnPropertyChanged(); }
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
