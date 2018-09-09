using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.Win32;
using System.IO;
using System.Threading;

namespace WpfApp2.Model
{
    public class DataModel
    {
        private string tb1;
        private string tb2;
        private string tb3;

        public DataModel()
        {
            tb1 = "";
            tb2 = "";
            tb3 = "";
        }

        public string Text1
        {
            get { return tb1; }
            set { tb1 = value; }
        }

        public string Text2
        {
            get { return tb2; }
            set { tb2 = value; }
        }

        public string Text3
        {
            get { return tb3; }
            set { tb3 = value; }
        }   
    }
}
