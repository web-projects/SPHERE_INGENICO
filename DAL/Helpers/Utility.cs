using IPA.DAL.RBADAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPA.LoggerManager;

namespace IPA.DAL.Helpers
{
    public class Utility
    {
        readonly char[] commaSeparator = new char[] { ',' };
        readonly string RBA_UPDATE_UTIL = "ibmeftdl";

        string javaCmd;
        string rba_install_cmd;

        StringBuilder stdOutput;
        StringBuilder stdError;

        int TCFinalPort;
        int portTimeout = -1;
        int win7PortTimeout = -1;

        List<string> originalPorts = null;

        /********************************************************************************************************/
        // HELPERS
        /********************************************************************************************************/
        #region -- helpers --

        private static bool IsWindows7()
        {
            return (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1);
        }

        void OutputDataHandler(object sendingProcess, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(e.Data) && !e.Data.StartsWith("Downloading "))
            {
                Debug.WriteLine($"PO: {e.Data}");
                stdOutput.AppendLine(e.Data);
                if (e.Data.Contains("Skipping"))
                { 
                    Debug.WriteLine(e.Data);
                }
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

        string Format(string filename, string arguments)
        {
            return $"'{filename}{(((string.IsNullOrEmpty(arguments)) ? string.Empty : " " + arguments))}'";
        }

        private static void RecordDifferences(string prefix, List<Process> one, List<Process> two, ref bool drvinstChanged)
        {
            var result = one.Where(p => !two.Any(p2 => p2.Id == p.Id));
            if (result.Count() > 0)
            {
                var sb = new System.Text.StringBuilder();
                sb.Append(prefix);
                foreach (Process proc in result)
                {
                    if (proc.ProcessName == "drvinst")
                        drvinstChanged = true;
                    sb.Append(proc.ProcessName);
                    sb.Append(", ");
                }
                Debug.WriteLine(sb.ToString());
            }
        }

        internal List<Process> ReportProcessChange(List<Process> origProcesses, ref bool added, ref bool removed)
        {
            var curProcessList = GetCurrentProcesses();
            if (!added)
            { 
                RecordDifferences("Added: ", curProcessList, origProcesses, ref added);
            }
            if (!removed)
            { 
                RecordDifferences("Removed: ", origProcesses, curProcessList, ref removed);
            }
            return curProcessList;
        }

        internal static void ConfirmUpdateProcessRunning(ref bool added, ref bool removed)
        {
            added = true;
            removed = Process.GetProcessesByName("drvinst").Length == 0;
        }

        void StoreOriginalPorts()
        {
            if (originalPorts == null)
            {
                try
                {
                    originalPorts = new List<string>(GetAvailablePorts());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message, 4);
                }
            }
        }

        string[] GetAvailablePorts()
        {
            return System.IO.Ports.SerialPort.GetPortNames();
        }

        bool DoesComPortExist(int port)
        {
            if (port <= 0)
            {
                Debug.WriteLine("Waiting 2 minutes for ports to be ready");
                System.Threading.Thread.Sleep(120000);
                return true;
            }
            bool deviceAtPortFound = false;
            try
            {
                string comPort = $"COM{port}";
                var ports = GetAvailablePorts();
                deviceAtPortFound = ports.Contains(comPort, StringComparer.InvariantCultureIgnoreCase);
            }
            catch (Exception xcp)
            {
                Debug.WriteLine(xcp.Message);
            }
            return deviceAtPortFound;
        }

        List<string> FindNewPorts(string prevExistingPorts = "")
        {
            StoreOriginalPorts();

            var origPorts = originalPorts;
            string[] curPorts;
            curPorts = GetAvailablePorts();
            if (!string.IsNullOrWhiteSpace(prevExistingPorts))
            {
                origPorts = new List<string>(prevExistingPorts.Split(commaSeparator, StringSplitOptions.RemoveEmptyEntries));
            }
            List<string> addedPorts = new List<string>();
            foreach (string port in curPorts)
            {
                if (!origPorts.Contains(port))
                {
                    addedPorts.Add(port);
                }
            }
            return addedPorts;
        }

