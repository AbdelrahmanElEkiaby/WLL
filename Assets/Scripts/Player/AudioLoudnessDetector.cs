using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AudioLoudnessDetector : NetworkBehaviour
{
    public int SampleWindow = 64;

    private AudioClip _microphoneClip;
    private string _microphoneName;

    private void Start()
    {
        if (!IsOwner) return;
        MicrophoneToAudioClip(0);
    }

    private void MicrophoneToAudioClip(int microphoneIndex)
    {
      
        _microphoneName = Microphone.devices[microphoneIndex];
        _microphoneClip = Microphone.Start(_microphoneName, true, 20, AudioSettings.outputSampleRate);

    }

    public float GetLoudnessFromMicrophone()
    {
        return GetLoudnessFromAudioClip(Microphone.GetPosition(_microphoneName),_microphoneClip);
    }

    public float GetLoudnessFromAudioClip(int clipPoistion, AudioClip clip)
    {
        int startPosition = clipPoistion - SampleWindow;

        if(startPosition < 0)
        {
            return 0;
        }

        float[] waveData = new float[SampleWindow];
        clip.GetData(waveData, startPosition);

        float totalLoudness = 0;

        foreach (var samplefloat in waveData)
        {
            totalLoudness += Mathf.Abs(samplefloat);
        }

        return totalLoudness / SampleWindow;

    }
}
