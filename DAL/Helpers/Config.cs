using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.IO;

using IPA.Core.Data.Entity;
using IPA.Core.Data.Entity.Other;
using IPA.Core.Data.Entity.Mapping;
using IPA.Core.Shared.Enums;
using IPA.Core.Shared.Helpers.Extensions;
using IPA.Core.XO.Config;
using PaymentSystemType = IPA.Core.Shared.Enums.PaymentSystemType;
using IPA.Core.Shared.Helpers;

namespace IPA.Core.Client.DataAccess.Helper
{
    public static class Config
    {
        #region -- public methods --

        public static void Init(Core.Shared.Enums.AppType appType, App app = null, bool update = false)
        {
            if ((appType == Core.Shared.Enums.AppType.Pedal) || (appType == Core.Shared.Enums.AppType.DAP))
            {

                General.CommunicationRoute = ConfigFileUtil.GetConfigValue("IPA.DAL.CommunicationRoute") == "File" ? CommunicationRoute.File : CommunicationRoute.Service;

                General.CommunicationRoute = ConfigFileUtil.GetConfigValue("IPA.DAL.CommunicationRoute") == "File" ? CommunicationRoute.File : CommunicationRoute.Service;
                Queue.InputFolder = ConfigFileUtil.GetConfigValue("IPA.DAL.Application.Folders.Queue");
                Queue.OutputFolder = ConfigFileUtil.GetConfigValue("IPA.DAL.Application.Folders.IPALink");
                Queue.ConfigFolder = ConfigFileUtil.GetConfigValue("IPA.DAL.Application.Folders.Config");
                Queue.AppManagerFolder = ConfigFileUtil.GetConfigValue("IPA.DAL.Application.Folders.Queue");
                try
                {                    
                    Data.RootCertificate = ConfigFileUtil.GetConfigValue("Services.RootCertificate");
                    if (string.IsNullOrWhiteSpace(Data.RootCertificate))
                        Data.RootCertificate = System.IO.File.ReadAllText(@".\Cert\tcipa.pki");
                }
                catch
                {
                    // TODO: Currently fails quietly. Decide to log later?
                }
                Data.ServicesBaseUrl = update ? ConfigFileUtil.GetConfigValue("IPA.Update.BaseURL") : ConfigFileUtil.GetConfigValue("IPA.Services.BaseURL");

                Data.Timeout = int.Parse(ConfigFileUtil.GetConfigValue("IPA.Services.Timeout"));
            }
            else
            {
                General.CommunicationRoute = ConfigFileUtil.GetConfigValue("CommunicationRoute") == "Service" ? CommunicationRoute.Service : CommunicationRoute.File;
                Queue.InputFolder = ConfigFileUtil.GetConfigValue("QueueFolder");
                Queue.OutputFolder = ConfigFileUtil.GetConfigValue("WriteFolder");
            }
            Data.StatusCodeFolderPath = $"{System.IO.Directory.GetCurrentDirectory()}\\{ConfigFileUtil.GetConfigValue("StatusCodeFile")}";
            string loggingOptionConfig = string.Empty;
            //Get general settings

            General.KeyType = ConfigFileUtil.GetConfigValue("IPA.Company.KeyType");
            General.ClientKey = ConfigFileUtil.GetConfigValue("IPA.Company.KeyValue");
            Data.POSBaseUrl = ConfigFileUtil.GetConfigValue("IPA.POS.BaseURL");
            loggingOptionConfig = ConfigFileUtil.GetConfigValue("LoggingOption");

            General.IPALinkAppKey = ConfigFileUtil.GetConfigValue("IPALink.AppKey");
            General.AcceptedPorts = ConfigFileUtil.GetConfigValue("IPA.DAL.Device.AcceptedComPorts")?.Split(',').ToArray();
            General.PaymentSystemType = PaymentSystemType.Epic; //TODO: purpose? 
            General.AppType = appType;
            General.DevicesFolder = ConfigFileUtil.GetConfigValue("IPA.DAL.Application.Folders.Devices");
            if (General.TransactionTimeout == 0)
                General.TransactionTimeout = 120000;
            if (General.RequestInterval == 0)
                General.RequestInterval = 5000;

            var loggingOn = companyConfigs?.FirstOrDefault(c => c.ConfigType?.ConfigTypeID == (int)ConfigTypeEnum.LoggingVerbose)?.ConfigValue;
            if (String.IsNullOrWhiteSpace(loggingOn))
                loggingOn = ConfigFileUtil.GetConfigValue("IPA.TurnLoggingOn");
            bool.TryParse(loggingOn, out General.LoggingOn);
                
            General.LoggingOption = loggingOptionConfig == LoggingOption.File.ToString() ? LoggingOption.File : LoggingOption.EventLog;
            
        }

        /// <summary>
        /// DAL initialization does not have the Company hydrated by the time Config.Init s called.
        /// Fr this reason and due to the fat that this file is shared between applications, I added this method to
        /// keep the Init method unchanged, but still hydrate configuration options needed when they are neeeded.
        /// </summary>
        /// <param name="config"></param>
        public static void InitLoggingOption()
        {
            var loggingOn = companyConfigs?.FirstOrDefault(c => c.ConfigType?.ConfigTypeID == (int)ConfigTypeEnum.LoggingVerbose)?.ConfigValue;
            if (String.IsNullOrWhiteSpace(loggingOn))
                loggingOn = ConfigFileUtil.GetConfigValue("IPA.TurnLoggingOn");
            bool.TryParse(loggingOn, out General.LoggingOn);

            // Dal Log Z Order of Window
            bool.TryParse(companyConfigs?.FirstOrDefault(c => c.ConfigType?.ConfigTypeID == (int)ConfigTypeEnum.DALLogZOrder)?.ConfigValue, out General.DALLogZOrder);
        }

