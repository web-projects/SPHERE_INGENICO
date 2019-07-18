using IPA.Core.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using IPA.Core.Data.Entity.Mapping;
//using IPA.Core.XO;
//using IPA.Core.XO.Bearer;
//using IPA.Core.XO.StatusCode;
using System.IO;
//using IPA.Core.Shared.Helpers.Extensions;
//using IPA.Core.Client.DataAccess.Shared;
using IPA.Core.Shared.Enums;
//using IPA.Core.XO.Payment;
using IPA.Core.Client.Dal.Models;
using System.Configuration;
//using IPA.Core.XO.Message;
//using IPA.Core.XO.PaymentSignature;
using IPA.Core.Data.Entity.Other;
//using IPA.Core.XO.Payment;
//using IPA.Core.XO.TCCustAttribute;

namespace IPA.DAL.RBADAL.Services
{
    public static class Transaction
    {

        //public static PaymentXO PaymentXO;
        ////internal static MessageXO MessageXO;
        ////internal static Signature Signature;
        ////internal static PaymentSignatureXO PaymentSignatureXO;
        ////internal static List<TCCustAttributeItem> TCCustAttributeItems;

        internal static System.Timers.Timer ACHTimer { get; set; }
        internal static System.Timers.Timer AutoCloseSelectorTimer { get; set; }
        internal static System.Timers.Timer ManualTimer { get; set; }
        internal static System.Timers.Timer MSRTimer { get; set; }
        internal static System.Timers.Timer ServiceCallerTimer { get; set; }
        internal static System.Timers.Timer SignatureTimer { get; set; }
        internal static System.Timers.Timer TransactionTimer { get; set; }

        internal static bool IsTest { get; set; }
        internal static bool IsManual { get; set; }


        public static event System.EventHandler<TimerEventArgs> TimeExpired;

        public static void Init()
        {
            //PaymentXO = new PaymentXO();

            //PaymentXO.Request = new Core.XO.Payment.Request();
            //PaymentXO.Response = new Core.XO.Payment.Response();

            //PaymentXO.Request.Payment = new Payment();
            //PaymentXO.Request.PaymentRequest = new PaymentRequest();
            //PaymentXO.Request.PaymentTender = new PaymentTender();
            //PaymentXO.Request.CreditCard = new CreditCard();

            //make sure ReadAttempts are initialized
            //PaymentXO.Request.ReadAttempts = 0;
            //PaymentXO.Request.DeviceEMVCapable = false;

            //PaymentXO.Response.Payment = new Payment();
            //PaymentXO.Response.PaymentRequest = new PaymentRequest();
            //PaymentXO.Response.PaymentResponse = new PaymentResponse();
            //PaymentXO.Response.PaymentTender = new PaymentTender();

            ////MessageXO = new MessageXO();

            ////Signature = new Signature();

            ////TCCustAttributeItems = new List<TCCustAttributeItem>();

            IsTest = false;
            IsManual = false;

            ////PaymentSignatureXO = new PaymentSignatureXO();
            ////PaymentSignatureXO.Request = new Core.XO.PaymentSignature.Request();
            ////PaymentSignatureXO.Response = new Core.XO.PaymentSignature.Response();

            //initial these and set the interval to the configuration value associated with the appropriate configuration
            InitMSRTimer();
            InitSignatureTimer();
            InitTransactionTimer();
            InitAutoCloseTimer();
            InitServiceCallerTimer();
            InitACHTimer();
            InitManualTimer();

        }

