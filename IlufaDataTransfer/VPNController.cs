using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPNConnector;

namespace IlufaDataTransfer
{
    class VPNController
    {
        VPNConnector.VPNConnector vpn;
        public SettingsManager AppSettings;

        public VPNController(SettingsManager sm)
        {
            SetSettings(sm);
            vpn = new VPNConnector.VPNConnector(AppSettings.remote_oracle_IP, AppSettings.vpn_name, "ilufa", "ilufa2018");
        }

        private void SetSettings(SettingsManager sm)
        {
            this.AppSettings = sm;
        }

        public bool ConnectJakarta()
        {
            //VPNConnector

            bool result =  vpn.TryConnect();

            result = vpn.WaitUntilActive();

                return result;

        }

        public bool CheckActive()
        {
            if (vpn == null)
                return false;

            return vpn.IsActive;
        }

        public bool DisconnectJakarta()
        {
            if (vpn == null)
                return false;

            return vpn.TryDisconnect();
        }

    }


}



