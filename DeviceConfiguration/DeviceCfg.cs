﻿using System;
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
        const string v3PostText = "";

        bool attached;
        bool formClosing;

        Device Device = new Device();

        static DeviceInformation deviceInformation;

        // Device Events back to Main Form
        public event EventHandler<DeviceNotificationEventArgs> OnDeviceNotification;

        readonly object discoveryLock = new object();

        internal static System.Timers.Timer MSRTimer { get; set; }

        string DevicePluginName;
        public string PluginName { get { return DevicePluginName; } }

        string modelFamily;
        string javaCmd;

        StringBuilder stdOutput;
        StringBuilder stdError;

        #endregion

        /********************************************************************************************************/
        // CONSTRUCTION AND INITIALIZATION
        /********************************************************************************************************/
        #region -- construction and initialization --

        public DeviceCfg()
        {
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
                    Debug.WriteLine("");
                    Debug.WriteLine("device capabilities ----------------------------------------------------------------");
                    Debug.WriteLine("DESCRIPTION                      : {0}", (object)description);
                    Debug.WriteLine("DEVICE ID                        : {0}", (object)deviceID);

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

                        Debug.WriteLine("device information ----------------------------------------------------------------");
                        Core.Data.Entity.Device device = Device.DeviceInfo;

                        if(device != null)
                        { 
                            deviceInformation.DeviceOS = "RBA";
                            Debug.WriteLine("device INFO[OS]              : {0}", (object) deviceInformation.DeviceOS);
                            deviceInformation.ModelName = device.ModelName;
                            Debug.WriteLine("device INFO[Model Name]      : {0}", (object) deviceInformation.ModelName);
                            deviceInformation.SerialNumber = device.SerialNumber;
                            Debug.WriteLine("device INFO[Serial Number]   : {0}", (object) deviceInformation.SerialNumber);
                            deviceInformation.FirmwareVersion = device.FirmwareVersion;
                            Debug.WriteLine("device INFO[Firmware Version]: {0}", (object) deviceInformation.FirmwareVersion);
                            deviceInformation.Port = device.AttachedPort;
                            Debug.WriteLine("device INFO[Port]            : {0}", (object) deviceInformation.Port);
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

        public void IdentifyUIADevice()
        {
            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);

            if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                SetJavaCmd(ConfigurationManager.AppSettings["JavaCmd"] ?? string.Empty);
                string directory = ConfigurationManager.AppSettings["UIAConverterDirectory"] ?? string.Empty;
                //System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(directory);
                string path = System.IO.Directory.GetCurrentDirectory(); 

                bool expectedResponses = false;
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

                    //string arguments = $"-jar \"{di.FullName}/UIAUtility.jar\" IDENTIFY \"{directory}/jpos/res/jpos.xml\" {modelPort.Model.Trim()} {modelPort.Port.Trim()}";
                    //string response = RunExternalExe(di.FullName, javaCmd, arguments);
                    string arguments = $"-jar \"{path}/UIAUtility/UIAUtility.jar\" IDENTIFY \"{path}/UIAUtility/jpos/res/jpos.xml\" {modelPort.Model.Trim()} {modelPort.Port.Trim()}";
                    string response = RunExternalExe($"{path}/UIAUtility", javaCmd, arguments);

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
                    deviceInformation.DeviceOS = "UIA";
                    Debug.WriteLine("device INFO[OS]              : {0}", (object) deviceInformation.DeviceOS);
                    deviceInformation.ModelName = model;
                    Debug.WriteLine("device INFO[Model Name]      : {0}", (object) deviceInformation.ModelName);
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
        }

        void OutputDataHandler(object sendingProcess, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(e.Data) && !e.Data.StartsWith("Downloading "))
            {
                Debug.WriteLine($"PO: {e.Data}");
                stdOutput.AppendLine(e.Data);
                if (e.Data.Contains("Skipping"))
                    Console.WriteLine(e.Data);
            }
        }

        void ErrorDataHandler(object sendingProcess, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(e.Data))
            {
                Debug.WriteLine($"PE: {e.Data}");
                stdError.AppendLine(e.Data);
            }
        }

        string RunExternalExe(string directory, string filename, string arguments = null, string[] environmentVariables = null)
        {
            Debug.WriteLine($"{directory}--{filename} {arguments ?? ""}");
            Process process = new Process();

            process.StartInfo.FileName = filename;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            if (environmentVariables != null)
            {
                for (int i = 1; i < environmentVariables.Length; i += 2)
                {
                    process.StartInfo.EnvironmentVariables[environmentVariables[i - 1]] = environmentVariables[i];
                }
            }

            stdOutput = new StringBuilder(10000);   //stdOutput gets large, so start the default value as large.
            stdError = new StringBuilder(100);      //stdError normally does not get large, so no need to start that off big.

            process.OutputDataReceived += new DataReceivedEventHandler(OutputDataHandler);
            process.ErrorDataReceived += new DataReceivedEventHandler(OutputDataHandler);

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (!string.IsNullOrEmpty(arguments))
            {
                process.StartInfo.Arguments = arguments;
            }
            process.StartInfo.WorkingDirectory = directory;
            process.StartInfo.RedirectStandardInput = true;

            try
            {
                bool killingLogged = false;
                DateTime startTime = DateTime.Now;
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                while (process.HasExited != true)
                {
                    System.Threading.Thread.Sleep(1000);
                    if((DateTime.Now - startTime).TotalMinutes < 12)
                    { 
                         continue;
                    }
                    if (process.HasExited != true)
                    { 
                        if (!killingLogged)
                        {
                            Debug.WriteLine($"Terminating process {process.StartInfo.FileName}");
                            killingLogged = true;
                        }
                        process.Kill();
                    }
                    if ((DateTime.Now - startTime).TotalMinutes < 13)
                    { 
                        continue;
                    }
                    Debug.WriteLine($"Aborting process wait {process.StartInfo.FileName}");
                    break;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"OS error while executing {(Format(filename, arguments))}: {e.Message}", e);
            }

            if (process.ExitCode == 0)
            {
                Debug.WriteLine("Process exited normally.");
                return stdOutput.ToString();
            }
            else if (process.ExitCode < 0)
            {
                Debug.WriteLine($"Process terminated abnormally {process.StartInfo.FileName}.");
                return stdOutput.ToString();  //Exit cleanly, don't throw, which breaks current execution pattern.
            }
            else
            {
                var message = new StringBuilder(256);
                if (stdOutput.Length != 0)
                {
                    message.AppendLine("Std output:");
                    message.AppendLine(stdOutput.ToString());
                }

                if (!string.IsNullOrEmpty(stdError.ToString()))
                {
                    message.AppendLine(stdError.ToString());
                }

                throw new Exception($"{(Format(filename, arguments))} finished with exit code = {process.ExitCode}: {message}");
            }
        }

        string RunExternalExeElevated(string directory, string filename, string arguments = null, string[] environmentVariables = null)
        {
            Debug.WriteLine($"{directory}--{filename} {arguments ?? ""}");

            var output = Path.GetTempFileName();
            var process = Process.Start(new ProcessStartInfo
            {
                FileName  = filename,
                Arguments = arguments,
                Verb      = "runas",
                CreateNoWindow = false,
                UseShellExecute = true
            });

            process.WaitForExit();

            string response = File.ReadAllText(output);
            File.Delete(output);

            if (process.ExitCode == 0)
            {
                Debug.WriteLine("Process exited normally.");
                return response;
            }
            else if (process.ExitCode < 0)
            {
                Debug.WriteLine($"Process terminated abnormally {process.StartInfo.FileName}.");
                return stdOutput.ToString();  //Exit cleanly, don't throw, which breaks current execution pattern.
            }
            else
            {
                var message = new StringBuilder(256);
                if (stdOutput.Length != 0)
                {
                    message.AppendLine("Std output:");
                    message.AppendLine(stdOutput.ToString());
                }

                if (!string.IsNullOrEmpty(stdError.ToString()))
                {
                    message.AppendLine(stdError.ToString());
                }

                throw new Exception($"{(Format(filename, arguments))} finished with exit code = {process.ExitCode}: {message}");
            }
        }

        string Format(string filename, string arguments)
        {
            return $"'{filename}{(((string.IsNullOrEmpty(arguments)) ? string.Empty : " " + arguments))}'";
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
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

        void SetJavaCmd(string inCmd)
        {
            var javaLocations = new string[]
            {
                @".\Resources\JavaFiles\FirmwareConverter\devices\ingenico\jre7\bin\java.exe",
                @"C:\TrustCommerce\devices\ingenico\jre7\bin\java.exe",
                @"C:\TrustCommerce\TCIPADALInstallation\TCIPAJDal\devices\ingenico\jre7\bin\java.exe",
                inCmd
            };
            foreach (string cmd in javaLocations)
            {
                if (System.IO.File.Exists(cmd))
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(cmd);
                    javaCmd = fi.FullName;
                    break;
                }
            }
        }

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
        #endregion
    }

    /********************************************************************************************************/
    // DEVICE INFORMATION
    /********************************************************************************************************/
    #region -- device information --
    internal class DeviceInformation
    {
        internal string DeviceOS;
        internal string SerialNumber;
        internal string FirmwareVersion;
        internal string ModelName;
        internal string Port;
        internal bool emvConfigSupported;
    }
    public class USBDeviceInfo
    {
        public USBDeviceInfo(string deviceID, string pnpDeviceID, string description)
        {
            this.DeviceID = deviceID;
            this.PnpDeviceID = pnpDeviceID;
            this.Description = description;
        }
        public string DeviceID { get; private set; }
        public string PnpDeviceID { get; private set; }
        public string Description { get; private set; }
    }
    public struct BoolStringDuple
    {
        public bool Item1 { get; set; }
        public string Item2 { get; set; }
        public BoolStringDuple(bool item1, string item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }

    public struct ModelPort
    {
        public string Model { get; set; }
        public string Port { get; set; }
    }

    #endregion
}