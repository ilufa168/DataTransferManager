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
using System.Configuration;
using System.Collections;
using IlufaDataTransfer.Models;
using System.IO;

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
        private string item_db = "ITEM";
        private string category_db = "CODES";

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

        public List<PosSale> GetCurrentStoreSales()
        {
            string sql = string.Format("select * from pos_sale where pos_sale_start_date <= SYSDATE and pos_sale_end_date >= SYSDATE");
            OracleManager om = new OracleManager(AppSettings.remote_oracle_IP);
            IDbConnection conn = om.connect();
            var results = conn.Query<PosSale>(sql);
            return results.ToList();
        }

        public bool ClearSales()
        {
            OracleManager om = new OracleManager(AppSettings.oracle_IP);
            string sql = "truncate table pos_sale";
            IDbConnection conn = om.connect();

            var result = conn.Execute(sql);

            return true;
        }

        public bool InsertSale(PosSale sale)
        {
            //string sql = BuildInsertFromObject(sale, "pos_sale_bak");
            string sql = String.Format(@"insert into pos_sale(Pos_Sale_Id, Pos_Sale_Start_date, Pos_Sale_End_Date, Pos_Sale_Source_Type, Pos_Sale_Type, Pos_Sale_Parameters) 
                                  values({0},{1},{2},'{3}','{4}',:p1)", sale.Pos_Sale_Id,
                                  "to_date('" + sale.Pos_Sale_Start_date.ToString("dd/MM/yyyy") + "', 'DD/MM/YYYY')",
                                  "to_date('" + sale.Pos_Sale_End_Date.ToString("dd/MM/yyyy") + "', 'DD/MM/YYYY')", 
                                  sale.Pos_Sale_Source_Type,sale.Pos_Sale_Type);

            //Build a connection to the Host computer
            OracleManager om = new OracleManager(AppSettings.oracle_IP);
            //the query 

            OracleConnection conn = om.connect();
            conn.Open();
            if (sql.Length > 0 && sale.Pos_Sale_Parameters.Length > 0)
            {
                if (conn.State.ToString().Equals("Open"))
                {
                    //                    byte[] byte_parms = System.Text.Encoding.Unicode.GetBytes(text);
                    byte[] bytes = System.Text.Encoding.ASCII.GetBytes(sale.Pos_Sale_Parameters);
                    MemoryStream stream = new MemoryStream(bytes);
                    BinaryReader r = new BinaryReader(stream);
                    //Transfer data to server
                    int streamLength = (int)bytes.Length;
                    OracleLob myLob = new OracleLob(conn, OracleDbType.Clob);
                    myLob.Write(r.ReadBytes(streamLength), 0, streamLength);

                    //Perform INSERT
                    OracleCommand myCommand = new OracleCommand(sql, conn);
                    OracleParameter myParam = myCommand.Parameters.Add("p1", OracleDbType.Clob);
                    //                    OracleParameter myParam = myCommand.Parameters.Add("parms", text);
                    myParam.OracleValue = myLob;

                    try
                    {
                        Console.WriteLine(myCommand.ExecuteNonQuery() + " rows affected.");
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return true;
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

        public int CheckItemExists(Item i)
        {
            
            //var records;
            int count = 0;
            try
            {
                OracleManager om = new OracleManager(AppSettings.remote_oracle_IP);
                IDbConnection conn = om.connect();
                var v = conn.Query<int>("Select count(*) from " + item_db + " where ITEM_CODE ='" + i.ITEM_CODE + "'").ToArray();

                if (v.Length > 0)
                    count = v[0];

                conn.Close();
            }
            catch (Exception e)
            {
                throw(e);
            }

           
            //if (records.Count == 0)
            //    result = false;
            //else
            //    result = true;

            return count;
            
        }
        
        private bool UpdateAnItem(Item i)
        {
            OracleManager om = new OracleManager(AppSettings.remote_oracle_IP);
            IDbConnection conn = om.connect();
            string sql = String.Format(@"UPDATE {0} set ITEM_TYPE='{1}', 
                                                        ITEM_NAME = '{2}',
                                                        CATEGORY ='{3}',
                                                        VENDOR_CODE ='{4}',
                                                        UOM = '{5}',
                                                        UNIT_RETAIL ={6}
                                        WHERE ITEM_CODE = '{7}'"
                , item_db, i.ITEM_TYPE, i.ITEM_NAME, i.CATEGORY, i.VENDOR_CODE, i.UOM, i.UNIT_RETAIL,i.ITEM_CODE);
            try
            {
                
                int rows = conn.Execute(sql);

                conn.Close();
                if (rows == 1)
                    return true;
                return false;
            }

            catch(Exception e)
            {
                throw (e);
            }           
        }
        private bool InsertAnItem(Item i)
        {
            OracleManager om = new OracleManager(AppSettings.remote_oracle_IP);
            IDbConnection conn = om.connect();
            string sql = String.Format(@"INSERT INTO {0} (ITEM_CODE, ITEM_TYPE, ITEM_NAME, CATEGORY, 
                                                          VENDOR_CODE,UOM, UNIT_RETAIL)
                                                 VALUES ('{1}','{2}','{3}','{4}','{5}','{6}',{7})"
                , item_db, i.ITEM_CODE, i.ITEM_TYPE, i.ITEM_NAME, i.CATEGORY, i.VENDOR_CODE, i.UOM, i.UNIT_RETAIL);
            try
            {

                int rows = conn.Execute(sql);

                conn.Close();
                if (rows == 1)
                    return true;
                return false;
            }

            catch (Exception e)
            {
                throw (e);
            }
            //return false;
        }

        public async Task<List<Category>> GetCategoryUpdates()
        {
            List<Category> result = new List<Category>();

            string sql = @"SELECT * FROM CODES WHERE KIND = 'ITEM_CATEGORY'";
            
            OracleManager om = new OracleManager(AppSettings.remote_oracle_IP);
            IDbConnection conn = om.connect();
            try
            {
                var r = await conn.QueryAsync<Category>(sql); //.ToList();
                result = r.ToList();
            }
            catch (Exception e)
            {

                throw (e);
            }
            return result;
        }

        public async Task<string> ProcessCategory(Category cat)
        {
            string result = "ERROR";
            if (await CheckCategoryExists(cat))
            {
                if (await UpdateACategory(cat))
                    result = "UPDATE";
                else
                    result = "ERROR";
            } else
            {
                if (await InsertACategory(cat))
                    result = "INSERT";
                else
                    result = "ERROR";
            }
            return result;
        }

        public async Task<bool> InsertACategory(Category cat)
        {
             OracleManager om = new OracleManager(AppSettings.remote_oracle_IP);
            IDbConnection conn = om.connect();
            string sql = String.Format(@"INSERT INTO {0} (KIND, CODE, DESCRIPTION, UPPER_KIND, UPPER_CODE)
                                                 VALUES ('{1}','{2}','{3}','{4}','{5}')"
                ,category_db, cat.KIND, cat.CODE, cat.DESCRIPTION, cat.UPPER_KIND, cat.UPPER_CODE);
            try
            {

                int rows = await conn.ExecuteAsync(sql);

                conn.Close();
                if (rows == 1)
                    return true;
                return false;
            }

            catch (Exception e)
            {
                throw (e);
            }
        }
        public async Task<bool> UpdateACategory(Category cat)
        {
            bool result = false;
            OracleManager om = new OracleManager(AppSettings.remote_oracle_IP);
            IDbConnection conn = om.connect();
            string sql = String.Format(@"UPDATE {0} set DESCRIPTION ='{1}', 
                                                        UPPER_KIND = '{2}',
                                                        UPPER_CODE  ='{3}'
                                        WHERE KIND ='ITEM_CATEGORY'
                                              AND CODE = '{4}'"
                , category_db, cat.DESCRIPTION,cat.UPPER_KIND, cat.UPPER_CODE, cat.CODE);
            try
            {

                int rows = await conn.ExecuteAsync(sql);

                conn.Close();
                if (rows == 1)
                    result = true;
            }

            catch (Exception e)
            {
                throw (e);
            }
            return result;
        }
        public async Task<bool> CheckCategoryExists(Category c)
        {
            bool result = false;
            //var records;
            try
            {
                OracleManager om = new OracleManager(AppSettings.remote_oracle_IP);
                IDbConnection conn = om.connect();
                var v = await conn.QueryAsync<int>(@"Select count(*) from " + category_db + 
                                                    " WHERE KIND = 'ITEM_CATEGORY' and CODE='" + c.CODE + "'");

                if (v.Count() > 0)
                {
                    if (v.ToArray()[0] > 0)
                        result = true;
                }
                conn.Close();
            }
            catch (Exception e)
            {
                throw (e);
            }

            return result;

        }
        public List<Item> GetItemUpdates(DateTime start)
        {
            List<Item> result = new List<Item>();
            List<string> item_list = new List<string>();
            //Get a timestamp for NOW, to save on success
            DateTime update_dt = DateTime.Now;
            string sql = string.Format(@"select distinct ITEM_CODE,update_date_time from ITEM_UPDATES where " +
                "                                               UPDATE_DATE_TIME >= TO_DATE('" 
                            + AppSettings.items_last_update.ToString("MM/dd/yyyy H:mm") + @"', 'MM/DD/YYYY HH24:MI')
                                                        ORDER BY UPDATE_DATE_TIME ASC");
            OracleManager om = new OracleManager(AppSettings.remote_oracle_IP);
            IDbConnection conn = om.connect();
            try
            {
                item_list = conn.Query<String>(sql).ToList();
            }
            catch (Exception e)
            {

                throw(e);
            }
            if (item_list.Count > 0)
            {
                //Now get the item data to update
                sql = @"SELECT ITEM_CODE,ITEM_TYPE,ITEM_NAME,CATEGORY,VENDOR_CODE,UOM,UNIT_RETAIL from ITEM where ITEM_CODE in ( " + GetCommaSeperatedCodes(item_list) + ")";
                result = conn.Query<Item>(sql).ToList();
            }
            //return results.ToList();

            return result;
        }


        public async Task<string> UpdateItem(Item i)
        {
            //string result = "ERROR";
            //check to see if insety or update
            await Task.Delay(1);
            if (CheckItemExists(i) == 1)
            {
                //update
                if (UpdateAnItem(i))
                    return "UPDATED";
                else
                    return "ERROR on UPDATE";
            }
            else //It do not exist so insert
            {
                if (InsertAnItem(i))
                    return "INSERTED";
                else
                    return "ERROR on INSERT";
            }

            //return result;
        }

        private string GetCommaSeperatedCodes(List<string> codes)
        {
            string result = "";
            foreach (string s in codes)
            {
                result += "'" + s + "',";
            }
            result = result.TrimEnd(',');

            return result;
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
                        {
                            //hack to make zeros appear 
                            var tmp = property.GetValue(obj);
                            string tmp2 = tmp.ToString();
                            sql_val += "'" + tmp2 + "',";
                        }
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
                    case "Int32":
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

        public OracleConnection connectAsync()
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
    public class Category
    {
        public string KIND { get; set; }
        public string CODE{ get; set; }
        public string DESCRIPTION { get; set; }
        public string UPPER_KIND { get; set; }
        public string UPPER_CODE { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public string UPDATE_BY { get; set; }
    }
}
