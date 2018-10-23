using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace IlufaDataTransfer
{
    class Payments
    {
        public String NOBUKTI { get; set; }
        public String JNSBYR { get; set; }
        public String NMCARD { get; set; }
        public String NOCARD { get; set; }
        public String PEMILIK { get; set; }
        public Double BAYAR { get; set; }
        public Double KEMBALI { get; set; }
        public DateTime TGL { get; set; }
        public String PAY_ID { get; set; }
        private string db_name = "PAY_JUAL_TEST";
    }

    class SalesItems
    {

        public String NOBUKTI { get; set; }
        public DateTime TGL { get; set; }
        public DateTime JAM { get; set; }
        public String KDBRG { get; set; }
        public String KDGOL { get; set; }
        public String KDSUPP { get; set; }
        public Double HBELI { get; set; }
        public Double HJUAL { get; set; }
        public Double QTY { get; set; }
        public Double DISC { get; set; }
        public Double DISCRP { get; set; }
        public String LOKASI { get; set; }
        public String NM_UPDATE { get; set; }
        public DateTime TG_UPDATE { get; set; }
        public DateTime TGL_INSERT { get; set; }
    }

    //[Table("H_JUAL_TEST")]
    class IlufaSaleTransaction
    {
        //[Key]
        public String NOBUKTI { get; set; }
        public DateTime TGL { get; set; }
        public DateTime JAM { get; set; }
        public String NMKAS { get; set; }
        public Double TOTAL { get; set; }
        public String JNSBYR { get; set; }
        public Double TVCH { get; set; }
        public Double TBYR { get; set; }
        public Double TDBT { get; set; }
        public Double TCCD { get; set; }
        public Double KEMBALI { get; set; }
        public String NOCARD { get; set; }
        public String PEMILIK { get; set; }
        public String NMCARD { get; set; }
        public String KET { get; set; }
        public String LOKASI { get; set; }
        public String NM_UPDATE { get; set; }
        public DateTime TG_UPDATE { get; set; }
        public DateTime TGL_INSERT { get; set; }
        public Double TPIUTANG { get; set; }
        public Double TOTHER { get; set; }
        public String MEMBERID { get; set; }

        //public HJUAL header_data;
        public List<SalesItems> item_records;
        public List<Payments> transaction_payments;

        public List<string> errors = new List<string>();
        //public LIST<PAY_JUAL> payment records;

        public IlufaSaleTransaction(string id)
        {
            this.NOBUKTI = id;
        }
        public IlufaSaleTransaction()
        {
            
            //this.NOBUKTI = id;
        }

        //Verify some basic data about this transaction to make sure we have 
        //all the records
        public bool ValidateTransaction(List<string> error_list)
        {
            bool result = true;

            return result;
        }

        public bool CheckToSeeIfAlreadyInDatabase(DataTransferManager dtm)
        {
            return dtm.CheckExistance(this.NOBUKTI);
        }

        public bool InsertHeaderRecord(DataTransferManager dtm)
        {
            bool result = true;

            try
            {
                dtm.InsertHjual(this);
            }
            catch (Exception e)
            {
                errors.Add("Something went wrong with the insert - See error message");
                errors.Add(e.ToString());
                return false;
            }
            return result;
        }

        public bool InsertItemRecords(DataTransferManager dtm)
        {
            bool result = true;
            foreach (SalesItems si in item_records)
            {
                try
                {
                    dtm.InsertDjual(si);
                }
                catch (Exception e)
                {
                    errors.Add("Something went wrong with the item insert - See error message");
                    errors.Add(e.ToString());
                    result = false;
                }
            }
            return result;
        }

        public bool InsertPaymentRecords(DataTransferManager dtm)
        {
            bool result = true;
            foreach (Payments pi in transaction_payments)
            {
                try
                {
                    dtm.InsertPayjual(pi);
                }
                catch (Exception e)
                {
                    errors.Add("Something went wrong with the item insert - See error message");
                    errors.Add(e.ToString());
                    result = false;
                }
            }
            return result;
        }

    }
}
