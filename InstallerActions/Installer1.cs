using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace InstallerActions
{
    [RunInstaller(true)]
    public partial class Installer1 : Installer
    {
        const int HWND_BROADCAST = 0xffff;
        const uint WM_SETTINGCHANGE = 0x001a;

        public Installer1()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool SendNotifyMessage(IntPtr hWnd, uint Msg,
            UIntPtr wParam, string lParam);
        //C:\Program Files (x86)\Mozilla Firefox;C:\ProgramData\Oracle\Java\javapath;C:\Program Files (x86)\Intel\iCLS Client\;C:\Program Files\Intel\iCLS Client\;C:\WINDOWS\system32;C:\WINDOWS;C:\WINDOWS\System32\Wbem;C:\WINDOWS\System32\WindowsPowerShell\v1.0\;C:\Program Files (x86)\Intel\Intel(R) Management Engine Components\DAL;C:\Program Files\Intel\Intel(R) Management Engine Components\DAL;C:\Program Files (x86)\Intel\Intel(R) Management Engine Components\IPT;C:\Program Files\Intel\Intel(R) Management Engine Components\IPT;C:\Program Files\Microsoft SQL Server\130\Tools\Binn\;C:\Program Files\Git\cmd;C:\Program Files\OpenSSH\bin;C:\Program Files (x86)\PuTTY\;C:\Program Files\MiKTeX 2.9\miktex\bin\x64\;C:\Program Files\dotnet\;C:\Program Files (x86)\GtkSharp\2.12\bin;C:\Users\Florian\AppData\Local\Microsoft\WindowsApps;C:\Users\Florian\AppData\Local\Microsoft\WindowsApps
        [SecurityPermission(SecurityAction.Demand)]
        public override void Install(IDictionary savedState)
        {
            base.Install(savedState);
            string oldPath = Environment.GetEnvironmentVariable("PATH");
            //Process.Start("https://www.google.de/?gws_rd=ssl#q=" + WebUtility.UrlEncode(oldPath));

            string targetDir = Context.Parameters["targetdir"].TrimEnd('\\') + "\\";
            if (!oldPath.Contains(targetDir))
            {
                using (var envKey = Registry.LocalMachine.OpenSubKey(
                    @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment",
                    true))
                {
                    Contract.Assert(envKey != null, @"registry key is missing!");
                    envKey.SetValue("PATH", oldPath + ";" + targetDir);
                    SendNotifyMessage((IntPtr)HWND_BROADCAST, WM_SETTINGCHANGE,
                        (UIntPtr)0, "Environment");
                }
            }
        }
    }
}
