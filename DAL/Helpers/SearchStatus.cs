using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPA.DAL.RBADAL
{
    public class SearchStatus
    {
        public enum StatusIndex
        {
            [System.ComponentModel.Description("searching for device...")]
            SEARCHING_FOR_DEVICE = 100,
            [System.ComponentModel.Description("found Ingenico device")]
            INGENICO_DEVICE_FOUND = 101,
            [System.ComponentModel.Description("querying for UIA device")]
            UIA_INGENICO_DEVICE_SEARCH = 102,
            [System.ComponentModel.Description("updating UIA firmware...")]
            UIA_INGENICO_FIRMWARE_UPDATE = 103,
            [System.ComponentModel.Description("UIA Firmware Update failed.")]
            UIA_INGENICO_FIRMWARE_FAILED = 104,
            [System.ComponentModel.Description("RBA Firmware Update failed.")]
            RBA_INGENICO_FIRMWARE_FAILED = 105,
        }
    }
}