        Process GetProcess(string processName)
        {
            System.Diagnostics.Process[] nameProcess = System.Diagnostics.Process.GetProcesses();
            var currentSessionID = Process.GetCurrentProcess().SessionId;
            Process[] sameAsthisSession = (from c in nameProcess where c.SessionId == currentSessionID select c).ToArray();

            foreach (var temp in sameAsthisSession)
            {
                string name = temp.ProcessName;
                if (name.ToLower() == (processName) || name.ToLower() == processName)
                {
                    return temp;
                }
            }
            return null;
        }

        void KillProcess(string [] processNames)
        {
            foreach (string processName in processNames)
            {
                Process process = null;
                int counter = 0;
                do
                {
                    try
                    {
                        process = GetProcess(processName);
                        if (process != null)
                        { 
                           process.Kill();
                        }
                    }
                    catch
                    {
                    }
                    System.Threading.Thread.Sleep(1000);
                } while (process != null && counter++ < 300);
            }
        }

        internal static List<Process> GetCurrentProcesses()
        {
            var curProcesses = Process.GetProcesses();
            var curProcessList = new List<Process>(curProcesses);
            return curProcessList;
        }

        void SetRBACmd(string inCmd)
        {
            rba_install_cmd = inCmd;
        }

        void CreateDownloadBat(string fileName, int portNum, string eftlNum = "0007", string eftpNum = "0007")  //TODO: refactor to.bat file and string replace  port#
        {
            var sb = new StringBuilder(3500);  //about 3200 hundred characters in the download.bat file
            sb.AppendLine("@echo off");
            sb.AppendLine("rem Download EFTL and EFTP files.");

            sb.AppendLine($"set EFTLVER={eftlNum}");
            sb.AppendLine("set EFTLHEADER=");
            sb.AppendLine($"set EFTPVER={eftpNum}");
            sb.AppendLine("set EFTPHEADER=");
            sb.AppendLine($"set COM={portNum}");
            sb.AppendLine("set BAUD=115200");
            sb.AppendLine("set PARITY=N");
            sb.AppendLine("set DATA=8");
            sb.AppendLine("set BITS=1");
            sb.AppendLine("set PRELOADTMO=0");
            sb.AppendLine("goto CONTINUE1");

            sb.AppendLine(":Err1");
            sb.AppendLine("@echo ÿ");
            sb.AppendLine("@echo  !!!!!!  --- please specify EFTL/EFTP revision level and baudrate ------ !!!!!!");
            sb.AppendLine("@echo  e.g. 'EFTload 0260 0260'");
            sb.AppendLine("@echo ÿ");
            sb.AppendLine("goto end");

            sb.AppendLine(":CONTINUE1");
            sb.AppendLine("if NOT exist EFTL%EFTLVER% echo Error - missing EFTL%EFTLVER% file");
            sb.AppendLine("if NOT exist EFTL%EFTLVER% goto end");
            sb.AppendLine("if NOT exist EFTP%EFTPVER% echo Error - missing EFTP%EFTPVER% file");
            sb.AppendLine("if NOT exist EFTP%EFTPVER% goto end");


            sb.AppendLine(":start");
            sb.AppendLine("rem --------------------------------------------------------------------------");
            sb.AppendLine("rem");
            sb.AppendLine("rem");
            sb.AppendLine("rem");
            sb.AppendLine("rem");
            sb.AppendLine("rem");
            sb.AppendLine("rem --------------------------------------------------------------------------");

            sb.AppendLine("@echo.");

            sb.AppendLine("rem ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ - ");
            sb.AppendLine("rem /c<port number> COM port number(1-20), /b<baudrate> Baud rate(default = 4800), /t<parity> Parity type('N', 'E', or 'O')(default = 'N')");
            sb.AppendLine("rem /d<data bits> Number of data bits(default=8), s<stop bits> Number of stop bits(default=2)");
            sb.AppendLine("rem /i<irq number> IRQ number(default: see the COM port settings), /a<port address> COM port address(default: see the COM port settings) MUST be specified as a hexadecimal value(i.e. 3F8).");
            sb.AppendLine("rem /RTO<seconds> Receive timeout in seconds.The amount of time the program will wait for a block request during program download(default = 3 sec)");
            sb.AppendLine("rem /TTO<seconds> Transmit timeout in seconds.The amount of time the program will wait for an ACK or NAK before retrying the packet(default = 10 sec)");
            sb.AppendLine("rem /PRTO<seconds> Last Parameter response timeout in seconds.The amount of time the program will wait for the online response message at the end of a parameter");
            sb.AppendLine("rem ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ - ");
            sb.AppendLine("@echo Eft download of EFTL%EFTLVER% and EFTP%EFTPVER% @ COM%COM% %BAUD% %PARITY% %DATA% %BITS%");
            sb.AppendLine("ibmeftdl /l%EFTLVER%,EFTL%EFTLVER% /p%EFTPVER%,EFTP%EFTPVER% /c%COM% /b%BAUD% /d%DATA% /t%PARITY% /s%BITS% /ER");
            sb.AppendLine("if errorlevel 2 goto cont3");
            sb.AppendLine("if errorlevel 1 goto cont2");

            sb.AppendLine(":cont1");
            sb.AppendLine("@echo Complete");
            sb.AppendLine("goto theEnd");

            sb.AppendLine(":cont2");
            sb.AppendLine("@echo No Need!");
            sb.AppendLine("goto theEnd");

            sb.AppendLine(":cont3");
            sb.AppendLine("@echo error!");
            sb.AppendLine("@echo.");
            sb.AppendLine("@echo Error - loading component upgrades.");
            sb.AppendLine("@echo.");
            sb.AppendLine("pause");
            sb.AppendLine("goto theEnd");

            sb.AppendLine(":theEnd");

            sb.AppendLine(":end");
            sb.AppendLine("set EFTLVER=");
            sb.AppendLine("set EFTLHEADER=");
            sb.AppendLine("set EFTPVER=");
            sb.AppendLine("set EFTPHEADER=");
            sb.AppendLine("set BAUD=");
            sb.AppendLine("set PARITY=");
            sb.AppendLine("set DATA=");
            sb.AppendLine("set BITS=");
            sb.AppendLine("set PRELOADTMO=");

            File.WriteAllText(fileName, sb.ToString());
        }

