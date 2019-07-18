using System.ComponentModel;

namespace IPA.Core.Shared.Helpers.StatusCode
{
    public enum DeclineType
    {
        decline = 200,
        avs = 201,
        cvv = 202,
        call = 203,
        expiredcard = 204,
        carderror = 205,
        authexpired = 206,
        fraud = 207,
        blacklist = 208,
        velocity = 209,
        swiper = 210,
        offlinedecline = 211
    }

    public enum DeviceError
    {
        missingfields = 300,
        extrafields = 301,
        badformat = 302,
        badlength = 303,
        merchantcantaccept = 304,
        mismatch = 305,
        rejectedcorevalidation = 306
    }

    public enum DisplayMessage
    {
        [System.ComponentModel.Description("Signature was not captured. Signature was canceled at device.")]
        SignatureCanceled = 151,
        [System.ComponentModel.Description("Device update in progress.")]
        DeviceUpdate = 171,
        [System.ComponentModel.Description("Device application mismatch or encryption key not found. Verify device encryption key and that the device OS is RBA. Please contact your local administrator for assistance.")]
        InitNoKeyFound = 1600,
        [System.ComponentModel.Description("The detected device encryption configuration has not been properly enabled. Please contact your TrustCommerce representative for assistance.")]
        InitEncryptionNotEnabled = 1601,
        [System.ComponentModel.Description("Application initialization could not be completed. Payment entry device not detected, connect a supported device and start the TCIPADAL application again. If the error persist please contact your TrustCommerce representative.")]
        InitDeviceNotConnected = 1602,
        [System.ComponentModel.Description("")]
        Package = 3055,
        [System.ComponentModel.Description("Application startup error detected.  Please contact your TrustCommerce representative.")]
        InitStartUpError = 3500,
        [System.ComponentModel.Description("An instance of TC IPA AppManager is already running. TC IPA AppManager will now exit. Please start the TC IPA AppManager.")]
        InitStartUpAnotherInstance = 3501,
        [System.ComponentModel.Description("Initializing - DO NOT RUN TRANSACTIONS")]
        InitStartUp = 3502,
        [System.ComponentModel.Description("Initialization Complete - Ready to run transactions")]
        InitStartUpComplete = 3503,
        [System.ComponentModel.Description("Initialization unsuccessful, no approved payment device detected. Please connect a device and start the TCIPADAL application again.")]
        InitErrorNoDevice = 3504,
        [System.ComponentModel.Description("Card device not supported. Please connect a supported card device and try again.")]
        InitErrorInvalidDevice = 3505,
        [System.ComponentModel.Description("Multiple devices detected. Only one supported \"card\" device can be connected to the workstation. Please unplug one of the \"card\" devices and restart TCIPADAL.")]
        MultipleDevice = 3506,
        [System.ComponentModel.Description("User credentials / client key incorrect or no internet access. Please contact TrustCommerce Support. ")]
        ExitErrorCredentialError = 3507,
        [System.ComponentModel.Description("Please close the Payment windows before closing the application.")]
        ExitErrorClosePOSWindow = 3508,
        [System.ComponentModel.Description("Device is disconnected.")]
        ProcessDeviceDisconnect = 3512,
        [System.ComponentModel.Description("Please swipe card.")]
        ProcessSwipeCard = 3513,
        [System.ComponentModel.Description("Invalid card read - please try again.")]
        DALProcessInvalidCardRead = 3514,
        [System.ComponentModel.Description("Processing Payment.")]
        DALProcessProcessingPayment = 3515,
        [System.ComponentModel.Description("Please sign on device.")]
        DALProcessSignature = 3517,
        [System.ComponentModel.Description("MSR, pin pad or signature reached a timeout")]
        DALProcessSignatureTimeout = 3521,
        [System.ComponentModel.Description("Welcome")]
        DALProcessWelcome = 3522,
        [System.ComponentModel.Description("TC IPA")]
        DALWindowTitle = 3523,
        [System.ComponentModel.Description("TrustCommerce TCIPADAL.")]
        DALWindowTitle2 = 3524,
        [System.ComponentModel.Description("A new device has been detected. The TCIPADAL application has closed. Please restart TCIPADAL to initialize the device.")]
        DALProcessDeviceSwap = 3525,
        [System.ComponentModel.Description("TCIPALink login unsuccessful, cannot get security key.  Please contact your TrustCommerce representative for assistance.")]
        IPALinkErrorCredentialError = 3526,
        [System.ComponentModel.Description("TCIPALink StartUp")]
        IPALinkStartupCaption = 3527,
        [System.ComponentModel.Description("Initialization unsuccessful, communication route config mismatch.  Please contact your TrustCommerce representative for assistance.")]
        InitCommRouteError = 3528,
        [System.ComponentModel.Description("TCIPALink login unsuccessful, cannot perform payment request at this time.  Please contact your TrustCommerce representative for assistance.")]
        IPALinkErrorPayment = 3529,
        [System.ComponentModel.Description("")]
        DALWindowIdle = 3530,
        [System.ComponentModel.Description("Connected")]
        DALSystrayConnected = 3531,
        [System.ComponentModel.Description("Transactions cannot be processed. Network or Internet connection was interrupted. Please contact your local administrator.")]
        DALConnectionInterrupted = 3532,
        [System.ComponentModel.Description("IDTECH Device will be set in USB KB Mode and TCIPADAL will close.")]
        DALKBModeMessage = 3533,
        [System.ComponentModel.Description("IDTECH Device USB KB mode update was unsuccessful.  Please contact your TrustCommerce representative for assistance.")]
        DALKBModeConfirmMessage = 3534,
        [System.ComponentModel.Description("Enter card information into device")]
        DALProcessManualEntry = 3535,
        [System.ComponentModel.Description("Invalid POS URL please check the config or contact your local administrator.")]
        POSInvalidURL = 3536,
        [System.ComponentModel.Description("Internet or network connection has been lost. Re-establish connection and retry.  If this error persists please contact your local administrator.")]
        POSNetworkLost = 3537,
        [System.ComponentModel.Description("TCIPALink login unsuccessful, Client Key validation unavailable. Please contact your TrustCommerce representative for assistance.")]
        InitIPALinkInvalidClientKey = 3538,
        [System.ComponentModel.Description("TCIPALink Initialization unsuccessful, invalid or incorrect URI [url.tcipa.com].  Please contact your TrustCommerce representative for assistance.")]
        InitURLErrorIPALink = 3539,
        [System.ComponentModel.Description("TCIPALink login unsuccessful. Unexpected error occurred [error here].  Please contact your TrustCommerce representative for assistance.")]
        IPALinkLoginError = 3540,
        [System.ComponentModel.Description("TCIPADAL initialization unsuccessful, invalid client key.  Please contact your TrustCommerce representative for assistance.")]
        InitDALInvalidClientKey = 3541,
        [System.ComponentModel.Description("TCIPADAL Initialization unsuccessful, invalid or incorrect URI [url.tcipa.com].  Please contact your TrustCommerce representative for assistance.")]
        InitURLErrorIPADAL = 3542,
        [System.ComponentModel.Description("TCIPADAL login unsuccessful, unexpected error occurred [error  code here].  Please contact your TrustCommerce representative for assistance.")]
        DALLoginError = 3543,
        [System.ComponentModel.Description("Page has timed out due to inactivity. Please select OK to close the page and select a POS tray option to continue.  If the error persists, please contact your Trust Commerce representative.")]
        POSAuthenticationError = 3544,
        [System.ComponentModel.Description("Unexpected error has occurred: Please start TCIPADAL. If the error persists, please contact your TrustCommerce representative.")]
        DALNetworkDisconnect = 3545,
        [System.ComponentModel.Description("Network is back online.")]
        DALNetworkOnline = 3546,
        [System.ComponentModel.Description("TCIPALink login unsuccessful, invalid POSClientSystemName [name]. Please contact your TrustCommerce representative for assistance.")]
        POSInvalidClientSysName = 3547,
        [System.ComponentModel.Description("Initialization unsuccessful, invalid TCIPADAL config. Please contact your TrustCommerce representative for assistance.")]
        InitInvalidConfig = 3548,
        [System.ComponentModel.Description("TCIPALink login unsuccessful, Invalid config. Please contact your TrustCommerce representative for assistance.")]
        IPALinkInvalidConfig = 3550,
        [System.ComponentModel.Description("ACH account information page has timed out.  Please process payment again.  If the error persists, please contact your Trust Commerce representative.")]
        DALACHTimeout = 3551,
        [System.ComponentModel.Description("Service unresponsive, a timeout has occurred. Please process payment again. If error persists, please contact your TrustCommerce representative for assistance.")]
        ServiceConfigTimeout = 3552,
        [System.ComponentModel.Description("Please swipe or enter card information.")]
        DALProcessIDTechSwipeCard = 3553,
        [System.ComponentModel.Description("Initialization unsuccessful, Company/TCCustID combination not supported. Please contact TrustCommerce representative for assistance.")]
        InitNoCustID = 3554,
        [System.ComponentModel.Description("ConnectionID could not be established. Please restart TCIPADAL and try again.")]
        DALNoSigR = 3555,
        [System.ComponentModel.Description("Device configuration warning, to resolve this issue please replace the device. Contact your local administrator for assistance.")]
        DALDeviceConfig = 3556,
        [System.ComponentModel.Description("Device firmware updating. Please do not disturb the device while update is in progress. Device will reboot when update has completed.")]
        DALSYSFirmwareUpdate = 3557,
        [System.ComponentModel.Description("Device firmware folder not found. Please contact your TrustCommerce representative for assistance.")]
        DALSYSFirmwareFolder = 3558,
        [System.ComponentModel.Description("Device firmware file not found. Please contact your TrustCommerce representative for assistance.")]
        DALSYSFirmwareFile = 3559,
        [System.ComponentModel.Description("Firmware update successful, device updated. Please restart TCIPADAL.")]
        DALFirmwareUpdated = 3560,
        [System.ComponentModel.Description("Firmware update was unsuccessful, device has not been updated. Please try again. If the error persists please contact your TrustCommerce representative for assistance.")]
        DALFirmwareUpdateFail = 3561,
        [System.ComponentModel.Description("Forms update was unsuccessful, device has not be updated. Please try again. If the error persists please contact your TrustCommerce representative for assistance.")]
        DALFormsUpdateFail = 3562,
        [System.ComponentModel.Description("Device forms updating. Please do not disturb the device while update is in progress. Device will reboot when update has completed.")]
        DALSYSFormUpdate = 3563,
        [System.ComponentModel.Description("Device forms folder not found, for Ingenico[model]. Please contact your TrustCommerce representative for assistance.")]
        DALSYSFormsFolder = 3564,
        [System.ComponentModel.Description("Device forms file not found, for Ingenico[model]. Please contact your TrustCommerce representative for assistance.")]
        DALSYSFormsFile = 3565,
        [System.ComponentModel.Description("Forms update successful, device updated. Please restart TCIPADAL.")]
        DALFormsUpdated = 3566,
        [System.ComponentModel.Description("Initialization unsuccessful, URL/ClientKey Config error or instance of TCIPADAL not available. Please contact your TrustCommerce representative for assistance.")]
        InitCredError = 3567,
        [System.ComponentModel.Description("Payment could not be processed. Payment canceled by user.")]
        CancelPayment = 3568,
        [System.ComponentModel.Description("Could not read card.")]
        BadCardSwipe = 3569,
        [System.ComponentModel.Description("TC IPA AppManager error, TCIPADAL configuration file cannot be read. Please contact your TrustCommerce representative for assistance.")]
        ConfigCanNotRead = 3572,
        [System.ComponentModel.Description("TCIPADAL cannot be initialized without TC IPA AppManager, please start AppManager. If the error persists please contact your TrustCommerce representative for assistance.")]
        LaunchFailNoAM = 3573,
        [System.ComponentModel.Description("TC IPA could not search for updates, invalid or incorrect [url.tcipa.com] or service unavailable. TCIPA AppManger will launch, please wait. Please contact your TrustCommerce representative if the update error persists.")]
        UpdateServerError = 3583,
        [System.ComponentModel.Description("TCIPADAL has closed. Please exit TC IPA AppManager and restart.")]
        DALClosed = 3584,
        [System.ComponentModel.Description("TC IPA application will close.  Company certificate is invalid or has expired.  Please contact your TrustCommerce representative for assistance.")]
        InvalidCertificate = 3587,
        [System.ComponentModel.Description("An instance of TCIPADAL is already running, application will now exit. Please close all instances of the TCIPADAL application before launching.")]
        InitStartUpAnotherInstanceVDI = 3588,
        [System.ComponentModel.Description("TCIPADAL has closed, please restart TCIPADAL before attempting to process payments.")]
        DALClosedVDI = 3589,
        [System.ComponentModel.Description("TCIPADAL and TC IPA AppManager have closed. Restart TC IPA AppManager, please verify TCIPADAL is ready before processing payment.")]
        DALClosedWithAppManager = 3590
    }