        //[Obsolete] - required by web admin
        public static async Task<ConfigXO> GetConfigValues(int appID, int companyID)
        {
            //Create Variables
            var request = new XO.Config.ConfigXO()
            {
                Request = new XO.Config.Request()
                {
                    CompanyID = companyID,
                    AppID = appID,
                    ConfigRequests = new List<ConfigRequest>()
                       {
                           new ConfigRequest()
                           { CompanyID = companyID,
                             AppID = appID
                           },
                       }
                }
            };

            //Call the Service and return the response
            return await Services.IPAServices.CallServices<ConfigXO, ConfigXO>(request, "config/getconfig");
        }
    
        public static async Task<ConfigXO> SaveConfig(Core.Data.Entity.Config input)
        {
            var request = new XO.Config.ConfigXO()
            {
                Request = new XO.Config.Request()
                {
                    Config = input
                }
            };
            return await Services.IPAServices.CallServices<ConfigXO, ConfigXO>(request, "config/saveconfig");
        }

        public static ConfigXO GetWebCommConfigurations(App app)
        {
            var respConfig = new ConfigXO();
           
            if (app?.CompanyID != null)
            {
                respConfig = Task.Run(() => Client.DataAccess.Helper.Config.GetConfigValues((int)app?.AppID, (int)app?.CompanyID)).Result;
            }

            return respConfig;
        }

        public static List<IPA.Core.Data.Entity.Config> GetFileIOConfigurations()
        {
            var configFileName = @"C:\TrustCommerce\Configs\config.json";  // or different file name in Citrix environment?
            bool fileFound = false;
            int maxWait = 30;    // wait for max 30 seconds in case DAL has not finished generating the file
            int waitSeconds = 0;

            while (!fileFound && waitSeconds < maxWait)
            {
                if (File.Exists(configFileName))
                {
                    companyConfigs = ObjectHelper.FromJson<List<Core.Data.Entity.Config>>(File.ReadAllText(configFileName));
                    fileFound = true;
                }
                else
                    Thread.Sleep(2000);
                waitSeconds += 2;
            }

            return companyConfigs;
        }

        public static T GetConfigValue<T>(ConfigTypeEnum configTypeEnum, List<IPA.Core.Data.Entity.Config> configList, int tenderTypeID = 0)
        {
            var item = configList?.Where(c => c.ConfigTypeID == (int)configTypeEnum && c.TenderTypeID == tenderTypeID).FirstOrDefault();
            //If we do not have a value for Tender Type ID passed-in, look for one without the tender type, to get the default value.
            if (item == null)
                item = configList?.Where(c => c.ConfigTypeID == (int)configTypeEnum).FirstOrDefault();

            return StringHelper.GetTypedConfigValue<T>(item?.ConfigValue ?? "");
        }

        #endregion

        #region -- properties

        public struct General
        {
            public static string ClientKey;
            public static string KeyType;
            public static string IPALinkAppKey;
            public static int AppID;
            public static Core.Shared.Enums.AppType AppType;
            public static PaymentSystemType PaymentSystemType;
            public static DataExchangeFormat DataExchangeFormat;
            public static bool PaymentUpdateSupport;
            public static CommunicationRoute CommunicationRoute;
            public static POSClientSystem Client;
            public static int TransactionTimeout;
            public static int RequestInterval;
            public static string LoggingPath;
            public static bool LoggingOn;
            public static LoggingOption LoggingOption;
            public static string[] AcceptedPorts;
            public static int DisplayTransactionTime;
            public static int DisplayFailuresTime;
            public static string DevicesFolder;
            public static bool EnableForms;
            public static bool EnableFirmware;
            public static int CompanyID;
            public static bool StatusLoggingOn;
            public static int FileDownloadBufferSize;
            public static string AdminUserLogin;
            public static bool ShowSysTrayDeployStatus;
            public static IngenicoLoggingLevel IngenicoLoggingLevel;
            public static int LoggingNumDays;
            public static bool IDTechDisable;
            public static int IPALinkInitTime;
            public static int AppTopMostInterval;
            public static bool AppTopMost;

            public static bool DALLogZOrder;

            public static bool DAPExitOnSessionSwitch;
            public static int DAPMaxLogFileAge;
            public static int DAPMaxLogFileStorageKB;

            public static int DAPConnectedInterval;
            public static int DAPServiceStatusReplayInterval;
            public static int DAPConfigReloadInterval;
            public static int DAPServiceTestInterval;
            public static int DAPServiceTestMaxLatency;
            public static int DAPServiceTestRequestSize;
            public static int DAPServiceTestMaxLoss;

            public static int DAPSignalRConnectInterval;
            public static int DAPSignalRTestInterval;
            public static int DAPSignalRTestPayloadSize;
            public static int DAPSignalRTestMaxLatency;
            public static int DAPSignalRTestMaxLoss;
        }

        //Data service config
        public struct Data
        {
            public static string ServicesBaseUrl;
            public static string RootCertificate;
            public static string POSBaseUrl;
            public static int Timeout;
            public static string StatusCodeFolderPath;
            public static string ApplicationPath;
        }

        //Queue Management settings
        public struct Queue
        {
            public static string InputFolder;
            public static string OutputFolder;
            public static string ConfigFolder;
            public static string AppManagerFolder;
        }

        //public static List<Core.Data.Entity.Config> GetConfigs
        //{
        //    get { return configXO?.Response?.Configs?[0]; }
        //}

        #endregion

        #region -- member variables --

        //private static ConfigXO configXO = null;
        public static List<Core.Data.Entity.Config> companyConfigs = null;
 
        #endregion
    }
}
