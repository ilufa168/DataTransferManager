using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IlufaDataTransfer.Models
{
    class PosSale
    {
        public int Pos_Sale_Id { get; set; }
        public DateTime Pos_Sale_Start_date { get; set; }
        public DateTime Pos_Sale_End_Date { get; set; }
        public String Pos_Sale_Source_Type { get; set; }
        public string Pos_Sale_Type { get; set; }
        public string Pos_Sale_Parameters { get; set; }

    }
}