    public enum DisplayResults
    {
        [System.ComponentModel.Description("Payment Accepted.")]
        ProcessPaymentAccept = 3509,
        [System.ComponentModel.Description("Payment Declined.")]
        ProcessPaymentDecline = 3510,
        [System.ComponentModel.Description("Payment has errored.")]
        ProcessPaymentError = 3511,
        [System.ComponentModel.Description("Payment Approved.")]
        DALProcessApproved = 3516,
    }

    public enum EntryModeStatus
    {
        [System.ComponentModel.Description("Payment canceled by customer.")]
        Canceled = 150,
        [System.ComponentModel.Description("Signature was not captured. Signature was canceled at device.")]
        SignatureCanceled = 151,
        [System.ComponentModel.Description("Error reading card. Maximum number of swipes or inserts has been reached. Transaction canceled.")]
        CardNotRead = 155,
        [System.ComponentModel.Description("Device has timed out due to inactivity.")]
        Timeout = 160,
        [System.ComponentModel.Description("Do not need")]
        Error = 165,
        [System.ComponentModel.Description("Card Blocked, please call card issuer for assistance. Swipe another card.")]
        cardblocked = 166,
        [System.ComponentModel.Description("This card is not supported, please provide another card.")]
        Unsupported = 170,
        [System.ComponentModel.Description("DAL no DAL")]
        NoTCIPADAL = 175,
        [System.ComponentModel.Description("Error encountered during pin entry.")]
        ErrorPinEntry = 178,
        [System.ComponentModel.Description("Pin try limit exceeded.")]
        PinEntryExceed = 179,
        [System.ComponentModel.Description("Successful")]
        Success = 180,
        [System.ComponentModel.Description("User retry the card.")]
        Retry = 1000,
    }

