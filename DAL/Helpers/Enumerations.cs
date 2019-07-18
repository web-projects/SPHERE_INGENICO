using System.ComponentModel;

namespace IPA.Core.Shared.Enums
{
    #region -- Enumerations ---

    public enum DeviceManufacturer
    {
        Unknown = 0,
        IDTech = 1,
        Ingenico = 2
    }
    public enum IDTECH_DEVICE_PID
    {
        MAGSTRIPE_HID = 2010,
        MAGSTRIPE_KYB = 2030,
        SECUREKEY_HID = 2610,
        SECUREKEY_KYB = 2620,
        SECUREMAG_HID = 2810,
        SECUREMAG_KYB = 2820,
        VP3000_HID    = 3530,
        VP3000_KYB    = 3531,
        AUGUSTA_KYB   = 3810,
        AUGUSTA_HID   = 3820,
        AUGUSTAS_HID  = 3920,
        AUGUSTAS_KYB  = 3910,
        VP5300_HID    = 4450,
        VP5300_KYB    = 4650,
        // ^^^ ADDITIONAL PID's HERE ^^^
        UNKNOWN       = 9999
    }

    public enum NotificationType
    {
        UI,
        Systray,
        Log,
        DeviceEvent,
        ACHWorkflow
    }

    public enum DeviceEvent
    {
        CardReadInit = 0,
        CardReadComplete,
        SignatureComplete,
        ManualInputComplete,
        DeviceUpdating,
        DeviceUpdated,
        DeviceUpdateError,
        DeviceError,
        ChipCardDetected,
        DeviceDisconnected
    }

    public enum DeviceStatus
    {
        NoEncryption = 1,
        Connected = 2,
        NoDevice = 3,
        MultipleDevice = 4,
        Unsupported = 5,
        EncyptionDisabled = 6,
        WrongComPort = 7

    }

    public enum ConfigTypeEnum
    {
        AllowPartialPayment = 100,
        AllowSplitTender = 110,
        AllowPartialRefund = 120,
        ForceRefundDays = 130,
        MaxiumumRefundDays = 140,
        RefundTenderTypeID = 150,
        SettlmentTime = 160,
        SettlmentTimeZoneID = 170,
        RequireWorkstationConfig = 180,
        AutomaticDeviceIntake = 190,
        MasterTCCustID = 200,
        CreditCardReceiptMinimum = 210,
        PaymentReceiptMinimum = 220,
        DefaultCCReceiptID = 230,
        CreditCardSignatureMinimum = 270,
        DefaultPaymentReceiptID = 280,
        DefaultLogoID = 290,
        DefaultSourceSystemID = 300,
        AllowPartialAuthorization = 310,
        StoreToken = 320,
        BridgeDNS = 495,
        BridgePort = 496,
        //IsCitrix = 505,
        //32BitOr64Bit = 550,
        TimeoutPedal = 551,
        MsrTimeout = 552,
        JavaAppPath = 553,
        LoggingVerbose = 554,
        LoggingPath = 555,
        JavaTestAppPath = 556,
        TimeoutHttpClient = 557,
        FormsVersionIpp350 = 600,
        FormsVersionIsc250 = 601,
        FormsVersionIsc480 = 602,
        PaymentPort = 603,
        SignaturePort = 604,
        PaymentStatusPort = 605,
        RequireHTTPS = 606,
        PaymentPort2 = 607,
        PaymentUDPIP = 608,
        ShowCvvForm = 675,
        StartMode = 676,
        EnvironmentType = 677,
        SignatureCaptureTimeout = 678,
        JDALConfigPath = 679,
        JDALConfigFileName = 680,
        VerifySignature = 681,
        OSVersion = 682,
        PackageDeployFolder = 683,
        WaitForPayment = 684,
        WaitForSignature = 685,
        SignalRObjectGarbageCollection = 686,
        TimeoutBridge = 687,
        SignalRServerReconnectAttempts = 688,
        SignalRServerReconnectDelay = 689,
        SignalRServerKeepAliveInterval = 690,
        SignalRDeadlockErrorTimeout = 691,
        DALCloseOnUserLock = 692,
        EmvEnabled = 693,
        EmvRetryAttempts = 694,
        ContactlessEnabled = 695,
        LoadTestMode = 696,
        LoadTestModeIterations = 697,
        IPAServiceURL = 698,
        IPASFTP = 699,
        JDALSocketBufferSize = 700,
        EMVMinFirmwareVersion = 701,
        AllowVoid = 702,
        WhiteList = 703,
        DevicePreference = 704,
        DisplayPaymentUI = 705,
        DeviceFirmwareUploadTimeout = 710,
        DeviceFormsUploadTimeout = 711,
        DeviceFirmwareResetTimeout = 712,
        DeviceFormsResetTimeout = 713,
        DeviceFirmwareUpdateTimeout = 714,
        ProcessingPlatform = 715,
        DeviceFormsUpdateTimeout = 716,
        DeviceSettingsIDTSecureKey = 722,
        DeviceSettingsIDTSecureMag = 723,
        DeviceSettingsIDTSRedKey = 724,
        PaymentReceiptFormat = 725,
        TCLinkSendTimeout = 726,
        TCLinkReceiveTimeout = 727,
        ClientLookupHint = 734,
        //TODO: configtypes to be approved by Jana
        DisplayTransactionTime = 735,
        DisplayFailuresTime = 736,
        EpicLookupHint = 737,
        AllLookupHint = 738,
        AchTimeout = 739,
        ManualCardTimeout = 740,
        AutoSelectTenderTimeout = 741,
        ServicePollingInterval = 742,
        ServiceMaxLatency = 743,
        IpaLinkPollingTimeout = 744,
        EnableFirmware = 745,
        EnableForms = 746,
        DownLoadFileBufferSize = 747,
        AdminUserLogin = 748,
        ShowSysTrayDeployStatus = 749,
        IngenicoLoggingLevel = 750,
        LoggingNumDays = 751,
        DeviceSelfCheck = 752,
        PackageCheckPollRate = 754,
        IPALinkInitTime = 759,
        IDTechDisable = 760,
        PorterRestartTime = 762,
        ClockSetThreshold = 763,
        ClockAdjustThreshold = 764,
        AliveReportingPeriod = 765,
        HttpTimeout = 767,
        AppTopMostInterval = 768,
        AppTopMost = 769
    }

    public enum TimerType
    {
        [Description("MsrTimeout")]
        MSR = ConfigTypeEnum.MsrTimeout,
        [Description("EMVTimeout")]
        EMV,                                        // ??
        [Description("SignatureCaptureTimeout")]
        Signature = ConfigTypeEnum.SignatureCaptureTimeout,
        [Description("TransactionTimeout")]
        Transaction,                                // Computed value, not Config
        [Description("AutoSelectTenderTimeout")]
        AutoClose = ConfigTypeEnum.AutoSelectTenderTimeout,
        [Description("TimeoutBridge")]
        ServiceCaller = ConfigTypeEnum.TimeoutBridge,
        [Description("AchTimeout")]
        ACH = ConfigTypeEnum.AchTimeout,
        [Description("ManualCardTimeout")]
        Manual = ConfigTypeEnum.ManualCardTimeout,
        [Description("ServicePollingInterval")]
        ServicePolling = ConfigTypeEnum.ServicePollingInterval,
        [Description("ServiceMaxLatency")]
        ServiceMaxLatency = ConfigTypeEnum.ServiceMaxLatency,
        [Description("IpaLinkPollingTimeout")]
        IPALinkPollingTimer = ConfigTypeEnum.IpaLinkPollingTimeout
    }

    #endregion
}
