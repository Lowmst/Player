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
        if (info.lossless == 0)
        {
            _provider = new BufferedWaveProvider(WaveFormat.CreateIeeeFloatWaveFormat(info.sample_rate, 2));
        }
        else
        {
            _provider = new BufferedWaveProvider(new WaveFormat(info.sample_rate, info.bits_per_sample, 2));
            
        }
        _wasapi.Init(_provider);
    }

    public void Play(Decoder decoder)
    {
        _wasapi.Play();

        while (true)
        {
            var pcm = decoder.Decode();
            if (pcm.size == 0)
            {
                break;
            }

            while (pcm.size + _provider.BufferedBytes > _provider.BufferLength)
            {
                Thread.Sleep(1000);
            }
            var bytes = new byte[pcm.size];
            Marshal.Copy(pcm.data, bytes, 0, pcm.size);
            _provider.AddSamples(bytes, 0, pcm.size);
        }

        while (_provider.BufferedBytes != 0)
        {
            Thread.Sleep(1000);
        }
    }
}