using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using WaitingBox;
using IlufaDataTransfer.Models;

namespace IlufaDataTransfer
{
    public partial class MainWindow : Form
    {
        SettingsManager AppSettings = new SettingsManager();
        DataTransferManager dtm;
        Boolean transfer_in_progress = false;
        bool _ENV_TEST = false;
        public MainWindow()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);

            InitializeComponent();
            
            AppSettings.LoadSettings();
            dtm = new DataTransferManager(AppSettings);
            Write_To_Message_Log("Application Started");
            UpdateLabels();
            
        }

        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            MessageBox.Show(e.ToString());
            Console.WriteLine("MyHandler caught : " + e.Message);
            Console.WriteLine("Runtime terminating: {0}", args.IsTerminating);
        }

        private void UpdateLabels()
        {
            Lbl_Location.Text = AppSettings.location;
            this.Refresh();
        }

        private void B_Settings_Click(object sender, EventArgs e)
        {
            SettingsWindow sw = new SettingsWindow();
            DialogResult dr = sw.ShowDialog();
            AppSettings.LoadSettings();
            UpdateLabels();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {


        }

        private void Write_To_Message_Log(string message)
        {
            Rtb_Activity_Log.AppendText("[INFO] " + message  + Environment.NewLine);
            Rtb_Activity_Log.SelectionStart = Rtb_Activity_Log.Text.Length;
            Rtb_Activity_Log.ScrollToCaret();
        }

        private void Write_Error_to_Activity_log(string error)
        {
            Rtb_Activity_Log.SelectionColor = Color.Red;
            Rtb_Activity_Log.AppendText("[ERROR]" + error + Environment.NewLine);
            Rtb_Activity_Log.SelectionColor = Color.Black;

        }

        private void B_Start_Transfer_To_Hq_Click(object sender, EventArgs e)
        {
            if (transfer_in_progress)
                return;

            DateTime begin_date = DTP_From_Date.Value;
            transfer_in_progress = true;

            begin_date = begin_date.Date;
            //MessageBox.Show(begin_date.Hour.ToString());
            Write_To_Message_Log("Retrieving sales since " + begin_date.ToShortDateString());
            Thread.Sleep(100);
            //this.dtm = new DataTransferManager(AppSettings);

            //First step get the stoore sales data 

            //Try to thread this suckah

            List<string> results = new List<string>();

            Thread backgroundThread = new Thread(
                new ThreadStart(() =>
                {
                     results = dtm.GetStoreSalesByDate(begin_date);
                }));
            backgroundThread.Start();
            //Update dialog box goes here
            ShowWaitingBox waiting = new ShowWaitingBox("Retrieving Transactions:", "This Can take awhile.");
            waiting.Start();
            int i = 1;
            while (backgroundThread.IsAlive)
            {
                if (i%100 == 0)
                    waiting.UpdateLabel("Still going[" + (i/100).ToString() + "]");
                Thread.Sleep(100);
                i++;
            }
            waiting.Stop();
            
            foreach (string msg in results)
            {
                Write_To_Message_Log(msg);
            }

            List<string> error_list = new List<string>();
            //
            //CONNECT TO VPN
            //
            VPNController VC = new VPNController(AppSettings);

            Write_To_Message_Log("Connecting to Jakarta to send transmission");
            //Check to make sure the connection is not active
            if (!VC.CheckActive())
            {
                Write_To_Message_Log("Opening VPN Connection to Jakarta");
                VC.ConnectJakarta();
            }
            if (!VC.CheckActive())
            {
                Write_Error_to_Activity_log("ERROR - Failed to connect to Jakarta - transfer aborting");
                return;
            }
            else
            {
                Write_To_Message_Log("VPN connection to Jakarta completed successfully");
            }

            int item_cnt = dtm.transaction_list.Count();
            int sent_count = 0;
            waiting = new ShowWaitingBox("Transmitting to Jakarta", "Transactions sent: [0/"+item_cnt.ToString()+"]");
            waiting.Start();
            foreach (KeyValuePair<string, IlufaSaleTransaction> entry in dtm.transaction_list)
            {
                waiting.UpdateLabel("Transactions sent: ["+sent_count.ToString() + "/" + item_cnt.ToString() + "]");
                sent_count++;
                //Write_To_Message_Log(entry.Key + "- " + entry.Value.item_records.Count.ToString() + " Items and " + entry.Value.transaction_payments.Count.ToString() + " payment record(s).");
                Write_To_Message_Log("Validating records for " + entry.Key);
                IlufaSaleTransaction current_transaction = entry.Value;
                if (!current_transaction.ValidateTransaction(error_list))
                {
                    //Handle errors
                }
                //Does this transaction exist already (possibly a problem)
                if (current_transaction.CheckToSeeIfAlreadyInDatabase(dtm))
                {
                    error_list.Add("Warning - NOBUKTI [" + entry.Key + "] is already in the Jakarta database.  Transaction skipped!!!");
                    continue;
                }
                Write_To_Message_Log("Transmitting transaction [" + entry.Key + "] to Jakarta.");
              bool result;
                result = current_transaction.InsertHeaderRecord(dtm);
                if (!result)
                {
                    error_list.Add("ERROR failed to insert NOBUKTI [" + entry.Key + "] - skipping the rest of the transaction");
                    foreach(string an_error in current_transaction.errors)
                    {
                        error_list.Add(an_error);
                    }
                    continue;
                }
                //Insert all the item records
                result = entry.Value.InsertItemRecords(dtm);
                if (!result)
                {
                    error_list.Add("ERROR failed to insert an item record for  [" + entry.Key + "] - skipping the rest of the transaction");
                    continue;
                }
                //Insert all the payment records
                result = entry.Value.InsertPaymentRecords(dtm);
                if (!result)
                {
                    error_list.Add("ERROR failed to insert a payment record for  [" + entry.Key + "] - skipping the rest of the transaction");
                    continue;
                }
                if (current_transaction.errors.Count == 0)
                {
                    Write_To_Message_Log("Transmission of transaction [" + entry.Key + "] Successful.");
                } else
                {
                    Write_Error_to_Activity_log("There were some issues with transaction [" + entry.Key + "]");
                    foreach (string an_error in current_transaction.errors)
                    {
                        error_list.Add(an_error);
                    }
                }
                //Thread.Sleep(50); //short pause to let the gui update
            }
            waiting.Stop();
            Write_To_Message_Log("Transmission complete.  Please check the log for any errors.");
            transfer_in_progress = false;
            if (VC.CheckActive())
            {
                VC.DisconnectJakarta();
            }

            DisplayErrors(error_list);

        }

        private void DisplayErrors(List<string> errors)
        {
            if (errors.Count == 0)
                return; //No errors to display get outta here

            ErrorViewer ev = new ErrorViewer(errors);
            DialogResult dr = ev.ShowDialog();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private bool ConnectToVpn()
        {
            bool result = false;
            //
            //CONNECT TO VPN
            //
            VPNController VC = new VPNController(AppSettings);

            //Write_To_Message_Log("Connecting to Jakarta to get current sales");
            //Check to make sure the connection is not active
            if (!VC.CheckActive())
            {
                Write_To_Message_Log("Opening VNP Connection to Jakarta");
                result = VC.ConnectJakarta();
            }
            if (!VC.CheckActive())
            {
                Write_Error_to_Activity_log("ERROR - Failed to connect to Jakarta - transfer aborting");
                return false;
            }
            else
            {
                Write_To_Message_Log("VPN connection to Jakarta completed successfully");
                return result;
            }
        }
        private void bRefreshSales_Click(object sender, EventArgs e)
        {
            //
            //CONNECT TO VPN
            //
            VPNController VC = new VPNController(AppSettings);

            Write_To_Message_Log("Connecting to Jakarta to get current sales");
            //Check to make sure the connection is not active
            if (!VC.CheckActive())
            {
                Write_To_Message_Log("Opening VNP Connection to Jakarta");
                VC.ConnectJakarta();
            }
            if (!VC.CheckActive())
            {
                Write_Error_to_Activity_log("ERROR - Failed to connect to Jakarta - transfer aborting");
                return;
            }
            else
            {
                Write_To_Message_Log("VPN connection to Jakarta completed successfully");
            }
            Write_To_Message_Log("Getting current sales, please wait....");
            //Get Jakarta Sales
            List<PosSale> results = dtm.GetCurrentStoreSales();

            VC.DisconnectJakarta();

            if (results.Count == 0)
                Write_Error_to_Activity_log("No sales found, something went wrong.");

            dtm.ClearSales();

            foreach (PosSale sale in results)
            {
                Write_To_Message_Log("Adding sale id:[ " + sale.Pos_Sale_Id.ToString() + "]");
                dtm.InsertSale(sale);
            }
            Write_To_Message_Log("Closing VPN Connection");
            


        }

        private async Task UpdateItems()
        {
            //
            //CONNECT TO VPN
            //
            VPNController VC = new VPNController(AppSettings);

            Write_To_Message_Log("Connecting to Jakarta to retrieve item updates"); //add in the date trigger
            if (!ConnectToVpn())
            {
                Write_Error_to_Activity_log("Failed to connect to VPN, check internet connection"); //add in the date trigger
                return;
            }
            DateTime checkpoint = DateTime.Now;
            var update_list = dtm.GetItemUpdates(AppSettings.items_last_update);
            Write_To_Message_Log("Found " + update_list.Count + " Item Updates");

            Write_To_Message_Log("Updating items on local database");
            //Now update the items one by one, because im lazy!
            int cnt_i = 0, cnt_u = 0, cnt_e = 0;
            foreach (Item i in update_list)
            {
                string result = await dtm.UpdateItem(i);
                if (result == "INSERTED")
                {
                    Write_To_Message_Log("Item [ " + i.ITEM_CODE + "] added to local database");
                    cnt_i++;
                }
                else if (result == "UPDATED")
                {
                    Write_To_Message_Log("Item [ " + i.ITEM_CODE + "] updated in local database");
                    cnt_u++;
                }
                else
                {
                    Write_Error_to_Activity_log(result + ": Item [ " + i.ITEM_CODE + "] SOMETHING WENT WRONG, ITEM SKIPPED");
                    cnt_e++;
                }
            }
            Write_To_Message_Log("Disconnecting from Jakarta");
            if (!(_ENV_TEST))
                VC.DisconnectJakarta();
            //Update the items last to the checkpoint where we started
            AppSettings.items_last_update = checkpoint;
            AppSettings.SaveSettings();
            Write_To_Message_Log("Finished!! " + cnt_i.ToString() + " New items added " + cnt_u + " items updated ");
            if (cnt_e > 0)
                Write_Error_to_Activity_log(cnt_e + " items were not able to get updated due to errors");


        }

        private async void bUpdateItems_Click(object sender, EventArgs e)

      //      private void bUpdateItems_Click(object sender, EventArgs e)
        {
            await UpdateItems();
        }

        private async Task UpdateCategories()
        {
            VPNController VC = new VPNController(AppSettings);

            Write_To_Message_Log("Connecting to Jakarta to retrieve category updates"); //add in the date trigger
            if (!ConnectToVpn())
            {
                Write_Error_to_Activity_log("Failed to connect to VPN, check internet connection"); //add in the date trigger
                return;
            }
            var category_list = await dtm.GetCategoryUpdates();
            Write_To_Message_Log(category_list.Count.ToString() +  "Categories retrieve to update or add"); //add in the date trigger

            int cnt_i = 0, cnt_u = 0, cnt_e = 0;
            foreach (var cat in category_list)
            {
                string result = await dtm.ProcessCategory(cat);

                if (result == "UPDATE")
                {
                    Write_To_Message_Log("Category " + cat.CODE + "-" + cat.DESCRIPTION + " updated on local database");
                    cnt_u++;
                } else
                {
                    if (result == "INSERT")
                    {
                        Write_To_Message_Log("[NEW] Category " + cat.CODE + "-" + cat.DESCRIPTION + " added to local database");
                        cnt_i++;
                    } else
                    {
                        Write_Error_to_Activity_log("[ERROR] Category " + cat.CODE + "-" + cat.DESCRIPTION + " was not updated.");
                        cnt_e++;
                    }
                } 

            }

            Write_To_Message_Log("Disconnecting from Jakarta");
            if (!(_ENV_TEST))
                VC.DisconnectJakarta();

            Write_To_Message_Log("Finished!! " + cnt_i.ToString() + " New categories added " + cnt_u + " categories refreshed ");
            if (cnt_e > 0)
                Write_Error_to_Activity_log(cnt_e + " items were not able to get updated due to errors");
            return;
        }
        private async void bUpdateCategories_Click(object sender, EventArgs e)
        {
            await UpdateCategories();
        }
    }
}
    