using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IniParser.Model;

namespace IlufaDataTransfer
{
    public partial class SettingsWindow : Form
    {
        SettingsManager App_Settings = new SettingsManager();
        public SettingsWindow()
        {
            InitializeComponent();

            App_Settings.LoadSettings();
            CB_Location.SelectedItem = App_Settings.location;
            Txt_Oracle_IP.Text = App_Settings.oracle_IP;
            Txt_Remote_IP.Text = App_Settings.remote_oracle_IP;
            Txt_VPN_Name.Text = App_Settings.vpn_name;
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            IniData ini_settings = new IniData();
            SectionData set = new SectionData("Settings");

            string loc = CB_Location.SelectedItem.ToString().Substring(0, 3);
            App_Settings.location = loc;
            App_Settings.oracle_IP = Txt_Oracle_IP.Text;
            App_Settings.remote_oracle_IP = Txt_Remote_IP.Text;
            App_Settings.vpn_name = Txt_VPN_Name.Text;


            App_Settings.SaveSettings();
        }
    }
}
