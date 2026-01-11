// Decompiled with JetBrains decompiler
// Type: OmniCommon.PodUSBComms
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;
using System.Runtime.InteropServices;
using System.Threading;

#nullable disable
namespace OmniCommon;

public class PodUSBComms
{
  public const uint GENERIC_READ = 2147483648 /*0x80000000*/;
  public const uint GENERIC_WRITE = 1073741824 /*0x40000000*/;
  public const uint OPEN_EXISTING = 3;
  public const uint FILE_FLAG_OVERLAPPED = 1073741824 /*0x40000000*/;
  public const uint FILE_ATTRIBUTE_NORMAL = 128 /*0x80*/;
  public const int CP210x_SUCCESS = 0;
  public const int CP210x_GPIO_0 = 1;
  private IntPtr handle = IntPtr.Zero;

  [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
  public static extern IntPtr CreateFile(
    [MarshalAs(UnmanagedType.LPTStr)] string filename,
    uint access,
    uint share,
    IntPtr securityAttributes,
    uint creationDisposition,
    uint flagsAndAttributes,
    IntPtr templateFile);

  [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
  public static extern bool CloseHandle(IntPtr handle);

  [DllImport("CP210xRuntime.dll", EntryPoint = "CP210xRT_WriteLatch", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
  private static extern int WriteLatch(IntPtr Handle, uint Mask, uint Latch);

  [DllImport("CP210xRuntime.dll", EntryPoint = "CP210xRT_ReadLatch", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
  private static extern int ReadLatch(IntPtr Handle, out uint Latch);

  public void Open(string ComPort)
  {
    this.handle = PodUSBComms.CreateFile("\\\\.\\" + ComPort, 3221225472U /*0xC0000000*/, 0U, IntPtr.Zero, 3U, 1073741952U /*0x40000080*/, IntPtr.Zero);
    if (this.handle == new IntPtr(-1))
      throw new Exception("Failed to open handle");
  }

  public void ResetPod()
  {
    if (this.handle == IntPtr.Zero)
      throw new Exception("ComPort is not open");
    uint Mask = 8;
    uint Latch1 = 8;
    uint Latch2 = 0;
    if (PodUSBComms.WriteLatch(this.handle, Mask, Latch2) != 0)
      throw new Exception("Failed to set pods");
    Thread.Sleep(50);
    if (PodUSBComms.WriteLatch(this.handle, Mask, Latch1) != 0)
      throw new Exception("Failed to set pods");
  }

  public bool IsBootloaderMode()
  {
    if (this.handle == IntPtr.Zero)
      throw new Exception("ComPort is not open");
    uint Latch = 0;
    if (PodUSBComms.ReadLatch(this.handle, out Latch) != 0)
      throw new Exception("Failed to set pods");
    return ((int) Latch & 1) == 0;
  }

  public void Close()
  {
    if (this.handle == IntPtr.Zero)
      throw new Exception("Call CreateFile before this method");
    this.handle = PodUSBComms.CloseHandle(this.handle) ? IntPtr.Zero : throw new Exception("Failed to close ComPort");
  }
}