        private static void InitTransactionTimer()
        {
            int transactionTimerInterval;
            transactionTimerInterval = 90000;
            //int defaultValue = 0;

            //int ServicePolling;
            //string ServicePollingInterval = Data.CompanyConfigs.FirstOrDefault(n => n.ConfigType.ConfigTypeID == (int)TimerType.ServicePolling)?.ConfigValue;
            //if (string.IsNullOrEmpty(ServicePollingInterval))
            //    ServicePolling = int.Parse(ConfigurationManager.AppSettings["IPA.Application.Timer.Default.ServicePolling"]);
            //else
            //    ServicePolling = int.Parse(ServicePollingInterval);

            //int ServiceLatency;
            //string ServiceMaxLatency = Data.CompanyConfigs.FirstOrDefault(n => n.ConfigType.ConfigTypeID == (int)TimerType.ServiceMaxLatency)?.ConfigValue;
            //if (string.IsNullOrEmpty(ServiceMaxLatency))
            //    ServiceLatency = int.Parse(ConfigurationManager.AppSettings["IPA.DAL.Application.Timer.Default.ServiceMaxLatency"]);
            //else
            //    ServiceLatency = int.Parse(ServiceMaxLatency);

            //int IPALinkTimer;
            //string IPALinkPollingInterval = Data.CompanyConfigs.FirstOrDefault(n => n.ConfigType.ConfigTypeID == (int)TimerType.IPALinkPollingTimer)?.ConfigValue;
            //if (string.IsNullOrEmpty(IPALinkPollingInterval))
            //    IPALinkTimer = int.Parse(ConfigurationManager.AppSettings["IPA.Application.Timer.Default.IPALinkPolling"]);
            //else
            //    IPALinkTimer = int.Parse(IPALinkPollingInterval);

            //transactionTimerInterval = IPALinkTimer - (ServiceLatency + ServicePolling);

            TransactionTimer = new System.Timers.Timer(transactionTimerInterval);
            TransactionTimer.AutoReset = false;
            TransactionTimer.Elapsed += (sender, e) => RaiseTimerExpired(new TimerEventArgs { Timer = TimerType.Transaction });
        }

        private static void InitSignatureTimer()
        {
            int signatureTimerInterval;
            signatureTimerInterval = 5000;

            ////look for the config value - if not available set the default
            //if (Data.CompanyConfigs.Any(n => n.ConfigType.ConfigTypeID == (int)TimerType.Signature))
            //{
            //    signatureTimerInterval = int.Parse(Data.CompanyConfigs.FirstOrDefault(n => n.ConfigType.ConfigTypeID == (int)TimerType.Signature).ConfigValue);
            //}
            //else
            //{
            //    signatureTimerInterval = int.Parse(ConfigurationManager.AppSettings["IPA.DAL.Application.Timer.Default.Signature"]);
            //}

            SignatureTimer = new System.Timers.Timer(signatureTimerInterval);
            SignatureTimer.AutoReset = false;
            SignatureTimer.Elapsed += (sender, e) => RaiseTimerExpired(new TimerEventArgs { Timer = TimerType.Signature });
        }

        private static void InitMSRTimer()
        {
            int msrTimerInterval;
            msrTimerInterval = 90000;
            //////look for the config value - if not available set the default
            ////if (Data.CompanyConfigs.Any(n => n.ConfigType.ConfigTypeID == (int)TimerType.MSR))
            ////{
            ////    msrTimerInterval = int.Parse(Data.CompanyConfigs.FirstOrDefault(n => n.ConfigType.ConfigTypeID == (int)TimerType.MSR).ConfigValue);
            ////}
            ////else
            ////{
            ////    msrTimerInterval = int.Parse(ConfigurationManager.AppSettings["IPA.DAL.Application.Timer.Default.MSR"]);
            ////}
            MSRTimer = new System.Timers.Timer(msrTimerInterval);
            MSRTimer.AutoReset = false;
            MSRTimer.Elapsed += (sender, e) => RaiseTimerExpired(new TimerEventArgs { Timer = TimerType.MSR });
        }

        private static void InitAutoCloseTimer()
        {
            int autoCloseTimerInterval;

            autoCloseTimerInterval = 5000;
            ////look for the config value - if not available set the default
            //if (Data.CompanyConfigs.Any(n => n.ConfigType.ConfigTypeID == (int)TimerType.AutoClose))
            //{
            //    autoCloseTimerInterval = int.Parse(Data.CompanyConfigs.FirstOrDefault(n => n.ConfigType.ConfigTypeID == (int)TimerType.AutoClose).ConfigValue);
            //}
            //else
            //{
            //    autoCloseTimerInterval = int.Parse(ConfigurationManager.AppSettings["IPA.DAL.Application.Timer.Default.AutoClose"]);
            //}
            AutoCloseSelectorTimer = new System.Timers.Timer(autoCloseTimerInterval);
            AutoCloseSelectorTimer.AutoReset = false;
            AutoCloseSelectorTimer.Elapsed += (sender, e) => RaiseTimerExpired(new TimerEventArgs { Timer = TimerType.AutoClose });
        }

