using System.Runtime.InteropServices;
using System.Text;

namespace PlayerCLI;

public struct WAVHead
{
    public byte[] ID = Encoding.ASCII.GetBytes(['R', 'I', 'F', 'F']); // "RIFF"

    public uint Size = 36;

    public byte[] FourCC = Encoding.ASCII.GetBytes(['W', 'A', 'V', 'E']); // "WAVE"

    // SubChunk "fmt"
    public byte[] fmtID = Encoding.ASCII.GetBytes(['f', 'm', 't', ' ']); // "fmt "
    public uint fmtSize = 16; // 16
                              // "fmt" Data
    public ushort encodeMode = 3; // 1 for PCM, ...
    public ushort numChannel = 2; // 1 for Mono, 2 for Stereo
    public uint samplingRate; // usually 44100 or higher for lossless
    public uint byteRate; // numChannel * samplingRate * bitDepth / 8
    public ushort blockAlign; // numChannel * bitDepth / 8
    public ushort bitDepth; // usually 16 for lossless

    // SubChunk "data"
    public byte[] dataID = Encoding.ASCII.GetBytes(['d', 'a', 't', 'a']); // "data"
    public uint dataSize;
    // "data" Data is the kind chosen in "encodeMode"

    public WAVHead() { }
}

public class WAVWriter
{
    private BinaryWriter file;
    private WAVHead head;

    private int size = 0;

    public WAVWriter(string filename, uint sample_rate, ushort bits_per_sample)
    {
        this.file = new BinaryWriter(File.Create(filename));
        this.head = new WAVHead();

        head.samplingRate = sample_rate;
        head.bitDepth = bits_per_sample;
        head.byteRate = 2 * sample_rate * bits_per_sample / 8;
        head.blockAlign = (ushort)(2 * bits_per_sample / 8);

        file.Seek(44, SeekOrigin.Begin);
    }



    public void WritePCM(PCMPacket pcm)
    {
        this.size += pcm.size;
        var bytes = new byte[pcm.size];

        Marshal.Copy(pcm.data, bytes, 0, pcm.size);

        file.Write(bytes);
    }

    public void WriteHead()
    {
        this.file.Seek(0, SeekOrigin.Begin);
        head.dataSize = (uint)this.size;
        head.Size += (uint)this.size;

        file.Write(head.ID);
        file.Write(head.Size);
        file.Write(head.FourCC);
        file.Write(head.fmtID);
        file.Write(head.fmtSize);
        file.Write(head.encodeMode);
        file.Write(head.numChannel);
        file.Write(head.samplingRate);
        file.Write(head.byteRate);
        file.Write(head.blockAlign);
        file.Write(head.bitDepth);
        file.Write(head.dataID);
        file.Write(head.dataSize);

        file.Dispose();
    }
}