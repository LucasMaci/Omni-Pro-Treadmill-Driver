// Decompiled with JetBrains decompiler
// Type: OmniCommon.tLMDFUDeviceInfo
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System.Runtime.InteropServices;

#nullable disable
namespace OmniCommon;

public struct tLMDFUDeviceInfo
{
  public ushort usVID;
  public ushort usPID;
  public ushort usDevice;
  public ushort usDetachTimeOut;
  public ushort usTransferSize;
  public byte ucDFUAttributes;
  public byte ucManufacturerString;
  public byte ucProductString;
  public byte ucSerialString;
  public byte ucDFUInterfaceString;
  public bool bSupportsStellarisExtensions;
  public bool bDFUMode;
  public uint ulPartNumber;
  public byte cRevisionMajor;
  public byte cRevisionMinor;
  [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
  public char[] pcPartNumber;
}
