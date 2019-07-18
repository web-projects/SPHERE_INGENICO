using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using IPA.DAL.RBADAL;
using mscoree;

namespace IPA.MainApp
{
    class AppDomainCfg
    {
        /**************************************************************************/
        // APPDOMAIN ARTIFACTS
        /**************************************************************************/
        #region -- appdomain artifacts --

        public AppDomain CreateAppDomain(string dllName)
        {
            AppDomainSetup setup = new AppDomainSetup()
            {
                ApplicationName = dllName,
                ConfigurationFile = dllName + ".dll.config",
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory
            };

            AppDomain appDomain = AppDomain.CreateDomain(setup.ApplicationName,
                                                        AppDomain.CurrentDomain.Evidence,
                                                        setup);
            
            // Share App.Config file with all assemblies
            string configFile = System.Reflection.Assembly.GetExecutingAssembly().Location + ".config";
            appDomain.SetData("APP_CONFIG_FILE", configFile);

            return appDomain;
        }

        public IDevicePlugIn InstantiatePlugin(AppDomain domain, string ASSEMBLY_NAME, string PLUGIN_NAME)
        {
            Debug.WriteLine("main: assembly fullname is=[{0}].", (object) ASSEMBLY_NAME);

            IDevicePlugIn plugIn = null;

            try
            {
                plugIn = domain.CreateInstanceAndUnwrap(ASSEMBLY_NAME, PLUGIN_NAME) as IDevicePlugIn;
            }
            catch (Exception e)
            {
                Debug.WriteLine("InsantiatePlugin: exception={0}", (object)e.Message);
            }

            return plugIn;
        }

        public void UnloadPlugin(AppDomain appdomain)
        {
            bool unloaded = true;
            bool domainfound = false;

            foreach (AppDomain appDomain in EnumAppDomains())
            {
                if(appDomain == appdomain)
                {
                    domainfound = true;
                    unloaded = false;
                    break;
                }
            }

            if(domainfound)
            { 
                try
                {
                    AppDomain.Unload(appdomain);
                    unloaded = true;
                }
                catch (CannotUnloadAppDomainException)
                {
                    unloaded = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            if (!unloaded)
            {
                Debug.WriteLine("main: appdomain could not be unloaded.");
            }
        }

        public void TestIfUnloaded(IDevicePlugIn plugin)
        {
            bool unloaded = false;

            try
            {
                Debug.WriteLine(plugin.PluginName);
            }
            catch (AppDomainUnloadedException)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            if (!unloaded)
            {
                Debug.WriteLine("It does not appear that the app domain successfully unloaded.");
            }
        }

        public static IEnumerable<AppDomain> EnumAppDomains()
        {
            IntPtr enumHandle = IntPtr.Zero;
            ICorRuntimeHost host = null;

            try
            {
                host = new CorRuntimeHostClass();
                host.EnumDomains(out enumHandle);
                object domain = null;

                host.NextDomain(enumHandle, out domain);
                while (domain != null)
                {
                    yield return (AppDomain)domain;
                    host.NextDomain(enumHandle, out domain);
                }
            }
            finally
            {
                if (host != null)
                {
                    if (enumHandle != IntPtr.Zero)
                    {
                        host.CloseEnum(enumHandle);
                    }

                    Marshal.ReleaseComObject(host);
                }
            }
        }

        #endregion
    }
}
