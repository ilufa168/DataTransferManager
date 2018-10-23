using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devart.Common;
using Devart.Data;
using Devart.Data.Oracle;
using System.Data;
using System.Reflection;
using Dapper;

namespace IlufaDataTransfer
{
    class DataTransferManager
    {
        //private string h_jual_db = "H_JUAL_TEST";
        //private string d_jual_db = "D_JUAL_TEST";
        //private string pay_jual_db = "PAY_JUAL_TEST";
        private string h_jual_db = "H_JUAL";
        private string d_jual_db = "D_JUAL";
        private string pay_jual_db = "PAY_JUAL";

        private SettingsManager AppSettings;
        //private List<IlufaSaleTransaction> transaction_list = new List<IlufaSaleTransaction>();
        public Dictionary<String, IlufaSaleTransaction> transaction_list = new Dictionary<string, IlufaSaleTransaction>();

        public DataTransferManager(SettingsManager sm)
        {
            SetSettings(sm);
        }

        public bool CheckExistance(string nobukti)
        {
            bool result = false;
            List<SalesItems> records;

            try
            {
                OracleManager om = new OracleManager(AppSettings.remote_oracle_IP);
                IDbConnection conn = om.connect();
                records = conn.Query<SalesItems>("Select * from " + h_jual_db + " where nobukti='" + nobukti + "'").ToList();
                conn.Close();
            }
            catch (Exception)
            {

                throw;
            }

            if (records.Count == 0)
                result = false;
            else
                result = true;

            return result;

        }

        public void SetSettings(SettingsManager sm)
        {
            AppSettings = sm;
        }

        public List<string> GetStoreSalesByDate(DateTime from_date)
        {
            List<string> results = new List<string>();

            //Build a connection to the Host computer
            OracleManager om = new OracleManager(AppSettings.oracle_IP);
            //the query 

            IDbConnection conn = om.connect();
            //var data = conn.Query(sql).ToList()

            transaction_list = new Dictionary<string, IlufaSaleTransaction>();
            conn.Query<IlufaSaleTransaction, SalesItems, IlufaSaleTransaction>(@"
                 SELECT h.NOBUKTI,
                        h.TGL,
                        h.JAM,
                        h.NMKAS,
                        h.TOTAL,
                        h.JNSBYR,
                        h.TVCH,
                        h.TBYR,
                        h.TDBT,
                        h.TCCD,
                        h.KEMBALI,
                        h.NOCARD,
                        h.PEMILIK,
                        h.NMCARD,
                        h.KET,
                        h.LOKASI,
                        h.NM_UPDATE,
                        h.TG_UPDATE,
                        h.TGL_INSERT,
                        h.TPIUTANG,
                        h.TOTHER,
                        h.MEMBERID,
                        d.NOBUKTI,
                        d.TGL,
                        d.KDBRG,
                        d.JAM,
                        d.KDGOL,
                        d.KDSUPP,
                        d.HBELI,
                        d.HJUAL,
                        d.QTY,
                        d.DISC,
                        d.DISCRP,
                        d.NM_UPDATE,
                        d.TG_UPDATE,
                        d.TGL_INSERT,
                        d.SPID,
                        d.LOKASI
                FROM H_JUAL h
                        INNER JOIN D_JUAL d
                        ON h.NOBUKTI = d.NOBUKTI     
                WHERE h.TGL >= TO_DATE('" + from_date.ToString("MM/dd/yyyy") + @"', 'MM/DD/YYYY')
                        AND h.LOKASI = '" + AppSettings.location + "'", (h, d) =>
            {
                IlufaSaleTransaction ifs;
                if (!transaction_list.TryGetValue(h.NOBUKTI, out ifs))
                {
                    transaction_list.Add(h.NOBUKTI, ifs = h);
                }
                if (ifs.item_records == null)
                    ifs.item_records = new List<SalesItems>();
                ifs.item_records.Add(d);
                return ifs;
            }, splitOn: "NOBUKTI").AsQueryable();

            //Now go out and get a list of payments for each item
            foreach (KeyValuePair<string, IlufaSaleTransaction> entry in transaction_list)
            {
                List<Payments> payments = conn.Query<Payments>("Select * from pay_jual where nobukti='" +entry.Key + "'").ToList();
                entry.Value.transaction_payments = payments;
            }
            if (transaction_list.Count > 0)
                results.Add("Primary transactions retrieved.");
            else
                results.Add("ERROR:Something went wrong with retrieving the results");

            conn.Close();

            return results;
        }