    public enum ErrorField
    {
        missingfields = 300,
        extrafields = 301,
        badformat = 302,
        badlength = 303,
        merchantcantaccept = 304,
        mismatch = 305,
        rejectedcorevalidation = 306,
        InvalidCustID = 901
    }

    public enum ErrorType
    {
        InactiveTCBillingID = 399,
        cantconnect = 400,
        dnsvalue = 401,
        linkfailure = 402,
        failtoprocess = 403,
        notallowed = 404,
        corrupted_trackdata = 405,
        failedatprocessor = 406,
        failedattclink = 408,
        DeviceSwitched = 409
    }

    public enum IPAErrorField
    {
        [System.ComponentModel.Description("Payment could not be processed. TCTransactionID referenced could not be found or is incorrect.")]
        TCTransIDNotFound = 110,
        [System.ComponentModel.Description("Payment could not be processed. TCTransactionID referenced has already been voided or reversed. No available balance left to void or reverse.")]
        TCTransIDCxlNoAmount = 112,
        [System.ComponentModel.Description("Payment could not be processed. TCTransactionID referenced  cannot be refunded. Payment must be processed as a void.")]
        TCTransIDRefund = 113,
        [System.ComponentModel.Description("Payment could not be processed. Payment request included data not allowed for the requested transaction type. Please contact your TrustCommerce representative for assistance.")]
        TransTypeOffender = 120,
        [System.ComponentModel.Description("Payment could not be processed. TCTransactionID does not match the original CompanyID or CustID from originating sale or Pre-Auth. Please contact your TrustCommerce representative for assistance.")]
        TCTransIDNoMatch = 125,
        [System.ComponentModel.Description("Token update does not match the BillingID's original CustID and cannot be processed.")]
        TCTokenCustIDMisMatch = 126,
        [System.ComponentModel.Description("Request cannot be processed. Token information missing from the request.")]
        TokenAttributeError = 127,
        [System.ComponentModel.Description("Payment could not be processed. TCTransactionID referenced cannot process a void on a declined payment. Please contact your TrustCommerce representative for assistance.")]
        TransTypeCxlDecline = 130,
        [System.ComponentModel.Description("Payment could not be processed. TCTransactionID referenced has already been refunded. Please contact your TrustCommerce representative for assistance.")]
        TCTransIDCreditNoAmount = 140,
        [System.ComponentModel.Description("Invalid Amount. Please correct amount and try again.")]
        InvalidAmount = 142,
        [System.ComponentModel.Description("Payment could not be processed. TransactionID for specified payment type was not sent in the payment request. Please contact your TrustCommerce representative for assistance.")]
        IPALinkErrorNoTransID = 143,
        [System.ComponentModel.Description("Invalid Token: Unstore request could not be processed. Please contact your TrustCommerce representative for assistance.")]
        UnstoreInvalidToken = 145,
        [System.ComponentModel.Description("Format of input received is incorrect, please contact your TrustCommerce representative for assistance.")]
        InputBadFormat = 186,
        [System.ComponentModel.Description("Device update in progress. Do not restart. Please wait until the process has completed.")]
        DeviceUpdate = 407,
        [System.ComponentModel.Description("New device has been detected. Please restart TCIPADAL to initialize the device.")]
        DeviceSwitched = 409,
        [System.ComponentModel.Description("Account number did not pass the required minimum validation.")]
        InvalidAcctNo = 455,
        [System.ComponentModel.Description("Payment has been processed successfully. Card information was not stored. Please contact your TrustCommerce representative.")]
        TCTransTokenNotStored = 706,
        [System.ComponentModel.Description("Account information entered did not pass validation, please re-enter")]
        InvalidAchRouteNo = 902,
        [System.ComponentModel.Description("Request cannot be processed.  Requested payment type is invalid. Please contact your TrustCommerce representative.")]
        PaymentTypeInvalid = 3575,
        [System.ComponentModel.Description("Not able to process request. Tender type information received invalid, please contact your TrustCommerce representative for assistance.")]
        UpdateTenderInvalid = 3581,
        [System.ComponentModel.Description("Payment request could not be processed. One or more required attributes could not be validated.")]
        ContractValidationFailed = 3582,
    }

    public enum IPAErrorType
    {
        [System.ComponentModel.Description("Initialization unsuccessful, no Internet Connection could be found. Please check user permissions or network settings. Contact your local administrator for assistance.")]
        NoNetwork = 141,
        [System.ComponentModel.Description("TCCustID config not supported. Please contact your TrustCommerce representative for assistance.")]
        InitConfigAttributeError = 144,
        [System.ComponentModel.Description("Unexpected error has occurred: Please start TCIPADAL. If the error persists, please contact your TrustCommerce representative for assistance.")]
        POSCommunicationAppClosed = 146,
        [System.ComponentModel.Description("Initialization unsuccessful, no Internet Connection could be found. Please check user permissions or network settings. Contact your local administrator for assistance.")]
        InitNoNetwork = 147,
        [System.ComponentModel.Description("Payment could not be processed. Please process payment again, if the error persists please contact your TrustCommerce representative for assistance.")]
        EntryModeUnknown = 152,
        [System.ComponentModel.Description("Cannot communicate with TCIPADAL at this time, please restart the application. If issue persists, please contact your TrustCommerce representative for assistance.")]
        NoTCIPADAL = 175,
        [System.ComponentModel.Description("TCIPALink unable to communicate with Services. If issue persists, please contact your TrustCommerce representative for assistance.")]
        NoBridge = 176,
        [System.ComponentModel.Description("Device not found")]
        NoDevice = 177,
        [System.ComponentModel.Description("Device configuration mismatch. Please restart TCIPADAL to initialize the device. If issue persists, please contact your TrustCommerce representative for assistance.")]
        DeviceConfigMismatch = 185,
        [System.ComponentModel.Description("Unable to decrypt request received. Please contact your TrustCommerce representative for assistance.")]
        DecryptError = 450,
        [System.ComponentModel.Description("TCIPA validation could not be completed.  Please contact your TrustCommerce representative for assistance.")]
        PKIDecryptError = 451,
        [System.ComponentModel.Description("Initialization cannot be performed. Workstation DNS mismatch. Please contact your Trust Commerce representative.")]
        MismatchedLinkTCIPADAL = 452,
        [System.ComponentModel.Description("Initialization cannot be performed. Please start the TCIPADAL application.")]
        ConnectToTCIPADALFailed = 453,
        [System.ComponentModel.Description("Payment could not be processed. Payment has timed out due to inactivity.")]
        TransactionTimeOut = 454,
        [System.ComponentModel.Description("TCIPADAL Not Ready")]
        DALNotReady = 1003,
        [System.ComponentModel.Description("Request could not be completed. ApplicationID could not be found.")]
        AppRollCallInvalid = 1004,
        [System.ComponentModel.Description("Payment request could not be processed. CustID's controller is not active, please contact your TrustCommerce representative for assistance. ")]
        InactiveControllerID = 3585,
    }

    public enum InitializationFlow
    {
        [Description("TCIPALink Initialization Request Received.")]
        InitIPALink = 1500,

        [Description("Hub received an Init request from an application")]
        InitHub = 1501,

        [Description("DAL received an Init Request from an application")]
        InitDAL = 1502,

        [Description("Service received an Init response from an application")]
        InitService = 1503,

        [Description("Initialization completed - send response to an application")]
        InitComplete = 1504,

        IPALinkInitSuccess = 1505
    }

