using RBA_SDK;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using IPA.DAL.RBADAL.Helpers;

namespace IPA.DAL.RBADAL.Ingenico
{

    public static class DeviceHandler
    {
        public static void Raise<T>(this EventHandler<T> handler, object sender, T args) where T : EventArgs
        {
            EventHandler<T> copy = handler;
            if (copy != null)
            {
                copy(sender, args);
            }
        }
    }

    public class Device
    {
        public delegate void TerminalDiscoveryCallback();

        public event EventHandler<DeviceEventArgs> DeviceInputReceived;

        public event EventHandler<DeviceConnectionArgs> DeviceConnectionChanged;
        
        public enum DeviceOS
        {
            [StringValue("UIA")]
            UIA,
            [StringValue("RBA")]
            RBA
        }

        public class DeviceEventArgs : EventArgs
        {
            public MESSAGE_ID MessageId { get; }

            public string DeviceForm { get; }

            public string KeyPressId { get; }


            public DeviceEventArgs(MESSAGE_ID messageId, string deviceForm, string keyPressId)
            {
                MessageId = messageId;
                DeviceForm = deviceForm;
                KeyPressId = keyPressId;
            }

        }

        public class DeviceConnectionArgs : EventArgs
        {
            public CONNECTION_STATUS ConnectionStatus { get; }

            public DeviceConnectionArgs(CONNECTION_STATUS connectionStatus)
            {
                ConnectionStatus = connectionStatus;
            }
        }

        #region -- ENUMS --
        public enum DeviceForms
        {
            [StringValue("MSG.K3Z")]
            Message = 1,
            [StringValue("AMTV.K3Z")]
            Amount = 2,
            [StringValue("COD.K3Z")]
            CardReadRequest = 3,
            [StringValue("APPDAPP.K3Z")]
            ApprovedDeclined = 4,
            [StringValue("ADS.K3Z")]
            Adverstisement = 5,
            [StringValue("OFFILINE.K3Z")]
            Offline = 6,
            [StringValue("SWIPE.K3Z")]
            Swipe = 7,
            [StringValue("PAY1.K3Z")]
            PaymentType = 8,
            [StringValue("INPUT.K3Z")]
            Pin = 9,
            [StringValue("CASHB.K3Z")]
            CashBack = 10,
            [StringValue("CASHBO.K3Z")]
            CashBackAmount = 11,
            [StringValue("CASHBV.K3Z")]
            CashBackVerify = 12,
            [StringValue("PRESIGN.K3Z")]
            Signature = 13,
            [StringValue("CCOD.K3Z")]
            CardReadContactLess = 14,
            [StringValue("TC-WAIT.K3Z")]
            DeviceBusy = 15,
            [StringValue("ECONFIRM.K3Z")]
            EMVAmountConfirm = 16
        }

        public enum DeviceFormTypeOf
        {
            [StringValue("T")]
            Text,
            [StringValue("B")]
            Button
        }

        public enum DeviceFormElementID
        {
            [StringValue("PROMPTLINE1")]
            PromptLine1,
            [StringValue("PROMPTLINE2")]
            PromptLine2,
            [StringValue("PROMPTLINE3")]
            PromptLine3,
            [StringValue("PROMPTLINE4")]
            PromptLine4,
            [StringValue("bbtnyes")]
            ButtonYes,
            [StringValue("bbtnno")]
            ButtonNo,
            [StringValue("bbtnc")]
            ButtonCashBack,
            [StringValue("bbtnpp")]
            ButtonPartial,
            [StringValue("bbtnman")]
            ButtonManual,
            [StringValue("PROMPT3")]
            Prompt3,
            [StringValue("linedisplay1")]
            LineDisplay1,
            [StringValue("bbtndebit")]
            ButtonDebit,
            [StringValue("bbtncred")]
            ButtonCredit,
            [StringValue("bbtncash")]
            ButtonEBTCash,
            [StringValue("bbtnfood")]
            ButtonEBTFood,
            [StringValue("bbtnst")]
            ButtonStore,
            [StringValue("bbtnclear")]
            ButtonClear

        }

        public enum TransactionType
        {
            [StringValue("01")]
            Sale = 1,
            [StringValue("02")]
            Void = 2,
            [StringValue("03")]
            Return = 3,
            [StringValue("04")]
            VoidReturn = 4
        }

        public enum ManualCardOptions
        {
            [StringValue("1")]
            DisplayAll,
            [StringValue("2")]
            DisplayNoCVV,
            [StringValue("3")]
            DisplayNoExp,
            [StringValue("4")]
            DisplayNoExpNoCVV
        }

        public enum DeviceEquipment
        {
            [StringValue("iSC250")]
            iSC250 = 1,
            [StringValue("iPP320")]
            iPP3250 = 2,
            [StringValue("iPP350")]
            iPP350 = 3,
            [StringValue("iSC480")]
            iSC480 = 4,
            [StringValue("iUP250")]
            iUP250 = 5
        }

        public enum DeviceUpdate
        {
            [StringValue("Firmware")]
            Firmware = 1,
            [StringValue("Forms")]
            Forms = 2
        }
        #endregion
        
        #region -- Public Members --
        public string Result { get; set; }
        public string CurrentForm { get; set; }
        public bool OnDemandSet { get; set; }
        public bool Connected { get; set; }
        public bool OnGuardEnabled { get; set; }
        public bool DebitKey { get; set; }
        public CardInfo CardDetails { get; set; }
        public CardInfo.OnGuardInfo OnGaurdData { get; set; }
        public string DeviceCardSource { get; set; }
        //public Form ConnectedForm { get; set; }
        public bool EncryptionEnabled { get; set; }
        public bool EncryptionKeyFound { get; set; }
        public string FormsVersion { get; set; }
        public string CardSource { get; set; }

        //public ParseTags pT = new ParseTags();

        public List<string> AIDs = new List<string>();

        public string OfflinePINDectected { get; private set; }
        public string OnlinePINReqested { get; private set; }
        public string EMVStarted { get; private set; }
        public string EMVCompleted { get; private set; }
        public string LanguageSelected { get; private set; }
        public string AppSelected { get; private set; }
        public string AppConfirmed { get; private set; }
        public string RewardReqReceived { get; private set; }
        public string PaymentTypeReceived { get; private set; }
        public string AmountConfirmed { get; private set; }
        public string LastPinTry { get; private set; }
        public string OfflinePINEntered { get; private set; }
        public string AccountTypeSelect { get; private set; }
        public string AuthReqSent { get; private set; }
        public string AuthResReceived { get; private set; }
        public string ConfirmationResSent { get; private set; }
        public string TransactionCancelled { get; private set; }
        public string CardCannotRead { get; private set; }
        public string CardOrAppBlocked { get; private set; }
        public string ErrorDetected { get; private set; }
        public string PrematureCardRemoval { get; private set; }
        public string CardNotSupported { get; private set; }
        public string MacVerification { get; private set; }
        public string PostConfirmStartToWait { get; private set; }
        public string SignatureRequest { get; private set; }
        public string TransactionPreparationSent { get; private set; }
        public string EMVFlowSuspended { get; private set; }
        public string CurrentEMVStep { get; private set; }
        public string EMVCashback { get; private set; }
        public int ComBaudRate;
        public int ComDataBits;
        public bool DeviceEMVCapable { get; set; }
        #endregion
        
        public Device()
        {
            CardDetails = new CardInfo();
            OnGaurdData = new CardInfo.OnGuardInfo();
        }
        
        public string Connect(string port)
        {
            //RBA_API.logHandler = new LogHandler(traceLog);
            //RBA_API.SetDefaultLogLevel((LOG_LEVEL)logLevel);

            RBA_API.Initialize();

            RBA_API.SetNotifyRbaDisconnected(new DisconnectHandler(DeviceConnectionEvent));

            RBA_API.pinpadHandler = new PinPadMessageHandler(pinpadHandler);

            ERROR_ID result = SetDeviceCommunications(port);

            //set configuration for on demand
            //TODO - make this configuration driven
            SetDeviceToOnDemand();
            SoftReset();

            if (result.ToString().Contains("SUCCESS") || result.ToString().Contains("RESULT_ERROR_ALREADY_CONNECTED"))
            {
                Connected = true;
                
                //make sure Encryption is on
                if (!IsEncryptionOn(CheckKeys()))
                {
                    Connected = false;
                }
                
            }

            return result.ToString();
        }

        #region --Device Functions
        public void ResetDeviceState()
        {
            OfflinePINDectected = string.Empty;
            OnlinePINReqested = string.Empty;
            EMVStarted = string.Empty;
            EMVCompleted = string.Empty;
            LanguageSelected = string.Empty;
            AppSelected = string.Empty;
            AppConfirmed = string.Empty;
            RewardReqReceived = string.Empty;
            PaymentTypeReceived = string.Empty;
            AmountConfirmed = string.Empty;
            LastPinTry = string.Empty;
            OfflinePINEntered = string.Empty;
            AccountTypeSelect = string.Empty;
            AuthReqSent = string.Empty;
            AuthResReceived = string.Empty;
            ConfirmationResSent = string.Empty;
            TransactionCancelled = string.Empty;
            CardCannotRead = string.Empty;
            CardOrAppBlocked = string.Empty;
            ErrorDetected = string.Empty;
            PrematureCardRemoval = string.Empty;
            CardNotSupported = string.Empty;
            MacVerification = string.Empty;
            PostConfirmStartToWait = string.Empty;
            SignatureRequest = string.Empty;
            TransactionPreparationSent = string.Empty;
            EMVFlowSuspended = string.Empty;
            CurrentEMVStep = string.Empty;
            CardSource = string.Empty;
            DeviceCardSource = string.Empty;
            SoftReset();
        }
        private ERROR_ID SetDeviceCommunications(string port)
        {
            SETTINGS_COMMUNICATION commSet = new SETTINGS_COMMUNICATION();

            SETTINGS_COMM_TIMEOUTS commTimeouts;

            uint connectTimeout = 5000;

            commTimeouts.ConnectTimeout = connectTimeout;
            commTimeouts.ReceiveTimeout = connectTimeout;
            commTimeouts.SendTimeout = connectTimeout;

            RBA_API.SetCommTimeouts(commTimeouts);

            commSet.interface_id = (uint)COMM_INTERFACE.SERIAL_INTERFACE;
            commSet.rs232_config.ComPort = port;
            commSet.rs232_config.BaudRate = Convert.ToUInt32(ComBaudRate);
            commSet.rs232_config.DataBits = Convert.ToUInt32(ComDataBits);
            commSet.rs232_config.Parity = (uint)0;
            commSet.rs232_config.StopBits = Convert.ToUInt32(1);
            commSet.rs232_config.FlowControl = (uint)0;

            //Connect to pin pad
            ERROR_ID result = RBA_API.Connect(commSet);
            return result;
        }
        public ERROR_ID Offline()
        {
            return RBA_API.ProcessMessage(MESSAGE_ID.M00_OFFLINE);
        }

        private void SetDeviceToOnDemand()
        {
            //check to see if the device is already in OnDemand
            RBA_API.SetParam(PARAMETER_ID.P61_REQ_GROUP_NUM, "7");
            RBA_API.SetParam(PARAMETER_ID.P61_REQ_INDEX_NUM, "15");
            RBA_API.ProcessMessage(MESSAGE_ID.M61_CONFIGURATION_READ);

            string dataValue = RBA_API.GetParam(PARAMETER_ID.P61_RES_DATA_CONFIG_PARAMETER);

            //Not Set - so set it
            if (!OnDemandSet || dataValue != "1")
            {
                RBA_API.SetParam(PARAMETER_ID.P60_REQ_GROUP_NUM, "7");
                RBA_API.SetParam(PARAMETER_ID.P60_REQ_INDEX_NUM, "15");
                RBA_API.SetParam(PARAMETER_ID.P60_REQ_DATA_CONFIG_PARAM, "1");
                RBA_API.ProcessMessage(MESSAGE_ID.M60_CONFIGURATION_WRITE);

                OnDemandSet = true;
            }
        }

