using System;

namespace IPA.DAL.RBADAL.Services
{
    [Flags]
    public enum CryptoStatus
    {
        T1Encrypted = 1,
        T2Encrypted = 2,
        T3Encrypted = 4,
        T1Hash = 8,
        T2Hash = 16,
        T3Hash = 32,
        SessionIdPresent = 64,
        KsnPresent = 128
    }

    [Flags]
    public enum MaskStatus
    {
        T1Masked = 1,
        T2Masked = 2,
        T3Masked = 4,
        //Reserved = 8,
        AesEncryption = 16,
        //Reserved = 32,
        PinKeyEncryption = 64,
        SerialNumberPresent = 128
    }

    [Flags]
    public enum TrackStatus
    {
        T1Decoded = 1,
        T2Decoded = 2,
        T3Decoded = 4,
        T1Sampling = 8,
        T2Sampling = 16,
        T3Sampling = 32,
        T2Only_Manual = 17,
        T2AndT3_Manual = 37
        //Reserved = 64

    }

    public enum CardEncodeType
    {
        ISOABA = 0x00,
        ISOABA_Enhanced = 0x80,
        AAMVA = 0x01,
        AAMVA_Enhanced = 0x81,
        Other = 0x03,
        Other_Enhanced = 0x83,
        Raw = 0x04,
        Raw_Enhanced = 0x84,
        manual = 0x85,
        manual_Enhanced = 0xC0
    }

    public enum StatusCode
    {
        NotSupported = 0,
        NotConnected = 1,
        NoResponse = 2,
        Failed = 3,
        Success = 4
    }

    public enum Token
    {
        STK = 0x02,
        ETK = 0X03,
        R = 0X52,
        S = 0X53,
        ACK = 0x06,
        NAK = 0X15
    }

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
        public static byte[] QuickChipModeOff    = { 0x02, 0x06, 0x00, 0x72, 0x53, 0x01, 0x29, 0x01, 0x30, 0x38, 0x20, 0x03 };
        public static byte[] QuickChipModeOn     = { 0x02, 0x06, 0x00, 0x72, 0x53, 0x01, 0x29, 0x01, 0x31, 0x39, 0x21, 0x03 };
        //public static byte[] QuickChipModeOff    = { 0x02, 0x72, 0x53, 0x01, 0x29, 0x01, 0x30, 0x03 };
        //public static byte[] QuickChipModeOn     = { 0x02, 0x72, 0x53, 0x01, 0x29, 0x01, 0x31, 0x03 };
    }
    public static class FeatureResponses
    {
        public static byte[] USBHIDResponse = { 0x06, 0x02, 0x23, 0x01, 0x30, 0x03 };
        public static byte[] USBKBResponse = { 0x06, 0x02, 0x23, 0x01, 0x38, 0x03 };
    }
    public static class ResetConfigCommand
    {
        public static string DTSecureKey = "18";
        public static string DTSecureMag = "18";
        public static string DTSRedKey = "18";
    }
    public static class MustRunCommandTokens
    {
        public static string DTSecureKey = "||230130";
        public static string DTSecureMag = "||230130";
        public static string DTSRedKey = "||230130";
    }
    public static class SetKBCommandTokens
    {
        public static string DTSecureKey = "||230138";
        public static string DTSecureMag = "||230138";
        public static string DTSRedKey = "||230138";
    }
    public enum FuncID
    {
        DefaultConfig = 0x18,  
        MSR = 0x1A,            // 0131 - Enable MSR
        DecodingMethod = 0x1D, // 0131 - Decode in both direction
        USBHIDFmtID = 0x23,    // 0130 - HSB HID ID Tech format, 0138 - USB-HID-KB 
        KeyType = 0x3E,        // 0100 - data key
        EncrytionType = 0x4C,  // 0131 - TDES
        EncryptStr = 0x85,     // 0131 - enhanced (for SecureKey device it only applies > FW v1.14)
        KeyedOptions = 0x8F,   // 0101 - enhanced format
        PrePANID = 0x49,       // 0104: leading 4 digits to display
        PostPANID = 0x4A,      // 0104: trailing 4 digits to display
        SerialNumber = 0x4E,   // 10-bytes serial number starting at byte 3 from the response bytes
        DeviceFormat = 0x77,   // for IDT/XML format values
        SecurityLevel = 0x7E   // Reader's encryption level - 1: no encryption, 2: key loaded, 3: encrypted reader.
    }

    public enum SecurityLevelID
    {
        DUKPTExhausted = 0x30,
        NoEncryption = 0x31,
        KeyLoaded = 0x32,
        EncryptedReader = 0x33,
        AuthenticationRequired = 0x34
    }

    public enum SecurityLevelNumber
    {
        DUKPTExhausted = 0,
        NoEncryption = 1,
        KeyLoaded = 2,
        EncryptedReader = 3,
        AuthenticationRequired = 4,
        NotChecked = 99
    }

    public enum SecureKeyModelFormat
    {
        M100IDT = 0x01,
        M130IDT = 0x05,
        M100XML = 0x09,
        M130XML = 0x0D
    }

    public class DeviceVersion
    {
        public const double V100 = 1.00;
        public const double V114 = 1.14;
        public const double V126 = 1.26;
        public const double V130 = 1.30;
    }

    public class DeviceModelType
    {
        public const string SecureKey = "ID TECH TM3 SecureKey";
        public const string SecureMag = "ID TECH TM3 SecureMag";
        public const string SRedKey = "ID TECH SREDKey";
        public const string SecuRED = "ID TECH TM3 SecuRED";
        public const string Augusta = "ID TECH Augusta";
    }

    public class DeviceModelNumber
    {
        public const string SRedKey = "IDSK-534833TEB";
        public const string SecureKeyM100Xml = "IDKE-504800BL";
        public const string SecureKeyM100Enhanced = "IDKE-504800BM";
        public const string SecureKeyM130Xml = "IDKE-534833BL";
        public const string SecureKeyM130Enhanced = "IDKE-534833BE";
        public const string SecureKeyM130NewFormat = "IDKE-534833BEM";
        public const string SecureMag = "IDRE-33X133B";
        public const string SecuRED = "IDSR-334133TEB";
        public const string AugustKYB = "IDEM-24XX";
        public const string AugustHID = "IDEM-25XX";
    }

    public class IDTSetStatus
    {
        public bool Success = false;
        public string ErrorMsg = string.Empty;
        public byte[] CurrentConfig = null;
        public string RequestedConfig = string.Empty;
    }
}