    public enum PaymentResultStatus
    {
        approved = 100,
        accepted = 101,
        decline = 102,
        baddata = 103,
        error = 104,
        rejected = 107,
        linkfailure = 402,
        failtoprocess = 403,
        failedatprocessor = 406,
        failedattclink = 408
    }

    public enum POR
    {
        White = 3100,
        Green = 3105,
        Red = 3110,
        Black = 3115,
        Unregistered = 3120,
        Inactive = 3125,
        EndOfLife = 3130,
        Invalid = 3135,
        InitNoKeyFound = 1600,
        InitEncryptionNotEnabled = 1601,
        InitDeviceNotConnected = 1602
    }

    public enum TransactionStatus
    {
        [System.ComponentModel.Description("Payment Accepted.")]
        ProcessPaymentAccept = 3509,
        [System.ComponentModel.Description("Payment Declined.")]
        ProcessPaymentDecline = 3510,
        [System.ComponentModel.Description("Payment has errored.")]
        ProcessPaymentError = 3511,
        [System.ComponentModel.Description("Payment Approved.")]
        DALProcessApproved = 3516,
        PaymentStarted = 900,
    }






    [System.Obsolete("This was deprecated and was split into multiple different enums. DO NOT USE THIS ENUM any more.")]
    public enum StatusCodeEnum
    {

        [System.ComponentModel.Description("Approved")]
        TransactionStatus_approved = 100,
        [System.ComponentModel.Description("Payment has been sumitted pending approval")]
        TransactionStatus_accepted = 101,
        [System.ComponentModel.Description("Do not need")]
        TransactionStatus_decline = 102,
        [System.ComponentModel.Description("Do not need")]
        TransactionStatus_baddata = 103,
        [System.ComponentModel.Description("Cannot communicate with Processor")]
        TransactionStatus_error = 104,
        //[System.ComponentModel.Description("Card Blocked, please call card issuer for assistance. Swipe another card.")]
        //cardblocked = 106,
        [System.ComponentModel.Description("Rejected on a transaction that may have been accepted but not yet approved")]
        TransactionStatus_rejected = 107,
        [System.ComponentModel.Description("TCTransactionID referenced could not be found or is incorrect.")]
        TransactionStatus_TCTransIDNotFound = 110,
        [System.ComponentModel.Description("TCTransactionID referenced has occurred in the past or settled and cannot be voided. Transaction must be processed as a credit.")]
        TransactionStatus_TCTransIDCredit = 111,
        [System.ComponentModel.Description("TCTransactionID referenced has already been voided or reversed. No available balance left to void or reverse.")]
        TransactionStatus_TCTransIDCxlNoAmount = 112,
        [System.ComponentModel.Description("TCTransactionID referenced cannot be refunded. Payment must be processed as a Void.")]
        TransactionStatus_TCTransIDRefund = 113,
        [System.ComponentModel.Description("Transaction request included data not allowed for the requested transaction type. Transaction could not be processed.")]
        TransactionStatus_TransTypeOffender = 120,
        [System.ComponentModel.Description("TCTransactionID does not match the original CompanyID or CustID from originating sale or Pre-Auth and cannot be processed.")]
        TransactionStatus_TCTransIDNoMatch = 125,
        [System.ComponentModel.Description("TCTransactionID referenced cannot process a void on a declined payment. Transaction could not be processed.")]
        TransactionStatus_TransTypeCxlDecline = 130,
        [System.ComponentModel.Description("TCTransactionID referenced has already been refunded. Transaction cannot be processed.")]
        TCTransIDCreditNoAmount = 140,
        [System.ComponentModel.Description("Login error, no Internet Connection could be found. Please check your permissions or network settings. Contact your local administrator for assistance.")]
        TransactionStatus_IPALink_NoNetwork = 141,
        [System.ComponentModel.Description("Invalid Amount format. Please correct amount and try again.")]
        TransactionStatus_IPALink_InvalidAmount = 142,
        [System.ComponentModel.Description("TransactionID for specified  payment type was not sent in the payment request. Please try payment again.")]
        TransactionStatus_IPALink_error_NoTransID = 143,
        [System.ComponentModel.Description("TCCustID config not supported. Please contact your TrustCommerce representative for assistance.")]
        IPAErrorType_InitConfigAttributeError = 144,
        [System.ComponentModel.Description("Invalid Token: Unstore request could not be processed. Please contact your TrustCommerce representative for assistance.")]
        TransactionStatus_DAL_Unstore_InvalidToken = 145,
        [System.ComponentModel.Description("Unexpected error has occurred: Please start TC IPADAL. If the error persists, please contact your TrustCommerce representative.")]
        TransactionStatus_POS_Communication_AppClosed = 146,
        [System.ComponentModel.Description("Login error, no Internet Connection could be found. Please check your permissions or network settings. Contact your local administrator for assistance.")]
        TransactionStatus_DAL_NoNetwork = 147,
        [System.ComponentModel.Description("Payment canceled by user")]
        EntryModeStatus_Canceled = 150,
        [System.ComponentModel.Description("Signature was not captured. Signature was canceled at device.")]
        DisplayMessage_SignatureCanceled = 151,
        [System.ComponentModel.Description("Entry Mode unknown, services encountered an error. Please try again.")]
        EntryModeStatus_EntryModeUnknown = 152,
        [System.ComponentModel.Description("Error reading card. Maximum number of swipes or inserts has been reached. Transaction canceled.")]
        EntryModeStatus_CardNotRead = 155,
        [System.ComponentModel.Description("Device has timed out due to inactivity.")]
        EntryModeStatus_Timeout = 160,
        [System.ComponentModel.Description("Do not need")]
        EntryModeStatus_Error = 165,
        [System.ComponentModel.Description("Card Blocked, please call card issuer for assistance. Swipe another card.")]
        EntryModeStatus_cardblocked = 166,
        [System.ComponentModel.Description("This card is not supported, please provide another card.")]
        EntryModeStatus_Unsupported = 170,
        [System.ComponentModel.Description("Device update in progress.")]
        EntryModeStatus_DeviceUpdate = 171,
        [System.ComponentModel.Description("No DAL or DAL failure.")]
        EntryModeStatus_NoDal = 175,
        [System.ComponentModel.Description("IPALink cannot find bridge.")]
        EntryModeStatus_NoBridge = 176,
        [System.ComponentModel.Description("Device not found")]
        EntryModeStatus_NoDevice = 177,
        [System.ComponentModel.Description("Error encountered during pin entry.")]
        EntryModeStatus_ErrorPinEntry = 178,
        [System.ComponentModel.Description("Pin try limit exceeded.")]
        EntryModeStatus_PinEntryExceed = 179,
        [System.ComponentModel.Description("Successful")]
        EntryModeStatus_Success = 180,
        [System.ComponentModel.Description("Device Configuration Mismatch")]
        EntryModeStatus_DeviceConfigMismatch = 185,
        [System.ComponentModel.Description("Declined, not enough funds available on card")]
        DeclineType_decline = 200,
        [System.ComponentModel.Description("Street number entered does not match billing address on card")]
        DeclineType_avs = 201,
        [System.ComponentModel.Description("cvv not valid, re-enter cvv.")]
        DeclineType_cvv = 202,
        [System.ComponentModel.Description("Call card issuer for authorization.")]
        DeclineType_call = 203,
        [System.ComponentModel.Description("Card is expired, re-enter exp date or swipe another card")]
        DeclineType_expiredcard = 204,
        [System.ComponentModel.Description("Card not valid, re-enter card number")]
        DeclineType_carderror = 205,
        [System.ComponentModel.Description("Post Auth submission is more than 14 days old")]
        DeclineType_authexpired = 206,
        [System.ComponentModel.Description("Payment does not meet the CrediGuard threshold requirments")]
        DeclineType_fraud = 207,
        [System.ComponentModel.Description("One or more fields contained a CrediGuard blacklist value, call for approval. Please contact your TrustCommerce representative for assistance.")]
        DeclineType_blacklist = 208,
        [System.ComponentModel.Description("Velocity rules were not met, check velocity type or velocity value")]
        DeclineType_velocity = 209,
        [System.ComponentModel.Description("Could not read, please re-swipe card")]
        DeclineType_swiper = 210,
        [System.ComponentModel.Description("Declined via offline transaction")]
        DeclineType_offlinedecline = 211,
        [System.ComponentModel.Description("Payment cannot be processed, one or more fields is missing required data. Please contact your TrustCommerce representative for assistance.")]
        ErrorField_missingfields = 300,
        [System.ComponentModel.Description("Payment cannot be processed, invalid fields.  Please contact your TrustCommerce representative for assistance.")]
        ErrorField_extrafields = 301,
        [System.ComponentModel.Description("Payment cannot be processed, invalid format.  Please contact your TrustCommerce representative for assistance.")]
        ErrorField_badformat = 302,
        [System.ComponentModel.Description("Payment cannot be processed, length requirements not met.  Please contact your TrustCommerce representative for assistance.")]
        ErrorField_badlength = 303,
        [System.ComponentModel.Description("Payment cannot be processed, data within request not supported.  Please contact your TrustCommerce representative for assistance.")]
        ErrorField_merchantcantaccept = 304,
        [System.ComponentModel.Description("Payment cannot be processed, validation requirements were not met.  Please contact your TrustCommerce representative for assistance.")]
        ErrorField_mismatch = 305,
        [System.ComponentModel.Description("Devices injected key is invalid. Please call your local administrator for assistance.")]
        ErrorType_cantconnect = 400,
        [System.ComponentModel.Description("Unable to resolve DNS Host Name")]
        ErrorType_dnsvalue = 401,
        [System.ComponentModel.Description("Communication lost with processor during the payment")]
        ErrorType_linkfailure = 402,
        [System.ComponentModel.Description("TC IPA message: Payment cannot be processed at this time. Contact Trust Commerce for assistance.")]
        ErrorType_failtoprocess = 403,
        [System.ComponentModel.Description("Not authorized to process this type of payment")]

