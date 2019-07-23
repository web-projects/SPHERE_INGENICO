using System;
using System.Diagnostics;
using System.Linq;
using HidLibrary;
using IPA.DAL.RBADAL.Helpers;
using System.Security.Permissions;
using IPA.Core.Shared.Enums;
using IPA.DAL.RBADAL.Services;
using System.Collections.Generic;
using System.Management;
using IPA.DeviceConfiguration.Helpers;
using System.Configuration;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading;
using static IPA.DAL.RBADAL.Ingenico.Device;
using IPA.DAL.Helpers;
using IPA.LoggerManager;

namespace IPA.DAL.RBADAL
{
    [Serializable]
    public class DeviceCfg : MarshalByRefObject, IDevicePlugIn
    {
        /********************************************************************************************************/
        // ATTRIBUTES
        /********************************************************************************************************/
        #region -- attributes --

        const string INGNAR = "0b00"; //Do NOT make this uppercase
        const string IDTECH = "0acd";
        const string IdTechString = "idtech";

        const string v4PostText = "v4";
        const string v3PostText = "v3";

        bool attached;
        bool formClosing;

        Device Device = new Device();

        static DeviceInformation deviceInformation;

        Utility utility = new Utility();

        // Device Events back to Main Form
        public event EventHandler<DeviceNotificationEventArgs> OnDeviceNotification;

        readonly object discoveryLock = new object();

        internal static System.Timers.Timer MSRTimer { get; set; }

        string DevicePluginName;
        public string PluginName { get { return DevicePluginName; } }

        string modelFamily;

        #endregion

        /********************************************************************************************************/
        // CONSTRUCTION AND INITIALIZATION
        /********************************************************************************************************/
        #region -- construction and initialization --

        public DeviceCfg()
        {
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void DeviceInit()
        {
            DevicePluginName = "DeviceCfg";

            // Create Device info object
            deviceInformation = new DeviceInformation 
            { 
                emvConfigSupported = false
            };

            // Device Discovery
            attached = false;

            try
            {
                string description = string.Empty;
                string deviceID = string.Empty;

                NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_STATUS_MESSAGE_UPDATE, Message = new [] { (object)SearchStatus.StatusIndex.SEARCHING_FOR_DEVICE } });

                if (FindIngenicoDevice(ref description, ref deviceID))
                {
                    Logger.info("device capabilities ----------------------------------------------------------------");
                    Logger.info("DESCRIPTION                  : {0}", (object)description);
                    Logger.info("DEVICE ID                    : {0}", (object)deviceID);

                    BoolStringDuple output = new BoolStringDuple(true, description.ToLower().Replace("ingenico ", ""));
                    modelFamily = output.Item2;

                    object [] message = new [] { (object)SearchStatus.StatusIndex.INGENICO_DEVICE_FOUND, $" ({modelFamily})" };
                    NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_STATUS_MESSAGE_UPDATE, Message = message });

                    Device.OnNotification += OnNotification;

                    Device.Init(SerialPortService.GetAvailablePorts());

                    // connect to device
                    Device.Connect(false);

                    // Set as Attached
                    attached = (Device.deviceStatus == DeviceStatus.Connected ? true : false);

                    if(attached)
                    { 
                        Array.Resize(ref message, message.Length + 1);
                        message[message.Length - 1] = " - RBA";
                        NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_STATUS_MESSAGE_UPDATE, Message = message });

                        Logger.info("device information ----------------------------------------------------------------");
                        Core.Data.Entity.Device device = Device.DeviceInfo;

                        if(device != null)
                        { 
                            deviceInformation.DeviceOS = Enum.GetName(typeof(DeviceOS), DeviceOS.RBA);
                            Logger.info("device INFO[OS]              : {0}", (object) deviceInformation.DeviceOS);
                            deviceInformation.ModelName = device.ModelName;
                            Logger.info("device INFO[Model Name]      : {0}", (object) deviceInformation.ModelName);
                            deviceInformation.SerialNumber = device.SerialNumber;
                            Logger.info("device INFO[Serial Number]   : {0}", (object) deviceInformation.SerialNumber);
                            deviceInformation.FirmwareVersion = device.FirmwareVersion;
                            Logger.info("device INFO[Firmware Version]: {0}", (object) deviceInformation.FirmwareVersion);
                            deviceInformation.Port = device.AttachedPort;
                            Logger.info("device INFO[Port]            : {0}", (object) deviceInformation.Port);
                        }

