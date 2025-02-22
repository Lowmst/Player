using System.Runtime.InteropServices;

namespace PlayerCLI;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct PCMParameters
{
    public int sample_rate;
    public int bits_per_sample;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct PCMPacket
{
    public int size;
    public IntPtr data;
}

public partial class Decoder
{
    [LibraryImport("Decoder.dll", StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr Init(string url);

    [LibraryImport("Decoder.dll")]
    public static partial PCMParameters Setup(IntPtr decoder);

    [LibraryImport("Decoder.dll", StringMarshalling = StringMarshalling.Utf8)]
    public static partial PCMPacket Decode(IntPtr decoder);

}