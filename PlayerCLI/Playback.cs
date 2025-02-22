using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Runtime.InteropServices;

namespace PlayerCLI;

public class Playback
{
    private readonly WasapiOut _wasapi = new WasapiOut(AudioClientShareMode.Exclusive, 100);
    private readonly BufferedWaveProvider _provider;

    public Playback(PCMParameters info)
    {
        _provider = new BufferedWaveProvider(new WaveFormat(info.sample_rate, info.bits_per_sample, 2));
        _wasapi.Init(_provider);
    }

    public void Play(IntPtr decoder)
    {
        _wasapi.Play();

        while (true)
        {
            var pcm = Decoder.Decode(decoder);
            if (pcm.size == 0)
            {
                break;
            }

            if (pcm.size + _provider.BufferedBytes > _provider.BufferLength)
            {
                Thread.Sleep(2500);
            }
            var bytes = new byte[pcm.size];
            Marshal.Copy(pcm.data, bytes, 0, pcm.size);
            _provider.AddSamples(bytes, 0, pcm.size);

        }
        Thread.Sleep(5000);
    }
}