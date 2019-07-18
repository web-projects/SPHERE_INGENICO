using System;
using IPA.DAL.RBADAL.Interfaces;
using IPA.DAL.RBADAL.Models;
using System.Management;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using IPA.Core.Shared.Enums;
using IPA.Core.Data.Entity;

namespace IPA.DAL.RBADAL.Services
{
    public class Device
    {
        #region -- member variables --

        private IDevice deviceInterface;

        public delegate void EventHandler(object sender, NotificationEventArgs args);
        public event EventHandler<NotificationEventArgs> OnNotification = delegate { };
        public static DeviceStatus deviceStatus;
        public static int retryTimes = 2;
        #endregion

        #region -- public methods --

        public void Init(string[] available)
        {
            BaudRate = int.Parse(ConfigurationManager.AppSettings["IPA.DAL.Device.COMBaudRate"]);
            DataBits = int.Parse(ConfigurationManager.AppSettings["IPA.DAL.Device.COMDataBits"]);
            AcceptedPorts = ConfigurationManager.AppSettings["IPA.DAL.Device.AcceptedComPorts"].Split(',');

            DeviceFolder = ConfigurationManager.AppSettings["IPA.DAL.Application.Folders.Devices"];
            LoggingLevel = ConfigurationManager.AppSettings["IPA.DAL.Device.LoggingLevel"];
            
            deviceStatus = DeviceStatus.NoDevice;
            var devices = GetUSBDevices();
            
            if (devices.Count == 1)
            {
                var vendor = devices[0].Vendor;
                Device.Manufacturer = devices[0].Vendor;

                switch (devices[0].Vendor)
                {
                    case DeviceManufacturer.Ingenico:
                    {
                        deviceInterface = new DeviceIngenico();
                        deviceInterface.OnNotification += DeviceOnNotification;
                        break;
                    }
                    default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(vendor), vendor, null);
                    }
                }
            }
            else if(devices.Count > 1)
            {
                throw new Exception(DeviceStatus.MultipleDevice.ToString());
            }
            else
            {
                throw new Exception(DeviceStatus.NoDevice.ToString());
            }
            deviceInterface?.Init(Device.AcceptedPorts, available, Device.BaudRate, Device.DataBits);            
        }
        
        public void Configure(object[] settings)
        {
            deviceInterface.Configure(settings);
        }

        public void Connect(bool transactionalMode)
        {
            deviceStatus = (DeviceStatus)deviceInterface?.Connect(transactionalMode);
        }
 
        public void Disconnect()
        {
            deviceInterface?.Disconnect();
        }

        public bool Reset()
        {
            return deviceInterface.Reset();
        }

        public List<USBDeviceInfo> GetUSBDevices()
        {
            List<USBDeviceInfo> devices = new List<USBDeviceInfo>();

            ManagementObjectCollection collection;

            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity"))
                collection = searcher.Get();

            foreach (var device in collection)
            {
                var deviceID = (string)device.GetPropertyValue("DeviceID");
///                if (deviceID.ToLower().Contains("usb\\") && ((deviceID.Contains($"VID_{IDTECH}") && !Configuration.General.IDTechDisable )|| deviceID.Contains($"VID_{INGNAR}")))
                if (deviceID.ToLower().Contains("usb\\") && ((deviceID.Contains($"VID_{IDTECH}")) || deviceID.Contains($"VID_{INGNAR}")))
                {
                    DeviceManufacturer vendor = deviceID.Contains($"VID_{IDTECH}") ? DeviceManufacturer.IDTech : DeviceManufacturer.Ingenico;
                    devices.Add(new USBDeviceInfo(
                    (string)device.GetPropertyValue("DeviceID"),
                    (string)device.GetPropertyValue("PNPDeviceID"),
                    (string)device.GetPropertyValue("Description"),
                    vendor
                    ));
                }
            }

            collection.Dispose();
            return devices;
        }
        public string GetSerialNumber()
        {
            return deviceInterface.GetSerialNumber();
        }
        public string GetFirmwareVersion()
        {
            return deviceInterface.GetFirmwareVersion();
        }

        public class USBDeviceInfo
        {
            public USBDeviceInfo(string deviceID, string pnpDeviceID, string description, DeviceManufacturer vendor)
            {
                this.DeviceID = deviceID;
                this.PnpDeviceID = pnpDeviceID;
                this.Description = description;
                this.Vendor = vendor;
            }
            public string DeviceID { get; private set; }
            public string PnpDeviceID { get; private set; }
            public string Description { get; private set; }
            public DeviceManufacturer Vendor { get; private set; }
        }
        #endregion

        #region event handlers --

        public void DeviceOnNotification(object sender, NotificationEventArgs e)
        {
            OnNotification?.Invoke(null, e);
        }

        #endregion

        //TODO: vette pattern of where class declaritions are located (top/bottom) - Mark
        #region -- public properties --   

        public  Core.Data.Entity.Device DeviceInfo => deviceInterface?.DeviceInfo;

        public  Core.Data.Entity.Model ModelInfo => deviceInterface?.ModelInfo;
        
        public bool Connected => deviceInterface?.Connected ?? false;
        public static int BaudRate;
        public static int DataBits;
        public static string[] AcceptedPorts;
        public static DeviceManufacturer Manufacturer;
        public string DeviceFolder;
        public string LoggingLevel;

        public const string IDTECH = "0ACD";
        public const string INGNAR = "0B00";

        public static class CommandTokens
        {
            public static byte[] SetDefaultConfig    = { 0x02, 0x53, 0x18, 0x03 };
            public static byte[] ReadConfiguration   = { 0x02, 0x52, 0x1F, 0x03 };
            public static byte[] ReadFirmwareVersion = { 0x02, 0x52, 0x22, 0x03 };
            public static byte[] GetSerialNumber     = { 0x02, 0x52, 0x4E, 0x03 };
            public static byte[] SetKeyedInOption    = { 0x02, 0x53, 0x8F, 0x01, 0x00, 0x03 };
            public static byte[] SetKeyedInCVV       = { 0x02, 0x53, 0x8F, 0x01, 0x02, 0x03 };
            public static byte[] EnableAdminKey      = { 0x02, 0x30, 0x8F, 0x01, 0x20, 0x03 };
            public static byte[] DisableAdminKey     = { 0x02, 0x31, 0x8F, 0x01, 0x20, 0x03 };
            public static byte[] SetUSBHIDMode       = { 0x02, 0x53, 0x23, 0x01, 0x30, 0x03 };
            public static byte[] SetUSBKYBMode       = { 0x01, 0x01, 0x01 };
            public static byte[] SetTDES             = { 0x02, 0x53, 0x4C, 0x01, 0x31, 0x03 };
            public static byte[] SetKeyedOption      = { 0x02, 0x53, 0x8F, 0x01, 0x01, 0x03 };
            public static byte[] SetPANMask          = { 0x02, 0x53, 0x49, 0x01, 0x06, 0x03 };
            public static byte[] DeviceReset         = { 0x02, 0x46, 0x49, 0x03 };
        }
        #endregion 
    }
}
