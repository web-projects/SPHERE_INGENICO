using IPA.Core.Data.Entity;
using IPA.Core.Shared.Enums;
using IPA.DAL.RBADAL.Interfaces;
using IPA.DAL.RBADAL.Models;
using RBA_SDK;
using System;
using System.Linq;
using System.Threading;
using IPA.DAL.RBADAL.Services;
using IPA.Core.Shared.Helpers.StatusCode;
using System.Text;
using HidLibrary;
using System.Diagnostics;
using System.Configuration;

namespace IPA.DAL.RBADAL.Services
{
    public class DeviceIngenico : IDevice
    {
        #region -- member variables --
        private Ingenico.Device _ingenicoDevice;
        private Ingenico.Device.Health _deviceHealth;
        private Ingenico.Device.Info _deviceInfo;
        private HidDevice device = null;

        public const int IngenicoVendorID = 0x0b00;

        private string _attachedPort;
        private string[] _acceptedPorts;
        private string[] _availablePorts;
        //private string _tagData;
        
        private const string CancelButton = "\u001b";

        bool Connected;
        internal static string port250;
        internal static string port320;
        internal static string port350;
        internal static string port480;

        readonly Device _device = new Device();

        #endregion

        #region  --Event Subscribers --
        public event EventHandler<NotificationEventArgs> OnNotification = delegate { };
        public delegate void EventHandler(object sender, NotificationEventArgs args);
        #endregion

        #region -- public methods --

        void IDevice.Init(string[] accepted, string[] available, int baudRate, int dataBits)
        {
            _acceptedPorts = accepted;
            _availablePorts = available;

            _ingenicoDevice = new Ingenico.Device();
            _deviceHealth = new Ingenico.Device.Health();
            _deviceInfo = new Ingenico.Device.Info();

            //Add event handlers
            _ingenicoDevice.ComBaudRate = baudRate;
            _ingenicoDevice.ComDataBits = dataBits;
            _ingenicoDevice.DeviceInputReceived += (sender3, deviceArgs) => DeviceInputReceived(deviceArgs.MessageId, deviceArgs.DeviceForm, deviceArgs.KeyPressId);
            _ingenicoDevice.DeviceConnectionChanged += (sender4, deviceConnectionArgs) => UpdateDeviceIngenico(deviceConnectionArgs.ConnectionStatus);

            port250 = ConfigurationManager.AppSettings["isc2xxCOMPort"] ?? "35";
            port480 = ConfigurationManager.AppSettings["isc350COMPort"] ?? "35";
            port350 = ConfigurationManager.AppSettings["ipp3xx/ipp4xxCOMPort"] ?? "35";
            port320 = ConfigurationManager.AppSettings["ipp3xx / ipp4xxCOMPort"] ?? "35";
        }

        public virtual void Configure(object[] settings)
        {
        }