        ErrorType_notallowed = 404,
        [System.ComponentModel.Description("Corrupted Trackdata identified. Please contact your TrustCommerce representative for assistance. ")]

        ErrorType_corrupted_trackdata = 405,
        [System.ComponentModel.Description("Communication with the processor can not be established. Please contact your TrustCommerce representative for assistance.")]

        ErrorType_failedatprocessor = 406,
        [System.ComponentModel.Description("Device update in progress. Do not restart. Please wait until the process has completed.")]

        ErrorType_DeviceUpdate = 407,
        [System.ComponentModel.Description("Payment cannot be processed at this time, communication with TrustCommerce cannot be established. \r\n Contact TrustCommerce for assistance.")]

        ErrorType_failedattclink = 408,
        [System.ComponentModel.Description("A new device has been detected. Please restart TCIPADAL to initialize the device.")]

        IPAErrorField_DeviceSwitched = 409,
        [System.ComponentModel.Description("Unable to decrypt request received.  If this error persists please contact your TrustCommerce representative for assistance.")]

        ErrorType_DecryptError = 450,
        [System.ComponentModel.Description("TC IPA validation could not be completed.  Please contact your TrustCommerce representative for assistance.")]

        ErrorType_PKIDecryptError = 451,
        [System.ComponentModel.Description("Process initialization error. Workstation DNS mismatch. Please contact your Trust Commerce representative.")]

        ErrorType_MismatchedLinkDALIP = 452,
        [System.ComponentModel.Description("Initialization cannot be performed . Please start the TC IPADAL application.")]
        ErrorType_ConnectToDALFailed = 453,
        [System.ComponentModel.Description("Payment has timed out due to inactivity.")]
        ErrorType_TransactionTimeOut = 454,
        [System.ComponentModel.Description("Account number did not pass the required minimum validation.")]
        IPAErrorField_InvalidAcctNo = 455,
        [System.ComponentModel.Description("CVV2/CVC2/Discover CID match")]
        CVVResponseCode_m = 500,
        [System.ComponentModel.Description("CVV2/CVC2/CID did not match")]
        CVVResponseCode_n = 501,
        [System.ComponentModel.Description("Not Processed")]
        CVVResponseCode_p = 502,
        [System.ComponentModel.Description("CVV2/CVC2/CID was not sent")]
        CVVResponseCode_s = 503,
        [System.ComponentModel.Description("Issuer does not support this field.")]
        CVVResponseCode_u = 504,
        [System.ComponentModel.Description("Street address matches, but five-digit and nine-digit postal code do not match.")]
        AddressVerificationSystem_A = 600,
        [System.ComponentModel.Description("Street address matches, but postal code not verified.")]
        AddressVerificationSystem_B = 601,
        [System.ComponentModel.Description("Street address and postal code do not match.")]
        AddressVerificationSystem_C = 602,
        [System.ComponentModel.Description("Street address and postal code match. Code \"M\" is equivalent.")]
        AddressVerificationSystem_D = 603,
        [System.ComponentModel.Description("AVS data is invalid or AVS is not allowed for this card type.")]
        AddressVerificationSystem_E = 604,
        [System.ComponentModel.Description("Card member name does not match, but billing postal code matches.")]
        AddressVerificationSystem_F = 605,
        [System.ComponentModel.Description("Non-U.S. issuing bank does not support AVS.")]
        AddressVerificationSystem_G = 606,
        [System.ComponentModel.Description("Card member name does not match. Street address and postal code match.")]
        AddressVerificationSystem_H = 607,
        [System.ComponentModel.Description("Address not verified.")]
        AddressVerificationSystem_I = 608,
        [System.ComponentModel.Description("Card member name, billing address, and postal code match.")]
        AddressVerificationSystem_J = 609,
        [System.ComponentModel.Description("Card member name matches but billing address and billing postal code do not match.")]
        AddressVerificationSystem_K = 610,
        [System.ComponentModel.Description("Card member name and billing postal code match, but billing address does not match.")]
        AddressVerificationSystem_L = 611,
        [System.ComponentModel.Description("Street address and postal code match. Code \"D\" is equivalent.")]
        AddressVerificationSystem_M = 612,
        [System.ComponentModel.Description("Street address and postal code do not match.")]
        AddressVerificationSystem_N = 613,
        [System.ComponentModel.Description("Card member name and billing address match, but billing postal code does not match.")]
        AddressVerificationSystem_O = 614,
        [System.ComponentModel.Description("Postal code matches, but street address not verified.")]
        AddressVerificationSystem_P = 615,
        [System.ComponentModel.Description("Card member name, billing address, and postal code match.")]
        AddressVerificationSystem_Q = 616,
        [System.ComponentModel.Description("Processor address verification system is unavailable, please try again.")]
        AddressVerificationSystem_R = 617,
        [System.ComponentModel.Description("Bank does not support AVS.")]
        AddressVerificationSystem_S = 618,
        [System.ComponentModel.Description("Card member name does not match, but street address matches.")]
        AddressVerificationSystem_T = 619,
        [System.ComponentModel.Description("Address information unavailable.")]
        AddressVerificationSystem_U = 620,
        [System.ComponentModel.Description("Card member name, billing address, and billing postal code match.")]
        AddressVerificationSystem_V = 621,
        [System.ComponentModel.Description("Street address does not match, but nine-digit postal code matches.")]
        AddressVerificationSystem_W = 622,
        [System.ComponentModel.Description("Street address and nine-digit postal code match.")]
        AddressVerificationSystem_X = 623,
        [System.ComponentModel.Description("Street address and five-digit postal code match.")]
        AddressVerificationSystem_Y = 624,
        [System.ComponentModel.Description("Street address does not match, but five-digit postal code matches")]
        AddressVerificationSystem_Z = 625,
        [System.ComponentModel.Description("One or more values required for this payment were not received.")]
        BadData_missingfields = 700,
        [System.ComponentModel.Description("Received values not supported.")]
        BadData_extrafields = 701,
        [System.ComponentModel.Description("Value received does not meet the length requirements")]
        BadData_badlength = 702,
        [System.ComponentModel.Description("Format of input is incorrect, please enter")]
        BadData_badformat = 703,
        [System.ComponentModel.Description("Card brand not accepted.")]

