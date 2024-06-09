using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Components;

public class ScaleFromMicrophone : NetworkBehaviour
{
    [SerializeField] AudioLoudnessDetector detector;
    [SerializeField] NetworkedAnimatorBoolController AnimatorController;
    Rigidbody2D rb;
    public bool isStart = false;

    public NetworkVariable<float> TotalScreamScore = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> CounterActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> loudnessValue = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> IsPanic = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public float LoudnessSensitivity = 100f;
    public float threshold = 0.1f;
    private float gravity = 0.05f;
    
    

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (!isStart) return;

        float loudness = detector.GetLoudnessFromMicrophone() * LoudnessSensitivity * threshold;
        loudnessValue.Value = loudness;  // Update the network variable

        if (CounterActive.Value)
        {
           
            AnimatorController.SetBoolParameterServerRpc("IsPanicing", true);
            //CalcScreamTot(loudness);
            //Debug.Log($"Player {NetworkManager.Singleton.LocalClientId} Scream Score: {TotalScreamScore.Value}");
        }
        else
        {
            
            AnimatorController.SetBoolParameterServerRpc("IsPanicing", false);
        }

        Vector2 playerVelocity = new Vector2(rb.velocity.x, rb.velocity.y + loudness - gravity);
        rb.velocity = playerVelocity;
        
    }

    private void CalcScreamTot(float loudness)
    {
        TotalScreamScore.Value += loudness;
        UpdateScreamScoreServerRpc(TotalScreamScore.Value);
    }

    [ServerRpc]
    private void UpdateScreamScoreServerRpc(float newScore)
    {
        TotalScreamScore.Value = newScore;
        Debug.Log($"Server updated TotalScreamScore to: {TotalScreamScore.Value}");
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

    public void ActivateCounter()
    {
        if (!IsOwner) return;
        ActivateCounterServerRpc();
        
    }

    public void DeactivateCounter()
    {
        if (!IsOwner) return;
        DeactivateCounterServerRpc();
        

    }

    [ServerRpc]
    private void ActivateCounterServerRpc(ServerRpcParams rpcParams = default)
    {
        CounterActive.Value = true;
        
    }

    [ServerRpc]
    private void DeactivateCounterServerRpc(ServerRpcParams rpcParams = default)
    {
        CounterActive.Value = false;
    }
}


