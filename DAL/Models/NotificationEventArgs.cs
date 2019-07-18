using IPA.Core.Shared.Enums;
using System;
using System.Collections.Generic;

namespace IPA.DAL.RBADAL.Models 
{
    public class NotificationEventArgs : EventArgs
    {

        public NotificationEventArgs()
        {
            UI = new UI();
            Systray = new Systray();
        }

        public NotificationType NotificationType { get; set; }
        public string Message { get; set; }
        public UI UI { get; set; }
        public Systray Systray { get; set; }
        public bool DisableClose { get; set; }
        public bool UserClosed { get; set; }
        public bool ForceRestart { get; set; }
        public Core.Shared.Enums.DeviceEvent DeviceEvent { get; set; }
    }

    public struct UI
    {        
        public string Title { get; set; }
        public string Text { get; set; }
        public ICollection<string> ButtonText { get; set; }
        public Action<object, EventArgs> ButtonClientNotification { get; internal set; }
        public bool HoldDisplayVisible { get; set; }
    }

    public struct Systray
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public bool Online { get; set; }
    }
}
