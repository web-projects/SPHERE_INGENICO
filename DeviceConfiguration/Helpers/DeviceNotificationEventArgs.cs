using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPA.DAL.RBADAL.Helpers
{
    public enum NOTIFICATION_TYPE
    {
        NT_INITIALIZE_DEVICE          = 1,
        // CONFIGURATION EVENTS
        NT_DEVICE_UPDATE_CONFIG,
        NT_UNLOAD_DEVICE_CONFIGDOMAIN,
        NT_UPDATE_SETUP_MESSAGE,
        NT_STATUS_MESSAGE_UPDATE
    }

    [Serializable]
    public class DeviceNotificationEventArgs
    {
        public NOTIFICATION_TYPE NotificationType { get; set; }
        public object [] Message { get; set; }
    }
}
