using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class Timer : NetworkBehaviour
{
    public NetworkVariable<float> timer = new NetworkVariable<float>();
    [SerializeField] private TMP_Text timerText;
    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            timer.Value = 60;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(!IsServer) { return; }
        timer.Value -= Time.deltaTime;
        UpdateTimerDisplay();
        UpdateTimerClientRpc();
    }

    [ClientRpc]
    private void UpdateTimerClientRpc()
    {
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        float minutes = Mathf.FloorToInt(timer.Value / 60) ;
        float seconds = Mathf.FloorToInt(timer.Value % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