        BadData_merchantcantaccept = 704,
        [System.ComponentModel.Description("More data was sent then required, causing a conflict in processing the payment")]

        BadData_mismatch = 705,
        [System.ComponentModel.Description("Payment has been processed successfully. Card information was not stored. Please contact your TrustCommerce representative.")]

        TCTransTokenNotStored = 706,
        [System.ComponentModel.Description("Device has Successfully received the package update")]

        PackageDeployStatus_Success = 801,
        [System.ComponentModel.Description("Error updating file")]

        PackageDeployStatus_Error = 802,
        [System.ComponentModel.Description("Device has been denied security access  package update")]

        PackageDeployStatus_Securitydenied = 803,
        [System.ComponentModel.Description("TC IPADAL has not replied")]

        PackageDeployStatus_NoReplyYet = 804,
        [System.ComponentModel.Description("Packaged delivered to client")]

        PackageDeployStatus_FileOnClient = 805,
        [System.ComponentModel.Description("Packaged delivered to device")]

        PackageDeployStatus_FileOnDevice = 806,
        [System.ComponentModel.Description("Package cleaned from client")]

        PackageDeployStatus_FileCleaned = 807,
        [System.ComponentModel.Description("Port error trying to deploy package. The device is running on the wrong port.")]

        PackageDeployStatus_ErrorOnPort = 808,
        [System.ComponentModel.Description("Device timed out during package upload")]

        PackageDeployStatus_DeviceTimeOut = 809,
        [System.ComponentModel.Description("Checking for Package updates")]

        PackageDeployStatus_CheckUpdate = 810,
        [System.ComponentModel.Description("Downloading pending, please wait. ")]

        PackageDeployStatus_DownloadPending = 811,
        [System.ComponentModel.Description("Package updated successfully.")]

        PackageDeployStatus_PackageUpdateSuccess = 812,
        [System.ComponentModel.Description("Package update could not be completed. Workstation will be returned to the previous version. Please wait, rollback in progress.")]

        PackageDeployStatus_PackageUpdateFailed = 813,
        [System.ComponentModel.Description("NA")]
        PackageDeployStatus_FormsDeviceMismatch = 814,
        [System.ComponentModel.Description("NA")]
        PackageDeployStatus_FirmwareDeviceMismatch = 815,
        [System.ComponentModel.Description("NA")]
        PackageDeployStatus_PackageDownloadFailed = 816,
        [System.ComponentModel.Description("Rollback successful.")]
        PackageDeployStatus_RollbackComplete = 817,
        [System.ComponentModel.Description("Workstation update to date.  No package updates required.")]
        PackageDeployStatus_UpdateNotRequired = 818,
        [System.ComponentModel.Description("Package update pending.")]
        PackageDeployStatus_UpdatePending = 819,
        [System.ComponentModel.Description("All package updates have been applied successfully. Workstation is up to date, exit both TCIPADAL and TC IPA AppManager from systray. Please restart TC IPA AppManager.")]
        DeployUpdateSuccess = 820,
        [System.ComponentModel.Description("NA")]
        PaymentStarted = 900,
        [System.ComponentModel.Description("Not able to process request. The Cust ID or password is invalid, please contact your Administrator.")]

        InvalidCustID = 901,
        [System.ComponentModel.Description("Account information entered did not pass validation, please re-enter")]

        InvalidAchRouteNo = 902,
        [System.ComponentModel.Description("Not able to process request. The Cust ID used has no access to the ServiceURL reached, please contact your Administrator.")]

        InvalidServiceURL = 910,
        [System.ComponentModel.Description("User retry the card.")]

        EntryModeStatus_Retry = 1000,
        [System.ComponentModel.Description("Not able to process request. The Cust ID used has no access to the ServiceURL reached, please contact your Administrator.")]

        EntryModeStatus_InvalidServiceURL = 1001,
        [System.ComponentModel.Description("Multiple devices connected.")]

        EntryModeStatus_MultipleDevice = 1002,
        [System.ComponentModel.Description("TCIPADAL Not Ready")]

        IPAErrorType_DALNotReady = 1003,
        [System.ComponentModel.Description("TCIPALink Initialization Request Received.")]

        StatusType_InitIPALink = 1500,
        [System.ComponentModel.Description("Hub received an Init request from an application")]

        InitializationFlow_InitHub = 1501,
        [System.ComponentModel.Description("DAL recevied an Init Request from an application")]

        InitializationFlow_InitDAL = 1502,
        [System.ComponentModel.Description("Service received an Init response from an application")]

        InitializationFlow_InitService = 1503,
        [System.ComponentModel.Description("Initialization completed - send response to an application")]

        InitializationFlow_InitComplete = 1504,
        [System.ComponentModel.Description("IPALink successfully sent Initialization Response to calling application")]

        InitializationFlow_IPALinkInitSuccess = 1505,
        [System.ComponentModel.Description("The application started polling awaiting complete")]

        MessagePolling_MessagePollingStarted = 1550,
        [System.ComponentModel.Description("Polling Completed")]

        MessagePolling_MessagePollingComplete = 1551,
        [System.ComponentModel.Description("Device application mismatch or encryption key not found. Verify device encryption key and that the device OS is RBA. Please contact your local administrator for assistance.")]

        DisplayMessage_InitNoKeyFound = 1600,
        [System.ComponentModel.Description("The detected device encryption configuration has not been properly enabled. Please contact your TrustCommerce representative for assistance.")]

        DisplayMessage_InitEncryptionNotEnabled = 1601,
        [System.ComponentModel.Description("Application initialization could not be completed. Payment entry device not detected, connect a supported device and start the TCIPADAL application again. If the error persist please contact your TrustCommerce representative.")]

        DisplayMessage_InitDeviceNotConnected = 1602,
        [System.ComponentModel.Description("NA")]

        TransactionType_Charge = 2000,
        [System.ComponentModel.Description("NA")]

        TransactionType_TokenRequest = 2005,
        [System.ComponentModel.Description("NA")]

        TransactionType_Identification = 2010,
        [System.ComponentModel.Description("NA")]