        public string EnableEMV()
        {

            //Result = RBA_API.ProcessMessage(MESSAGE_ID.M00_OFFLINE);
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_GROUP_NUM, "0019");
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_INDEX_NUM, "0001");
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_DATA_CONFIG_PARAM, "1");
            RBA_API.ProcessMessage(MESSAGE_ID.M60_CONFIGURATION_WRITE);

            string status = RBA_API.GetParam(PARAMETER_ID.P60_RES_STATUS);

            return status;
        }

        public string AllowCVMModification()
        {
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_GROUP_NUM, "0019");
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_INDEX_NUM, "0009");
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_DATA_CONFIG_PARAM, "0");
            RBA_API.ProcessMessage(MESSAGE_ID.M60_CONFIGURATION_WRITE);

            string status = RBA_API.GetParam(PARAMETER_ID.P60_RES_STATUS);

            return status;
        }

        public string Get24RebootEnabled()
        {
            RBA_API.SetParam(PARAMETER_ID.P61_REQ_GROUP_NUM, "0007");
            RBA_API.SetParam(PARAMETER_ID.P61_REQ_INDEX_NUM, "0045");
            RBA_API.ProcessMessage(MESSAGE_ID.M61_CONFIGURATION_READ);

            string status = RBA_API.GetParam(PARAMETER_ID.P61_RES_DATA_CONFIG_PARAMETER);

            return status;
        }

        public string Set24RebootEnabled()
        {
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_GROUP_NUM, "0007");
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_INDEX_NUM, "0045");
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_DATA_CONFIG_PARAM, "1");
            RBA_API.ProcessMessage(MESSAGE_ID.M60_CONFIGURATION_WRITE);

            string status = RBA_API.GetParam(PARAMETER_ID.P60_RES_STATUS);

            return status;
        }


        public string Get24RebootTime()
        {
            RBA_API.SetParam(PARAMETER_ID.P61_REQ_GROUP_NUM, "0007");
            RBA_API.SetParam(PARAMETER_ID.P61_REQ_INDEX_NUM, "0046");
            RBA_API.ProcessMessage(MESSAGE_ID.M61_CONFIGURATION_READ);

            string status = RBA_API.GetParam(PARAMETER_ID.P61_RES_DATA_CONFIG_PARAMETER);

            return status;
        }

        public string Set24RebootTime(string rebootTime)
        {
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_GROUP_NUM, "0007");
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_INDEX_NUM, "0046");
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_DATA_CONFIG_PARAM, rebootTime);
            RBA_API.ProcessMessage(MESSAGE_ID.M60_CONFIGURATION_WRITE);

            string status = RBA_API.GetParam(PARAMETER_ID.P60_RES_STATUS);

            return status;
        }

        public string EnableAIDSelection()
        {
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_GROUP_NUM, "0019");
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_INDEX_NUM, "0020");
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_DATA_CONFIG_PARAM, "1");
            RBA_API.ProcessMessage(MESSAGE_ID.M60_CONFIGURATION_WRITE);

            string status = RBA_API.GetParam(PARAMETER_ID.P60_RES_STATUS);

            return status;
        }

        public string CheckVariable(string variableId, out string status, out string variable)
        {
            RBA_API.SetParam(PARAMETER_ID.P29_RES_VARIABLE_ID, variableId);
            RBA_API.ProcessMessage(MESSAGE_ID.M29_GET_VARIABLE);

            status = RBA_API.GetParam(PARAMETER_ID.P29_RES_STATUS);
            variable = RBA_API.GetParam(PARAMETER_ID.P29_RES_VARIABLE_ID);

            string value = RBA_API.GetParam(PARAMETER_ID.P29_RES_VARIABLE_DATA);

            return value;
        }

        public string DisableEMV()
        {
            RBA_API.ProcessMessage(MESSAGE_ID.M00_OFFLINE);
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_GROUP_NUM, "0019");
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_INDEX_NUM, "0001");
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_DATA_CONFIG_PARAM, "0");
            RBA_API.ProcessMessage(MESSAGE_ID.M60_CONFIGURATION_WRITE);

            string status = RBA_API.GetParam(PARAMETER_ID.P60_RES_STATUS);

            if (status == "Success")
                DeviceEMVCapable = false;

            return status;
        }

        public string GetFileFromDevice(string fileName)
        {
            RBA_API.SetParam(PARAMETER_ID.P65_REQ_RECORD_TYPE, "1");

            //dataformat - 0 = plain text and 1 = BASE64 Format - all should be plain text 0
            RBA_API.SetParam(PARAMETER_ID.P65_REQ_DATA_TYPE, "0");
            RBA_API.SetParam(PARAMETER_ID.P65_REQ_FILE_NAME, fileName);
            RBA_API.ProcessMessage(MESSAGE_ID.M65_RETRIVE_FILE);

            //get value that will determine is the file was actually found and retreived
            string resultMessage = RBA_API.GetParam(PARAMETER_ID.P65_RES_RESULT);
            string fileData = RBA_API.GetParam(PARAMETER_ID.P65_RES_DATA);

            FormsVersion = fileData;
            return resultMessage;
        }

        public ERROR_ID UpdateDevice(string formFilePath)
        {

            string filename = Path.GetFileName(formFilePath);
            RBA_API.SetParam(PARAMETER_ID.P62_REQ_RECORD_TYPE, "0");
            RBA_API.SetParam(PARAMETER_ID.P62_REQ_ENCODING_FORMAT, "8");

            //this will be a tgz file for all forms - application files are set to 1
            if (formFilePath.ToLower().Contains("ogz"))
            {
                RBA_API.SetParam(PARAMETER_ID.P62_REQ_UNPACK_FLAG, "0");
            }
            else
            {
                RBA_API.SetParam(PARAMETER_ID.P62_REQ_UNPACK_FLAG, "1");
            }

            RBA_API.SetParam(PARAMETER_ID.P62_REQ_FAST_DOWNLOAD, "1");
            RBA_API.SetParam(PARAMETER_ID.P62_REQ_OS_FILE_NAME, formFilePath);
            RBA_API.SetParam(PARAMETER_ID.P62_REQ_FILE_NAME, filename);

            ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.FILE_WRITE);

            return result;
        }

        public ERROR_ID HardDeviceReset()
        {
            ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.M97_REBOOT);
            return result;
        }

        public void Reset()
        {
            RBA_API.ProcessMessage(MESSAGE_ID.M10_HARD_RESET);
        }

        public ERROR_ID SetPaymentType(string paymentType, string amount)
        {
            RBA_API.SetParam(PARAMETER_ID.P04_REQ_FORCE_PAYMENT_TYPE, "0");
            RBA_API.SetParam(PARAMETER_ID.P04_REQ_PAYMENT_TYPE, paymentType);
            RBA_API.SetParam(PARAMETER_ID.P04_REQ_AMOUNT, amount);

            ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.M04_SET_PAYMENT_TYPE);

            return result;
        }

        public ERROR_ID SetLineItem(string lineItemLabel, string lineItemAmount)
        {
            RBA_API.SetParam(PARAMETER_ID.P28_REQ_RESPONSE_TYPE, "1");
            RBA_API.SetParam(PARAMETER_ID.P28_REQ_VARIABLE_ID, "104");
            RBA_API.SetParam(PARAMETER_ID.P28_REQ_VARIABLE_DATA, lineItemLabel + lineItemAmount);
            ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.M28_SET_VARIABLE);

            return result;
        }

        public ERROR_ID SetTotal(string totalLabel, string totalAmount)
        {
            RBA_API.SetParam(PARAMETER_ID.P28_REQ_RESPONSE_TYPE, "1");
            RBA_API.SetParam(PARAMETER_ID.P28_REQ_VARIABLE_ID, "120");
            RBA_API.SetParam(PARAMETER_ID.P28_REQ_VARIABLE_DATA, totalLabel + totalAmount);
            ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.M28_SET_VARIABLE);

            return result;
        }

        public ERROR_ID SoftReset()
        {
            RBA_API.SetParam(PARAMETER_ID.P15_REQ_RESET_TYPE, "9");
            ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.M15_SOFT_RESET);

            return result;
        }
        public ERROR_ID SetAmount(string amount, string cashBackAmount)
        {
            RBA_API.SetParam(PARAMETER_ID.P13_REQ_AMOUNT, amount);
            if (!string.IsNullOrEmpty(cashBackAmount))
            {
                RBA_API.SetParam(PARAMETER_ID.P13_REQ_CASHBACK, cashBackAmount);
            }
            ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.M13_AMOUNT);

            return result;
        }
        public string SendDetectedAIDs(List<string> aids)
        {
            RBA_API.SetParam(PARAMETER_ID.P33_12_REQ_STATUS, "00");
            RBA_API.SetParam(PARAMETER_ID.P33_12_REQ_EMVH_CURRENT_PACKET_NBR, "0");
            RBA_API.SetParam(PARAMETER_ID.P33_12_REQ_EMVH_PACKET_TYPE, "0");

            int counter = 0;

            foreach (var item in AIDs)
            {
                string strAId = item;
                byte[] byteAId = StringToByteArray(strAId);

                RBA_API.AddTagParam(MESSAGE_ID.M33_12_EMV_EXTERNAL_AID_SELECT, 0x4F, byteAId);
                counter++;
            }

            if (counter > 1)
                RBA_API.SetParam(PARAMETER_ID.P33_12_REQ_EMV_AID_FLAG, "0");
            else
                RBA_API.SetParam(PARAMETER_ID.P33_12_REQ_EMV_AID_FLAG, "1");

            ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.M33_12_EMV_EXTERNAL_AID_SELECT);
            return "ProcessMessage 33.12 " + result;
        }
        public ERROR_ID ShowMessageForm(DeviceForms form, string msg)
        {
            //RequestForm(form, Ingenico.Device.DeviceFormTypeOf.Text, Ingenico.Device.DeviceFormElementID.PromptLine1, msg);

            ERROR_ID result = ProcessMessage(MESSAGE_ID.M24_FORM_ENTRY);

            return result;
        }
        public ERROR_ID SetManualEntryOff(MESSAGE_ID message)
        {
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_GROUP_NUM, "0007");
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_INDEX_NUM, "0029");
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_DATA_CONFIG_PARAM, "0");

            ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.M60_CONFIGURATION_WRITE);

            return result;
        }
        public ERROR_ID ProcessMessage(MESSAGE_ID message)
        {
            ERROR_ID result = RBA_API.ProcessMessage(message);

            return result;
        }
        public string RetrieveFile(string filename)
        {
            RBA_API.SetParam(PARAMETER_ID.P65_REQ_RECORD_TYPE, "1");
            //dataformat - 0 = plain text and 1 = BASE64 Format - all should be plain text 0
            RBA_API.SetParam(PARAMETER_ID.P65_REQ_DATA_TYPE, "0");
            RBA_API.SetParam(PARAMETER_ID.P65_REQ_FILE_NAME, filename);
            ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.M65_RETRIVE_FILE);

            //get value that will determine is the file was actually found and retreived
            string resultMessage = RBA_API.GetParam(PARAMETER_ID.P65_RES_RESULT);
            string fileData = RBA_API.GetParam(PARAMETER_ID.P65_RES_DATA);

            return fileData;
        }
        /*public void RequestForm(DeviceForms deviceForm, DeviceFormTypeOf elementType, DeviceFormElementID elementId, string prompt)
        {
            RBA_API.SetParam(PARAMETER_ID.P24_REQ_FORM_NUMBER, StringEnum.GetStringValue(deviceForm));
            RBA_API.SetParam(PARAMETER_ID.P24_REQ_TYPE_OF_ELEMENT, StringEnum.GetStringValue(elementType));
            RBA_API.SetParam(PARAMETER_ID.P24_REQ_TEXT_ELEMENTID, StringEnum.GetStringValue(elementId));
            RBA_API.SetParam(PARAMETER_ID.P24_REQ_PROMPT_IDX, prompt);
        }*/
        public ERROR_ID OnDemandReset()
        {
            ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.M10_HARD_RESET);

            return result;
        }
        public string CheckKeys()
        {
            //Check keys
            RBA_API.SetParam(PARAMETER_ID.P29_REQ_VARIABLE_ID, "000810");
            RBA_API.ProcessMessage(MESSAGE_ID.M29_GET_VARIABLE);

            string checkKeys = null;

            if (RBA_API.GetParam(PARAMETER_ID.P29_RES_STATUS) == "2")
            {
                checkKeys = RBA_API.GetParam(PARAMETER_ID.P29_RES_VARIABLE_DATA);
            }
            if (string.IsNullOrEmpty(checkKeys))
            {
                EncryptionKeyFound = false;
            }
            else
            {
                if (checkKeys.ToLower().Contains("ksn_4"))
                {
                    RBA_API.SetParam(PARAMETER_ID.P60_REQ_GROUP_NUM, "91");
                    RBA_API.SetParam(PARAMETER_ID.P60_REQ_INDEX_NUM, "1");
                    RBA_API.SetParam(PARAMETER_ID.P60_REQ_DATA_CONFIG_PARAM, "1");

                    RBA_API.ProcessMessage(MESSAGE_ID.M60_CONFIGURATION_WRITE);
                }

                //set device configuration so it can be checked later
                if (checkKeys.ToLower().Contains("ksn_5"))
                    OnGuardEnabled = true;

                //set device configuration so it can be checked later
                if (checkKeys.ToLower().Contains("ksn_0"))
                    DebitKey = true;

                EncryptionKeyFound = true;
            }


            return checkKeys;
        }
        public string CheckDeviceEncryption()
        {
            //Check injected keys on the terminal
            RBA_API.SetParam(PARAMETER_ID.P61_REQ_GROUP_NUM, "0091");
            RBA_API.SetParam(PARAMETER_ID.P61_REQ_INDEX_NUM, "0001");
            RBA_API.ProcessMessage(MESSAGE_ID.M61_CONFIGURATION_READ);
            RBA_API.GetParam(PARAMETER_ID.P61_RES_GROUP_NUM);
            RBA_API.GetParam(PARAMETER_ID.P61_RES_INDEX_NUM);

            string data = RBA_API.GetParam(PARAMETER_ID.P61_RES_DATA_CONFIG_PARAMETER);

            return data;
        }
        public ERROR_ID EMVTransInitiation()
        {
            RBA_API.SetParam(PARAMETER_ID.P33_00_REQ_STATUS, "00");
            RBA_API.SetParam(PARAMETER_ID.P33_00_REQ_EMVH_CURRENT_PACKET_NBR, "0");
            RBA_API.SetParam(PARAMETER_ID.P33_00_REQ_EMVH_PACKET_TYPE, "0");
            RBA_API.SetParam(PARAMETER_ID.P33_00_REQ_SUSPEND_STEP_LIST, "U");
            RBA_API.SetParam(PARAMETER_ID.P33_00_REQ_RESEND_TIMER, "5000");
            ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.M33_00_EMV_TRANSACTION_INITIATION);
            return result;
        }
        public string GetFinalTagData()
        {
            return RBA_API.GetParam(PARAMETER_ID.P33_03_REQ_STATUS);
        }
        string Send19Response()
        {
            string message = null;

            //This message is used in standard flow only. Not applicable for On-demand
            string cardSource = RBA_API.GetParam(PARAMETER_ID.P19_REQ_ACCOUNT_DATA_ORIGIN);
            message = "P19_REQ_ACCOUNT_DATA_ORIGIN: " + cardSource;
            message += "P19_REQ_TRACK1_DATA_STATUS: " + RBA_API.GetParam(PARAMETER_ID.P19_REQ_TRACK1_DATA_STATUS);
            message += "P19_REQ_TRACK2_DATA_STATUS: " + RBA_API.GetParam(PARAMETER_ID.P19_REQ_TRACK2_DATA_STATUS);
            string counter = RBA_API.GetParam(PARAMETER_ID.P19_REQ_COUNTER);
            message += "P19_REQ_COUNTER: " + counter;
            string accountNumber = RBA_API.GetParam(PARAMETER_ID.P19_REQ_ACCOUNT_NUMBER);
            message += "P19_REQ_ACCOUNT_NUMBER: " + accountNumber;
            string track1Data = RBA_API.GetParam(PARAMETER_ID.P19_REQ_TRACK1_DATA);
            message += "P19_REQ_TRACK1_DATA: " + track1Data;
            string track2Data = RBA_API.GetParam(PARAMETER_ID.P19_REQ_TRACK2_DATA);
            message += "P19_REQ_TRACK2_DATA: " + track2Data;
            string serviceCode = "";

            ERROR_ID result;

            // To retrieve service code using RBA 10.0.2 and above
            RBA_API.SetParam(PARAMETER_ID.P29_REQ_VARIABLE_ID, "000413");
            result = RBA_API.ProcessMessage(MESSAGE_ID.M29_GET_VARIABLE);
            message += "ProcessMessage 29. " + result;
            if (RBA_API.GetParam(PARAMETER_ID.P29_RES_STATUS) == "2")
                serviceCode = RBA_API.GetParam(PARAMETER_ID.P29_RES_VARIABLE_DATA);
            message += "Service Code: " + serviceCode;

            //Only for encryption type '14'
            string panLength;
            string leadingDigits;
            string trailingdigits;
            if (cardSource == "T" && ReadConfiguration("91", "1") == "14")
            {
                panLength = RBA_API.GetParam(PARAMETER_ID.P19_REQ_PAN_LENGTH);
                leadingDigits = RBA_API.GetParam(PARAMETER_ID.P19_REQ_LEADING_PAN_DIGS);
                trailingdigits = RBA_API.GetParam(PARAMETER_ID.P19_REQ_TRAILING_PAN_DIGS);
                /*'w' --> both expiration date and security code must be entered.
                  'x' --> only expiration date must be entered.
                  'y' --> only security code must be entered.
                  'z' --> both expiration date and security code are skipped.*/
                EnterYourOwnMessage("19.y" + counter + panLength + "[FS]" + leadingDigits + "[FS]" + trailingdigits);
                return null;
            }



            if (!string.IsNullOrEmpty(serviceCode) && (serviceCode[0] == '2' || serviceCode[0] == '6') && (!string.IsNullOrEmpty(cardSource) && (cardSource != "h" || cardSource != "d" || cardSource != "b")))
            {
                //For EMV Payment type
                RBA_API.SetParam(PARAMETER_ID.P19_RES_PAYMENT_TYPE_SELECTED, "-");
                RBA_API.SetParam(PARAMETER_ID.P19_RES_COUNTER, counter);
                RBA_API.SetParam(PARAMETER_ID.P19_RES_ACCOUNT_NUMBER, accountNumber);
                result = RBA_API.ProcessMessage(MESSAGE_ID.M19_BIN_LOOKUP);
                message += "ProcessMessage 19. " + result;
                message += "EMV Card detected, 19.- response sent to RBA";

            }
            else
            {
                //For payment type other than smart card
                string cardType = "9";
                
                RBA_API.SetParam(PARAMETER_ID.P19_RES_PAYMENT_TYPE_SELECTED, cardType);
                RBA_API.SetParam(PARAMETER_ID.P19_RES_COUNTER, counter);
                RBA_API.SetParam(PARAMETER_ID.P19_RES_ACCOUNT_NUMBER, accountNumber);
                result = RBA_API.ProcessMessage(MESSAGE_ID.M19_BIN_LOOKUP);
                message += "ProcessMessage 19. " + result;
                message += "19." + cardType + " response sent to RBA";
            }

            //Parse OnGuard data if it's onGuard encrypted
            if (track2Data.Length > 0)
            {
                RBA_API.SetParam(PARAMETER_ID.P61_REQ_GROUP_NUM, "91");
                RBA_API.SetParam(PARAMETER_ID.P61_REQ_INDEX_NUM, "1");
                RBA_API.ProcessMessage(MESSAGE_ID.M61_CONFIGURATION_READ);
                string dataConfig = RBA_API.GetParam(PARAMETER_ID.P61_RES_DATA_CONFIG_PARAMETER);
                if (dataConfig == "2")
                {
                    ParseOnGuardData(track2Data, cardSource, out message);
                }
            }
            return message;
        }
        private string ReadConfiguration(string group, string index)
        {
            RBA_API.SetParam(PARAMETER_ID.P61_REQ_GROUP_NUM, group);
            RBA_API.SetParam(PARAMETER_ID.P61_REQ_INDEX_NUM, index);
            RBA_API.ProcessMessage(MESSAGE_ID.M61_CONFIGURATION_READ);

            group = RBA_API.GetParam(PARAMETER_ID.P61_RES_GROUP_NUM);
            index = RBA_API.GetParam(PARAMETER_ID.P61_RES_INDEX_NUM);
            string Data = RBA_API.GetParam(PARAMETER_ID.P61_RES_DATA_CONFIG_PARAMETER);

            return Data;
        }
        public ERROR_ID WriteConfiguration(string group, string index, string data)
        {
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_GROUP_NUM, group);
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_INDEX_NUM, index);
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_DATA_CONFIG_PARAM, data);
            ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.M60_CONFIGURATION_WRITE);

            return result;
        }
        private void EnterYourOwnMessage(string rawString)
        {
            string newString = rawString;
            int fsInt = 28;//0x1C
            char fsChar = (char)fsInt;
            int gsInt = 29;// 0x1D
            char gsChar = (char)gsInt;
            if (rawString != "")
            {
                //Convert [FS] to 0x1C
                if (rawString.IndexOf("[FS]") != -1)
                {
                    newString = rawString.Replace("[FS]", fsChar.ToString());
                }
                //Convert [GS] to 0x1D
                if (rawString.IndexOf("[GS]") != -1)
                {
                    newString = newString.Replace("[GS]", gsChar.ToString());
                }
            }

            RBA_API.SendCustomMessage(newString, false);
            string rawResponseData = RBA_API.GetParam(PARAMETER_ID.RAW_PINPAD_RESPONSE_DATA);
            string rawResponseWithGsfs = rawResponseData;
            if (rawResponseWithGsfs.IndexOf(gsChar) != -1)
            {
                rawResponseWithGsfs = rawResponseWithGsfs.Replace(gsChar.ToString(), "[GS]");
            }
            if (rawResponseWithGsfs.IndexOf(fsChar) != -1)
            {
                rawResponseWithGsfs = rawResponseWithGsfs.Replace(fsChar.ToString(), "[FS]");
            }

        }
        public ERROR_ID SetEMVFinal()
        {
            RBA_API.SetParam(PARAMETER_ID.P33_09_REQ_STATUS, "00");
            RBA_API.SetParam(PARAMETER_ID.P33_09_REQ_EMVH_CURRENT_PACKET_NBR, "0");
            RBA_API.SetParam(PARAMETER_ID.P33_09_REQ_EMVH_PACKET_TYPE, "0");
            RBA_API.SetParam(PARAMETER_ID.P33_09_REQ_COMMAND_TYPE, "J");

            ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.M33_09_EMV_SET_DATA);

            return result;
        }
        public string GetVariable_29(string varId)
        {
            RBA_API.SetParam(PARAMETER_ID.P29_REQ_VARIABLE_ID, varId);
            RBA_API.ProcessMessage(MESSAGE_ID.M29_GET_VARIABLE);

            string status = RBA_API.GetParam(PARAMETER_ID.P29_RES_STATUS);
            string variable = RBA_API.GetParam(PARAMETER_ID.P29_RES_VARIABLE_ID);
            string value = RBA_API.GetParam(PARAMETER_ID.P29_RES_VARIABLE_DATA);
            return value;
        }
        public ERROR_ID SetVarible28(string varId, string varData)
        {
            RBA_API.SetParam(PARAMETER_ID.P28_REQ_RESPONSE_TYPE, "1");
            RBA_API.SetParam(PARAMETER_ID.P28_REQ_VARIABLE_ID, varId);
            RBA_API.SetParam(PARAMETER_ID.P28_REQ_VARIABLE_DATA, varData);
            ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.M28_SET_VARIABLE);
            return result;

        }
        public string GetSignatureData()
        {
            String sigBlock = "";
            String signatureData = "";

            string signatureStatus = RBA_API.GetParam(PARAMETER_ID.P20_RES_STATUS);

            if (signatureStatus == "0")
            {
                RBA_API.SetParam(PARAMETER_ID.P29_REQ_VARIABLE_ID, "000712");
                RBA_API.ProcessMessage(MESSAGE_ID.M29_GET_VARIABLE);
                int signatureBlocks = Convert.ToInt32(RBA_API.GetParam(PARAMETER_ID.P29_RES_VARIABLE_DATA));

                for (int i = 0; i < signatureBlocks; i++)
                {
                    RBA_API.SetParam(PARAMETER_ID.P29_REQ_VARIABLE_ID, ("70" + Convert.ToString(i)));
                    RBA_API.ProcessMessage(MESSAGE_ID.M29_GET_VARIABLE);
                    sigBlock = RBA_API.GetParam(PARAMETER_ID.P29_RES_VARIABLE_DATA);
                    signatureData += sigBlock;

                }

                if (signatureData == string.Empty)
                {
                    //Don't process signature if no data was received
                    return "No signature data received";

                }

                return signatureData;

            }
            else if (signatureStatus == "1")
                return ("Signature Canceled by user");
            else
            {
                return ("No signature data available");
            }
        }
        #endregion

        #region --Functions --
        public bool IsEncryptionOn(string ksnSlot)
        {
            EncryptionEnabled = false;
            string encryptionStatus = CheckDeviceEncryption();

            //not set for encrypting - check the PGZ file loaded on the device
            if (encryptionStatus == "0" || string.IsNullOrEmpty(encryptionStatus))
            {
                EncryptionEnabled = false;
            }

            //2 = OnGuard / 11 = TDES / DUKPT
            if (encryptionStatus == "2" || encryptionStatus == "11")
            {
                //check to see that the key slot matches the Encryption activated
                if (ksnSlot.ToLower().Contains("ksn_5") && encryptionStatus == "2")
                {
                    EncryptionEnabled = true;
                }

                if (ksnSlot.ToLower().Contains("ksn_4") && encryptionStatus == "11")
                {
                    EncryptionEnabled = true;
                }
            }

            return EncryptionEnabled;
        }
        public static byte[] StringToByteArray(String hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
        public string ByteArrayToString(byte[] data)
        {
            StringBuilder strHex = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
                strHex.AppendFormat("{0:x2}", b);
            return strHex.ToString();

        }
        public CardInfo PatchUpTrackData(CardInfo cardDetails)
        {
            if (cardDetails.Track3?.Length > 0 && cardDetails.Track1?.Length == 0)
            {
                // Some test stuff was sending us the TCLink-style track3 which is track13track2|track3 and not splitting it out separately
                // AND the track1/track2 is not formatted correctly anyway (from some devices). So let's just split it all back out and 
                // carry on...
                string[] parts = cardDetails.Track3.Split('|');
                if (parts.Length >= 3)
                {
                    //safety
                    cardDetails.Track1 = parts[0];
                    cardDetails.Track2 = parts[1];
                    cardDetails.Track3 = parts[2];
                }
            }


            if (cardDetails.Track1?.Length > 0 && cardDetails.Track1?[0] != '%')
            {
                cardDetails.Track1 = "%" + cardDetails.Track1.Replace(";", "?");
            }

            if (cardDetails.Track2?.Length > 0 && cardDetails.Track2?[0] != ';')
            {
                cardDetails.Track2 = ";" + cardDetails.Track2.Replace(";", "?");
            }

            return cardDetails;
        }

        #endregion


        private void GetDeviceStep()
        {
            string flag1 = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F1_CHIP_CARD);
            EMVStarted = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F2_EMV_STARTED);
            EMVCompleted = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F3_EMV_COMPLETED);
            LanguageSelected = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F4_LANGUAGE_SELECTED);
            AppSelected = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F5_APP_SELECTED);
            AppConfirmed = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F6_APP_CONFIRMED);
            RewardReqReceived = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F7_REWARD_REQ_RECEIVED);
            PaymentTypeReceived = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F8_PAYMENT_TYPE_RECEIVED);
            AmountConfirmed = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F9_AMOUNT_CONFIRMED);
            LastPinTry = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F10_LAST_PIN_TRY);
            OfflinePINEntered = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F11_OFFLINE_PIN_ENTERED);
            AccountTypeSelect = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F12_ACCOUNT_TYPE_SELECTED);
            AuthReqSent = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F13_AUTH_REQ_SENT);
            AuthResReceived = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F14_AUTH_RES_RECEIVED);
            ConfirmationResSent = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F15_CONFIRMATION_RES_SENT);
            TransactionCancelled = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F16_TRANSACTION_CANCELLED);
            CardCannotRead = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F17_CARD_CANNOT_READ);
            CardOrAppBlocked = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F18_CARD_OR_APP_BLOCKED);
            ErrorDetected = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F19_ERROR_DETECTED);
            PrematureCardRemoval = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F20_PREMATURE_CARD_REMOVAL);
            CardNotSupported = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F21_CARD_NOT_SUPPORTED);
            MacVerification = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F22_MAC_VERIFICATION);
            PostConfirmStartToWait = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F23_POST_CONFIRM_START_TO_WAIT);
            SignatureRequest = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F24_SIGNATURE_REQUEST);
            TransactionPreparationSent = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F25_TRANSACTION_PREPARATION_SENT);
            EMVFlowSuspended = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F26_EMV_FLOW_SUSPENDED);
            OnlinePINReqested = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F27_ONLINE_PIN_REQUESTED);
            CurrentEMVStep = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F28_CURRENT_EMV_STEP);
            string flag29 = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F29_RESERVED);
            string flag30 = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F30_RESERVED);
            EMVCashback = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F31_EMV_CASHBACK);
            string flag32 = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F32_CONTACTLESS_STATUS);
            string flag33 = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F33_CONTACTLESS_ERROR);

            ParseStatusEventFlags(flag1, EMVStarted, EMVCompleted, LanguageSelected, AppSelected, AppConfirmed, RewardReqReceived, PaymentTypeReceived, AmountConfirmed, LastPinTry, OfflinePINEntered,
                AccountTypeSelect, AuthReqSent, AuthResReceived, ConfirmationResSent, TransactionCancelled, CardCannotRead, CardOrAppBlocked, ErrorDetected, PrematureCardRemoval, CardNotSupported,
                MacVerification, PostConfirmStartToWait, SignatureRequest, TransactionPreparationSent, EMVFlowSuspended, OnlinePINReqested, CurrentEMVStep, flag29, flag30, EMVCashback, flag32, flag33);
        }
        

        #region --Device Events --
        public void DeviceConnectionEvent()
        {
            CONNECTION_STATUS connectionStatus = RBA_API.GetConnectionStatus();

            if (connectionStatus == CONNECTION_STATUS.DISCONNECTED || connectionStatus == CONNECTION_STATUS.CONNECTED_NOT_READY)
            {
                Connected = false;
            }

            DeviceConnectionChanged.Raise(null, new DeviceConnectionArgs(RBA_API.GetConnectionStatus()));
        }
        public void Disconnect()
        {
            try
            {
                if (RBA_API.GetConnectionStatus().Equals(CONNECTION_STATUS.CONNECTED))
                {
                    Result = RBA_API.Disconnect().ToString();
                }
            }
            catch
            {
                //log error
                //if (ex.Source == "RBA_SDK_CS")
                //MessageBox.Show("Exception Occured during Disconnect:" + Environment.NewLine + ex.ToString() + Environment.NewLine + Environment.NewLine + "Suggested Resolution:" + Environment.NewLine + "For 32-bit machine: Add directory path for the dll to Environment variable PATH" + Environment.NewLine + "For 64-bit machine: Copy the dll to C:\\Windows\\SysWOW64");
            }

        }
        public void pinpadHandler(MESSAGE_ID msgId)
        {
            ERROR_ID Result;
            string message = string.Empty;

            switch (msgId)
            {

                case MESSAGE_ID.M00_OFFLINE:
                    {
                        RBA_API.GetParam(PARAMETER_ID.P00_RES_REASON_CODE);
                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P33_00_RES_STATUS), message));
                        break;
                    }
                case MESSAGE_ID.M09_SET_ALLOWED_PAYMENT:
                    {
                        string cardType = RBA_API.GetParam(PARAMETER_ID.P09_RES_CARD_TYPE);
                        string cardStatus = RBA_API.GetParam(PARAMETER_ID.P09_RES_CARD_STATUS);
                        if ((cardType == "02" || cardType == "99") && cardStatus == "I")
                        {
                            message = "**Card inserted";
                        }
                        else if ((cardType == "02" || cardType == "99") && cardStatus == "R")
                        {
                            message = "**Card Removed";
                        }
                        else if (cardStatus == "P")
                        {
                            message = "**Unknown problem with Card Insertion / Removal";
                        }
                        else
                        {
                            message = "**Unknown Card activity";
                        }

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P09_RES_CARD_TYPE), message));
                        break;
                    }
                case MESSAGE_ID.M10_HARD_RESET:
                    {

                        DeviceInputReceived(null, new DeviceEventArgs(MESSAGE_ID.M10_HARD_RESET, "", "EMVAmtVerifyNo"));

                        break;
                    }
                case MESSAGE_ID.M19_BIN_LOOKUP:
                    {
                        message = "Unsolicited message: " + msgId + "\n";
                        message += Send19Response();

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P19_REQ_LAYOUT_ID), message));
                        break;
                    }
                case MESSAGE_ID.M20_SIGNATURE:
                    {
                        message = ("Unsolicited message: " + msgId + "\n");

                        var keyPress = RBA_API.GetParam(PARAMETER_ID.P20_RES_KEY);

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P20_REQ_FORM_NAME), keyPress));
                        break;

                    }
                case MESSAGE_ID.M21_NUMERIC_INPUT:
                    {
                        message += ("Unsolicited message: " + msgId + "\n");
                        message += ("P21_RES_EXIT_TYPE: " + RBA_API.GetParam(PARAMETER_ID.P21_RES_EXIT_TYPE));
                        message += ("P21_RES_INPUT_DATA: " + RBA_API.GetParam(PARAMETER_ID.P21_RES_INPUT_DATA).ToString());

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P21_REQ_FORMAT_SPECIFIER), message));
                        break;

                    }
                case MESSAGE_ID.M23_CARD_READ:
                    {

                        message += ("Unsolicited message: " + msgId + "\n");
                        message += ("P23_RES_EXIT_TYPE: " + RBA_API.GetParam(PARAMETER_ID.P23_RES_EXIT_TYPE));

                        string track1 = (RBA_API.GetParam(PARAMETER_ID.P23_RES_TRACK1));

                        CardDetails.Track1 = track1;

                        string track2 = (RBA_API.GetParam(PARAMETER_ID.P23_RES_TRACK2));

                        CardDetails.Track2 = track2;

                        string track3 = (RBA_API.GetParam(PARAMETER_ID.P23_RES_TRACK3));

                        CardDetails.Track3 = track3;

                        CardDetails = ParseTrackData(CardDetails);
                        CardDetails = PatchUpTrackData(CardDetails);

                        CardSource = (RBA_API.GetParam(PARAMETER_ID.P23_RES_CARD_SOURCE));
                        DeviceCardSource = CardSource;

                        //Update account number in 31 request section
                        string pan = GetVariable_29("000398");
                        CardDetails.PAN = pan;
                        CardDetails.EncryptedTrack = string.Format("{0}|{1}|{2}", CardDetails.Track1, CardDetails.Track2, CardDetails.Track3);

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P23_REQ_FORM_NAME), message));
                        break;
                    }
                case MESSAGE_ID.M24_FORM_ENTRY:
                    {
                        message += ("Unsolicited message: " + msgId + "\n");
                        string exitType = RBA_API.GetParam(PARAMETER_ID.P24_RES_EXIT_TYPE);
                        message += ("P24_RES_EXIT_TYPE: " + exitType);
                        //pinpadLogger("P24_RES_BUTTON_STATE: " + RBA_API.GetParam(PARAMETER_ID.P24_RES_BUTTON_STATE));
                        string keyId = RBA_API.GetParam(PARAMETER_ID.P24_RES_KEYID);
                        message += ("P24_RES_KEYID: " + keyId);
                        int length = RBA_API.GetParamLen(PARAMETER_ID.P24_RES_BUTTONID);

                        //For checkboxes / Radio buttons
                        while (length > 0)
                        {
                            string buttonState = RBA_API.GetParam(PARAMETER_ID.P24_RES_BUTTON_STATE);
                            string buttonId = RBA_API.GetParam(PARAMETER_ID.P24_RES_BUTTONID);
                            message += ("P24_REQ_BUTTONID: : " + buttonId);
                            message += ("P24_REQ_BUTTON_STATE : " + buttonState);
                            length = RBA_API.GetParamLen(PARAMETER_ID.P24_RES_BUTTONID);
                        }

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P24_REQ_FORM_NUMBER), keyId));
                        break;
                    }
                case MESSAGE_ID.M25_TERMS_AND_CONDITIONS:
                    {
                        message += ("Unsolicited message: " + msgId + "\n");
                        string sigOnAccept = RBA_API.GetParam(PARAMETER_ID.P25_RES_SIGNATURE_ON_ACCEPT);
                        message += ("P25_RES_SIGNATURE_ON_ACCEPT: " + sigOnAccept);
                        string keyPressed = RBA_API.GetParam(PARAMETER_ID.P25_RES_KEY_PRESSED);
                        message += ("P25_RES_KEY_PRESSED: " + keyPressed);

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P24_REQ_FORM_NUMBER), keyPressed));
                        break;
                    }
                case MESSAGE_ID.M27_ALPHA_INPUT:
                    {
                        message += ("Unsolicited message: " + msgId + "\n");
                        message += ("P27_RES_EXIT_TYPE: " + RBA_API.GetParam(PARAMETER_ID.P27_RES_EXIT_TYPE));
                        message += ("P27_RES_DATA_INPUT: " + RBA_API.GetParam(PARAMETER_ID.P27_RES_DATA_INPUT));

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P27_REQ_FORM_SPECIFICID), message));
                        break;
                    }
                case MESSAGE_ID.M31_PIN_ENTRY:
                    {
                        message += ("Unsolicited message: " + msgId + "\n");
                        message += ("P31_RES_STATUS: " + RBA_API.GetParam(PARAMETER_ID.P31_RES_STATUS));

                        CardDetails.EncryptedPIN = RBA_API.GetParam(PARAMETER_ID.P31_RES_PIN_DATA);     //RBA_API only allows this value to be retrieved once; second read returns empty string

                        message += ("P31_RES_PIN_DATA: " + CardDetails.EncryptedPIN);
                        
                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P31_REQ_FORM_NAME), message));
                        break;
                    }
                case MESSAGE_ID.M33_01_EMV_STATUS:
                    {
                        message += ("Unsolicited message: " + msgId + "\n");
                        message += ("P33_01_RES_TRANSACTION_CODE: " + RBA_API.GetParam(PARAMETER_ID.P33_01_RES_TRANSACTION_CODE));
                       
                        GetDeviceStep();

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P33_01_REQ_STATUS), message));
                        
                        break;
                    }
                case MESSAGE_ID.M33_02_EMV_TRANSACTION_PREPARATION_RESPONSE:
                    {
                        message += ("");
                        message += ("****************************************************************");
                        message += ("Unsolicited message: " + msgId + "\n");
                        message += ("****************************************************************");
                        string status = RBA_API.GetParam(PARAMETER_ID.P33_02_RES_STATUS);

                        ///*Method to retrieve EMV Tag Data returns everything in a byte array, 
                        //application does not need to convert the data to the required data type*/
                        byte[] byteTagData = new byte[1];
                        if (status == "E")
                            message += (" ERROR RECEIVED");
                        while (true)
                        {
                            int tagParamLength = RBA_API.GetTagParamLen(msgId);
                            if (tagParamLength <= 0)
                                break;
                            int tagId = RBA_API.GetTagParam(msgId, out byteTagData);
                            string strTagData = ByteArrayToString(byteTagData);
                            //pT.ParseEMVTags(tagId.ToString("X"), tagParamLength, strTagData, byteTagData);
                        }
                        ///End of Method1
                        message += ("****************************************************************");
                        message += ("");

                        RBA_API.ResetParam(PARAMETER_ID.P_ALL_PARAMS);

                        //set the emconfirm.k3z form as current form for device
                        //CurrentForm = StringEnum.GetStringValue(DeviceForms.EMVAmountConfirm);

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P33_02_RES_STATUS), message));
                        break;
                    }
                case MESSAGE_ID.M33_03_EMV_AUTHORIZATION_REQUEST:
                    {
                        message += ("");
                        message += ("****************************************************************");
                        message += ("Unsolicited message: " + msgId + "\n");
                        message += ("****************************************************************");
                        string status = RBA_API.GetParam(PARAMETER_ID.P33_03_REQ_STATUS);

                        ///*Method to retrieve EMV Tag Data returns everything in a byte array, 
                        //application does not need to convert the data to the required data type*/
                        byte[] byteTagData = new byte[1];
                        if (status == "E")
                            message += (" ERROR RECEIVED");

                        while (true)
                        {
                            int tagParamLength = RBA_API.GetTagParamLen(msgId);
                            if (tagParamLength <= 0)
                                break;
                            int tagId = RBA_API.GetTagParam(msgId, out byteTagData);
                            string strTagData = ByteArrayToString(byteTagData);
                            //pT.ParseEMVTags(tagId.ToString("X"), tagParamLength, strTagData, byteTagData);


                        }
                        message += ("****************************************************************");
                        message += ("");
                        RBA_API.ResetParam(PARAMETER_ID.P_ALL_PARAMS);

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P33_03_REQ_STATUS), message));
                        break;
                    }
                case MESSAGE_ID.M33_05_EMV_AUTHORIZATION_CONFIRMATION:
                    {
                        message += ("");
                        message += ("****************************************************************");
                        message += ("Unsolicited message: " + msgId + "\n");
                        message += ("****************************************************************");
                        string status = RBA_API.GetParam(PARAMETER_ID.P33_05_RES_STATUS);

                        ///*Method to retrieve EMV Tag Data gives everything as a byte array, 
                        //application does not need to convert the data to the required type*/
                        byte[] byteTagData = new byte[1];
                        if (status == "E")
                            message += (" ERROR RECEIVED");

                        while (true)
                        {
                            int tagParamLength = RBA_API.GetTagParamLen(msgId);
                            if (tagParamLength <= 0)
                                break;
                            int tagId = RBA_API.GetTagParam(msgId, out byteTagData);
                            string strTagData = ByteArrayToString(byteTagData);
                            //pT.ParseEMVTags(tagId.ToString("X"), tagParamLength, strTagData, byteTagData);

                        }
                        message += ("****************************************************************");
                        message += ("");
                        RBA_API.ResetParam(PARAMETER_ID.P_ALL_PARAMS);

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P33_05_RES_STATUS), message));
                        break;
                    }
                case MESSAGE_ID.M33_07_EMV_TERMINAL_CAPABILITIES:
                    {
                        message += ("");
                        message += ("****************************************************************");
                        message += ("Unsolicited message: " + msgId + "\n");
                        message += ("****************************************************************");
                        string status = RBA_API.GetParam(PARAMETER_ID.P33_07_REQ_STATUS);

                        byte[] byteTagData = new byte[1];
                        byte[] byteAID = new byte[1];
                        if (status == "E")
                            message += (" ERROR RECEIVED");

                        while (true)
                        {
                            int tagParamLength = RBA_API.GetTagParamLen(msgId);
                            if (tagParamLength <= 0)
                                break;
                            int tagId = RBA_API.GetTagParam(msgId, out byteTagData);
                            message += ("TAG ID = " + tagId.ToString("X") + " Tag Param Length = " + tagParamLength + " TagData = " + ByteArrayToString(byteTagData));
                            int tag84 = 0;

                            if (tagId == 132)
                            {
                                tag84 = tagId;
                                byteAID = byteTagData;
                                //this.Invoke((MethodInvoker)delegate ()
                                //{
                                //    txtAID.Text = ByteArrayToString(byteTagData).ToUpper();
                                //});
                            }
                        }
                        message += ("****************************************************************");
                        message += ("");
                        RBA_API.ResetParam(PARAMETER_ID.P_ALL_PARAMS);

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P33_07_RES_STATUS), message));
                        ////To automatically send 33.07 response for testing purposes.
                        //SetEMVTerminalCapabilities();
                        break;
                    }
                case MESSAGE_ID.M33_11_EMV_EXTERNAL_AID_SELECT_NOTIFICATION:
                    {
                        message += "";
                        message += "****************************************************************";
                        message += "Unsolicited message: " + msgId + "\n";
                        message += "****************************************************************";
                        string status = RBA_API.GetParam(PARAMETER_ID.P33_11_REQ_STATUS);

                        AIDs = new List<string>();

                        byte[] byteTagData = new byte[1];
                        byte[] byteAID = new byte[1];
                        if (status == "E")
                            message += "ERROR RECEIVED";

                        string strTagData = string.Empty;

                        while (true)
                        {
                            int tagParamLength = RBA_API.GetTagParamLen(msgId);
                            if (tagParamLength <= 0)
                                break;
                            int tagId = RBA_API.GetTagParam(msgId, out byteTagData);
                            int tag79 = 0;

                            strTagData = ByteArrayToString(byteTagData);

                            //pT.ParseEMVTags(tagId.ToString("X"), tagParamLength, strTagData, byteTagData);

                            if (tagId == 79)
                            {
                                tag79 = tagId;
                                byteAID = byteTagData;

                                AIDs.Add(strTagData);
                            }

                        }

                        message += "****************************************************************";
                        message += "";
                        RBA_API.ResetParam(PARAMETER_ID.P_ALL_PARAMS);
                        message += "****************************************************************";
                        message += "";
                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P33_11_REQ_STATUS), message));

                        break;
                    }
                case MESSAGE_ID.M35_MENU:
                    {
                        message += "Unsolicited message: " + msgId + "\n";
                        message += "P35_RES_STATUS: " + RBA_API.GetParam(PARAMETER_ID.P35_RES_STATUS);
                        message += "P35_RES_STATUS: " + RBA_API.GetParam(PARAMETER_ID.P35_RES_ID);
                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P35_RES_ID), message));
                        break;
                    }
                case MESSAGE_ID.M41_CARD_READ:
                    {
                        message += ("Unsolicited message: " + msgId + "\n");
                        message += ("P41_RES_SOURCE: " + RBA_API.GetParam(PARAMETER_ID.P41_RES_SOURCE));
                        message += ("P41_RES_ENCRYPTION: " + RBA_API.GetParam(PARAMETER_ID.P41_RES_ENCRYPTION));
                        message += ("P41_RES_TRACK_1_STATUS: " + RBA_API.GetParam(PARAMETER_ID.P41_RES_TRACK_1_STATUS));
                        message += ("P41_RES_TRACK_2_STATUS: " + RBA_API.GetParam(PARAMETER_ID.P41_RES_TRACK_2_STATUS));
                        message += ("P41_RES_TRACK_3_STATUS: " + RBA_API.GetParam(PARAMETER_ID.P41_RES_TRACK_3_STATUS));
                        message += ("P41_RES_TRACK_1: " + RBA_API.GetParam(PARAMETER_ID.P41_RES_TRACK_1));
                        message += ("P41_RES_TRACK_2: " + RBA_API.GetParam(PARAMETER_ID.P41_RES_TRACK_2));
                        message += ("P41_RES_TRACK_3: " + RBA_API.GetParam(PARAMETER_ID.P41_RES_TRACK_3));
                        string pan = RBA_API.GetParam(PARAMETER_ID.P41_RES_PAN);
                        if (!string.IsNullOrEmpty(pan))
                            message += ("P41_RES_PAN: " + pan);
                        string maskedPan = RBA_API.GetParam(PARAMETER_ID.P41_RES_MASKED_PAN);
                        if (!string.IsNullOrEmpty(maskedPan))
                            message += ("P41_RES_MASKED_PAN: " + maskedPan);
                        string expiryDate = RBA_API.GetParam(PARAMETER_ID.P41_RES_EXPIRATION_DATE);
                        if (!string.IsNullOrEmpty(expiryDate))
                            message += ("P41_RES_EXPIRATION_DATE: " + expiryDate);
                        string accountName = RBA_API.GetParam(PARAMETER_ID.P41_RES_ACCOUNT_NAME);
                        if (!string.IsNullOrEmpty(accountName))
                            message += ("P41_RES_ACCOUNT_NAME: " + accountName);

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P41_RES_PAN), message));
                        break;
                    }
                case MESSAGE_ID.M50_AUTHORIZATION:
                    {
                        message += ("Unsolicited message: " + msgId + "\n");
                        // Displaying the parameters received from 50. request
                        message += ("P50_REQ_ACCQUIRING_BANK: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_ACQUIRING_BANK));
                        message += ("P50_REQ_MERCHANT_NUMBER: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_MERCHANT_ID));
                        message += ("P50_REQ_STORE: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_STORE_ID));
                        message += ("P50_REQ_PIN_PAD: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_PIN_PAD_ID));
                        message += ("P50_REQ_STANDARD_INDUSTRY_CLASSIFICAION: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_STANDARD_INDUSTRY_CLASSIFICATION));
                        message += ("P50_REQ_CURRENCY_TYPE: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_COUNTRY_OR_CURRENCY_TYPE));
                        message += ("P50_REQ_ZIP_CODE: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_ZIP_CODE));
                        message += ("P50_REQ_TIME_ZONE_DIFF_FROM_GMT: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_TIME_ZONE_DIFF_FROM_GMT));
                        message += ("P50_REQ_TRANSACTION_CODE: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_TRANSACTION_CODE));
                        string pinPadSerialNumber = RBA_API.GetParam(PARAMETER_ID.P50_REQ_PIN_PAD_SERIAL_NUM);
                        message += ("P50_REQ_PIN_PAD_SERIAL_NUM: " + pinPadSerialNumber);
                        string posTransactionNumber = RBA_API.GetParam(PARAMETER_ID.P50_REQ_POS_TRANSACTION_NUM);
                        message += ("P50_REQ_POS_TRANSACTION_NUM: " + posTransactionNumber);
                        message += ("P50_REQ_ACC_DATA_SOURCE: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_ACC_DATA_SOURCE));
                        message += ("P50_REQ_MAG_SWIPE_INFO: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_MAG_SWIPE_INFO));
                        message += ("P50_REQ_PIN_LENGTH: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_PIN_LENGTH));
                        message += ("P50_REQ_PIN_BLOCK: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_PIN_ENCRYPTED_BLOCK));
                        message += ("P50_REQ_PIN_KEY_SET_IDENTIFIER: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_PIN_KEY_SET_IDENTIFIER));
                        message += ("P50_REQ_PIN_DEVICE_ID: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_PIN_DEVICE_ID));
                        message += ("P50_REQ_PIN_ENCRYPTION_COUNTER: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_PIN_ENCRYPTION_COUNTER));
                        message += ("P50_REQ_TRANSACTION_AMOUNT: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_TRANSACTION_AMOUNT));

                        //Sending a dummy approval message
                        Result = RBA_API.SetParam(PARAMETER_ID.P50_RES_PIN_PAD_SERIAL_NUM, pinPadSerialNumber);
                        Result = RBA_API.SetParam(PARAMETER_ID.P50_RES_POS_TXN_NUM, posTransactionNumber);
                        
                        Result = RBA_API.SetParam(PARAMETER_ID.P50_RES_APPROVAL_CODE, "100001");
                        DateTime dtNow = DateTime.Now;
                        string sDate = dtNow.ToString("yyMMdd");
                        Result = RBA_API.SetParam(PARAMETER_ID.P50_RES_TODAYS_DATE_YYMMDD, sDate);
                        Result = RBA_API.ProcessMessage(MESSAGE_ID.M50_AUTHORIZATION);
                        message += ("ProcessMessage 50. " + Result);

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P50_REQ_PIN_DEVICE_ID), message));
                        break;
                    }
                case MESSAGE_ID.M58_DISCOVER_DEVICES:
                    {
                        //not implemented this way - call device health or device info - it will have data
                        //if any other protocol besides Serial / USBCDC - revisit this method
                        break;
                    }

                case MESSAGE_ID.M65_RETRIVE_FILE:
                    {
                        message += ("Unsolicited message: " + msgId + "\n");
                        message += ("P65_RES_RESULT: " + RBA_API.GetParam(PARAMETER_ID.P65_RES_RESULT));
                        message += ("P65_RES_TOTAL_NUMBER_OF_BLOCKS: " + RBA_API.GetParam(PARAMETER_ID.P65_RES_TOTAL_NUMBER_OF_BLOKS));
                        message += ("P65_RES_RESULT: " + RBA_API.GetParam(PARAMETER_ID.P65_RES_BLOK_NUMBER));
                        message += ("P65_RES_CRC: " + RBA_API.GetParam(PARAMETER_ID.P65_RES_CRC));
                        message += ("P65_RES_DATA: " + RBA_API.GetParam(PARAMETER_ID.P65_RES_DATA));
                        message += ("P65_RES_DATA_TYPE: " + RBA_API.GetParam(PARAMETER_ID.P65_RES_DATA_TYPE));
                        break;
                    }
                case MESSAGE_ID.M87_E2EE_CARD_READ:
                    {
                        message += ("Unsolicited message: " + msgId + "\n");
                        try
                        {
                            //Removal of FS  - workaround for RBASDK older than 5.3.0
                            int fsInt = 28;//0x1C
                            char fsChar = (char)fsInt;
                            //
                            // Add this to 23 event handler also
                            string exitType = RBA_API.GetParam(PARAMETER_ID.P87_RES_EXIT_TYPE);
                            message += ("P87_RES_EXIT_TYPE: " + exitType);

                            //check if device canceled
                            if (exitType != "0")
                            {
                                DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P87_REQ_FORM_NAME), exitType));
                                break;
                            }

                            string cardData = RBA_API.GetParam(PARAMETER_ID.P87_RES_CARD_DATA);
                            //Removal of redundant FS character from cardData  - workaround for RBASDK older than 5.3.0
                            if (!string.IsNullOrEmpty(cardData))
                            {
                                if (cardData.IndexOf(fsChar) != -1)
                                {
                                    cardData = cardData.Replace(fsChar.ToString(), "");
                                }
                            }

                            CardSource = RBA_API.GetParam(PARAMETER_ID.P87_RES_CARD_SOURCE);

                            if (!string.IsNullOrEmpty(CardSource))
                            {
                                message += ("P87_RES_CARD_SOURCE: " + CardSource);
                            }
                            else
                            {
                                message += ("P87_RES_CARD_SOURCE: " + CardSource);
                                message += ("Could not detect card source, assuming source as MSD");
                            }
                            if (!(CardSource == "M" || CardSource == "C" || CardSource == "H"))
                            {
                                DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P87_REQ_FORM_NAME), message));
                                break;
                            }

                            message += ("P87_RES_CARD_DATA: " + cardData);
                            if (!(cardData?.Length > 0))
                            {
                                DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P87_REQ_FORM_NAME), message));
                                break;
                            }

                            //Parse OnGuard data if it's onGuard encrypted
                            ParseOnGuardData(cardData, CardSource, out message);
                            //Update account number in 31 request section
                            //string PAN = GetVariable_29("000398");
                            CardDetails.PAN = GetVariable_29("000398");
                            CardDetails.EncryptedTrack = string.Format("ONGUARD{0}", cardData);
                            DeviceCardSource = CardSource;
                            //this.Invoke((MethodInvoker)delegate ()
                            //{
                            //    txtPinAccountNumber.Text = PAN;
                            //});
                            DeviceInputReceived.Raise(null, new DeviceEventArgs(msgId, RBA_API.GetParam(PARAMETER_ID.P87_REQ_FORM_NAME), message));
                        }
                        catch (Exception ex)
                        {
                            message += ("Exception occurred" + ex.ToString());
                        }
                        break;
                    }
                case MESSAGE_ID.M95_BARCODE_GET:
                    {
                        //not implemented in this application
                    }
                    break;
            }

        }

        #endregion

        #region --Parsing --
        public void ParseOnGuardData(string cardData, string cardSource, out string message)
        {

            message = null;

            try
            {

                switch (CardSource.ToString())
                {
                    case "H":
                    case "T":
                        {
                            message = "**********************************************";
                            message += "Parsed OnGuard Manually entered Data";
                            message += "**********************************************";
                            if (cardData.IndexOf("=") > 20)
                            {
                                int index;
                                string FirstSix, LastFour, PANLength, Mod10CheckFlag, Expiry, ServiceCode, LanguageCode, EncryptedFlag;
                                ParseCardData(cardData, out index, out FirstSix, out LastFour, out PANLength, out Mod10CheckFlag, out Expiry, out ServiceCode, out LanguageCode, out EncryptedFlag);

                                index += 1;
                                if (EncryptedFlag.ToString() == "1")
                                {
                                    message += "First 6: " + FirstSix;
                                    message += "Last 4: " + LastFour;
                                    message += "PAN Length: " + PANLength;
                                    message += "Mod10 check flag: " + Mod10CheckFlag;
                                    message += "Expiry: " + Expiry;
                                    message += "Service Code: " + ServiceCode;
                                    message += "Language Code: " + LanguageCode;
                                    message += "Encrypted Flag: " + EncryptedFlag;
                                    string EncFormat = cardData.Substring(index, 1);
                                    OnGaurdData.EncryptedFormat = cardData.Substring(index, 1);
                                    message += "Encryption Format: " + EncFormat;
                                    index += 1;
                                    string KSN = cardData.Substring(index, 24);
                                    message += "KSN + 4-digit Extension: " + KSN;
                                    OnGaurdData.KSNPlus4 = cardData.Substring(index, 24);
                                    index += 24;
                                    int ICEncDataLength = Convert.ToInt32(cardData.Substring(index, 2));
                                    message += "IC Encrypted Data Length: " + ICEncDataLength;
                                    index += 2;
                                    string ICEncData = cardData.Substring(index, ICEncDataLength);
                                    OnGaurdData.ICEncrytedData = cardData.Substring(index, ICEncDataLength);
                                    message += "IC Encrypted Data (Encrypted Track 2): " + ICEncData;
                                    index += ICEncDataLength;
                                    int AESPANLength = Convert.ToInt32(cardData.Substring(index, 2));
                                    OnGaurdData.AESPANLength = Convert.ToInt32(cardData.Substring(index, 2));
                                    message += "AES Pan Length: " + AESPANLength;
                                    index += 2;
                                    string AESPAN = cardData.Substring(index, AESPANLength);
                                    OnGaurdData.AESPAN = cardData.Substring(index, AESPANLength);
                                    message += "AES PAN: " + AESPAN;
                                    index += AESPANLength;
                                    int LSEncDataLength = Convert.ToInt32(cardData.Substring(index, 2));
                                    OnGaurdData.LSEncryptedLength = Convert.ToInt32(cardData.Substring(index, 2));
                                    message += "LS Encrypted Data Length: " + LSEncDataLength;
                                    index += 2;
                                    string LSEncData = cardData.Substring(index, LSEncDataLength);
                                    OnGaurdData.LSEncryptedData = cardData.Substring(index, LSEncDataLength);
                                    message += "LS Encrypted Data: " + LSEncData;
                                    index += LSEncDataLength;
                                    string ExtLangCode = cardData.Substring(index, 1);
                                    OnGaurdData.ExtendedCode = cardData.Substring(index, 1);
                                    message += "Extended Language Code: " + ExtLangCode;
                                    index += 1;
                                    message += "**********************************************";


                                }
                                else if (EncryptedFlag.ToString() == "0")
                                {
                                    message += "First 6: " + FirstSix;
                                    message += "Last 4: " + LastFour;
                                    message += "PAN Length: " + PANLength;
                                    message += "Mod10 check flag: " + Mod10CheckFlag;
                                    message += "Expiry: " + Expiry;
                                    message += "Service Code: " + ServiceCode;
                                    message += "Language Code: " + LanguageCode;
                                    message += "Encrypted Flag: " + EncryptedFlag;
                                    int cardDataLength = Convert.ToInt32(cardData.Substring(index, 2));
                                    index += 2;
                                    message += "Card data length: " + cardDataLength;
                                    string cardDataField = cardData.Substring(index, cardDataLength);
                                    message += "Card data: " + cardDataField;
                                    index += cardDataLength;
                                    string ExtLangCode = cardData.Substring(index, 1);
                                    message += "Extended Language Code: " + ExtLangCode;
                                    index += 1;

                                }

                            }
                            else
                            {
                                //Data whitelisted using secbin.dat
                                if (!string.IsNullOrEmpty(cardData))
                                {
                                    message += "Card data whitelisted using secbin.dat: " + cardData;
                                }
                                else
                                {
                                    message += "Card Data: " + "Empty";
                                }
                                message += "**********************************************";
                            }


                            break;
                        }
                    case "S":
                        {
                            break;
                        }

                    default:
                        {
                            message += "**********************************************";
                            message += "Parsed OnGuard MSD Data";
                            message += "**********************************************";
                            if (cardData.IndexOf("=") > 20)
                            {
                                int index = 0;
                                string FirstSix = cardData.Substring(index, 6);
                                CardDetails.FirstSix = cardData.Substring(index, 6);
                                index += 6;
                                string LastFour = cardData.Substring(index, 4);
                                CardDetails.LastFour = cardData.Substring(index, 4);
                                index += 4;
                                string PANLength = cardData.Substring(index, 2);
                                OnGaurdData.PANLength = cardData.Substring(index, 2);
                                index += 2;
                                string Mod10CheckFlag = cardData.Substring(index, 1);
                                OnGaurdData.MOD10Check = cardData.Substring(index, 1);
                                index += 1;
                                string Expiry = cardData.Substring(index, 4);
                                CardDetails.Expiry = cardData.Substring(index, 4);
                                index += 4;
                                string ServiceCode = cardData.Substring(index, 3);
                                OnGaurdData.ServiceCode = cardData.Substring(index, 3);
                                index += 3;
                                string LanguageCode = cardData.Substring(index, 1);
                                OnGaurdData.LanguageCode = cardData.Substring(index, 1);
                                index += 1;
                                int NameLengthint;
                                string NameLength = cardData.Substring(index, 2);
                                bool parseResult = Int32.TryParse(NameLength, out NameLengthint);
                                if (parseResult == false)
                                {
                                    message += "Could not find valid Name length, consider setting 0013_0014 = 1 to differentiate between MSD and Hand entry";
                                    message += "**********************************************";
                                    break;
                                }
                                index += 2;
                                string Name = cardData.Substring(index, NameLengthint);
                                CardDetails.CardHolderName = cardData.Substring(index, NameLengthint);
                                index += NameLengthint;
                                string EncryptedFlag = cardData.Substring(index, 1);
                                OnGaurdData.EncryptedFlag = cardData.Substring(index, 1);
                                index += 1;
                                if (EncryptedFlag.ToString() == "1")
                                {
                                    message += "First 6: " + FirstSix;
                                    message += "Last 4: " + LastFour;
                                    message += "PAN Length: " + PANLength;
                                    message += "Mod10 check flag: " + Mod10CheckFlag;
                                    message += "Expiry: " + Expiry;
                                    message += "Service Code: " + ServiceCode;
                                    message += "Language Code: " + LanguageCode;
                                    message += "Cardholder Name Length: " + NameLengthint;
                                    message += "Cardholder Name: " + Name;
                                    message += "Encrypted Flag: " + EncryptedFlag;
                                    string EncFormat = cardData.Substring(index, 1);
                                    OnGaurdData.EncryptedFormat = cardData.Substring(index, 1);
                                    message += "Encryption Format: " + EncFormat;
                                    index += 1;
                                    string KSN = cardData.Substring(index, 24);
                                    OnGaurdData.KSNPlus4 = cardData.Substring(index, 24);
                                    message += "KSN + 4-digit Extension: " + KSN;
                                    index += 24;
                                    int ICEncDataLength = Convert.ToInt32(cardData.Substring(index, 2));
                                    message += "IC Encrypted Data Length: " + ICEncDataLength;
                                    index += 2;
                                    string ICEncData = cardData.Substring(index, ICEncDataLength);
                                    OnGaurdData.ICEncrytedData = cardData.Substring(index, ICEncDataLength);
                                    CardDetails.Track2 = cardData.Substring(index, ICEncDataLength);
                                    message += "IC Encrypted Data (Encrypted Track 2): " + ICEncData;
                                    index += ICEncDataLength;
                                    int AESPANLength = Convert.ToInt32(cardData.Substring(index, 2));
                                    OnGaurdData.AESPANLength = Convert.ToInt32(cardData.Substring(index, 2));
                                    message += "AES Pan Length: " + AESPANLength;
                                    index += 2;
                                    string AESPAN = cardData.Substring(index, AESPANLength);
                                    OnGaurdData.AESPAN = cardData.Substring(index, AESPANLength);
                                    message += "AES PAN: " + AESPAN;
                                    index += AESPANLength;
                                    int LSEncDataLength = Convert.ToInt32(cardData.Substring(index, 2));
                                    OnGaurdData.LSEncryptedLength = Convert.ToInt32(cardData.Substring(index, 2));
                                    message += "LS Encrypted Data Length: " + LSEncDataLength;
                                    index += 2;
                                    string LSEncData = cardData.Substring(index, LSEncDataLength);
                                    OnGaurdData.LSEncryptedData = cardData.Substring(index, LSEncDataLength);
                                    message += "LS Encrypted Data: " + LSEncData;
                                    index += LSEncDataLength;
                                    string ExtLangCode = cardData.Substring(index, 1);
                                    OnGaurdData.ExtendedCode = cardData.Substring(index, 1);
                                    message += "Extended Language Code: " + ExtLangCode;
                                    index += 1;
                                    message += "**********************************************";
                                }
                                else if (EncryptedFlag.ToString() == "0")
                                {

                                    message += ("First 6: " + FirstSix);
                                    message += ("Last 4: " + LastFour);
                                    message += ("PAN Length: " + PANLength);
                                    message += ("Mod10 check flag: " + Mod10CheckFlag);
                                    message += ("Expiry: " + Expiry);
                                    message += ("Service Code: " + ServiceCode);
                                    message += ("Language Code: " + LanguageCode);
                                    message += ("Encrypted Flag: " + EncryptedFlag);
                                    int Track1Length = Convert.ToInt32(cardData.Substring(index, 2));
                                    index += 2;
                                    message += ("Track1 length: " + Track1Length);
                                    if (Track1Length > 0)
                                    {
                                        string Track1 = cardData.Substring(index, Track1Length);
                                        CardDetails.Track1 = cardData.Substring(index, Track1Length);
                                        message += ("Track1: " + Track1);
                                        index += Track1Length;
                                    }
                                    int Track2Length = Convert.ToInt32(cardData.Substring(index, 2));
                                    index += 2;
                                    message += ("Track2 length: " + Track2Length);
                                    if (Track2Length > 0)
                                    {
                                        string Track2 = cardData.Substring(index, Track2Length);
                                        CardDetails.Track2 = cardData.Substring(index, Track2Length);
                                        message += ("Track2: " + Track2);
                                        index += Track2Length;
                                    }
                                    string ExtLangCode = cardData.Substring(index, 1);
                                    OnGaurdData.ExtendedCode = cardData.Substring(index, 1);
                                    message += ("Extended Language Code: " + ExtLangCode);
                                    index += 1;
                                }
                            }
                            else
                            {
                                //Data whitelisted using secbin.dat
                                if (!string.IsNullOrEmpty(cardData))
                                {
                                    message += ("Card data whitelisted using secbin.dat: " + cardData);
                                }
                                else
                                {
                                    message += ("Card Data: " + "Empty");
                                }
                                message += ("**********************************************");
                            }

                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                message += ("Exception occurred" + ex.ToString());
            }

        }
        public void ParseCardData(string cardData, out int index, out string firstSix, out string lastFour, out string panLength, out string mod10CheckFlag, out string expiry, out string serviceCode, out string languageCode, out string encryptedFlag)
        {
            index = 0;
            firstSix = cardData.Substring(index, 6);
            CardDetails.FirstSix = cardData.Substring(index, 6);

            index += 6;
            lastFour = cardData.Substring(index, 4);
            CardDetails.LastFour = cardData.Substring(index, 4);

            index += 4;
            panLength = cardData.Substring(index, 2);
            OnGaurdData.PANLength = cardData.Substring(index, 2);

            index += 2;
            mod10CheckFlag = cardData.Substring(index, 1);
            OnGaurdData.MOD10Check = cardData.Substring(index, 1);

            index += 1;
            expiry = cardData.Substring(index, 4);
            CardDetails.Expiry = cardData.Substring(index, 4);

            index += 4;
            serviceCode = cardData.Substring(index, 3);
            OnGaurdData.ServiceCode = cardData.Substring(index, 3);

            index += 3;
            languageCode = cardData.Substring(index, 1);
            OnGaurdData.LanguageCode = cardData.Substring(index, 1);

            index += 1;
            encryptedFlag = cardData.Substring(index, 1);
            OnGaurdData.EncryptedFlag = cardData.Substring(index, 1);
        }
        //TODO - need to refactor this so that it can send the message back out
        public void ParseStatusEventFlags(string flag1, string flag2, string flag3, string flag4, string flag5, string flag6, string flag7, string flag8, string flag9, string flag10, string flag11, string flag12, string flag13, string flag14,
            string flag15, string flag16, string flag17, string flag18, string flag19, string flag20, string flag21, string flag22, string flag23, string flag24, string flag25, string flag26, string flag27, string flag28, string flag29,
            string flag30, string flag31, string flag32, string flag33)
        {
            string defaultFlag = "-";
            string message = null;
            message += ("**********************************************");
            message += ("Updated flags");
            message += ("**********************************************");
            switch (flag1.ToString())
            {
                case "I":
                    {
                        if (flag1.ToString() == defaultFlag)
                            break;
                        message += ("Flag 1 - " + flag1.ToString() + " Chip Card inserted");
                        break;
                    }
                case "R":
                    {
                        if (flag1.ToString() == defaultFlag)
                            break;
                        message += ("flag 1 - " + flag1.ToString() + " Chip Card removed");
                        break;
                    }
                default:
                    {
                        if (flag1.ToString() == defaultFlag)
                            break;
                        message += ("flag 1 - " + flag1.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag2.ToString())
            {
                case "S":
                    {
                        if (flag2.ToString() == defaultFlag)
                            break;
                        message += ("Flag 2 - " + flag2.ToString() + " EMV Process started");
                        break;
                    }
                default:
                    {
                        if (flag2.ToString() == defaultFlag)
                            break;
                        message += ("Flag 2 - " + flag2.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag3.ToString())
            {
                case "C":
                    {
                        if (flag3.ToString() == defaultFlag)
                            break;
                        message += ("Flag 3 - " + flag3.ToString() + " EMV process completed");
                        break;
                    }
                case "A":
                    {
                        if (flag3.ToString() == defaultFlag)
                            break;
                        message += ("Flag 3 - " + flag3.ToString() + " EMV process completed with approval");
                        break;
                    }
                case "D":
                    {
                        if (flag3.ToString() == defaultFlag)
                            break;
                        message += ("Flag 3 - " + flag3.ToString() + " EMV process completed with decline");
                        break;
                    }
                case "E":
                    {
                        if (flag3.ToString() == defaultFlag)
                            break;
                        message += ("Flag 3 - " + flag3.ToString() + " EMV process completed with error");
                        break;
                    }
                default:
                    {
                        if (flag3.ToString() == defaultFlag)
                            break;
                        message += ("Flag 3 - " + flag3.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag4.ToString())
            {
                case "M":
                    {
                        if (flag4.ToString() == defaultFlag)
                            break;
                        message += ("Flag 4 - " + flag4.ToString() + " Language manually selected");
                        break;
                    }
                case "A":
                    {
                        if (flag4.ToString() == defaultFlag)
                            break;
                        message += ("Flag 4 - " + flag4.ToString() + " Language automatically selected");
                        break;
                    }
                default:
                    {
                        if (flag4.ToString() == defaultFlag)
                            break;
                        message += ("Flag 4 - " + flag4.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag5.ToString())
            {
                case "M":
                    {
                        if (flag5.ToString() == defaultFlag)
                            break;
                        message += ("Flag 5 - " + flag5.ToString() + " Application manually selected");
                        break;
                    }
                case "A":
                    {
                        if (flag5.ToString() == defaultFlag)
                            break;
                        message += ("Flag 5 - " + flag5.ToString() + " Application automatically selected");
                        break;
                    }
                default:
                    {
                        if (flag5.ToString() == defaultFlag)
                            break;
                        message += ("Flag 5 - " + flag5.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag6.ToString())
            {
                case "A":
                    {
                        if (flag6.ToString() == defaultFlag)
                            break;
                        message += ("Flag 6 - " + flag6.ToString() + " Application confirmation accepted");
                        break;
                    }
                case "R":
                    {
                        if (flag6.ToString() == defaultFlag)
                            break;
                        message += ("Flag 6 - " + flag6.ToString() + " Application confirmation rejected");
                        break;
                    }
                default:
                    {
                        if (flag6.ToString() == defaultFlag)
                            break;
                        message += ("Flag 6 - " + flag6.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag7.ToString())
            {
                case "R":
                    {
                        if (flag7.ToString() == defaultFlag)
                            break;
                        message += ("Flag 7 - " + flag7.ToString() + " Rewards request is received");
                        break;
                    }
                case "S":
                    {
                        if (flag7.ToString() == defaultFlag)
                            break;
                        message += ("Flag 7 - " + flag7.ToString() + " Rewards response sent");
                        break;
                    }
                default:
                    {
                        if (flag7.ToString() == defaultFlag)
                            break;
                        message += ("Flag 7 - " + flag7.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag8.ToString())
            {
                case "R":
                    {
                        if (flag8.ToString() == defaultFlag)
                            break;
                        message += ("Flag 8 - " + flag8.ToString() + " Payment type request is received");
                        break;
                    }
                case "S":
                    {
                        if (flag8.ToString() == defaultFlag)
                            break;
                        message += ("Flag 8 - " + flag8.ToString() + " Payment type response is sent");
                        break;
                    }
                default:
                    {
                        if (flag8.ToString() == defaultFlag)
                            break;
                        message += ("Flag 8 - " + flag8.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag9.ToString())
            {
                case "A":
                    {
                        if (flag9.ToString() == defaultFlag)
                            break;
                        message += ("Flag 9 - " + flag9.ToString() + " Amount confirmation accepted");
                        break;
                    }
                case "R":
                    {
                        if (flag9.ToString() == defaultFlag)
                            break;
                        message += ("Flag 9 - " + flag9.ToString() + " Amount confirmation rejected");
                        break;
                    }
                default:
                    {
                        if (flag9.ToString() == defaultFlag)
                            break;
                        message += ("Flag 9 - " + flag9.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag10.ToString())
            {
                case "L":
                    {
                        if (flag10.ToString() == defaultFlag)
                            break;
                        message += ("Flag 10  - " + flag10.ToString() + " This is the last PIN try");
                        break;
                    }
                default:
                    {
                        if (flag10.ToString() == defaultFlag)
                            break;
                        message += ("Flag 10  - " + flag10.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag11.ToString())
            {
                case "P":
                    {
                        if (flag11.ToString() == defaultFlag)
                            break;
                        message += ("Flag 11 - " + flag11.ToString() + " PIN is entered");
                        break;
                    }
                case "B":
                    {
                        if (flag11.ToString() == defaultFlag)
                            break;
                        message += ("Flag 11 - " + flag11.ToString() + " PIN bypassed");
                        break;
                    }
                default:
                    {
                        if (flag11.ToString() == defaultFlag)
                            break;
                        message += ("Flag 11 - " + flag11.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag12.ToString())
            {
                case "C":
                    {
                        if (flag12.ToString() == defaultFlag)
                            break;
                        message += ("Flag 12 - " + flag12.ToString() + " Checking account type is selected");
                        break;
                    }
                case "S":
                    {
                        if (flag12.ToString() == defaultFlag)
                            break;
                        message += ("Flag 12 - " + flag12.ToString() + " Savings account type is selected");
                        break;
                    }
                default:
                    {
                        if (flag12.ToString() == defaultFlag)
                            break;
                        message += ("Flag 12 - " + flag12.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag13.ToString())
            {
                case "S":
                    {
                        if (flag13.ToString() == defaultFlag)
                            break;
                        message += ("Flag 13 - " + flag13.ToString() + " Authorization request is sent");
                        break;
                    }
                case "F":
                    {
                        if (flag13.ToString() == defaultFlag)
                            break;
                        message += ("Flag 13 - " + flag13.ToString() + " Authorization request failed to be sent");
                        break;
                    }
                default:
                    {
                        if (flag13.ToString() == defaultFlag)
                            break;
                        message += ("Flag 13 - " + flag13.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag14.ToString())
            {
                case "R":
                    {
                        if (flag14.ToString() == defaultFlag)
                            break;
                        message += ("Flag 14 - " + flag14.ToString() + " Authorization response is received");
                        break;
                    }
                case "T":
                    {
                        if (flag14.ToString() == defaultFlag)
                            break;
                        message += ("Flag 14 - " + flag14.ToString() + " Internal device timeout on Auth Response");
                        break;
                    }
                case "H":
                    {
                        if (flag14.ToString() == defaultFlag)
                            break;
                        message += ("Flag 14 - " + flag14.ToString() + " Register indication of no host available");
                        break;
                    }
                default:
                    {
                        if (flag14.ToString() == defaultFlag)
                            break;
                        message += ("Flag 14 - " + flag14.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag15.ToString())
            {
                case "S":
                    {
                        if (flag15.ToString() == defaultFlag)
                            break;
                        message += ("Flag 15 - " + flag15.ToString() + " Confirmation response is sent");
                        break;
                    }
                case "F":
                    {
                        if (flag15.ToString() == defaultFlag)
                            break;
                        message += ("Flag 15 - " + flag15.ToString() + " Confirmation response failed to be sent");
                        break;
                    }
                default:
                    {
                        if (flag15.ToString() == defaultFlag)
                            break;
                        message += ("Flag 15 - " + flag15.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag16.ToString())
            {
                case "C":
                    {
                        if (flag16.ToString() == defaultFlag)
                            break;
                        message += ("Flag 16 - " + flag16.ToString() + " Transaction is canceled");
                        break;
                    }
                default:
                    {
                        if (flag16.ToString() == defaultFlag)
                            break;
                        message += ("Flag 16 - " + flag16.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag17.ToString())
            {
                case "I":
                    {
                        if (flag17.ToString() == defaultFlag)
                            break;
                        message += ("Flag 17 - " + flag17.ToString() + " Card data invalid but fallback is allowed");
                        break;
                    }
                case "N":
                    {
                        if (flag17.ToString() == defaultFlag)
                            break;
                        message += ("Flag 17 - " + flag17.ToString() + " Card data invalid, fallback not allowed");
                        break;
                    }
                default:
                    {
                        if (flag17.ToString() == defaultFlag)
                            break;
                        message += ("Flag 17 - " + flag17.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag18.ToString())
            {
                case "B":
                    {
                        if (flag18.ToString() == defaultFlag)
                            break;
                        message += ("Flag 18 - " + flag18.ToString() + " Card blocked");
                        break;
                    }
                case "A":
                    {
                        if (flag18.ToString() == defaultFlag)
                            break;
                        message += ("Flag 18 - " + flag18.ToString() + " Application blocked");
                        break;
                    }
                default:
                    {
                        if (flag18.ToString() == defaultFlag)
                            break;
                        message += ("Flag 18 - " + flag18.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag19.ToString())
            {
                case "F":
                    {
                        if (flag19.ToString() == defaultFlag)
                            break;
                        message += ("Flag 19 - " + flag19.ToString() + " Fatal error is detected");
                        break;
                    }
                case "K":
                    {
                        if (flag19.ToString() == defaultFlag)
                            break;
                        message += ("Flag 19 - " + flag19.ToString() + " Track 2 data consistency failed");
                        break;
                    }
                case "O":
                    {
                        if (flag19.ToString() == defaultFlag)
                            break;
                        message += ("Flag 19 - " + flag19.ToString() + " cardholder and/or transaction timeout");
                        break;
                    }
                case "X":
                    {
                        if (flag19.ToString() == defaultFlag)
                            break;
                        message += ("Flag 19 - " + flag19.ToString() + " Card's application is expired");
                        break;
                    }
                case "C":
                    {
                        if (flag19.ToString() == defaultFlag)
                            break;
                        message += ("Flag 19 - " + flag19.ToString() + " Cashback error");
                        break;
                    }
                case "B":
                    {
                        if (flag19.ToString() == defaultFlag)
                            break;
                        message += ("Flag 19 - " + flag19.ToString() + " pre-pin Cashback requested but pin bypassed");
                        break;
                    }
                default:
                    {
                        if (flag19.ToString() == defaultFlag)
                            break;
                        message += ("Flag 19 - " + flag19.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag20.ToString())
            {
                case "R":
                    {
                        if (flag20.ToString() == defaultFlag)
                            break;
                        message += ("Flag 20 - " + flag20.ToString() + " Premature card removal detected");
                        break;
                    }
                default:
                    {
                        if (flag20.ToString() == defaultFlag)
                            break;
                        message += ("Flag 20 - " + flag20.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag21.ToString())
            {
                case "N":
                    {
                        if (flag21.ToString() == defaultFlag)
                            break;
                        message += ("Flag 21 - " + flag21.ToString() + " Card and/or app is not supported");
                        break;
                    }
                default:
                    {
                        if (flag21.ToString() == defaultFlag)
                            break;
                        message += ("Flag 21 - " + flag21.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag22.ToString())
            {
                case "P":
                    {
                        if (flag22.ToString() == defaultFlag)
                            break;
                        message += ("Flag 22 - " + flag23.ToString() + " MAC verification passed");
                        break;
                    }
                case "F":
                    {
                        if (flag22.ToString() == defaultFlag)
                            break;
                        message += ("Flag 22 - " + flag23.ToString() + " MAC verification failed");
                        break;
                    }
                default:
                    {
                        if (flag22.ToString() == defaultFlag)
                            break;
                        message += ("Flag 22 - " + flag23.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag23.ToString())
            {
                case "S":
                    {
                        if (flag23.ToString() == defaultFlag)
                            break;
                        message += ("Flag 23 - " + flag23.ToString() + " Post confirmation wait has started");
                        break;
                    }
                default:
                    {
                        if (flag23.ToString() == defaultFlag)
                            break;
                        message += ("Flag 23 - " + flag23.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag24.ToString())
            {
                case "S":
                    {
                        if (flag24.ToString() == defaultFlag)
                            break;
                        message += message += ("Flag 24 - " + flag24.ToString() + " Signature started");
                        break;
                    }
                case "E":
                    {
                        if (flag24.ToString() == defaultFlag)
                            break;
                        message += ("Flag 24 - " + flag24.ToString() + " Signature completed");
                        break;
                    }
                case "R":
                    {
                        if (flag24.ToString() == defaultFlag)
                            break;
                        message += ("Flag 24 - " + flag24.ToString() + " Paper signature requested");
                        break;
                    }
                default:
                    {
                        if (flag24.ToString() == defaultFlag)
                            break;
                        message += ("Flag 24 - " + flag24.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag25.ToString())
            {
                case "S":
                    {
                        if (flag25.ToString() == defaultFlag)
                            break;
                        message += ("Flag 25 - " + flag25.ToString() + " Transaction preparation response sent");
                        break;
                    }
                case "F":
                    {
                        if (flag25.ToString() == defaultFlag)
                            break;
                        message += ("Flag 25 - " + flag25.ToString() + " Transaction preparation response failed to be sent");
                        break;
                    }
                default:
                    {
                        if (flag25.ToString() == defaultFlag)
                            break;
                        message += ("Flag 25 - " + flag25.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag26.ToString())
            {
                case "1":
                    {
                        if (flag26.ToString() == defaultFlag)
                            break;
                        message += ("Flag 26 - " + flag26.ToString() + " EMV flow is suspended");
                        break;
                    }
                case "0":
                    {
                        if (flag26.ToString() == defaultFlag)
                            break;
                        message += ("Flag 26 - " + flag26.ToString() + " EMV flow is resumed");
                        break;
                    }
                default:
                    {
                        if (flag26.ToString() == defaultFlag)
                            break;
                        message += ("Flag 26 - " + flag26.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag27.ToString())
            {
                case "R":
                    {
                        if (flag27.ToString() == defaultFlag)
                            break;
                        message += ("Flag 27 - " + flag27.ToString() + " Online pin requested");
                        break;
                    }
                case "C":
                    {
                        if (flag27.ToString() == defaultFlag)
                            break;
                        message += ("Flag 27 - " + flag27.ToString() + " Online pin canceled");
                        break;
                    }
                case "A":
                    {
                        if (flag27.ToString() == defaultFlag)
                            break;
                        message += ("Flag 27 - " + flag27.ToString() + " Online pin entered and verified");
                        break;
                    }
                case "B":
                    {
                        if (flag27.ToString() == defaultFlag)
                            break;
                        message += ("Flag 27 - " + flag27.ToString() + " Online pin bypassed");
                        break;
                    }
                default:
                    {
                        if (flag27.ToString() == defaultFlag)
                            break;
                        message += ("Flag 27 - " + flag27.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag28.ToString())
            {
                default:
                    {
                        if (flag28.ToString() == defaultFlag)
                            break;
                        switch (flag28.ToString())
                        {
                            case "A":
                                message += ("Flag 28 - Current Step: " + flag28.ToString() + " - EMV start");
                                break;
                            case "B":
                                message += ("Flag 28 - Current Step: " + flag28.ToString() + " - Select language service");
                                break;
                            case "C":
                                message += ("Flag 28 - Current Step: " + flag28.ToString() + " - Select AID service");
                                break;
                            case "D":
                                message += ("Flag 28 - Current Step: " + flag28.ToString() + " - AID confirmation");
                                break;
                            case "E":
                                message += ("Flag 28 - Current Step: " + flag28.ToString() + " - Application selected");
                                break;
                            case "F":
                                message += ("Flag 28 - Current Step: " + flag28.ToString() + " - Get first purchase amount");
                                break;
                            case "G":
                                message += ("Flag 28 - Current Step: " + flag28.ToString() + " - Set EMV tags");
                                break;
                            case "H":
                                message += ("Flag 28 - Current Step: " + flag28.ToString() + " - PAN available");
                                break;
                            case "I":
                                message += ("Flag 28 - Current Step: " + flag28.ToString() + " - Set payment type");
                                break;
                            case "J":
                                message += ("Flag 28 - Current Step: " + flag28.ToString() + " - Get cashback");
                                break;
                            case "K":
                                message += ("Flag 28 - Current Step: " + flag28.ToString() + " - Change purchase amount");
                                break;
                            case "L":
                                message += ("Flag 28 - Current Step: " + flag28.ToString() + " - Confirm purchase/cashback amount");
                                break;
                            case "M":
                                message += ("Flag 28 - Current Step: " + flag28.ToString() + " - Select account");
                                break;
                            case "N":
                                message += ("Flag 28 - Current Step: " + flag28.ToString() + " - Offline PIN entry");
                                break;
                            case "O":
                                message += ("Flag 28 - Current Step: " + flag28.ToString() + " - Online PIN entry");
                                break;
                            case "P":
                                message += ("Flag 28 - Current Step: " + flag28.ToString() + " - Last transaction data processed");
                                break;
                            case "Q":
                                message += ("Flag 28 - Current Step: " + flag28.ToString() + " - Terminal action analysis processed");
                                break;
                            case "R":
                                message += ("Flag 28 - Current Step: " + flag28.ToString() + " - EMV transaction authorized online");
                                break;
                            case "S":
                                message += ("Flag 28 - Current Step: " + flag28.ToString() + " - EMV transaction stopped");
                                break;
                            default:
                                message += ("Flag 28 - " + flag28.ToString() + " - Current EMV Step");
                                break;
                        }

                        break;
                    }
            }
            switch (flag29.ToString())
            {
                case "0":
                    {
                        if (flag29.ToString() == defaultFlag)
                            break;
                        message += ("Flag 29 - " + flag29.ToString() + " reserved");
                        break;
                    }
                case "1":
                    {
                        if (flag29.ToString() == defaultFlag)
                            break;
                        message += ("Flag 29 - " + flag29.ToString() + " reserved");
                        break;
                    }
                default:
                    {
                        if (flag29.ToString() == defaultFlag)
                            break;
                        message += ("Flag 29 - " + flag29.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag30.ToString())
            {
                case "C":
                    {
                        if (flag30.ToString() == defaultFlag)
                            break;
                        message += ("Flag 30 - " + flag30.ToString() + " reserved");
                        break;
                    }
                case "R":
                    {
                        if (flag30.ToString() == defaultFlag)
                            break;
                        message += ("Flag 30 - " + flag30.ToString() + " reserved");
                        break;
                    }
                default:
                    {
                        if (flag30.ToString() == defaultFlag)
                            break;
                        message += ("Flag 30 - " + flag30.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag31.ToString())
            {
                case "C":
                    {
                        if (flag31.ToString() == defaultFlag)
                            break;
                        message += ("Flag 31 - " + flag31.ToString() + " Post pin-entry cashback complete");
                        break;
                    }
                case "R":
                    {
                        if (flag31.ToString() == defaultFlag)
                            break;
                        message += ("Flag 31 - " + flag31.ToString() + " Post pin-entry cashback ready");
                        break;
                    }
                default:
                    {
                        if (flag31.ToString() == defaultFlag)
                            break;
                        message += ("Flag 31 - " + flag31.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag32.ToString())
            {
                case "1":
                    {
                        if (flag32.ToString() == defaultFlag)
                            break;
                        message += ("Flag 32 - " + flag32.ToString() + " Contactless transaction started");
                        break;
                    }
                case "0":
                    {
                        if (flag32.ToString() == defaultFlag)
                            break;
                        message += ("Flag 32 - " + flag32.ToString() + " Contactless transaction stopped");
                        break;
                    }
                default:
                    {
                        if (flag32.ToString() == defaultFlag)
                            break;
                        message += ("Flag 32 - " + flag32.ToString() + " Unknown");
                        break;
                    }
            }
            switch (flag33.ToString())
            {
                case "C":
                    {
                        if (flag33.ToString() == defaultFlag)
                            break;
                        message += ("Flag 33 - " + flag33.ToString() + " Collision detected");
                        break;
                    }
                case "R":
                    {
                        if (flag33.ToString() == defaultFlag)
                            break;
                        message += ("Flag 33 - " + flag33.ToString() + " Re-tap is required..");
                        break;
                    }
                default:
                    {
                        if (flag33.ToString() == defaultFlag)
                            break;
                        message += ("Flag 33 - " + flag33.ToString() + " Unknown");
                        break;
                    }
            }
            message += ("**********************************************");
        }
        public CardInfo ParseTrackData(CardInfo cardDetails)
        {
            if (cardDetails.Track1?.Contains("^") == true)
            {
                var track1array = cardDetails.Track1.Split('^');
                cardDetails.CardHolderName = track1array[1];

                if (track1array.Length == 3 && track1array[2].Length > 4)
                    cardDetails.Expiry = track1array[2].Substring(0, 4);     // Set Expiry from Track1, in case Track2 was not captured
            }

            if (cardDetails.Track2?.Contains("=") == true)
            {
                cardDetails.Expiry = cardDetails.Track2.Substring(cardDetails.Track2.IndexOf("=") + 1, 4);
            }

            return cardDetails;
        }
        #endregion
        
        public class Health
        {
            public ERROR_ID RESULT { get; set; }
            public string MSR_SWIPES { get; set; }
            public string BAD_TRACK1_READS { get; set; }
            public string BAD_TRACK2_READS { get; set; }
            public string BAD_TRACK3_READS { get; set; }
            public string SIGNATURES { get; set; }
            public string REBOOT { get; set; }
            public string DEVICE_NAME { get; set; }
            public string SERIAL_NUMBER { get; set; }
            public string OS_VERSION { get; set; }
            public string APP_VERSION { get; set; }
            public string SECURITY_LIB_VERSION { get; set; }
            public string EFTL_VERSION { get; set; }
            public string EFTP_VERSION { get; set; }
            public string RAM_SIZE { get; set; }
            public string FLASH_SIZE { get; set; }
            public string MANUFACTURE_DATE { get; set; }
            public string CPEM_TYPE { get; set; }
            public string PEN_STATUS { get; set; }
            public string APP_NAME { get; set; }
            public string MANUFACTURE_ID { get; set; }
            public string DIGITIZER_VERSION { get; set; }
            public string MANUFACTURING_SERIAL_NUMBER { get; set; }

            public Health()
            {
                MSR_SWIPES = string.Empty;
                BAD_TRACK1_READS = string.Empty;
                BAD_TRACK2_READS = string.Empty;
                BAD_TRACK3_READS = string.Empty;
                SIGNATURES = string.Empty;
                REBOOT = string.Empty;
                DEVICE_NAME = string.Empty;
                SERIAL_NUMBER = string.Empty;
                OS_VERSION = string.Empty;
                APP_VERSION = string.Empty;
                SECURITY_LIB_VERSION = string.Empty;
                EFTL_VERSION = string.Empty;
                EFTP_VERSION = string.Empty;
                RAM_SIZE = string.Empty;
                FLASH_SIZE = string.Empty;
                MANUFACTURE_DATE = string.Empty;
                CPEM_TYPE = string.Empty;
                PEN_STATUS = string.Empty;
                APP_NAME = string.Empty;
                MANUFACTURE_ID = string.Empty;
                DIGITIZER_VERSION = string.Empty;
                MANUFACTURING_SERIAL_NUMBER = string.Empty;
            }

            public void GetDeviceHealth()
            {

                RESULT = RBA_API.SetParam(PARAMETER_ID.P08_REQ_REQUEST_TYPE, "0");
                RESULT = RBA_API.ProcessMessage(MESSAGE_ID.M08_HEALTH_STAT);
                MSR_SWIPES = RBA_API.GetParam(PARAMETER_ID.P08_RES_COUNT_MSR_SWIPES);
                BAD_TRACK1_READS = RBA_API.GetParam(PARAMETER_ID.P08_RES_COUNT_BAD_TRACK1_READS);
                BAD_TRACK2_READS = RBA_API.GetParam(PARAMETER_ID.P08_RES_COUNT_BAD_TRACK2_READS);
                BAD_TRACK3_READS = RBA_API.GetParam(PARAMETER_ID.P08_RES_COUNT_BAD_TRACK3_READS);
                SIGNATURES = RBA_API.GetParam(PARAMETER_ID.P08_RES_COUNT_SIGNATURES);
                REBOOT = RBA_API.GetParam(PARAMETER_ID.P08_RES_COUNT_REBOOT);
                DEVICE_NAME = RBA_API.GetParam(PARAMETER_ID.P08_RES_DEVICE_NAME);
                SERIAL_NUMBER = RBA_API.GetParam(PARAMETER_ID.P08_RES_SERIAL_NUMBER);
                OS_VERSION = RBA_API.GetParam(PARAMETER_ID.P08_RES_OS_VERSION);
                APP_VERSION = RBA_API.GetParam(PARAMETER_ID.P08_RES_APP_VERSION);
                SECURITY_LIB_VERSION = RBA_API.GetParam(PARAMETER_ID.P08_RES_SECURITY_LIB_VERSION);
                EFTL_VERSION = RBA_API.GetParam(PARAMETER_ID.P08_RES_EFTL_VERSION);
                EFTP_VERSION = RBA_API.GetParam(PARAMETER_ID.P08_RES_EFTP_VERSION);
                RAM_SIZE = RBA_API.GetParam(PARAMETER_ID.P08_RES_RAM_SIZE);
                FLASH_SIZE = RBA_API.GetParam(PARAMETER_ID.P08_RES_FLASH_SIZE);
                MANUFACTURE_DATE = RBA_API.GetParam(PARAMETER_ID.P08_RES_MANUFACTURE_DATE);
                CPEM_TYPE = RBA_API.GetParam(PARAMETER_ID.P08_RES_CPEM_TYPE);
                PEN_STATUS = RBA_API.GetParam(PARAMETER_ID.P08_RES_PEN_STATUS);
                APP_NAME = RBA_API.GetParam(PARAMETER_ID.P08_RES_APP_NAME);
                MANUFACTURE_ID = RBA_API.GetParam(PARAMETER_ID.P08_RES_MANUFACTURE_ID);
                DIGITIZER_VERSION = RBA_API.GetParam(PARAMETER_ID.P08_RES_DIGITIZER_VERSION);
                MANUFACTURING_SERIAL_NUMBER = RBA_API.GetParam(PARAMETER_ID.P08_RES_MANUFACTURING_SERIAL_NUMBER);
            }
        }
        public class Info
        {
            public ERROR_ID RESULT { get; set; }
            public string MANUFACTURE { get; set; }
            public string DEVICE { get; set; }
            public string UNIT_SERIAL_NUMBER { get; set; }
            public string RAM_SIZE { get; set; }
            public string FLASH_SIZE { get; set; }
            public string DIGITIZER_VERSION { get; set; }
            public string SECURITY_MODULE_VERSION { get; set; }
            public string OS_VERSION { get; set; }
            public string APPLICATION_VERSION { get; set; }
            public string EFTL_VERSION { get; set; }
            public string EFTP_VERSION { get; set; }
            public string MANUFACTURING_SERIAL_NUMBER { get; set; }
            public string EMV_DC_KERNEL_TYPE { get; set; }
            public string EMV_ENGINE_KERNEL_TYPE { get; set; }
            public string CLESS_DISCOVER_KERNEL_TYPE { get; set; }
            public string CLESS_EXPRESSPAY_V3_KERNEL_TYPE { get; set; }
            public string CLESS_EXPRESSPAY_V2_KERNEL_TYPE { get; set; }
            public string CLESS_PAYPASS_V3_KERNEL_TYPE { get; set; }
            public string CLESS_PAYPASS_V3_APP_TYPE { get; set; }
            public string CLESS_VISA_PAYWAVE_KERNEL_TYPE { get; set; }
            public string CLESS_INTERAC_KERNEL_TYPE { get; set; }

            public Info()
            {
                MANUFACTURE = string.Empty;
                DEVICE = string.Empty;
                UNIT_SERIAL_NUMBER = string.Empty;
                RAM_SIZE = string.Empty;
                FLASH_SIZE = string.Empty;
                DIGITIZER_VERSION = string.Empty;
                SECURITY_MODULE_VERSION = string.Empty;
                OS_VERSION = string.Empty;
                APPLICATION_VERSION = string.Empty;
                EFTL_VERSION = string.Empty;
                EFTP_VERSION = string.Empty;
                MANUFACTURING_SERIAL_NUMBER = string.Empty;
                EMV_DC_KERNEL_TYPE = string.Empty;
                EMV_ENGINE_KERNEL_TYPE = string.Empty;
                CLESS_DISCOVER_KERNEL_TYPE = string.Empty;
                CLESS_EXPRESSPAY_V2_KERNEL_TYPE = string.Empty;
                CLESS_EXPRESSPAY_V3_KERNEL_TYPE = string.Empty;
                CLESS_PAYPASS_V3_KERNEL_TYPE = string.Empty;
                CLESS_PAYPASS_V3_APP_TYPE = string.Empty;
                CLESS_VISA_PAYWAVE_KERNEL_TYPE = string.Empty;
                CLESS_INTERAC_KERNEL_TYPE = string.Empty;
            }

            public void GetDeviceInfo()
            {
                // Enable Extended Info
                SetDeviceExtendedInfo(true);

                ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.M07_UNIT_DATA);
                MANUFACTURE =  RBA_API.GetParam(PARAMETER_ID.P07_RES_MANUFACTURE);
                DEVICE = RBA_API.GetParam(PARAMETER_ID.P07_RES_DEVICE);//iMP350
                UNIT_SERIAL_NUMBER =  RBA_API.GetParam(PARAMETER_ID.P07_RES_UNIT_SERIAL_NUMBER);
                RAM_SIZE = RBA_API.GetParam(PARAMETER_ID.P07_RES_RAM_SIZE);
                FLASH_SIZE =  RBA_API.GetParam(PARAMETER_ID.P07_RES_FLASH_SIZE);
                DIGITIZER_VERSION = RBA_API.GetParam(PARAMETER_ID.P07_RES_DIGITIZER_VERSION);
                SECURITY_MODULE_VERSION = RBA_API.GetParam(PARAMETER_ID.P07_RES_SECURITY_MODULE_VERSION);
                OS_VERSION = RBA_API.GetParam(PARAMETER_ID.P07_RES_OS_VERSION);
                APPLICATION_VERSION = RBA_API.GetParam(PARAMETER_ID.P07_RES_APPLICATION_VERSION);
                EFTL_VERSION = RBA_API.GetParam(PARAMETER_ID.P07_RES_EFTL_VERSION);
                EFTP_VERSION = RBA_API.GetParam(PARAMETER_ID.P07_RES_EFTP_VERSION);
                MANUFACTURING_SERIAL_NUMBER = RBA_API.GetParam(PARAMETER_ID.P07_RES_MANUFACTURING_SERIAL_NUMBER);
                EMV_DC_KERNEL_TYPE = RBA_API.GetParam(PARAMETER_ID.P07_RES_EMV_DC_KERNEL_TYPE);
                EMV_ENGINE_KERNEL_TYPE = RBA_API.GetParam(PARAMETER_ID.P07_RES_EMV_ENGINE_KERNEL_TYPE);
                CLESS_DISCOVER_KERNEL_TYPE = RBA_API.GetParam(PARAMETER_ID.P07_RES_CLESS_DISCOVER_KERNEL_TYPE);
                CLESS_EXPRESSPAY_V3_KERNEL_TYPE = RBA_API.GetParam(PARAMETER_ID.P07_RES_CLESS_EXPRESSPAY_V3_KERNEL_TYPE);
                CLESS_EXPRESSPAY_V2_KERNEL_TYPE = RBA_API.GetParam(PARAMETER_ID.P07_RES_CLESS_EXPRESSPAY_V2_KERNEL_TYPE);
                CLESS_PAYPASS_V3_KERNEL_TYPE = RBA_API.GetParam(PARAMETER_ID.P07_RES_CLESS_PAYPASS_V3_KERNEL_TYPE);
                CLESS_PAYPASS_V3_APP_TYPE = RBA_API.GetParam(PARAMETER_ID.P07_RES_CLESS_PAYPASS_V3_APP_TYPE);
                CLESS_VISA_PAYWAVE_KERNEL_TYPE = RBA_API.GetParam(PARAMETER_ID.P07_RES_CLESS_VISA_PAYWAVE_KERNEL_TYPE);
                CLESS_INTERAC_KERNEL_TYPE = RBA_API.GetParam(PARAMETER_ID.P07_RES_CLESS_INTERAC_KERNEL_TYPE);

                // Check for Proper DEVICE name (iPP320, iPP350, iSC250, iSC480, etc.)
                RESULT = RBA_API.SetParam(PARAMETER_ID.P08_REQ_REQUEST_TYPE, "0");
                RESULT = RBA_API.ProcessMessage(MESSAGE_ID.M08_HEALTH_STAT);
                string deviceName = RBA_API.GetParam(PARAMETER_ID.P08_RES_DEVICE_NAME) ?? string.Empty;
                if(deviceName.Length > 0 && (DEVICE == null || DEVICE.IndexOf(deviceName, StringComparison.CurrentCultureIgnoreCase) == -1))
                {
                    DEVICE = deviceName;
                }
            }

            public int SetDeviceExtendedInfo(bool enabled)
            {
                RBA_API.SetParam(PARAMETER_ID.P60_REQ_GROUP_NUM, "0013");
                RBA_API.SetParam(PARAMETER_ID.P60_REQ_INDEX_NUM, "0023");
                RBA_API.SetParam(PARAMETER_ID.P60_REQ_DATA_CONFIG_PARAM, enabled ? "1" : "0");
                RBA_API.ProcessMessage(MESSAGE_ID.M60_CONFIGURATION_WRITE);
                int.TryParse(RBA_API.GetParam(PARAMETER_ID.P60_RES_STATUS), out int resultOut);
                return resultOut;
            }
        }
        public class CardInfo
        {
            public string CardHolderName { get; set; }
            public string Expiry { get; set; }
            public string FirstSix { get; set; }
            public string LastFour { get; set; }
            public string PAN { get; set; }
            public string Track1 { get; set; }
            public string Track2 { get; set; }
            public string Track3 { get; set; }
            public string EncryptedTrack { get; set; }

            public string EncryptedPIN { get; set; }

            public class OnGuardInfo
            {
                public string PANLength { get; set; }
                public string MOD10Check { get; set; }    
                public string ServiceCode { get; set; }
                public string LanguageCode { get; set; }
                public string EncryptedFlag { get; set; }
                public string EncryptedFormat { get; set; }
                public string KSNPlus4 { get; set; }
                public string ICEncrytedData { get; set; }
                public int AESPANLength { get; set; }
                public string AESPAN { get; set;  }
                public int LSEncryptedLength { get; set; }
                public string LSEncryptedData { get; set; }
                public string ExtendedCode { get; set; }

            }

        }
    }
}
