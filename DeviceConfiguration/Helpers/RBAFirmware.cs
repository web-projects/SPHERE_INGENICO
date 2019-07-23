using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPA.DeviceConfiguration.Helpers
{
    public class RBAFirmware
    {
        public enum IngenicoRBA
        {
            [System.ComponentModel.Description("FDRC")]
            FDRC = 100,
            [System.ComponentModel.Description("VITAL")]
            VITAL = 101,
        }
    }
}