        public void InsertHjual(IlufaSaleTransaction the_header)
        {
            //Record record = new Record();

            //
            //Build the insert based on the properties if the object
            //
            string sql = BuildInsertFromObject(the_header,h_jual_db);
            
            try
            {
                OracleManager om = new OracleManager(AppSettings.remote_oracle_IP);
                IDbConnection conn = om.connect();
                conn.Execute(sql);
                //records = conn.Query<HJUAL>("Select * from " + h_jual_db + " where nobukti='" + nobukti + "'").ToList();
                conn.Close();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void InsertDjual(SalesItems si)
        {
            string sql = BuildInsertFromObject(si, d_jual_db);

            try
            {
                OracleManager om = new OracleManager(AppSettings.remote_oracle_IP);
                IDbConnection conn = om.connect();
                conn.Execute(sql);
                //records = conn.Query<HJUAL>("Select * from " + h_jual_db + " where nobukti='" + nobukti + "'").ToList();
                conn.Close();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void InsertPayjual(Payments pi)
        {
            string sql = BuildInsertFromObject(pi, pay_jual_db);

            try
            {
                OracleManager om = new OracleManager(AppSettings.remote_oracle_IP);
                IDbConnection conn = om.connect();
                conn.Execute(sql);
                //records = conn.Query<HJUAL>("Select * from " + h_jual_db + " where nobukti='" + nobukti + "'").ToList();
                conn.Close();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private string BuildInsertFromObject(Object obj, string table_name)
        {
            //string sql = "";
            Type tst = obj.GetType();
            PropertyInfo[] properties = obj.GetType().GetProperties();

            string sql_ins = "insert into " +table_name + " ("; //now dynamically add the columns
            string sql_val = " values (";
            foreach (PropertyInfo property in properties)
            {

                string nm = property.Name;
                Type tp = property.PropertyType;
                sql_ins += nm + ",";
                switch (tp.Name)
                {
                    case "String":                      
                        if (property.GetValue(obj) == null)
                            sql_val += "'',";
                        else
                            sql_val += "'" + property.GetValue(obj).ToString() + "',";
                        break;
                    case "DateTime":
                        if (property.GetValue(obj) == null)
                            sql_val += "'',";
                        else
                        {
                            DateTime dt = DateTime.Parse(property.GetValue(obj).ToString());
                            sql_val += " to_date('" + dt.ToString("MM/dd/yyyy") + "','MM/dd/YYYY'),";
                        }
                        break;
                    case "Double":
                        if (property.GetValue(obj) == null)
                            sql_val += "'',";
                        else
                            sql_val += property.GetValue(obj).ToString() + ",";
                        break;
                    default:
                        sql_val += "'',";
                        break;
                }
                //property.SetValue(record, value);
            }
            //Now strip the comma and add in a ")" for the insert part and vals
            sql_ins = sql_ins.Substring(0, sql_ins.Length - 1) + ")";
            sql_val = sql_val.Substring(0, sql_val.Length - 1) + ")";
            sql_ins += sql_val;
            return sql_ins;
        }
    }

    class OracleManager
    {
        //public static IEnumerable<T> Query<T>(this IDbConnection cnn, string sql, object param = null, SqlTransaction transaction = null, bool buffered = true)

        private string HostIP = "192.168.1.9";
        public List<string> error_list = new List<string>();
        public OracleManager(string HIP)
        {
            this.HostIP = HIP;
        }

        public OracleConnection connect()
        {
            OracleConnection oc = null;
            
            //Build the key
            OracleConnectionStringBuilder oraCSB = new OracleConnectionStringBuilder();
            oraCSB.Direct = true;
            oraCSB.Server = HostIP;
            oraCSB.Port = 1521;
            oraCSB.UserId = "erp";
            oraCSB.Password = "erp";
            oraCSB.ConnectionTimeout = 30;
            
            oc = new OracleConnection(oraCSB.ConnectionString);


            return oc;
        }
    }
}
