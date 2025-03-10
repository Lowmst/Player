﻿using System;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;


namespace PlayerUI3;

public class Playback
{
    private WasapiOut _wasapi = new WasapiOut(AudioClientShareMode.Exclusive, false, 5);
    private readonly BufferedWaveProvider _provider;
    public bool PlayState = false;
    private readonly Decoder _decoder;


    public Playback(Decoder decoder)
    {
        _decoder = decoder;
        var info = decoder.GetPCMParameters();
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

    public void StartPlayTask()
    {
        Task.Run(() =>
        {
             while (true)
             {
                 var pcm = _decoder.Decode();
                 if (pcm.size == 0) break;
                 var bytes = new byte[pcm.size];
                 Marshal.Copy(pcm.data, bytes, 0, pcm.size);

                 while (pcm.size + _provider.BufferedBytes > _provider.BufferLength || PlayState == false)
                 {
                     Thread.Sleep(1000);
                 }

                 _provider.AddSamples(bytes, 0, pcm.size);
             }

             while (_provider.BufferedBytes != 0)
             {
                 Thread.Sleep(1000);
             }
        });
    }

    public void Play()
    {
        _wasapi.Play();
        PlayState = true;
    }

    public void Pause()
    {
        _wasapi.Pause();
        PlayState = false;
        //_wasapi.Dispose();
        //_wasapi = new WasapiOut(AudioClientShareMode.Exclusive, 100);
        //_wasapi.Init(_provider);
    }

}