        private static void InitServiceCallerTimer()
        {
            int serviceCallerTimerInterval;
            serviceCallerTimerInterval = 10000;
            ServiceCallerTimer = new System.Timers.Timer(serviceCallerTimerInterval);
            ServiceCallerTimer.AutoReset = false;
            ServiceCallerTimer.Elapsed += (sender, e) => RaiseTimerExpired(new TimerEventArgs { Timer = TimerType.ServiceCaller });
        }

        private static void InitACHTimer()
        {
            int ACHTimerInterval;
            ACHTimerInterval = 10000;
            //if (Data.CompanyConfigs.Any(n => n.ConfigType.ConfigTypeID == (int)TimerType.ACH))
            //{
            //    ACHTimerInterval = int.Parse(Data.CompanyConfigs.FirstOrDefault(n => n.ConfigType.ConfigTypeID == (int)TimerType.ACH).ConfigValue);
            //}
            //else
            //{
            //    ACHTimerInterval = int.Parse(ConfigurationManager.AppSettings["IPA.DAL.Application.Timer.Default.ACH"]);
            //}
            ACHTimer = new System.Timers.Timer(ACHTimerInterval);
            ACHTimer.AutoReset = false;
            ACHTimer.Elapsed += (sender, e) => RaiseTimerExpired(new TimerEventArgs { Timer = TimerType.ACH });
        }

        private static void InitManualTimer()
        {
            int manualTimerInterval;
            manualTimerInterval = 10000;
            //if (Data.CompanyConfigs.Any(n => n.ConfigType.ConfigTypeID == (int)TimerType.Manual))
            //{
            //    manualTimerInterval = int.Parse(Data.CompanyConfigs.FirstOrDefault(n => n.ConfigType.ConfigTypeID == (int)TimerType.Manual).ConfigValue);
            //}
            //else
            //{
            //    manualTimerInterval = int.Parse(ConfigurationManager.AppSettings["IPA.DAL.Application.Timer.Default.Manual"]);
            //}
            ManualTimer = new System.Timers.Timer(manualTimerInterval);
            ManualTimer.AutoReset = false;
            ManualTimer.Elapsed += (sender, e) => RaiseTimerExpired(new TimerEventArgs { Timer = TimerType.Manual });
        }

        public static void StartTransactionTimer()
        {
            TransactionTimer.Start();
        }

        public static void StopTransactionTimer()
        {
            if (TransactionTimer != null)
                TransactionTimer?.Stop();
        }

        public static void StartMSRTimer()
        {
            MSRTimer.Start();
        }

        public static void StopMSRTimer()
        {
            MSRTimer?.Stop();
        }

        public static void StartSignatureTimer()
        {
            SignatureTimer.Start();
        }

        public static void StopSignatureTimer()
        {
            SignatureTimer?.Stop();
        }

        public static void StartAutoCloseTimer()
        {
            AutoCloseSelectorTimer.Start();
        }
        public static void StopAutoCloseTimer()
        {
            AutoCloseSelectorTimer?.Stop();
        }
        public static void StartServiceCallerTimer()
        {
            ServiceCallerTimer.Start();
        }
        public static void StopServiceCallerTimer()
        {
            ServiceCallerTimer?.Stop();
        }
        public static void StartACHTimer()
        {
            ACHTimer.Start();
        }

        public static void StopACHTimer()
        {
            ACHTimer?.Stop();
        }

        public static void StartManualTimer()
        {
            ManualTimer.Start();
        }

        public static void StopManualTimer()
        {
            ManualTimer?.Stop();
        }

        public static void RaiseTimerExpired(TimerEventArgs e)
        {
            TimeExpired?.Invoke(null, e);
        }
        public static void StopAllTimers()
        {
            if (ACHTimer != null)
                ACHTimer.Stop();
            if (AutoCloseSelectorTimer != null)
                AutoCloseSelectorTimer.Stop();
            if (ManualTimer != null)
                ManualTimer.Stop();
            if (MSRTimer != null)
                MSRTimer.Stop();
            if (ServiceCallerTimer != null)
                ServiceCallerTimer.Stop();
            if (SignatureTimer != null)
                SignatureTimer.Stop();
            if (TransactionTimer != null)
                TransactionTimer.Stop();
        }
    }
}