        #endregion

        /********************************************************************************************************/
        // GENERAL UTILITIES
        /********************************************************************************************************/
        #region -- general utilities --

        public void SetJavaCmd(string inCmd)
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

        public string GetJavaCmd()
        {
            return javaCmd;
        }

        public string RunExternalExe(string directory, string filename, bool abortOnError, string arguments = null, string[] environmentVariables = null)
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
                        if(!abortOnError || stdOutput.ToString().IndexOf("ERROR") == -1)
                        {
                            continue;
                        }
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
                        if(!abortOnError || stdOutput.ToString().IndexOf("ERROR") == -1)
                        {
                            continue;
                        }
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

        public string RunExternalExeElevated(string directory, string filename, string arguments = null, string[] environmentVariables = null)
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

        public void WaitForIngenicoDevice(List<Process> processes, int portExpected, string model)
        {
            Debug.Write("Waiting for device setup to complete.");
            if (portTimeout < 0)
            { 
                int.TryParse(ConfigurationManager.AppSettings["PortSetupTimeout"] ?? "300", out portTimeout);
            }
            if (win7PortTimeout < 0)
            { 
                int.TryParse(ConfigurationManager.AppSettings["Win7PortSetupTimeout"] ?? "600", out win7PortTimeout);
            }
            if (IsWindows7() && win7PortTimeout < 301)
            {
                Debug.Write(" [Port Setup can sometimes exceed 5 minutes.  If problems occur, consider extending the PortSetupTImout.]");
            }
            int timeout = IsWindows7() ? win7PortTimeout : portTimeout;
            var now = DateTime.Now;
            bool added = false;
            bool removed = false;
            while ((DateTime.Now - now).TotalSeconds < timeout)
            {
                processes = ReportProcessChange(processes, ref added, ref removed);
                if (added && removed)
                {
                    Console.WriteLine();
                    if (portExpected <= 0)
                    {
                        //Impossible to connect to negative or 0 port, so break and move forward.
                        Debug.WriteLine($"Not waiting for COM0.");
                        break;
                    }                    
                   Debug.WriteLine($"Confirming setup successful for COM{portExpected}.");
                    System.Threading.Thread.Sleep(10000); //Normally this goes very quickly, but let's wait extra just to be sure.
                    int i = 0;
                    while ((DateTime.Now - now).TotalSeconds < timeout)
                    {
                        i++;
                        if (DoesComPortExist(portExpected))
                        {
                            Debug.WriteLine(".");
                            Debug.WriteLine("Setup successful.  Port ready.");
                            System.Threading.Thread.Sleep(1000);
                        }
                        else
                        {
                            Debug.Write(i == 1 ? "Attempting to confirm setup successful." : ".");
                            System.Threading.Thread.Sleep(5000);
                            if (i == 6)
                            {
                                //We waited 30 seconds, reboot the device, hopefully it will appear at the correct port.
                                if (string.IsNullOrWhiteSpace(model))
                                {
                                    Debug.WriteLine("Device Type unknown.  Unable to programatically Restart Device.  Please restart the device manually.");
                                }
                                timeout += 60;
                            }
                            continue;
                        }
                        break;
                    }
                    break;
                }
                else if (added)
                {
                    System.Threading.Thread.Sleep(4000);
                    Debug.Write(".");
                }
                else if (!added && (DateTime.Now - now).TotalMilliseconds > 15000)  //15 seconds, and we haven't added, maybe we missed it.
                {
                    ConfirmUpdateProcessRunning(ref added, ref removed);
                }
                else
                {
                    System.Threading.Thread.Sleep(3000);
                    Debug.Write(".");
                }
            }
        }

