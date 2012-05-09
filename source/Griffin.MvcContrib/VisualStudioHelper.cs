using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Griffin.MvcContrib
{
    /// <summary>
    /// Detects if the code is running within visual studio
    /// </summary>
    public static class VisualStudioHelper
    {
        /// <summary>
        /// Gets if visual studio is running (or any other debugger)
        /// </summary>
        public static bool IsInVisualStudio
        {
            get
            {
                if (Debugger.IsAttached)
                    return true;

                using (var process = Process.GetCurrentProcess())
                {
                    return process.ProcessName.ToLowerInvariant().Contains("devenv");
                }
            }
        }
    }
}