        TransactionType_Preauthorization = 2015,
        [System.ComponentModel.Description("US Dollar")]

        CurrencyCode_USD = 2100,
        [System.ComponentModel.Description("Currency Code not supported, USD only")]

        CurrencyCode_EUR = 2105,
        [System.ComponentModel.Description("Currency Code not supported, USD only")]

        CurrencyCode_GBP = 2110,
        [System.ComponentModel.Description("Currency Code not supported, USD only")]

        CurrencyCode_INR = 2115,
        [System.ComponentModel.Description("Currency Code not supported, USD only")]

        CurrencyCode_AUD = 2120,
        [System.ComponentModel.Description("Currency Code not supported, USD only")]

        CurrencyCode_CAD = 2125,
        [System.ComponentModel.Description("Currency Code not supported, USD only")]

        CurrencyCode_SGD = 2130,
        [System.ComponentModel.Description("Currency Code not supported, USD only")]

        CurrencyCode_CHF = 2135,
        [System.ComponentModel.Description("Currency Code not supported, USD only")]

        CurrencyCode_MYR = 2140,
        [System.ComponentModel.Description("Currency Code not supported, USD only")]

        CurrencyCode_JPY = 2145,
        [System.ComponentModel.Description("Currency Code not supported, USD only")]

        CurrencyCode_CNY = 2150,
        [System.ComponentModel.Description("Email")]

        EmailStatus_Sent = 3001,
        [System.ComponentModel.Description("Email")]

        EmailStatus_Canceled = 3002,
        [System.ComponentModel.Description("Email")]

        EmailStatus_Error = 3003,
        [System.ComponentModel.Description("")]

        WorkflowType_EMVAuth = 3051,
        [System.ComponentModel.Description("")]

        WorkflowType_EMVAuthConfirm = 3052,
        [System.ComponentModel.Description("")]

        WorkflowType_NonEMVPay = 3053,
        [System.ComponentModel.Description("")]

        WorkflowType_NonEMVPayResult = 3054,
        [System.ComponentModel.Description("")]

        WorkflowType_Package = 3055,
        [System.ComponentModel.Description("White Listed")]

        POR_White = 3100,
        [System.ComponentModel.Description("Green Listed")]

        POR_Green = 3105,
        [System.ComponentModel.Description("Red Listed")]

        POR_Red = 3110,
        [System.ComponentModel.Description("Black Listed")]

        POR_Black = 3115,
        [System.ComponentModel.Description("Unregistered")]

        POR_Unregistered = 3120,
        [System.ComponentModel.Description("Inactive")]

        POR_Inactive = 3125,
        [System.ComponentModel.Description("Inactive")]

        POR_EndOfLife = 3130,
        [System.ComponentModel.Description("Invalid")]

        POR_Invalid = 3135,
        [System.ComponentModel.Description("Application startup error detected.  Please contact TrustCommerce support.")]

        DAL_StartUp_Error = 3500,
        [System.ComponentModel.Description("An instance of TC IPA AppManager is already running. TC IPA AppManager will now exit. Please start the TC IPA AppManager.")]

        DisplayMessage_InitStartUpAnotherInstance = 3501,
        [System.ComponentModel.Description("Initializing - DO NOT RUN TRANSACTIONS")]

        DAL_StartUp_Initializing = 3502,
        [System.ComponentModel.Description("Initialization Complete - Ready to run transactions")]

        DAL_StartUp_InitializeComplete = 3503,
        [System.ComponentModel.Description("No approved Payment Entry Device detected.  Please connect a device and start the TC IPADAL application again.")]

        DAL_Exit_Error_NoDevice = 3504,
        [System.ComponentModel.Description("Card device not supported. Please connect a supported card device and try again.")]

        DAL_Exit_Error_InvalidDevice = 3505,
        [System.ComponentModel.Description("Multiple devices detected. Only one supported \"card\" device can be connected to the workstation. Please unplug one of the \"card\" devices and try again.")]

        DisplayMessage_MultipleDevice = 3506,
        [System.ComponentModel.Description("User credentials / client key incorrect or no internet access. Please contact TrustCommerce Support. ")]

        DAL_Exit_Error_CredentialError = 3507,
        [System.ComponentModel.Description("Please close the Payment windows before closing the application.")]

        DAL_Exit_Error_ClosePOSWindow = 3508,
        [System.ComponentModel.Description("Payment Accepted.")]

        DAL_Process_PaymentAccept = 3509,
        [System.ComponentModel.Description("Payment Declined.")]

        DAL_Process_PaymentDecline = 3510,
        [System.ComponentModel.Description("Payment has errored.")]

        DisplayResults_ProcessPaymentError = 3511,
        [System.ComponentModel.Description("Device is disconnected.")]

        DAL_Process_DeviceDisconnect = 3512,
        [System.ComponentModel.Description("Please swipe card.")]

        DisplayMessage_ProcessSwipeCard = 3513,
        [System.ComponentModel.Description("Invalid card read - please try again.")]

        DisplayMessage_DALProcessInvalidCardRead = 3514,
        [System.ComponentModel.Description("Processing Payment.")]

        DisplayMessage_DALProcessProcessingPayment = 3515,
        [System.ComponentModel.Description("Payment Approved.")]

        DisplayResults_DALProcessApproved = 3516,
        [System.ComponentModel.Description("Please sign on device.")]

        DisplayMessage_DALProcessSignature = 3517,
        [System.ComponentModel.Description("Declined")]

        DAL_Process_Declined = 3518,
        [System.ComponentModel.Description("Transaction Error.")]

        DAL_Process_TransactionError = 3519,
        [System.ComponentModel.Description("Transaction canceled by user")]

        DAL_Process_CanceledByUser = 3520,
        [System.ComponentModel.Description("MSR, pin pad or signature reached a timeout")]

        DAL_Process_SignatureTimeout = 3521,
        //[Description("Welcome")]
        DAL_Process_Welcome = 3522,
        [System.ComponentModel.Description("TC IPA")]

        DisplayMessage_DALWindowTitle = 3523,
        [System.ComponentModel.Description("TC IPADAL")]

        DAL_Window_Title2 = 3524,
        [System.ComponentModel.Description("A new device has been detected. The TCIPADAL application has closed. Please restart TCIPADAL to initialize the device.")]

        DisplayMessage_DALProcessDeviceSwap = 3525,
        [System.ComponentModel.Description("TCIPALink login unsuccessful, cannot get security key.  Please contact your TrustCommerce representative for assistance.")]

        DisplayMessage_IPALinkErrorCredentialError = 3526,
        [System.ComponentModel.Description("TCIPALink StartUp")]

        IPALink_Startup_Caption = 3527,
        [System.ComponentModel.Description("Initialization unsuccessful, communication route config mismatch.  Please contact your TrustCommerce representative for assistance.")]

        DisplayMessage_InitCommRouteError = 3528,
        [System.ComponentModel.Description("TrustCommerce TC IPALink Login Error: Cannot perform payment.  Please contact TrustCommerce Support. ")]

        IPALink_Error_Payment = 3529,
        [System.ComponentModel.Description("")]

        DAL_Window_Idle = 3530,
        [System.ComponentModel.Description("Connected")]

        DAL_Systray_Connected = 3531,
        [System.ComponentModel.Description("NotConnected")]

        DAL_Systray_Interrupted = 3532,
        [System.ComponentModel.Description("IDTECH Device will be set in USB KB Mode and TCIPADAL will close.")]

