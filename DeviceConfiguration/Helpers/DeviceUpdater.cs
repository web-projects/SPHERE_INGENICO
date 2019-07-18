using IPA.DAL.RBADAL.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPA.DeviceConfiguration.Helpers
{
    class DeviceUpdater
    {
        public const string NoDevicesAttached = "No supported device";
        public const string MultipleDevicesAttached = "Multiple supported device";

        private static bool IsRBADevice(string model)
        {
            string rbaVersion;
            DeviceIngenico device = new DeviceIngenico();
            bool isRBA = device.GetRBAVersion(ref model, out rbaVersion);
            if (isRBA)
            {
                if (rbaVersion.Contains("- 21"))
                {
                    Debug.WriteLine("RBA Version 21 Found.");
                }
                Debug.WriteLine("Already RBA.");
                return true;
            }
            return false;
        }
    }
}
