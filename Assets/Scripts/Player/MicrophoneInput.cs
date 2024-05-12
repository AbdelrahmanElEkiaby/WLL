using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Unity.Netcode;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneInput : NetworkBehaviour
{
    private string microphone;          // microphone name
    private AudioSource audio;
    private List<string> allMicrophones = new List<string>();
    public AudioMixerGroup _mixerGroupMicrophone;
    public float sensitivity = 100;         // for tweaking the output value
    public float loudness = 0;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
        if(!IsOwner) { return; }
        audio = GetComponent<AudioSource>();

        for (int i = 0; i < Microphone.devices.Length; i++)
        {
            if (microphone == null)
            {
                microphone = Microphone.devices[i];
                audio.outputAudioMixerGroup = _mixerGroupMicrophone;
            }

            allMicrophones.Add(Microphone.devices[i]);
        }

        // Start Microphone
        audio.clip = Microphone.Start(microphone, true, 10, 44100);
        audio.loop = true;      // Set the AudioClip to loop
        audio.mute = false;     // (true): Mute the sound, we don't want the player to hear it


        if (Microphone.IsRecording(microphone))
        {
            while (!(Microphone.GetPosition(microphone) > 0)) { }     // Wait until the recording has started
            Debug.Log("Playing..");
            audio.Play();   // Play the audio source
        }
        else
        {
            Debug.Log("Fail..");
            return;
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }
        loudness = GetAveragedVolume() * sensitivity;
        //Debug.Log(loudness.ToString());
    }

    public float GetAveragedVolume()
    {
        // sample size = 256
        int sampleSize = 256;
        float[] data = new float[sampleSize];
        float a = 0;
        audio.GetOutputData(data, 0);

        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }

        // return average volume (sum / samplesize)
        return a / sampleSize;
    }
}
