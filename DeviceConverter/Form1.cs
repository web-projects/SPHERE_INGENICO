﻿using HidLibrary;
using IPA.DAL.RBADAL;
using IPA.DAL.RBADAL.Helpers;
using IPA.DAL.RBADAL.Services;
using IPA.DeviceConfiguration.Helpers;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using static IPA.DAL.RBADAL.Ingenico.Device;
using IPA.LoggerManager;
using static IPA.DeviceConfiguration.Helpers.RBAFirmware;

namespace IPA.MainApp
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        /********************************************************************************************************/
        // ATTRIBUTES SECTION
        /********************************************************************************************************/
        #region -- attributes section --

        // Always on TOP
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        bool tc_always_on_top;
        bool formClosing;

        // AppDomain Artifacts
        AppDomainCfg appDomainCfg;
        AppDomain appDomainDevice;
        IDevicePlugIn devicePlugin;
        const string MODULE_NAME = "DeviceConfiguration";
        const string PLUGIN_NAME = "IPA.DAL.RBADAL.DeviceCfg";
        string ASSEMBLY_NAME = typeof(IPA.DAL.RBADAL.DeviceCfg).Assembly.FullName;

        #endregion

        /********************************************************************************************************/
        // FORM CONTROL
        /********************************************************************************************************/
        #region -- form control --

        public Form1()
        {
            InitializeComponent();

            this.Text = string.Format("UIA to RBA Conversion Utility - Version {0}", Assembly.GetEntryAssembly().GetName().Version);

            string fullName = Assembly.GetEntryAssembly().Location;
            Logger.info("{0} VERSION {1}.", System.IO.Path.GetFileNameWithoutExtension(fullName).ToUpper(), Assembly.GetEntryAssembly().GetName().Version);
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.MaximizeBox = false;

            if(tc_always_on_top)
            {
                SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            }

            // Initialize Device
            InitalizeDevice();
        }

        private void OnFormFormClosing(object sender, FormClosingEventArgs e)
        {
            formClosing = true;

            if (devicePlugin != null)
            {
                try
                {
                    devicePlugin.SetFormClosing(formClosing);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine("main: OnFormFormClosing() - exception={0}", (object) ex.Message);
                }
            }
        }

        #endregion

        /********************************************************************************************************/
        // DELEGATES SECTION
        /********************************************************************************************************/
        #region -- delegates section --

        protected void OnDeviceNotificationUI(object sender, DeviceNotificationEventArgs args)
        {
            Debug.WriteLine("main: notification type={0}", args.NotificationType);

            switch (args.NotificationType)
            {
                case NOTIFICATION_TYPE.NT_INITIALIZE_DEVICE:
                {
                    break;
                }

                case NOTIFICATION_TYPE.NT_DEVICE_UPDATE_CONFIG:
                {
                    UpdateUI();
                    break;
                }

                case NOTIFICATION_TYPE.NT_UNLOAD_DEVICE_CONFIGDOMAIN:
                {
                    UnloadDeviceConfigurationDomain(sender, args);
                    break;
                }

                case NOTIFICATION_TYPE.NT_STATUS_MESSAGE_UPDATE:
                {
                    StatusMessageUI(sender, args, false);
                    break;
                }

                case NOTIFICATION_TYPE.NT_STATUS_MESSAGE_FINAL:
                {
                    StatusMessageUI(sender, args, true);
                    break;
                }
            }
        }

        #endregion

        /********************************************************************************************************/
        // GUI - DELEGATE SECTION
        /********************************************************************************************************/
        #region -- gui delegate section --

        private void InitalizeDeviceUI(object sender, DeviceNotificationEventArgs e)
        {
            InitalizeDevice(true);
        }

        private void ClearUI()
        {
            if (InvokeRequired)
            {
                MethodInvoker Callback = new MethodInvoker(ClearUI);
                Invoke(Callback);
            }
            else
            {
                this.ApplicationlblDeviceOS.Text = "UNKNOWN";
                this.ApplicationlblSerialNumber.Text = "UNKNOWN";
                this.ApplicationFirmwarelblVersion.Text = "UNKNOWN";
                this.ApplicationlblModelName.Text = "UNKNOWN";
                this.ApplicationlblPort.Text = "UNKNOWN";
            }
        }

        private void UpdateUI()
        {
            if (InvokeRequired)
            {
                MethodInvoker Callback = new MethodInvoker(UpdateUI);
                Invoke(Callback);
            }
            else
            {
                SetConfiguration();
            }
        }

        private void UnloadDeviceConfigurationDomain(object sender, DeviceNotificationEventArgs e)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                ClearUI();

                // wait for a new device to connect
                InitalizeDevice(true);

            }).Start();
        }

        private void StatusMessageUI(object sender, DeviceNotificationEventArgs e, bool final)
        {
            MethodInvoker mi = () =>
            {
                try
                {
                    //expected collection: object, string
                    object [] data = e.Message?.Cast<object>().Select(x => x ?? "").ToArray() ?? null;
                    if(data != null && data.Length > 0)
                    {
                        string status = IPA.DAL.Helpers.StatusCode.GetDisplayMessage((SearchStatus.StatusIndex)data[0]);
                        if(data.Length > 1)
                        {
                            status += data[1].ToString();
                            if(data.Length > 2)
                            {
                                status += data[2].ToString();
                            }
                        }
                        this.ApplicationlblStatus.Text = $"STATUS: {status}";
                        // Error handling in final state
                        if(final)
                        {
                            this.ApplicationPicBoxWait.Enabled = false;
                            this.ApplicationPicBoxWait.Visible = false;
                            if(this.ApplicationbtnUpdate.Visible)
                            { 
                                this.ApplicationbtnUpdate.Enabled = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"main: StatusMessageUI() - exception={ex.Message}");
                }
            };

            if (InvokeRequired)
            {
                BeginInvoke(mi);
            }
            else
            {
                Invoke(mi);
            }
        }

        #endregion

        /********************************************************************************************************/
        // DEVICE INTERACTION
        /********************************************************************************************************/
        #region -- device interaction --

        private void InitalizeDevice(bool unload = false)
        {
            // Unload Domain
            if (unload && appDomainCfg != null)
            {
                appDomainCfg.UnloadPlugin(appDomainDevice);

                // Test Unload
                appDomainCfg.TestIfUnloaded(devicePlugin);
            }

            appDomainCfg = new AppDomainCfg();

            // AppDomain Interface
            appDomainDevice = appDomainCfg.CreateAppDomain(MODULE_NAME);

            // Load Interface
            devicePlugin = appDomainCfg.InstantiatePlugin(appDomainDevice, ASSEMBLY_NAME, PLUGIN_NAME);

            // Initialize interface
            if (devicePlugin != null)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    this.ApplicationPicBoxWait.Enabled = true;
                    this.ApplicationPicBoxWait.Visible = true;
                    this.ApplicationlblStatus.Visible = true;
                }));

                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    Debug.WriteLine("\nmain: new device detected! +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n");

                    // Setup DeviceCfg Event Handlers
                    devicePlugin.OnDeviceNotification += new EventHandler<DeviceNotificationEventArgs>(this.OnDeviceNotificationUI);

                    System.Windows.Forms.Application.DoEvents();

                    try
                    {
                        // Initialize Device
                        devicePlugin.DeviceInit();
                        Debug.WriteLine("main: loaded plugin={0} ++++++++++++++++++++++++++++++++++++++++++++", (object)devicePlugin.PluginName);
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine("main: InitalizeDevice() exception={0}", (object)ex.Message);
                        if(ex.Message.Equals("NoDevice"))
                        {
                            WaitForDeviceToConnect(false);
                        }
                        else if (ex.Message.Equals("MultipleDevice"))
                        {
                            this.Invoke(new MethodInvoker(() =>
                            {
                                MessageBoxEx.Show(this, "Multiple Devices Detected\r\nDisconnect One of them !!!", "ERROR: MULTIPLE DEVICES DETECTED", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                WaitForDeviceToConnect(false);
                            }));
                        }
                        else if(ex.Message.Equals("UIADevice"))
                        {
                            try
                            {
                                this.Invoke(new MethodInvoker(() =>
                                {
                                    this.ApplicationlblStatus.Text += " => UIA DISCOVERY...";
                                }));

                                new Thread(() =>
                                {
                                    Thread.CurrentThread.IsBackground = true;
                                    devicePlugin.IdentifyUIADevice();
                                }).Start();
                            }
                            catch(Exception exc)
                            {
                                Debug.WriteLine($"main: InitalizeDevice() exception={exc.Message}");
                            }
                        }
                    }
                }).Start();
            }
        }

        private void WaitForDeviceToConnect(bool firmwareIsUpdating)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                this.ApplicationPicBoxWait.Enabled = true;
                this.ApplicationPicBoxWait.Visible = true;
            }));

            // Wait for a new device to connect
            new Thread(() =>
            {
                bool foundit = false;
                Thread.CurrentThread.IsBackground = true;

                Debug.Write("Waiting for new device to connect");

                string description = "";
                string deviceID = "";

                // Wait for a device to attach
                while (!formClosing && !foundit)
                {
                    foundit = devicePlugin.FindIngenicoDevice(ref description, ref deviceID);

                    if(!foundit)
                    {
                        Debug.Write(".");
                        Thread.Sleep(1000);
                    }
                }

                // Initialize Device
                if (!formClosing && foundit)
                {
                    Debug.WriteLine("found one with ID={0}", (object) deviceID);

                    Thread.Sleep(3000);

                    // Initialize Device
                    InitalizeDeviceUI(this, new DeviceNotificationEventArgs());
                }

            }).Start();
        }

        #endregion

        /********************************************************************************************************/
        // DEVICE ARTIFACTS
        /********************************************************************************************************/
        #region -- device artifacts --

        private void SetConfiguration()
        {
            Debug.WriteLine("main: update GUI elements =========================================================");

            this.ApplicationlblDeviceOS.Text = "UNKNOWN";
            this.ApplicationlblSerialNumber.Text = "UNKNOWN";
            this.ApplicationFirmwarelblVersion.Text = "UNKNOWN";
            this.ApplicationlblModelName.Text = "UNKNOWN";
            this.ApplicationlblPort.Text = "UNKNOWN";
            this.ApplicationPicBoxWait.Enabled = false;
            this.ApplicationPicBoxWait.Visible = false;
            this.ApplicationlblStatus.Visible = false;

            try
            {
                string[] config = devicePlugin.GetConfig();

                if (config != null)
                {
                    this.ApplicationlblDeviceOS.Text = config[0];
                    this.ApplicationlblSerialNumber.Text = config[1];
                    this.ApplicationFirmwarelblVersion.Text = config[2];
                    this.ApplicationlblModelName.Text = config[3];
                    this.ApplicationlblHardwareVersion.Text = config[4];

                    // value expected: either dashed or space separated
                    string [] worker = null;
                    if(config[5] != null)
                    {
                        if(config[5].Trim().Contains(' '))
                        {
                            worker = config[5].Trim().Split(' ');
                        }
                        else
                        {
                            worker = config[5].Trim().Split('-');
                        }
                    }
                    if(worker != null)
                    {
                        this.ApplicationlblPort.Text = worker[0];
                    }
                    else
                    {
                        this.ApplicationlblPort.Text = "UNKNOWN";
                    }
                    // Firmware update
                    if(config[0].Equals(Enum.GetName(typeof(DeviceOS), DeviceOS.UIA), StringComparison.CurrentCultureIgnoreCase))
                    {
                        if(config[2].StartsWith("13.", StringComparison.CurrentCultureIgnoreCase))
                        {
                            this.ApplicationlblUpdate.Text = $"UPDATE TO RBA {Enum.GetName(typeof(IngenicoRBA), IngenicoRBA.FDRC)}/{Enum.GetName(typeof(IngenicoRBA), IngenicoRBA.VITAL)}";
                            this.ApplicationrBtnFDRC.Checked = true;
                            this.ApplicationgrpBoxRBAUpdateVersion.Visible = true;
                        }
                        else
                        { 
                            this.ApplicationlblUpdate.Text = "UPDATE TO UIA v13.1.12";
                            this.ApplicationgrpBoxRBAUpdateVersion.Visible = false;
                        }
                        this.ApplicationgroupBoxFirmwareUpdate.Visible = true;
                        this.ApplicationlblUpdate.Visible = true;
                        this.ApplicationbtnUpdate.Visible = true;
                        this.ApplicationbtnUpdate.Enabled = true;
                    }
                    //20190723: to allow RBA FDRC -> VITAL (and viceversa), one of the EFT loads needs to be other than 007
                    //else
                    //{
                    //    this.ApplicationgrpBoxRBAUpdateVersion.Visible = true;
                    //    this.ApplicationgroupBoxFirmwareUpdate.Visible = true;
                    //    this.ApplicationlblUpdate.Text = $"UPDATE RBA ({Enum.GetName(typeof(IngenicoRBA), IngenicoRBA.FDRC)} / {Enum.GetName(typeof(IngenicoRBA), IngenicoRBA.VITAL)})";
                    //    this.ApplicationlblUpdate.Visible = true;
                    //    this.ApplicationrBtnFDRC.Checked = true;
                    //    this.ApplicationbtnUpdate.Visible = true;
                    //    this.ApplicationbtnUpdate.Enabled = true;
                    //}
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("main: SetConfiguration() exception={0}", (object)ex.Message);
            }
        }

        #endregion

        /********************************************************************************************************/
        // FORM ACTIONS
        /********************************************************************************************************/
        #region -- form actions --

        void UpdateUIAFirmware()
        {
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;

                    try 
                    { 
                        // UIA firmware update
                        devicePlugin.UpdateUIAFirmware();
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine("main: UpdateUIAFirmware() exception={0}", (object)ex.Message);
                        if(ex.Message.Contains("disconnected"))
                        {
                            UnloadDeviceConfigurationDomain(this, null);
                        }
                    }

                }).Start();
        }

        void UpdateRBAFirmware()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                try 
                { 
                    // RBA firmware update
                    devicePlugin.UpdateRBAFirmware(ApplicationrBtnFDRC.Checked ? Enum.GetName(typeof(IngenicoRBA), IngenicoRBA.FDRC)
                                                                               : Enum.GetName(typeof(IngenicoRBA), IngenicoRBA.VITAL));
                }
                catch(Exception ex)
                {
                    Debug.WriteLine("main: UpdateRBAFirmware() exception={0}", (object)ex.Message);
                    if(ex.Message.Contains("disconnected"))
                    {
                        UnloadDeviceConfigurationDomain(this, null);
                    }
                }
                //20190723: to allow RBA FDRC -> VITAL (and viceversa), one of the EFT loads needs to be other than 007
                finally
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        this.ApplicationgrpBoxRBAUpdateVersion.Visible = false;
                        this.ApplicationgroupBoxFirmwareUpdate.Visible = false;
                    }));
                }

            }).Start();
        }

        private void ApplicationbtnUpdate_Click(object sender, EventArgs e)
        {
            this.ApplicationPicBoxWait.Enabled = true;
            this.ApplicationPicBoxWait.Visible = true;
            this.ApplicationlblStatus.Visible = true;
            this.ApplicationbtnUpdate.Enabled = false;

            if(this.ApplicationlblUpdate.Text.IndexOf("RBA") >= 0)
            {
                UpdateRBAFirmware();
            }
            else
            {
                UpdateUIAFirmware();
            }
        }

        #endregion
    }
}
