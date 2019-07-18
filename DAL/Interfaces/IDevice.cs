using System;
using IPA.Core.Data.Entity.Other;
using IPA.DAL.RBADAL.Models;
using System.Threading;
using System.Threading.Tasks;
using IPA.DAL.RBADAL.Services;
using IPA.Core.Shared.Enums;
using IPA.Core.Data.Entity;

namespace IPA.DAL.RBADAL.Interfaces
{
    public enum IDeviceMessage
    {
        DeviceBusy = 1,
        Offline    = 2
    }

    interface IDevice
    {
        event EventHandler<NotificationEventArgs> OnNotification;
        
        // Readonly Properties
        bool Connected { get; }
        Core.Data.Entity.Device DeviceInfo { get; }
        Core.Data.Entity.Model ModelInfo { get; }
        
        //Public methods
        void Init(string[] accepted, string[] available, int baudRate, int dataBits);
        void Configure(object[] settings);
        DeviceStatus Connect(bool transactionalMode);
        void Disconnect();
        string GetSerialNumber();
        string GetFirmwareVersion();
        bool Reset();
    }
}