        DisplayMessage_DALKBModeMessage = 3533,
        [System.ComponentModel.Description("IDTECH Device USB KB mode update was unsuccessful.  Please contact your TrustCommerce representative for assistance.")]

        DisplayMessage_DALKBModeConfirmMessage = 3534,
        [System.ComponentModel.Description("Enter card information into device")]

        DAL_Process_ManualEntry = 3535,
        [System.ComponentModel.Description("Invalid POS URL please check the config or contact your local administrator.")]

        POS_InvalidURL = 3536,
        [System.ComponentModel.Description("Login unsuccessful: Invalid Client key.  Please contact your TrustCommerce representative for assistance.")]

        IPALink_InvalidClientKey = 3538,
        [System.ComponentModel.Description("Login unsuccessful: Invalid URI [url.tcipa.com].  Please contact your TrustCommerce representative for assistance.")]

        IPALink_InvalidURL = 3539,
        [System.ComponentModel.Description("Login unsuccessful. Unexpected error occurred [error here].  Please contact your TrustCommerce representative for assistance.")]

        IPALink_LoginError = 3540,
        [System.ComponentModel.Description("Login unsuccessful: Invalid Client key.  Please contact your TrustCommerce representative for assistance.")]

        DAL_InvalidClientKey = 3541,
        [System.ComponentModel.Description("Login unsuccessful: Invalid URI [url.tcipa.com].  Please contact your TrustCommerce representative for assistance.")]

        DAL_InvalidURL = 3542,
        [System.ComponentModel.Description("Page has timed out due to inactivity. Please select OK to close the page and select a POS tray option to continue.  If the error persists, please contact your Trust Commerce representative.")]

        DisplayMessage_POSAuthenticationError = 3544,
        [System.ComponentModel.Description("Unexpected error has occurred: Please start TC IPADAL. If the error persists, please contact your TrustCommerce representative.")]

        DAL_NetworkDisconnect = 3545,
        [System.ComponentModel.Description("Network is back online.")]

        DAL_NetworkOnline = 3546,
        [System.ComponentModel.Description("Login error, Invalid POSClientSystemName [name] Please contact your TrustCommerce representative for assistance.")]

        POS_InvalidClientSysName = 3547,
        [System.ComponentModel.Description("Login unsuccessful: Invalid config.  Please contact your TrustCommerce representative for assistance.")]

        DAL_InvalidConfig = 3548,
        [System.ComponentModel.Description("Unexpected error has occurred: Please start TC IPADAL. If the error persists, please contact your TrustCommerce representative.")]

        DAL_Communication_AppClosed = 3549,
        [System.ComponentModel.Description("Login unsuccessful: Invalid config. Please contact your TrustCommerce representative for assistance.")]

        IPALink_InvalidConfig = 3550,
        [System.ComponentModel.Description("ACH account information form time out.")]

        DAL_ACHTimeout = 3551,
        [System.ComponentModel.Description("TC IPA service has timed out, please try again.")]

        DAL_NoServiceResponse = 3552,
        [System.ComponentModel.Description("Please swipe or enter card information.")]

        DAL_ProcessIDTech_SwipeCard = 3553,
        [System.ComponentModel.Description("Company/TCCustID combination not supported. Please restart TC IPADAL and contact your TrustCommerce representative for assistance.")]

        Dal_NoCustID = 3554,
        [System.ComponentModel.Description("ConnectionID could not be established. Please restart TC IPADAL and try again.")]

        DAL_NoSigR = 3555,
        [System.ComponentModel.Description("Device configuration warning: To resolve this issue please replace the device. Contact your local administrator for assistance.")]

        DAL_DeviceConfig = 3556,
        [System.ComponentModel.Description("Device firmware updating. Please do not disturb the device while update is in progress. Device will reboot when update has completed.")]

        DAL_SYS_FirmwareUpdate = 3557,
        [System.ComponentModel.Description("Device firmware folder not found. Please contact your TrustCommerce representative for assistance.")]

        DAL_SYS_FirmwareFolder = 3558,
        [System.ComponentModel.Description("Device firmware file not found. Please contact your TrustCommerce representative for assistance.")]

        DAL_SYS_FirmwareFile = 3559,
        [System.ComponentModel.Description("Device firmware has been updated. Please restart TC IPADAL.")]

        DAL_FirmwareUpdated = 3560,
        [System.ComponentModel.Description("Device update error: Firmware update was unsuccessful. Please try again. If the error persists please contact your TrustCommerce representative for assistance.")]

        DAL_FirmwareUpdateFail = 3561,
        [System.ComponentModel.Description("Device update error: Forms update was unsuccessful. Please try again. If the error persists please contact your TrustCommerce representative for assistance.")]

        DAL_FormsUpdateFail = 3562,
        [System.ComponentModel.Description("Device forms updating. Please do not disturb the device while update is in progress. Device will reboot when update has completed.")]

        DAL_SYS_FormUpdate = 3563,
        [System.ComponentModel.Description("Device forms folder not found: Ingenico[model]. Please contact your TrustCommerce representative for assistance.")]

        DAL_SYS_FormsFolder = 3564,
        [System.ComponentModel.Description("Device forms file not found: Ingenico[model]. Please contact your TrustCommerce representative for assistance.")]

        DAL_SYS_FormsFile = 3565,
        [System.ComponentModel.Description("Device forms has been updated. Please restart TC IPADAL.")]

        DAL_FormsUpdated = 3566,
        [System.ComponentModel.Description("Initialization Error:URL/ClientKey Config error or instance of TC IPADAL not available. Please contact your TrustCommerce representative for assistance.")]

        IPALink_InitializationError = 3567,
        [System.ComponentModel.Description("Transaction canceled by user")]

        DAL_ACHCancelPayment = 3568,
        [System.ComponentModel.Description("Could not read card.")]
        DisplayMessage_BadCardSwipe = 3569,

        [System.ComponentModel.Description("Startup error: Communication with services cannot be established.  Application will close.  Please Contact you TrustCommerce representative for assistance. ")]
        AppMgr_NoService = 3570,

        [System.ComponentModel.Description("TC IPA AppManager error, TCIPADAL configuration file cannot be read. Please contact your TrustCommerce representative for assistance.")]
        DisplayMessage_ConfigCanNotRead = 3572,

        [System.ComponentModel.Description("Request cannot be processed.  Requested payment type is invalid. Please contact your TrustCommerce representative.")]
        IPAErrorField_PaymentTypeInvalid = 3575,

        [System.ComponentModel.Description("Couldn't restart the device in time.")]
        DAL_SYS_DeviceRestartFail = 3576,

        [System.ComponentModel.Description("Start Device Update RegisterDevice encounter an error.")]
        DAL_SYS_DeviceRegisterFail = 3577,
        [System.ComponentModel.Description("DAL received Initialization request.")]
        DAL_Acknowledgement_Initialize = 3578,
        [System.ComponentModel.Description("DAL received Process Payment request.")]
        DAL_Acknowledgement_Payment = 3579,
        [System.ComponentModel.Description("Contract request is missing one or more of the required tags.")]
        ContactValidationFailed = 3582,
        [System.ComponentModel.Description("TC IPA could not search for updates, invalid or incorrect [url.tcipa.com] or service unavailable. TCIPA AppManger will launch, please wait. Please contact your TrustCommerce representative if the update error persists.")]
        UpdateServerError = 3583

    }
}
