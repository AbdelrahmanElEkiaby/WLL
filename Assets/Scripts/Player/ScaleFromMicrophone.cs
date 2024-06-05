using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class ScaleFromMicrophone : NetworkBehaviour
{
    [SerializeField] AudioLoudnessDetector detector;
    [SerializeField] Image soundMeter;
    [SerializeField] TMP_Text ScreamTotalText;
    Rigidbody2D rb;
    public bool isStart = false;


    public NetworkVariable<float> TotalScreamScore = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> MeterActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> loudnessValue = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> CounterActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public float LoudnessSensitivity = 100f;
    public float threshold = 0.1f;
    private float gravity = 0.05f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        soundMeter.fillAmount = 0;  // Initialize the sound meter fill amount to 0
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (!isStart) return;

        float loudness = detector.GetLoudnessFromMicrophone() * LoudnessSensitivity * threshold;
        loudnessValue.Value = loudness;  // Update the network variable
        
        if(MeterActive.Value == true)
        {
            TotalScreamScore.Value += loudness;
            DisplayScreamScoreClientRpc();
        }

        Vector2 playerVelocity = new Vector2(rb.velocity.x, rb.velocity.y + loudness - gravity);
        rb.velocity = playerVelocity;
    }

    private void Update()
    {
        //if (MeterActive.Value)
        //{
        //    soundMeter.fillAmount = loudnessValue.Value;  // Update the UI for all clients when MeterActive is true
        //}
        //else
        //{
        //    soundMeter.fillAmount = 0;  // Ensure the meter is set to 0 when MeterActive is false
        //}
    }

    public void StartMovement()
    {
        isStart = true;
    }

    public void StopMovement()
    {
        isStart = false;
        rb.velocity = Vector2.zero;
        gravity = 0;
    }

    public void ActivateMeter()
    {
        if (IsOwner)
        {
            MeterActive.Value = true;
            ActivateMeterClientRpc();
        }
    }

    public void DeactivateMeter()
    {
        if (IsOwner)
        {
            MeterActive.Value = false;
            DeactivateMeterClientRpc();
        }
    }

    [ClientRpc]
    private void ActivateMeterClientRpc()
    {
        if (!IsOwner)
        {
            MeterActive.Value = true;
        }
    }

    [ClientRpc]
    private void DeactivateMeterClientRpc()
    {
        if (!IsOwner)
        {
            MeterActive.Value = false;
        }
        soundMeter.fillAmount = 0;
    }

    public void ActivateCounter()
    {
        if (!IsOwner) return;
        ActivateCounterClientRpc();
    }

    public void DeactivateCounter()
    {
        if (!IsOwner) return;
        DeactivateCounterClientRpc();
    }

    [ClientRpc]
    private void ActivateCounterClientRpc()
    {
        CounterActive.Value = true;
        ScreamTotalText.gameObject.SetActive(true);
    }

    [ClientRpc]
    private void DeactivateCounterClientRpc()
    {
        CounterActive.Value = false ;
    }

    [ClientRpc]
    private void DisplayScreamScoreClientRpc()
    {
        ScreamTotalText.text = $"{TotalScreamScore.Value}";
    }
}
