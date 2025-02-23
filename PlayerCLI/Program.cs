namespace PlayerCLI;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.Unicode;
        var arguments = Environment.GetCommandLineArgs();

        if (arguments.Length < 2)
        {
            Console.WriteLine($"Usage: {Path.GetFileNameWithoutExtension(arguments[0]) + ".exe"} <input file>");
            return;
        }

        var filepath = args[0];
        var decoder =  new Decoder(filepath);
        var info = decoder.GetPCMParameters();

        Console.WriteLine(Path.GetFileName(filepath));
        Console.WriteLine($"{info.sample_rate} Hz");
        Console.WriteLine($"{info.bits_per_sample} Bit");

        var player = new Playback(info);
        player.Play(decoder);


        //var filename = Path.GetFileNameWithoutExtension(filepath) + ".wav";

        //var wav = new WAVWriter(filename, (uint)info.sample_rate, (ushort)info.bits_per_sample);

        //Console.WriteLine($"采样率: {info.sample_rate}");
        //Console.WriteLine($"采样位深: {info.bits_per_sample}");

        //var pcm = Decoder.Decode(decoder);

        //while (pcm.size != 0)
        //{
        //    wav.WritePCM(pcm);

        //    pcm = Decoder.Decode(decoder);
        //}
        //wav.WriteHead();
    }
}