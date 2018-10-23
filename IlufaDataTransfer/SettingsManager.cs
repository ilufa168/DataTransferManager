using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using IniParser;
using IniParser.Model;

namespace IlufaDataTransfer
{
    class SettingsManager
    {
        private string settings_folder = "IlufaDataTransfer";
        private string settings_file_name = "Settings.ini";
        private string _location = "A99";
        private string _oracle_IP = "192.168.1.3";
        private string _remote_oracle_IP = "192.168.1.9";
        private string _vpn_name = "VPN";

        public string location
        {
            get { return _location; }
            set { _location = value; }
        }

        public string oracle_IP
        {
            get { return _oracle_IP; }
            set { _oracle_IP = value; }
        }

        public string remote_oracle_IP
        {
            get { return _remote_oracle_IP; }
            set { _remote_oracle_IP = value; }
        }

        public string vpn_name
        {
            get { return _vpn_name; }
            set { _vpn_name = value; }
        }

        public bool SaveSettings()
        {
            string file_path = GetSettingsDirectory();

            //Make sure directory exists if not create it
            DirectoryInfo di = new DirectoryInfo(file_path);
            if (!di.Exists)
                di.Create();

            var parser = new FileIniDataParser();
           IniData ini_settings = new IniData();

           //SectionData set = new SectionData("Settings");
            ini_settings["Settings"]["location"] = location;
            ini_settings["Settings"]["oracle_ip_address"] = oracle_IP;
            ini_settings["Settings"]["remote_oracle_ip_address"] = remote_oracle_IP;
            ini_settings["Settings"]["vpn_name"] = vpn_name;

            parser.WriteFile(di.ToString() + "\\" + settings_file_name, ini_settings);


            return true;
        }

        public void LoadSettings()
        {

            var parser = new FileIniDataParser();
            
            DirectoryInfo di = new DirectoryInfo(GetSettingsDirectory());

            FileInfo fi = new FileInfo(di.ToString() + "\\" + settings_file_name);

            if (!fi.Exists)
            {
                this.SaveSettings();
            }

            IniData ini_settings = parser.ReadFile(di.ToString() + "\\" + settings_file_name);
            string test = ini_settings.GetKey("location");
            location = ini_settings["Settings"]["location"];
            oracle_IP = ini_settings["Settings"]["oracle_ip_address"];
            remote_oracle_IP = ini_settings["Settings"]["remote_oracle_ip_address"];
            vpn_name = ini_settings["Settings"]["vpn_name"];


            return;
        }

        private string GetSettingsDirectory()
        {
            string settings_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            settings_path += "\\" + settings_folder;

            return settings_path;
        }
    }
}
