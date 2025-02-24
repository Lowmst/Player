using System.Runtime.InteropServices;

namespace PlayerCLI;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct PCMParameters
{
    public int sample_rate;
    public int bits_per_sample;
    public int lossless;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct PCMPacket
{
    public int size;
    public IntPtr data;
}

public partial class Decoder
{

    private readonly IntPtr _object;
    public Decoder(string url)
    {
        _object = init(url);
    }

    public PCMParameters GetPCMParameters()
    {
        return setup(_object);
    }

    public PCMPacket Decode()
    {
        return decode(_object);
    }
    

    [LibraryImport("Decoder.dll", StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr init(string url);

    [LibraryImport("Decoder.dll")]
    private static partial PCMParameters setup(IntPtr decoder);

    [LibraryImport("Decoder.dll", StringMarshalling = StringMarshalling.Utf8)]
    private static partial PCMPacket decode(IntPtr decoder);

}