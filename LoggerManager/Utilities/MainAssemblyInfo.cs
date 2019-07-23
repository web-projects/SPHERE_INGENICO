using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace IPA.LoggerManager.Helpers
{
    public class Win32
    {
      [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
      public static extern IntPtr GetModuleHandle(String moduleName);
      [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
      public static extern UInt32 GetModuleFileName(IntPtr hModule, StringBuilder filename, UInt32 size);
    }

    public class MainAssemblyInfo
    {
        string assemblyName;

        public MainAssemblyInfo()
        {
            IntPtr hMod = Win32.GetModuleHandle(null);
            Debug.Assert(hMod != null);
            StringBuilder nameBuilder = new StringBuilder(255);
            Win32.GetModuleFileName(hMod, nameBuilder, (uint)nameBuilder.Capacity);
            assemblyName = nameBuilder.ToString();
        }

        public string Description { get { return assemblyName; } }
    }
}
