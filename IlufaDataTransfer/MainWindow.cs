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

namespace IlufaDataTransfer
{
    public partial class MainWindow : Form
    {
        SettingsManager AppSettings = new SettingsManager();
        DataTransferManager dtm;
        Boolean transfer_in_progress = false;
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
    }
}
    