        public string GetSerialNumber()
        {
            _deviceInfo.GetDeviceInfo();
            string result = _deviceInfo.UNIT_SERIAL_NUMBER;
            return result;
        }
        DeviceStatus IDevice.Connect(bool transactionalMode)
        {
            try
            {
                //initialize attached device because application detected USB device change
                _attachedPort = string.Empty;

                var ports = _availablePorts.Where(s => _acceptedPorts.Contains(s)).Select(s => s).Distinct();

                if (!ports.Any())//no matching port
                {
                    return DeviceStatus.WrongComPort;
                }

                foreach (var port in ports)
                {
                    //this is the attached device and it is already known
                    if (String.Equals(_attachedPort, port, StringComparison.Ordinal))
                        return DeviceStatus.Connected;

                    if (ConnectToDevice(port, transactionalMode))
                    {
                        _attachedPort = port;
                        return DeviceStatus.Connected;
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"IDevice::Connected() exception={ex.Message}");
            }

            if (!_ingenicoDevice.EncryptionKeyFound)
                return DeviceStatus.NoEncryption;
            else if (!_ingenicoDevice.EncryptionEnabled)
                return DeviceStatus.EncyptionDisabled;

            return DeviceStatus.NoDevice;
        }

        public string Connect(string port)
        {
            if (Connected)
            {
                return "Already Connected";
            }
            ERROR_ID InitResult = RBA_API.Initialize();
            if (!InitResult.ToString().Contains("SUCCESS"))
            {
                Debug.WriteLine("Initialize Failed.");
            }

            ERROR_ID Result = SetDeviceCommunications(port);

            if (Result.ToString().Contains("SUCCESS"))
            {
                Connected = true;
            }
            else
            { 
               Debug.WriteLine($"Connect failed: {Result.ToString()}.");
            }

            return Result.ToString();
        }

        void IDevice.Disconnect()
        {
            _ingenicoDevice?.Offline();
            _ingenicoDevice?.OnDemandReset();

            //_ingenicoDevice?.RequestForm(Ingenico.Device.DeviceForms.Message,
            //    Ingenico.Device.DeviceFormTypeOf.Text, Ingenico.Device.DeviceFormElementID.PromptLine1,
            //    "TC IPADAL Shutdown");

            //TODO: is this required on disconnect? only on application shutdown
            _ingenicoDevice?.ProcessMessage(MESSAGE_ID.M24_FORM_ENTRY);

            Thread.Sleep(1000);

            _ingenicoDevice?.Offline();
            _ingenicoDevice?.Disconnect();
        }

        bool IDevice.Reset()
        {
            if (_ingenicoDevice.Connected)
            {
                _ingenicoDevice.OnDemandReset();
                _ingenicoDevice.Offline();
            }
            return true;
        }

        public void GoOffline(string model)
        {
            try
            {
                string originalPortNumber = ConfigurationManager.AppSettings[$"{model}COMPort"];
                if (Connected || Connect(originalPortNumber).ToLower().Contains("success"))
                {
                    /*If the device has RBA already installed, the process leaves it in a "swipe card" state. Reset to the ready for transaction state.*/
                    //Non Reflection Based Code (The RBA_SDK_CS.dll is referenced in the references section).
                    //RBA_SDK.ERROR_ID errorId = IngenicoManager.SetDeviceCommunications(originalPortNumber);
                    if (_ingenicoDevice?.Offline() == RBA_SDK.ERROR_ID.RESULT_SUCCESS) //Offline is called only if the connection to the device succeeded.
                        Debug.WriteLine("RBA Payment Device set to Offline.");
                    else
                        Debug.WriteLine("Unable to set RBA Payment Device to Offline.");
                }
                else
                {
                    //LoggingManager.ConsoleWriteLine("Unable to Set RBA to Offline.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to connect to the RBA Ingeico Device: {ex.Message}");
            }
            finally
            {
                _ingenicoDevice?.Disconnect();
            }
        }

        public bool GetRBAVersion(ref string model, out string rbaVersion, bool minimalOutput = false, bool portAlreadyReset = false)
        {
            bool output = false;
            rbaVersion = string.Empty;
            try
            {
                if (!minimalOutput)
                { 
                    Debug.WriteLine("Checking for RBA device...");
                }
                string expectedTcComPort = ConfigurationManager.AppSettings[$"{model}COMPort"] ?? ExpectedTcComPort(model).ToString();
                bool connectSuccess = Connect(expectedTcComPort).ToLower().Contains("success");
                if (!connectSuccess)
                {
                    //Try Connecting on Any Port
                    var ports = System.IO.Ports.SerialPort.GetPortNames();
                    string expectedPorts = "COM35,COM109,COM110,COM111,COM112,COM113,COM34,COM33";
                    string portToTry = string.Empty;
                    foreach (string port in ports)
                    {
                        if (expectedPorts.Contains(port))
                        {
                            if (Connect(port).ToLower().Contains("success"))
                            {
                                connectSuccess = true;
                                break;
                            }
                        }
                    }
                }
                if (connectSuccess)
                {
                    if (RBA_API.SetParam(PARAMETER_ID.P08_REQ_REQUEST_TYPE, "0") == RBA_SDK.ERROR_ID.RESULT_SUCCESS)
                    {
                        if (RBA_API.ProcessMessage(MESSAGE_ID.M08_HEALTH_STAT) == RBA_SDK.ERROR_ID.RESULT_SUCCESS)
                        {
                            string OS_VERSION = RBA_API.GetParam(PARAMETER_ID.P08_RES_OS_VERSION);
                            string APP_VERSION = RBA_API.GetParam(PARAMETER_ID.P08_RES_APP_VERSION);
                            string SECURITY_LIB_VERSION = RBA_API.GetParam(PARAMETER_ID.P08_RES_SECURITY_LIB_VERSION);
                            model = RBA_API.GetParam(PARAMETER_ID.P08_RES_DEVICE_NAME);
                            rbaVersion = $"{OS_VERSION} - {APP_VERSION} - {SECURITY_LIB_VERSION}";
                            output = true;
                        }
                    }
                }
                else
                {
                    /*If the device has RBA already installed, the process leaves it in a "swipe card" state. Reset to the ready for transaction state.*/
                    if (!minimalOutput)
                    { 
                        Debug.WriteLine("Unable to Connect as RBA device.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to connect to the RBA Ingeico Device: {ex.Message}");
            }
            finally
            {
                GoOffline(model); //GoOffline submits both an Offline Message and a Disconnect.
            }
            if (rbaVersion.Length > 0 && !portAlreadyReset)
            {
//                if ((model.Contains("320") && !Utility.PortAvailable(FileManager.port320))
//                || (model.Contains("350") && !Utility.PortAvailable(FileManager.port350))
//                || (model.Contains("480") && !Utility.PortAvailable(FileManager.port480))
//                || (model.Contains("250") && !Utility.PortAvailable(FileManager.port250)))
                {
//                    ExecuteSetPorts(model);
                    GetRBAVersion(ref model, out rbaVersion, minimalOutput, true);
                }
            }

            return output;
        }

        public ERROR_ID SetDeviceCommunications(string port)
        {
            SETTINGS_COMMUNICATION CommSet = new SETTINGS_COMMUNICATION();
            SETTINGS_COMM_TIMEOUTS CommTimeouts;
            uint comm_timeouts;

            comm_timeouts = 5000;

            CommTimeouts.ConnectTimeout = comm_timeouts;
            CommTimeouts.ReceiveTimeout = comm_timeouts;
            CommTimeouts.SendTimeout = comm_timeouts;

            RBA_API.SetCommTimeouts(CommTimeouts);

            CommSet.interface_id = (uint)COMM_INTERFACE.SERIAL_INTERFACE;
            if (!port.ToUpper().StartsWith("COM"))
                port = "COM" + port;
            CommSet.rs232_config.ComPort = port;

            // From TCIAPDAL.exe.config
            int ComBaudRate = 115200;
            int ComDataBits = 8;
            CommSet.rs232_config.BaudRate = Convert.ToUInt32(ComBaudRate);
            CommSet.rs232_config.DataBits = Convert.ToUInt32(ComDataBits);
            CommSet.rs232_config.Parity = (uint)0;
            CommSet.rs232_config.StopBits = Convert.ToUInt32(1);
            CommSet.rs232_config.FlowControl = (uint)0;

            //Connect to pin pad
            RBA_SDK.ERROR_ID Result = RBA_API.Connect(CommSet);
            return Result;
        }

        #endregion

        #region -- public properties --

        Core.Data.Entity.Device IDevice.DeviceInfo => new Core.Data.Entity.Device
        {
            ManufacturerID = (int)DeviceManufacturer.Ingenico,
            SerialNumber = _deviceHealth.SERIAL_NUMBER,
            FirmwareVersion = _deviceHealth.APP_VERSION,
            ModelName = _deviceHealth.DEVICE_NAME,
            OSVersion = _deviceHealth.OS_VERSION,
            AssetNumber = _deviceInfo.MANUFACTURING_SERIAL_NUMBER,
            AttachedPort = _attachedPort
        };

        Model IDevice.ModelInfo => new Model
        {
            ManufacturerID = (int)DeviceManufacturer.Ingenico,
            ModelNumber = _deviceInfo.DEVICE,
        };

        bool IDevice.Connected => _attachedPort != null;

        private EntryModeStatus PrepareGetCommand(byte inputToken, out byte[] output)
        {
            var commandLine = new byte[5];
            commandLine[0] = (byte)Token.STK;
            commandLine[1] = (byte)Token.R;
            commandLine[2] = inputToken;
            commandLine[3] = (byte)Token.ETK;
            commandLine[4] = 0x00;
            commandLine[4] = GetCheckSumValue(commandLine);

            return SetupCommand(commandLine, out output);
        }

        private EntryModeStatus SetupCommand(byte[] command, out byte[] response)
        {
            var status = EntryModeStatus.Success;
            const int bufferLength = 1000;
            var deviceDataBuffer = new byte[bufferLength];
            response = null;

            try
            {
                for (int i = 0; i < bufferLength; i++)
                {
                    deviceDataBuffer[i] = 0;
                }

                if (status == EntryModeStatus.Success)
                {
                    int featureReportLen = device.Capabilities.FeatureReportByteLength;
                    //WriteFeatureData works better if we send the entire feature length array, not just the length of command plus checksum
                    var reportBuffer = new byte[featureReportLen];
                    //Assume featureCommand is not 0 prepended, and contains a checksum.
                    var zeroReportIdCommand = new byte[Math.Max(command.Length + 2, featureReportLen)];
                    //Prepend 0x00 to command[...] since HidLibrary expects Features to start with reportID, and we use 0.
                    zeroReportIdCommand[0] = 0x00;
                    Array.Copy(command, 0, zeroReportIdCommand, 1, command.Length);

                    var result = false;
                    if (device.WriteFeatureData(zeroReportIdCommand))
                    {
                        Thread.Sleep(1200); //Emperical data shows this is a good time to wait.
                        result = ReadFeatureDataLong(out deviceDataBuffer);
                    }


                    if (result || deviceDataBuffer.Length > 0)//as long as we have data in result, we are ok with failed reading later.
                    {
                        int dataIndex;
                        for (dataIndex = bufferLength - 1; dataIndex > 1; dataIndex--)
                        {
                            if (deviceDataBuffer[dataIndex] != 0)
                                break;
                        }

                        response = new byte[dataIndex + 1];

                        for (var ind = 0; ind <= dataIndex; ind++)
                        {
                            response[ind] += deviceDataBuffer[ind];
                        }

                        status = EntryModeStatus.Success;
                    }
                    else
                        status = EntryModeStatus.CardNotRead;
                }
            }
            catch (Exception)
            {
                status = EntryModeStatus.Error;
            }

            return status;
        }

        public bool ReadFeatureDataLong(out byte[] resBuffer, byte reportId = 0x00)
        {
            bool success = false;
            resBuffer = new byte[1000];
            if (device != null && device.IsConnected)
            {
                bool isFirstNonZeroBlock = false;
                int responseLength = 0;
                int reportLength = device.Capabilities.FeatureReportByteLength;
                byte[] reportBuffer = new byte[reportLength];
                try
                {
                    // Get response data from HID Device
                    success = true;
                    for (int k = 0; k < 100 && success; k++)  // 1 second in total
                    {
                        for (int indx = 0; indx < reportBuffer.Length; indx++)
                            reportBuffer[indx] = 0;

                        success = device.ReadFeatureData(out reportBuffer, reportId);
                        if (success)
                        {
                            for (int i = 0; i < reportLength; i++)
                            {
                                if (reportBuffer[i] != 0)
                                {
                                    isFirstNonZeroBlock = true;
                                    break;
                                }
                            }
                        }

                        // Pack the data after first non zero data block 
                        if (isFirstNonZeroBlock)
                        {
                            Array.Copy(reportBuffer, 1, resBuffer, responseLength, reportLength - 1);
                            responseLength += reportLength - 1;
                        }

                        if (responseLength + reportLength > resBuffer.Length)
                        {
                            success = false;
                        }

                        Thread.Sleep(10);
                    }
                }
                catch (Exception xcp)
                {
                    throw xcp;
                }
            }
            return success;
        }

        public virtual string GetFirmwareVersion()
        {
            // declare variables
            string firmwareVersion = null;

            // setup the command to get the firmware version
            var getFirmware = new byte[CommandTokens.ReadFirmwareVersion.Length + 1];
            Array.Copy(CommandTokens.ReadFirmwareVersion, getFirmware, CommandTokens.ReadFirmwareVersion.Length);
            getFirmware[CommandTokens.ReadFirmwareVersion.Length] = 0x00;
            getFirmware[getFirmware.Length - 1] = GetCheckSumValue(getFirmware);

            //execute the command
            var status = SetupCommand(getFirmware, out byte[] result);
            if (status == EntryModeStatus.Success && result[0] == (byte)Token.ACK)
            {
                firmwareVersion = Encoding.ASCII.GetString(result);
            }

            return firmwareVersion;
        }

        #endregion 

        #region --= event handlers --

        private void DeviceInputReceived(MESSAGE_ID message, string deviceForm, string keyPressId)
        {
            /*try
            {
                switch (message)
                {
                    case (MESSAGE_ID.M01_ONLINE):
                        {
                            DeviceOnline(deviceForm, keyPressId);

                            break;
                        }
                    case (MESSAGE_ID.M04_SET_PAYMENT_TYPE):
                        {
                            _ingenicoDevice.SetAmount(Transaction.PaymentXO.Request.PaymentRequest.AmountRequested.ToString(), string.Empty);

                            break;
                        }
                    case (MESSAGE_ID.M13_AMOUNT):
                        {
                            _tagData = _ingenicoDevice.GetFinalTagData();

                            CardReadComplete();

                            break;
                        }
                    case (MESSAGE_ID.M09_SET_ALLOWED_PAYMENT):
                        {
                            //bypass if this is a canceled transaction
                            if (Transaction.PaymentXO?.Request?.CreditCard?.AbortType == DeviceAbortType.UserCancel ||
                                Transaction.PaymentXO?.Request?.CreditCard?.AbortType == DeviceAbortType.BadRead)
                                return;

                            //check to make sure that the card removal is only expect for an EMV payment when the reader used was the smartcard reader
                            if (Transaction.PaymentXO.Request.Payment.IsEMV && String.Equals(_ingenicoDevice.CardSource, ReaderStates.CHIP_READ, StringComparison.Ordinal))
                            {

                                //check to see if the device returned 33_05 - if so - good to remove otherwise unexpected removal
                                if (!string.IsNullOrWhiteSpace(_ingenicoDevice.pT.Tag1003))
                                {
                                    CardReadComplete();
                                }
                            }

                            break;
                        }
                    case (MESSAGE_ID.M10_HARD_RESET):
                        {
                            if (String.Equals(keyPressId, "EMVAmtVerifyNo", StringComparison.Ordinal))
                            {
                                //process as cancel payment
                                DeviceCancelTransaction();
                            }
                            break;
                        }
                    case (MESSAGE_ID.M20_SIGNATURE):
                        {
                            //Get the _signature data
                            DeviceSignature();

                            break;
                        }
                    case (MESSAGE_ID.M23_CARD_READ):
                        {
                            DeviceCardRead(keyPressId);

                            break;
                        }
                    case (MESSAGE_ID.M24_FORM_ENTRY):
                        {
                            if (String.Equals(_ingenicoDevice.CurrentForm, "PAY1.K3Z", StringComparison.Ordinal))
                            {
                                //check to see if the keyPressID was debit - if so, show pin entry form
                                //A - Debit
                                //B - Credit
                                switch (keyPressId)
                                {
                                    case ("A"):
                                        {
                                            DisplayPINForm();

                                            return;
                                        }
                                    case ("B"):
                                        {
                                            DisplayLineItemAmount();

                                            //determine if the Amount Form should be shown
                                            if (!Core.Client.DataAccess.Helpers.TCCustAttribute.GetTCCustAttributeVal<bool>(TCCustAttributeNameEnum.VerifyAmountEnabled, _tcCustAttributes))
                                            {
                                                CardReadComplete();
                                                return;
                                            }

                                            DisplayVerifyAmount();

                                            return;
                                        }
                                    //TODO: verify valid for all device types
                                    case (CancelButton):
                                        {
                                            //Canceled
                                            DeviceCancelTransaction();
                                            return;
                                        }
                                }
                            }

                            if (String.Equals(_ingenicoDevice.CurrentForm, "AMTV.K3Z", StringComparison.Ordinal))
                            {
                                //yes button on screen and enter (green) button on device
                                if (String.Equals(keyPressId, "Y", StringComparison.Ordinal) || String.Equals(keyPressId, "\r", StringComparison.Ordinal))
                                {
                                    if (_isEmv && !string.IsNullOrWhiteSpace(_ingenicoDevice.pT.Tag1003))
                                    {
                                        _tagData = _ingenicoDevice.GetFinalTagData();
                                    }
                                    else
                                    {
                                        CardReadComplete();
                                    }

                                }
                                //now testing for enabled cancel button on the device
                                else if (String.Equals(keyPressId, "N", StringComparison.Ordinal) || String.Equals(keyPressId, CancelButton, StringComparison.Ordinal))
                                {
                                    //Canceled
                                    DeviceCancelTransaction();
                                    return;
                                }
                            }

                            if (String.Equals(_ingenicoDevice.CurrentForm, "PIN.K3Z", StringComparison.Ordinal))
                            {
                                //need to get the encrypted PIN
                                DisplayLineItemAmount();

                                //determine if the Amount Form should be shown
                                if (!Core.Client.DataAccess.Helpers.TCCustAttribute.GetTCCustAttributeVal<bool>(TCCustAttributeNameEnum.VerifyAmountEnabled, _tcCustAttributes))
                                {
                                    CardReadComplete();
                                    return;
                                }

                                DisplayVerifyAmount();

                            }

                            if (String.Equals(keyPressId, CancelButton, StringComparison.Ordinal))
                            {
                                //Canceled
                                DeviceCancelTransaction();
                            }

                            break;
                        }

                    case (MESSAGE_ID.M31_PIN_ENTRY):
                        {
                            if (keyPressId.Contains("P31_RES_STATUS: 1"))
                            {
                                DeviceCancelTransaction();
                                return;
                            }
                            //IsCredit = true;
                            DisplayLineItemAmount();

                            //determine if the Amount Form should be shown
                            if (!Core.Client.DataAccess.Helpers.TCCustAttribute.GetTCCustAttributeVal<bool>(TCCustAttributeNameEnum.VerifyAmountEnabled, _tcCustAttributes))
                            {
                                CardReadComplete();
                                return;
                            }

                            DisplayVerifyAmount();

                            return;
                        }
                    case (MESSAGE_ID.M33_01_EMV_STATUS):
                        {
                            break;
                        }
                    case (MESSAGE_ID.M33_02_EMV_TRANSACTION_PREPARATION_RESPONSE):
                        {
                            if (!CheckTransactionTimerEnabled()) // If transaction already done, we no longer process the incoming input.
                            {
                                return;
                            }

                            DisplayLineItemAmount();

                            _ingenicoDevice.SetPaymentType("B", ConvertDecimalAmountToCents());
                            _ingenicoDevice.SetAmount(ConvertDecimalAmountToCents(), string.Empty);
                            
                            OnNotification(this, new NotificationEventArgs
                            {
                                NotificationType = NotificationType.UI,
                                UI = new Models.UI
                                {
                                    Title = Core.Client.DataAccess.Shared.StatusCode.GetDisplayMessage((int)DisplayMessage.DALWindowTitle),
                                    Text = "Please verify amount",
                                    UserControl = UserControls.Message
                                },
                                DisableClose = true
                            });
                            
                            break;
                        }
                    case (MESSAGE_ID.M33_03_EMV_AUTHORIZATION_REQUEST):
                        {
                            Thread.Sleep(1000);

                            _ingenicoDevice.SendEMVAuthorizationResponse(0x1004, "0");

                            OnNotification(this, new NotificationEventArgs
                            {
                                NotificationType = NotificationType.UI,
                                UI = new Models.UI
                                {
                                    Title = Core.Client.DataAccess.Shared.StatusCode.GetDisplayMessage((int)DisplayMessage.DALWindowTitle),
                                    Text = "Processing...",
                                    UserControl = UserControls.Message
                                },
                                DisableClose = false
                            });
                            ProcessEMVAuth();

                            break;
                        }
                    case (MESSAGE_ID.M33_05_EMV_AUTHORIZATION_CONFIRMATION):
                        {
                            //check to see if D1003 is in error
                            if (_ingenicoDevice.pT.Tag1003.Contains("Error") && String.Equals(_ingenicoDevice.CurrentForm, "ECONFIRM.K3Z", StringComparison.Ordinal))
                            {
                                //look through other statuses to figure out what might have happened
                                DeviceCancelTransaction();
                                return;
                            }
                            else if (_ingenicoDevice.pT.Tag1010?.Contains("Error response code = CNSUP") ?? false)
                            {
                                //enter EMVFallback mode
                                Transaction.PaymentXO.Request.ReadAttempts = Configuration.GetConfigValue<int>(ConfigTypeEnum.EmvRetryAttempts, Data.CompanyConfigs) - 2;
                                Transaction.PaymentXO.Request.CreditCard.EMVFallBackTypeID = (int)IPA.Core.Shared.Enums.EMVFallBackType.MSR_STATUS_ID;
                                ((IDevice)this).BadRead();
                                return;
                            }
                            else if (_ingenicoDevice.pT.Tag1003.Contains("Error"))
                            {
                                //look through other statuses to figure out what might have happened
                                ((IDevice)this).BadRead();
                                return;
                            }
                            
                            if (Transaction.PaymentXO.Request.CreditCard.AbortType != DeviceAbortType.UserCancel)
                            {
                                //remove card
                                _ingenicoDevice.ShowMessage("Please remove card");


                                OnNotification(this, new NotificationEventArgs
                                {
                                    NotificationType = NotificationType.UI,
                                    UI = new Models.UI
                                    {
                                        Title = Core.Client.DataAccess.Shared.StatusCode.GetDisplayMessage((int)DisplayMessage.DALWindowTitle),
                                        Text = "Please remove card",
                                        UserControl = UserControls.Message
                                    },
                                    DisableClose = false
                                });

                                //check to see of the credit card object has the encrypted track - if not request here
                                //not sure if this is valid logic - adding to support Collis Test Case AXP004 - since it did not fire a 33.03 message
                                if (string.IsNullOrWhiteSpace(Transaction.PaymentXO.Request.CreditCard.EncryptedTracks))
                                    ProcessEMVAuth();

                                _ingenicoDevice.SetEMVFinal();
                            }

                            break;
                        }
                    case (MESSAGE_ID.M33_11_EMV_EXTERNAL_AID_SELECT_NOTIFICATION):
                        {
                            _ingenicoDevice.SendDetectedAIDs(_ingenicoDevice.AIDs);

                            break;
                        }
                    case (MESSAGE_ID.M87_E2EE_CARD_READ):
                        {
                            //this defends against EMV fallback and someone tries to perform a dip again
                            if (Transaction.PaymentXO.Request.EMVFallback && String.Equals(_ingenicoDevice.CardSource, ReaderStates.CHIP_READ, StringComparison.Ordinal))
                            {
                                ((IDevice)this).BadRead();
                                return;
                            }

                            if (String.Equals(keyPressId, "1", StringComparison.Ordinal))
                            {
                                //bad card read - set abort type and return back 
                                if (Transaction.PaymentXO.Request.Payment.IsEMV)
                                    Device.badUnknownCardTypeReadAttempts++;
                                ((IDevice)this).BadRead();
                                return;
                            }

                            if (String.Equals(keyPressId, "2", StringComparison.Ordinal))
                            {
                                DeviceCancelTransaction();
                                return;
                            }

                            //if read is good make sure to set the AbortType
                            Transaction.PaymentXO.Request.CreditCard.AbortType = DeviceAbortType.NoAbort;

                            //Check to see if this is an EMV transaction - check for the smartcard reader
                            if (Transaction.PaymentXO.Request.Payment.IsEMV && Transaction.PaymentXO.Request.PaymentEntryMode != Core.Shared.Enums.EntryModeType.Keyed)
                            {
                                if (!Transaction.PaymentXO.Request.EMVFallback)
                                {
                                    bool EMVReady = DetectEMV();
                                    if (EMVReady)
                                    {
                                        Transaction.PaymentXO.Request.PaymentEntryMode = Core.Shared.Enums.EntryModeType.EMVChipRead;
                                        return;
                                    }
                                }
                            }

                            //if no debit then credit - no need to show on device
                            if (!Core.Client.DataAccess.Helpers.TCCustAttribute.GetTCCustAttributeVal<bool>(TCCustAttributeNameEnum.DebitEnabled, _tcCustAttributes)
                               || (Transaction.PaymentXO.Request.PaymentEntryMode == Core.Shared.Enums.EntryModeType.Keyed))
                            {
                                //determine if the Amount Form should be shown
                                if (!Core.Client.DataAccess.Helpers.TCCustAttribute.GetTCCustAttributeVal<bool>(TCCustAttributeNameEnum.VerifyAmountEnabled, _tcCustAttributes))
                                {
                                    CardReadComplete();
                                    return;
                                }
                                else
                                {
                                    //IsCredit = true;
                                    DisplayLineItemAmount();

                                    DisplayVerifyAmount();
                                    return;
                                }
                            }

                            //set payment data
                            _ingenicoDevice.SelectPaymentType(Core.Client.DataAccess.Helpers.TCCustAttribute.GetTCCustAttributeVal<bool>(TCCustAttributeNameEnum.DebitEnabled, _tcCustAttributes));
                            DisplayCreditDebit();

                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                if (String.Equals(ex.Message, "Payment Canceled", StringComparison.Ordinal))
                {
                    //TODO: Determine why there is not a payment record, it generates a null exception
                    //Payment.Response.ErrorMessages.Add(e.Message);
                }
                else
                {
                    //Payment.Response.ErrorMessages.Add("Payment Transaction Failed.");
                    _ingenicoDevice.ShowMessage("Payment Transaction Failed");

                    IPA.Core.Client.DataAccess.Shared.Logging.logEntry(ex, Core.Shared.Helpers.StatusCode.DisplayResults.ProcessPaymentError, LoggingOption.File);

                    NotificationRaise(new NotificationEventArgs { NotificationType = NotificationType.DeviceEvent, DeviceEvent = DeviceEvent.DeviceError, Message = "Device Error Detected - Transaction Canceled" });

                }

                //null out the credit card information so that error handling above handles this properly
                if (Transaction.PaymentXO?.Request?.CreditCard != null)
                    Transaction.PaymentXO.Request.CreditCard = null;

                ((IDevice)this).Process(DeviceProcess.Reset);

            }*/
        }

        private void UpdateDeviceIngenico(CONNECTION_STATUS connectionStatus)
        {
            if (connectionStatus == CONNECTION_STATUS.CONNECTED)
            {
                _ingenicoDevice.Connected = true;
                
                if (_attachedPort == null)
                {
                    ((IDevice)this).Connect(true);
                }
                OnNotification(this, new NotificationEventArgs { NotificationType = NotificationType.Log, Message = $"Device Connect detected at: { DateTime.Now.ToLongDateString()} {Environment.NewLine}" });
            }

            if (connectionStatus == CONNECTION_STATUS.DISCONNECTED)
            {
                _attachedPort = null;

                _ingenicoDevice.Connected = false;

                //write disconnection to log so we can track it
                OnNotification(this, new NotificationEventArgs { NotificationType = NotificationType.Log, Message = $"Device Disconnect detected at: {DateTime.Now.ToLongDateString()} {Environment.NewLine}"  });

                //notify DAL that the device is disconnected
                NotificationRaise(new NotificationEventArgs { NotificationType = NotificationType.DeviceEvent, DeviceEvent = DeviceEvent.DeviceDisconnected });
            }
        }

        public void NotificationRaise(NotificationEventArgs e)
        {
            OnNotification?.Invoke(null, e);
        }

        #endregion

        #region -- Ingenico stuff --

        private bool ConnectToDevice(string port, bool transactionalMode)
        {
            bool returnVal = false;
            string version_file_txt = @"/HOST/FORMSVER.TXT";

            if (_acceptedPorts.Any(ap => ap == port))
            {

                //call Ingenico device libraries
                _ingenicoDevice.Connect(port);

                //check to see if the device is connected
                if (_ingenicoDevice.Connected)
                {
                    //reset any latent device values
                    _ingenicoDevice.OnDemandReset();

                    //enable partnumber look up
                    _ingenicoDevice.WriteConfiguration("13", "23", "1");

                    _deviceHealth = new Ingenico.Device.Health();
                    _deviceHealth.GetDeviceHealth();

                    _deviceInfo = new Ingenico.Device.Info();
                    _deviceInfo.GetDeviceInfo();

                    //InitializeDeviceXORequest();

                    //check time and date configuration
                    //SetDeviceDate();
                    //SetDeviceTime();

                    //check device type - if v4 set configuration for device reboot time
                    //if (IsV4Device() && transactionalMode)
                    //{
                    //    Set24Reboot();
                    //}
                    
                    GetDeviceVersionText(version_file_txt);

                    //SetDeviceXORequest(port);

                    _ingenicoDevice.Offline();

                    //_ingenicoDevice.ShowMessage("TC IPADAL Initialization");

                    Thread.Sleep(2000);

                    _ingenicoDevice.Offline();

                    returnVal = true;
                }
            }
            //DetectFailureModesAndLog();

            return returnVal;
        }

        private string GetDevicePartNumber()
        {
            string[] modelPartno = _deviceInfo.DEVICE?.Split('-');
            if (modelPartno.Length > 1)
                return modelPartno[1];
            else
                return string.Empty;
        }

        private void GetDeviceVersionText(string versionFileTxt)
        {
            //this will retrieve the formsver.txt file from the device for inspection
            string resultMessage = _ingenicoDevice.GetFileFromDevice(versionFileTxt);

            try
            {
                //file retrieved
                if (String.Equals(resultMessage, "0", StringComparison.Ordinal))
                {
                    //Data.DeviceXO.Request.Device.FormsVersion = _ingenicoDevice.FormsVersion;
                }
                //file not found
                else if (String.Equals(resultMessage, "1", StringComparison.Ordinal))
                {
                    OnNotification(this, new NotificationEventArgs { NotificationType = NotificationType.Log, Message = $"File not found {versionFileTxt} on device" });
                }
                //file format incompatible with the dataformat provided
                else if (String.Equals(resultMessage, "2", StringComparison.Ordinal))
                {
                    OnNotification(this, new NotificationEventArgs { NotificationType = NotificationType.Log, Message = $"File {versionFileTxt} not compatible" });
                }
                //all others are generic IO errors
                else
                {
                    OnNotification(this, new NotificationEventArgs { NotificationType = NotificationType.Log, Message = $"Generic FileIO error accessing  {versionFileTxt} "});
                }
            }
            catch (Exception ex)
            {
                OnNotification(this, new NotificationEventArgs { NotificationType = NotificationType.Log, Message = $"File not found {versionFileTxt} on device {Environment.NewLine}{ex.InnerException}" });
            }
        }

        #endregion

        #region -- Private Methods --
        private static byte GetCheckSumValue(byte[] dataBytes)
        {
            return dataBytes.Aggregate<byte, byte>(0x0, (current, t) => (byte)(current ^ t));
        }
        private int IntFromString(string stringVal, int defaultVal)
        {
            if (Int32.TryParse(stringVal, out int intVal))
                return intVal;
            return defaultVal;
        }
        private int ExpectedTcComPort(string model)
        {
            if (string.Equals("isc350", model, StringComparison.OrdinalIgnoreCase) || model.Contains("480"))
                return IntFromString(port480, 35);
            if (model.Contains("320"))
                return IntFromString(port320, 35); ;
            if (model.Contains("350") || model.IndexOf("ipp3xx/ipp4xx", 0, StringComparison.OrdinalIgnoreCase) > -1)
                return IntFromString(port350, 35); ;
            return IntFromString(port250, 35); ;
        }

        #endregion
    }

    public class DeviceInfo
    {
        public string SerialNumber;
        public string FirmwareVersion;
        public string EMVKernelVersion;
        public string ModelName;
        public string ModelNumber;
        public string Port;
        public byte[] ConfigValues;

        public SecurityLevelNumber SecurityLevel;
    }
}
