// Decompiled with JetBrains decompiler
// Type: OmniCommon.TivaUpdater
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

#nullable disable
namespace OmniCommon;

public class TivaUpdater
{
  public const string FailedInitializing = "Failed to initialize";
  public const string OmniNotStartInRuntimeMode = "Omni did not start in runtime mode";
  public const string FailedSwitchToDFUMode = "Failed to switch to DFU mode";
  public const string FailedSwitchToDFUModeCheck = "Check Failed. Failed to switch to DFU mode";
  public const string FailedToProgram = "Failed to upgrade Tiva";
  public const string OK = "OK";
  public const string FailedToResetOmni = "Failed to reset the omni. Please manually reset the omni.";
  public const string FailedToReadFile = "Failed to read data from file.";
  private IntPtr lmdfuHandle = IntPtr.Zero;

  [SuppressUnmanagedCodeSecurity]
  [DllImport("lmdfu.dll", CallingConvention = CallingConvention.StdCall)]
  private static extern int LMDFUInit();

  [SuppressUnmanagedCodeSecurity]
  [DllImport("lmdfu.dll", CallingConvention = CallingConvention.StdCall)]
  private static extern int LMDFUDeviceOpen(
    int iDeviceIndex,
    ref tLMDFUDeviceInfo psDevInfo,
    ref IntPtr phHandle);

  [SuppressUnmanagedCodeSecurity]
  [DllImport("lmdfu.dll", CallingConvention = CallingConvention.StdCall)]
  private static extern int LMDFUDeviceClose(IntPtr phHandle, bool bReset);

  [SuppressUnmanagedCodeSecurity]
  [DllImport("lmdfu.dll", CallingConvention = CallingConvention.StdCall)]
  public static extern tLMDFUErr LMDFUDownloadBin(
    IntPtr hHandle,
    [MarshalAs(UnmanagedType.LPArray)] byte[] pcBinaryImage,
    uint ulImageLen,
    uint ulStartAddr,
    bool bVerify,
    ref IntPtr hwndNotify);

  [SuppressUnmanagedCodeSecurity]
  [DllImport("lmdfu.dll", CallingConvention = CallingConvention.StdCall)]
  private static extern int LMDFUModeSwitch(IntPtr phHandle);

  private bool Init() => TivaUpdater.LMDFUInit() == 0;

  private bool OmniConnectedAndInRuntime() => this.FindOmni();

  private bool OmniConnectedAndInDFU() => this.FindOmni(PID: 170);

  private bool FindOmni(int VID = 10731, int PID = 255 /*0xFF*/)
  {
    int iDeviceIndex = 0;
    bool flag = false;
    tLMDFUDeviceInfo psDevInfo = new tLMDFUDeviceInfo();
    int num = 0;
    try
    {
      do
      {
        num = TivaUpdater.LMDFUDeviceOpen(iDeviceIndex, ref psDevInfo, ref this.lmdfuHandle);
        if ((int) psDevInfo.usVID == VID && (int) psDevInfo.usPID == PID)
          flag = true;
        else
          num = TivaUpdater.LMDFUDeviceClose(this.lmdfuHandle, false);
        ++iDeviceIndex;
      }
      while (num == 0 && !flag);
    }
    catch (Exception ex)
    {
      Console.WriteLine("Exception: " + ex.Message);
      flag = false;
    }
    return flag && num == 0;
  }

  private bool OmniSwitchToDFU() => TivaUpdater.LMDFUModeSwitch(this.lmdfuHandle) == 0;

  private bool UpgradeOmni(byte[] data)
  {
    tLMDFUErr tLmdfuErr = tLMDFUErr.DFU_OK;
    try
    {
      IntPtr hwndNotify = new IntPtr();
      tLmdfuErr = TivaUpdater.LMDFUDownloadBin(this.lmdfuHandle, data, (uint) data.Length, 0U, false, ref hwndNotify);
    }
    catch (Exception ex)
    {
      Console.WriteLine("Exception: " + ex.Message);
    }
    return tLmdfuErr == tLMDFUErr.DFU_OK;
  }

  private bool ResetOmni() => TivaUpdater.LMDFUDeviceClose(this.lmdfuHandle, true) == 0;

  public string Update(string fileName)
  {
    try
    {
      if (!this.Init())
        return "Failed to initialize";
      Thread.Sleep(10);
      if (!this.OmniConnectedAndInRuntime())
        return "Omni did not start in runtime mode";
      byte[] data;
      try
      {
        data = File.ReadAllBytes(fileName);
      }
      catch (Exception ex)
      {
        return $"{"Failed to read data from file."}{ex.Message}";
      }
      if (!this.OmniSwitchToDFU())
        return "Failed to switch to DFU mode";
      bool flag = false;
      for (int index = 0; index < 10 & !flag; ++index)
      {
        Thread.Sleep(1000);
        if (this.OmniConnectedAndInDFU())
          flag = true;
      }
      if (!flag)
        return "Check Failed. Failed to switch to DFU mode";
      if (!this.UpgradeOmni(data))
        return "Failed to upgrade Tiva";
      Console.WriteLine("Upgraded, resetting now!");
      return this.ResetOmni() ? "OK" : "Failed to reset the omni. Please manually reset the omni.";
    }
    catch (Exception ex)
    {
      return ex.Message;
    }
  }

  public string Update(byte[] firmware)
  {
    try
    {
      if (!this.Init())
        return "Failed to initialize";
      Thread.Sleep(10);
      if (!this.OmniConnectedAndInRuntime())
        return "Omni did not start in runtime mode";
      if (!this.OmniSwitchToDFU())
        return "Failed to switch to DFU mode";
      bool flag = false;
      for (int index = 0; index < 10 & !flag; ++index)
      {
        Thread.Sleep(1000);
        if (this.OmniConnectedAndInDFU())
          flag = true;
      }
      if (!flag)
        return "Check Failed. Failed to switch to DFU mode";
      if (!this.UpgradeOmni(firmware))
        return "Failed to upgrade Tiva";
      Console.WriteLine("Upgraded, resetting now!");
      return this.ResetOmni() ? "OK" : "Failed to reset the omni. Please manually reset the omni.";
    }
    catch (Exception ex)
    {
      return ex.Message;
    }
  }
}