                        NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_DEVICE_UPDATE_CONFIG });
                    }
                    else
                    {
                        Array.Resize(ref message, message.Length + 1);
                        message[message.Length - 1] = " - NOT RBA";
                        NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_STATUS_MESSAGE_UPDATE, Message = message });
                        Device.Disconnect();
                        throw new Exception("UIADevice");
                    }
                }
                else
                {
                    throw new Exception("NoDevice");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool FindIngenicoDevice(ref string description, ref string deviceID)
        {
            List<USBDeviceInfo> devices = GetUSBDevices();
            if (devices.Count == 1)
            {
                BoolStringDuple output = output = new BoolStringDuple(true, devices[0].Description.ToLower().Replace("ingenico ", ""));
                deviceID = devices[0].DeviceID;
                description = devices[0].Description;

                return true;
            }
            return false;
        }

        public bool IdentifyUIADevice()
        {
            bool expectedResponses = false;

            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);

            if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                utility.SetJavaCmd(ConfigurationManager.AppSettings["JavaCmd"] ?? string.Empty);
                string directory = ConfigurationManager.AppSettings["UIAConverterDirectory"] ?? string.Empty;
                string path = System.IO.Directory.GetCurrentDirectory(); 

                string uiaVersion = string.Empty;
                string model = string.Empty;
                string modelVer = string.Empty;
                string serialNumber = string.Empty;
                int portFound = 0;

                var modelPortList = GetModelPorts(modelFamily);
                foreach (var modelPort in modelPortList)
                {
                    object [] message = new [] { (object)SearchStatus.StatusIndex.UIA_INGENICO_DEVICE_SEARCH, $"  {modelPort.Model.Trim()}  on {modelPort.Port.Trim()}" };
                    NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_STATUS_MESSAGE_UPDATE, Message = message });

                    string arguments = $"-jar \"{path}\\UIAUtilities\\UIAUtility.jar\" IDENTIFY \"{path}\\UIAUtilities\\jpos\\res\\jpos.xml\" {modelPort.Model.Trim()} {modelPort.Port.Trim()}";
                    string response = utility.RunExternalExe($"{path}\\UIAUtilities", utility.GetJavaCmd(), false, arguments);

                    if(!string.IsNullOrWhiteSpace(response))
                    { 
                        expectedResponses = ParseUIACheckResponse(response, out uiaVersion, out model, out modelVer, out portFound, out serialNumber);
                        if (expectedResponses)
                        { 
                            break;
                        }
                    }
                }
                if (expectedResponses)
                { 
                    attached = true;
                    Debug.WriteLine("device information ----------------------------------------------------------------");
                    deviceInformation.DeviceOS = Enum.GetName(typeof(DeviceOS), DeviceOS.UIA);
                    Debug.WriteLine("device INFO[OS]              : {0}", (object) deviceInformation.DeviceOS);
                    deviceInformation.ModelName = model;
                    Debug.WriteLine("device INFO[Model Name]      : {0}", (object) deviceInformation.ModelName);
                    deviceInformation.ModelVersion = modelVer;
                    Debug.WriteLine("device INFO[Model Version]   : {0}", (object) deviceInformation.ModelVersion);
                    deviceInformation.SerialNumber = serialNumber;
                    Debug.WriteLine("device INFO[Serial Number]   : {0}", (object) deviceInformation.SerialNumber);
                    deviceInformation.FirmwareVersion = uiaVersion;
                    Debug.WriteLine("device INFO[Firmware Version]: {0}", (object) deviceInformation.FirmwareVersion);
                    deviceInformation.Port = $"COM{portFound}";
                    Debug.WriteLine("device INFO[Port]            : {0}", (object) deviceInformation.Port);

                    NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_DEVICE_UPDATE_CONFIG });

                    // connect to device to receive disconnect notifications
                    if(Device.deviceStatus != DeviceStatus.Connected)
                    { 
                        new Thread(() =>
                        {
                            Thread.CurrentThread.IsBackground = true;
                            Device.Connect(false);
                        }).Start();
                    }
                }
            }
            else
            {
                NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_STATUS_MESSAGE_UPDATE, Message = new [] { "cannot proceed as non-Administrator." } });
            }

            return expectedResponses;
        }

        #endregion

        /********************************************************************************************************/
        // MAIN INTERFACE
        /********************************************************************************************************/
        #region -- main interface --

        public string [] GetConfig()
        {
            if (!attached) 
            { 
                return null; 
            }

            // Get Configuration
            string [] config = { deviceInformation.DeviceOS,
                                 deviceInformation.SerialNumber,
                                 deviceInformation.FirmwareVersion,
                                 deviceInformation.ModelName,
                                 deviceInformation.Port
            };

            return config;
        }

        public void SetFormClosing(bool state)
        {
            formClosing = state;
            if(formClosing)
            {
                if(deviceInformation.emvConfigSupported)
                {
    //                Debug.WriteLine("DeviceCfg::DISCONNECTING FOR device TYPE={0}", IDT_Device.getDeviceType());
    //                Device.CloseDevice();
                }
            }
        }
    
        protected void OnNotification(object sender, Models.NotificationEventArgs args)
        {
            Debug.WriteLine("device: notification type={0}", args.NotificationType);

            switch (args.NotificationType)
            {
                case NotificationType.DeviceEvent:
                {
                    switch(args.DeviceEvent)
                    {
                        case DeviceEvent.DeviceDisconnected:
                        {
                            DeviceRemovedHandler();
                            break;
                        }
                    }
                    break;
                }
            }
        }

        public void NotificationRaise(DeviceNotificationEventArgs e)
        {
            OnDeviceNotification?.Invoke(null, e);
        }

        #endregion

        /********************************************************************************************************/
        // DEVICE EVENTS INTERFACE
        /********************************************************************************************************/
        #region -- device event interface ---

        private void DeviceRemovedHandler()
        {
            Debug.WriteLine("\ndevice: removed !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\n");

            if(attached)
            {
                attached = false;
                // Unload Device Domain
                NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_UNLOAD_DEVICE_CONFIGDOMAIN });
            }
        }

        private void DeviceAttachedHandler()
        {
            Debug.WriteLine("device: attached ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
        }

        public static List<USBDeviceInfo> GetUSBDevices()
        {
            List<USBDeviceInfo> devices = new List<USBDeviceInfo>();
            ManagementObjectCollection collection;
            try
            {
                using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity"))
                {
                    collection = searcher.Get();
                }
                foreach (var device in collection)
                {
                    var deviceID = ((string)device.GetPropertyValue("DeviceID") ?? "").ToLower();
                    if (string.IsNullOrWhiteSpace(deviceID))
                        continue;
                    if (deviceID.Contains("usb\\") && (deviceID.Contains($"vid_{INGNAR}") || deviceID.Contains($"vid_{IDTECH}")))
                    {
                        devices.Add(new USBDeviceInfo(
                            (string)device.GetPropertyValue("DeviceID"),
                            (string)device.GetPropertyValue("PNPDeviceID"),
                            (deviceID.Contains($"vid_{IDTECH}") ? DeviceCfg.IdTechString : (string)device.GetPropertyValue("Description"))
                        ));
                    }
                }
                collection.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return devices;
        }

        #endregion

        /********************************************************************************************************/
        // INTERNAL COMMANDS
        /********************************************************************************************************/
        #region -- internal commands --

        string[] GetAvailablePorts()
        {
            return System.IO.Ports.SerialPort.GetPortNames();
        }

        List<ModelPort> PortsToCheck(IEnumerable<string> comPorts, string devices, params int[] preferredPorts)
        {
            string[] deviceArray = devices.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<ModelPort> checkThesePorts = new List<ModelPort>();
            List<string> ports = new List<string>(comPorts);
            foreach (int port in preferredPorts)
            {
                string comport = $"COM{port}";
                bool added = false;
                foreach (string device in deviceArray)
                {
                    if (ports.Contains(comport))
                    {
                        checkThesePorts.Add(new ModelPort() { Port = comport, Model = device });
                        added = true;
                    }
                }
                if (added)
                {
                    ports.Remove(comport);
                }
            }
            foreach (string port in ports)
            {
                foreach (string device in deviceArray)
                {
                    checkThesePorts.Add(new ModelPort() { Port = port, Model = device });
                }
            }
            return checkThesePorts;
        }

        List<ModelPort> GetModelPorts(string portFamily)
        {
            var ports = new List<string>(GetAvailablePorts());
            if (portFamily.IndexOf("isc3", 0, StringComparison.OrdinalIgnoreCase) > -1  //isc480 Windows device name
                || portFamily.IndexOf("isc480", 0, StringComparison.OrdinalIgnoreCase) > -1) //TC Device name
            { 
                return PortsToCheck(ports, "iSC480", 35, 111);
            }
            else if (portFamily.IndexOf("isc2", 0, StringComparison.OrdinalIgnoreCase) > -1)  //isc250 windows device name, and TC Device name
            { 
                return PortsToCheck(ports, "iSC250", 35, 109);
            }
            else if (portFamily.IndexOf("ipp", 0, StringComparison.OrdinalIgnoreCase) > -1)  //ipp320 and ipp350 windows device name and TC Device name
            {
                return PortsToCheck(ports, "iPP320, iPP350", 35, 110, 113);
            }
            return PortsToCheck(ports, string.Empty, 0);
        }

        bool ParseUIACheckResponse(string response, out string versionFound, out string model, out string modelVer, out int portFound, out string serialNumber)
        {
            portFound = 0;
            versionFound = model = modelVer = serialNumber = string.Empty;
            if (response.IndexOf("No device found", 0, StringComparison.OrdinalIgnoreCase) > -1)
            { 
                return false;
            }
            if (RetrieveData(response, "FirmwareVersion", out versionFound))
            {
                RetrieveData(response, "ModelDescription", out model);
                if (versionFound.Contains("19.6."))
                {
                    ConvertFullModelToDevVer(model, out model, out modelVer);
                }
                else
                {
                    modelVer = v3PostText;
                }
                portFound = RetrieveComPort(response);
                if (RetrieveData(response, "SerialNumber", out serialNumber))
                {
                    return true;
                }
            }
            return false;
        }

        bool RetrieveData(string contents, string tag, out string data)
        {
            bool response = false;
            data = string.Empty;
            int modDesc = contents.IndexOf(tag);
            if (modDesc > 0)
            {
                int firstQuote = contents.IndexOf('"', modDesc);
                if (firstQuote > modDesc)
                {
                    int secondQuote = contents.IndexOf('"', firstQuote + 1);
                    if (secondQuote > firstQuote)
                    {
                        int lastQuote = contents.IndexOf('"', secondQuote + 1);
                        data = contents.Substring(secondQuote + 1, lastQuote - (secondQuote + 1));
                        response = !string.IsNullOrEmpty(data);
                    }
                }
            }
            return response;
        }

        int RetrieveComPort(string response)
        {
            int port = 0;
            if (RetrieveData(response, "COMMPort", out string comport))
            {
                if (comport.StartsWith("COM", StringComparison.OrdinalIgnoreCase))
                {
                    string portUsed = comport.Substring(3);
                    Int32.TryParse(portUsed, out port);
                }
            }
            return port;
        }

        void ConvertFullModelToDevVer(string fullModel, out string model, out string modelVersion)
        {
            modelVersion = v3PostText;
            model = fullModel;
            int dashLoc = fullModel.IndexOf('-');
            if (dashLoc > -1 && dashLoc + 1 < fullModel.Length)
            {
                model = fullModel.Substring(0, dashLoc);
                char afterDash = fullModel[dashLoc + 1];
                if (afterDash == '3')
                    modelVersion = v4PostText;
            }
        }

        public void UpdateUIAFirmware()
        {
            object [] message = new [] { (object)SearchStatus.StatusIndex.UIA_INGENICO_FIRMWARE_UPDATE, $"  {deviceInformation.ModelName.Trim()}  on {deviceInformation.Port.Trim()}" };
            NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_STATUS_MESSAGE_UPDATE, Message = message });

            // disconnect regardless of connection state (to remove notifications)
            Device.Disconnect();

            string response = utility.UpdateUIAFirmware(deviceInformation);
            if(!string.IsNullOrEmpty(response))
            {
                message = new [] { (object)SearchStatus.StatusIndex.UIA_INGENICO_FIRMWARE_FAILED, $" {response}" };
                NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_STATUS_MESSAGE_FINAL, Message = message });
            }
            else
            {
                message = new [] { (object)SearchStatus.StatusIndex.INGENICO_DEVICE_REBOOTING };
                NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_STATUS_MESSAGE_FINAL, Message = message });

                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;

                    // Scan for device on reboot
                    DeviceRemovedHandler();

                }).Start();
            }
        }

        public void UpdateRBAFirmware(int version)
        {
            object [] message = new [] { (object)SearchStatus.StatusIndex.RBA_INGENICO_FIRMWARE_UPDATE, $"  {deviceInformation.ModelName.Trim()}  on {deviceInformation.Port.Trim()}" };
            NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_STATUS_MESSAGE_UPDATE, Message = message });

            // disconnect regardless of connection state (to remove notifications)
            Device.Disconnect();

            string response = utility.UpdateRBAFirmware(deviceInformation, version);
            if(!string.IsNullOrEmpty(response))
            {
                int index = response.ToString().IndexOf("ERROR");
                if(index != -1)
                {
                    string error = response.Substring(index).Split(new string[] {"\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                    message = new [] { (object)SearchStatus.StatusIndex.RBA_INGENICO_FIRMWARE_FAILED, $" {error}" };
                    NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_STATUS_MESSAGE_FINAL, Message = message });
                }
            }
            else
            {
                message = new [] { (object)SearchStatus.StatusIndex.INGENICO_DEVICE_REBOOTING };
                NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_STATUS_MESSAGE_FINAL, Message = message });

                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;

                    // Scan for device on reboot
                    DeviceRemovedHandler();

                }).Start();
            }
        }

        #endregion
    }
}
