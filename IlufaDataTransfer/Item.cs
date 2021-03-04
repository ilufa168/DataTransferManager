using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IlufaDataTransfer
{
    class Item
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_TYPE { get; set; }
        public string ITEM_NAME { get; set; }
        public string CATEGORY { get; set; }
        public string VENDOR_CODE { get; set; }
        public string UOM { get; set; }
        //skip unit cost
        public double UNIT_RETAIL { get; set; }
    }
}