        #endregion

        /********************************************************************************************************/
        // UPDATE
        /********************************************************************************************************/
        #region -- update interface --

        public string UpdateUIAFirmware(DeviceInformation deviceInformation)
        {
            string path = System.IO.Directory.GetCurrentDirectory();
            //arguments: true displays the java window, NULL says don't specify a file, take the default
            string arguments = $"-jar \"{path}\\UIAUtilities\\fileUploader.jar\" 7 true NULL {deviceInformation.ModelName.Trim()}";
            Logger.debug($"device: UIA update args={arguments}");
            string result = RunExternalExe($"{path}\\UIAUtilities", javaCmd, false, arguments);

            if(!string.IsNullOrWhiteSpace(result))
            {
                int index = 0;
                Debug.WriteLine($"device::UpdateUIAFirmware(): result={result}");
                string failure = IPA.DAL.Helpers.StatusCode.GetDisplayMessage(SearchStatus.StatusIndex.UIA_INGENICO_FIRMWARE_FAILED);
                if((index = result.IndexOf("File Upload failed.")) >= 0)
                {
                    result = result.Substring(failure.Length + index).Trim();
                }
                else
                {
                    result = string.Empty;
                }
            }

            return result;
        }

        public string UpdateRBAFirmware(DeviceInformation deviceInformation, string version)
        {
            string result = string.Empty;
            try
            { 
                SetRBACmd(ConfigurationManager.AppSettings["download_bat"] ?? "download.bat");
                string path = System.IO.Directory.GetCurrentDirectory();
                string firmwareDir = Path.Combine(path, $"RBAUtilities\\firmware");
                string destinationDirectory = Path.Combine(path, $"{firmwareDir}\\{version}\\{deviceInformation.ModelName.Trim()}\\{deviceInformation.ModelVersion.Trim()}");
                string batchToRun = Path.Combine(destinationDirectory, $"{rba_install_cmd}");
                TCFinalPort = Convert.ToInt32(deviceInformation.Port.Trim().TrimStart(new char [] { 'C', 'O', 'M' }));
                CreateDownloadBat(batchToRun, TCFinalPort);
                if(File.Exists(batchToRun))
                {
                    KillProcess(new string [] { RBA_UPDATE_UTIL + ".exe", RBA_UPDATE_UTIL });

                    // Copy ibmeftdl to executing directory
                    string firmwareSrc = Path.Combine(firmwareDir, RBA_UPDATE_UTIL + ".exe");

                    if(File.Exists(firmwareSrc))
                    {
                        string firmwareDst = Path.Combine(destinationDirectory, RBA_UPDATE_UTIL + ".exe");
                        File.Copy(firmwareSrc, firmwareDst, true);
                        var curProcesses = GetCurrentProcesses();
                        result = RunExternalExe(destinationDirectory, batchToRun, true);
                        if(result.ToString().IndexOf("ERROR") == -1)
                        {
                            WaitForIngenicoDevice(curProcesses, TCFinalPort, string.Empty);
                            result = string.Empty;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return result;
        }

        #endregion
    }
}
