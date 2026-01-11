using System.Runtime.InteropServices;
using OmniCommon;
using OmniCommon.Messages;

public class MinimalOmniReader
{
    private OmniMotionDataHandler _handler;
    private nint _callbackPtr;

    [UnmanagedCallersOnly(EntryPoint = "OmniReader_Create")]
    public static nint Create()
    {
        var reader = new MinimalOmniReader();
        var handle = GCHandle.Alloc(reader);
        return GCHandle.ToIntPtr(handle);
    }

    [UnmanagedCallersOnly(EntryPoint = "OmniReader_Initialize")]
    public static bool Initialize(nint handle, nint comPortPtr, int omniMode, int baudRate)
    {
        var reader = GetReader(handle);
        string comPort = Marshal.PtrToStringUTF8(comPortPtr) ?? "COM3";
        
        reader._handler = new OmniMotionDataHandler(comPort, baudRate, 100, (OmniMode)omniMode);

        var selection = new MotionDataSelection
        {
            RingAngle = true,
            GamePadData = true,
            Timestamp = false,
            StepCount = false,
            RingDelta = false,
            GunButtonData = false,
            StepTrigger = false
        };

        if (!reader._handler.Connect(selection, (OmniMode)omniMode))
        {
            return false;
        }

        reader._handler.MotionDataReceived += (sender, data) =>
        {
            if (data.EnableRingAngle && data.EnableGamePadData && reader._callbackPtr != 0)
            {
                var callback = Marshal.GetDelegateForFunctionPointer<OmniDataCallback>(reader._callbackPtr);
                callback(data.RingAngle, data.GamePad_X, data.GamePad_Y);
            }
        };
        
        return true;
    }

    [UnmanagedCallersOnly(EntryPoint = "OmniReader_RegisterCallback")]
    public static void RegisterCallback(nint handle, nint callbackPtr)
    {
        var reader = GetReader(handle);
        reader._callbackPtr = callbackPtr;
    }

    [UnmanagedCallersOnly(EntryPoint = "OmniReader_Disconnect")]
    public static void Disconnect(nint handle)
    {
        var reader = GetReader(handle);
        reader._handler?.Disconnect();
        reader._handler?.Dispose();
    }

    [UnmanagedCallersOnly(EntryPoint = "OmniReader_Destroy")]
    public static void Destroy(nint handle)
    {
        var gcHandle = GCHandle.FromIntPtr(handle);
        if (gcHandle.IsAllocated)
        {
            var reader = (MinimalOmniReader)gcHandle.Target;
            reader._handler?.Dispose();
            gcHandle.Free();
        }
    }

    private static MinimalOmniReader GetReader(nint handle)
    {
        var gcHandle = GCHandle.FromIntPtr(handle);
        return (MinimalOmniReader)gcHandle.Target;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void OmniDataCallback(float ringAngle, int gamePadX, int gamePadY);
}