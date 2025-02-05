using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Services.Lobbies.Models;
using System;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public NetworkVariable<float> timer = new NetworkVariable<float>();
    public NetworkVariable<float> CountDowntimer = new NetworkVariable<float>();

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text CountDownTimerText;
    [SerializeField] GameObject EndingHUD;
    [SerializeField] private TMP_Text EndingText;
    [SerializeField] private GameObject OverTimeHUD;
    [SerializeField] private Image Player1Meter;
    [SerializeField] private Image Player2Meter;
    [SerializeField] BackgroundController BgController;
    [SerializeField] ObstacleMovement obstacle;
    
    private float Player1Total;
    private float Player2Total;
    private ulong player1; // Host
    private ulong player2; // First client

    private bool OnStart = false;
    private bool CountDownStart;
    private bool OverTime = false;

    private int playersConnected;
    

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of GameManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (IsServer)
        {
            player1 = NetworkManager.Singleton.LocalClientId; // Host is player1
            Debug.Log($"Player 1 (Host) ID: {player1}");

            timer.Value = 0;
            CountDowntimer.Value = 6;
        }
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;

        playersConnected++;

        if (playersConnected == 1)
        {
            player2 = clientId; // First connected client is player2
            Debug.Log($"Player 2 (Client) ID: {player2}");

            CountDownStart = true;
            OnStart = true;
            SetCountDownTimerActiveClientRpc(true);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (!IsServer) return;

        playersConnected--;

        if (clientId == player2)
        {
            player2 = ulong.MaxValue;
            Debug.Log("Player 2 disconnected");
        }

        if (playersConnected == 0)
        {
            OnStart = false;
            StopGame();
        }
    }

    void Update()
    {
        if (!IsServer) return;

        if (CountDownStart)
        {
            if ((int)CountDowntimer.Value == 2)
            {
                ChangeTextColorClientRpc();
            }
            if ((int)CountDowntimer.Value == 0)
            {
                CountDownStart = false;
                SetCountDownTimerActiveClientRpc(false);
                SetTimerTextActiveClientRpc(true);
                StartGame();
            }
            else
            {
                CountDowntimer.Value -= Time.deltaTime;
                UpdateCountDownDisplayClientRpc();
            }
        }
        if (OnStart && !CountDownStart)
        {
            if ((int)timer.Value == 0)
            {
                //ActivateOverTimeBackgroundClientRpc();
                OverTime = true;
                OverTimeHUD.SetActive(true);
                //ObstacleMoveClientRpc(true, 1);
                //ActivatePanic();

            }
            else if ((int)timer.Value == 5)
            {
                //DeactivateScoreCalcClientRpc();
                //ObstacleMoveClientRpc(false, 0);
                //ActivateMainBackgroundClientRpc();
                //DeactivatePanic();
                //DetermineWinner();
            }
            else if((int)timer.Value == 15)
            {
                //ActivatePanic();
                //ActivateScoreCalcClientRpc();
                //ObstacleMoveClientRpc(true, 2);
                //DetermineWinner();
            }
            else if((int)timer.Value == 20)
            {
                //DeactivatePanic();
                //ObstacleMoveClientRpc(false, 0);
                //DeactivateScoreCalcClientRpc();
            }
            else if ((int)timer.Value == 30)
            {
                //ScreamMeterFillClientRpc();
                ActivateOverTimeBackgroundClientRpc();
            }
            else if((int)timer.Value == 40)
            {
                //ActivateMainBackgroundClientRpc();
                //StopGame();
            }

            if(OverTime)
            {
                ActivateOverTimeMeter();
            }
            timer.Value += Time.deltaTime;
            UpdateTimerDisplay();
            UpdateTimerClientRpc();
        }
    }

    [ClientRpc]
    private void ObstacleMoveClientRpc(bool flag, int num)
    {
        obstacle.Move(flag, num);
    }

    private void ActivateMeter()
    {
        ActivateMeterClientRpc();
    }

    [ClientRpc]
    void ActivateMeterClientRpc()
    {
        OverTimeHUD.gameObject.SetActive(true);
    }

    [ClientRpc]
    private void ChangeTextColorClientRpc()
    {
        CountDownTimerText.color = Color.red;
    }

    [ClientRpc]
    void UpdateCountDownDisplayClientRpc()
    {
        UpdateCountDownDisplay();
    }

    void UpdateCountDownDisplay()
    {
        float seconds = Mathf.FloorToInt(CountDowntimer.Value % 60);
        CountDownTimerText.text = $"{seconds}";
    }

    [ClientRpc]
    private void SetCountDownTimerActiveClientRpc(bool isActive)
    {
        CountDownTimerText.gameObject.SetActive(isActive);
    }

    [ClientRpc]
    private void SetTimerTextActiveClientRpc(bool isActive)
    {
        timerText.gameObject.SetActive(isActive);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
    }

    public override void OnNetworkDespawn()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    [ClientRpc]
    private void ActivateMainBackgroundClientRpc()
    {
        BgController.ActivateMainBG();
       
    }

    [ClientRpc]
    private void ActivateOverTimeBackgroundClientRpc()
    {
        BgController.ActivatOverTimeBG();
       
    }

    [ClientRpc]
    private void UpdateTimerClientRpc()
    {
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        float minutes = Mathf.FloorToInt(timer.Value / 60);
        float seconds = Mathf.FloorToInt(timer.Value % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void DetermineWinner()
    {
        if (!IsServer) return;

        ulong winnerId;
        if (Player1Total > Player2Total)
        {
            winnerId = player1;
        }
        else
        {
            winnerId = player2;
        }

        UserData userData = HostSingleton.Instance.GameManger.networkServer.GetUserDataByClientId(winnerId);
        string winner = userData.userName;
        StopGame();
        OnStart = false;
        DisplayWinnerClientRpc(winner);
    }

    public void OnPlayerDied(ulong playerId)
    {
        if (!IsServer) return;
        ulong winnerId = GetWinner(playerId);
        UserData userData = HostSingleton.Instance.GameManger.networkServer.GetUserDataByClientId(winnerId);
        string winner = userData.userName;
        StopGame();
        ObstacleMoveClientRpc(false, 0);
        OnStart = false;
        DisplayWinnerClientRpc(winner);
    }

    private ulong GetWinner(ulong deadPlayerId)
    {
        if (!IsServer) return ulong.MaxValue;

        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            if (client.Key != deadPlayerId)
            {
                return client.Key;
            }
        }
        return ulong.MaxValue; // No winner found
    }

    [ClientRpc]
    private void DisplayWinnerClientRpc(string winner)
    {
        EndingHUD.SetActive(true);
        EndingText.text = $"{winner} won";
    }

    private void StartGame()
    {
        if (!IsServer) return;

        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            StartGameClientRpc(client.Key);
        }
    }

    private void StopGame()
    {
        if (!IsServer) return;

        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            StopGameClientRpc(client.Key);
        }
    }

    [ClientRpc]
    private void StopGameClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent(out ScaleFromMicrophone playerScript))
            {
                playerScript.StopMovement();
            }
        }
    }

    private void ActivateOverTimeMeter()
    {
        int f = 0;
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            ActivateOverTimeMeterServerRpc(client.Key,f);
            Debug.Log(client.Key + " test");
            f++;
            
        }
    }
    [ServerRpc]
    private void ActivateOverTimeMeterServerRpc(ulong clientId, int flag)
    {
        float num = 0f;
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            var playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
            if (playerObject != null)
            {
                AudioLoudnessDetector SoundNum = playerObject.GetComponentInChildren<AudioLoudnessDetector>();
                num = SoundNum.GetLoudnessFromMicrophone() * 100f * 0.1f;
                Debug.Log(clientId + $" {num}");
                ActivateOverTimeMeterClientRpc(num, flag);
            }

        }
    }

    [ClientRpc]
    private void ActivateOverTimeMeterClientRpc(float num,int flag)
    {
        if(flag == 0)
        {
            Player1Meter.fillAmount = num;
        }
        else
        {
            Player2Meter.fillAmount = num;
        }
    }

    [ClientRpc]
    private void StartGameClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent(out ScaleFromMicrophone playerScript))
            {
                playerScript.StartMovement();
            }
        }
    }
    private void ActivatePanic()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            //Debug.LogWarning(client.Key + " ActivateFunc");
            ActivatePanicClientRpc(client.Key);
        }
    }

    private void DeactivatePanic()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            //Debug.LogWarning(client.Key + " ActivateFunc");
            DeactivatePanicClientRpc(client.Key);
        }
    }

    [ClientRpc]
    private void ActivatePanicClientRpc(ulong clientId)
    {
        
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            var playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
            if(playerObject != null)
            {
                NetworkedAnimatorBoolController playerScript = playerObject.GetComponentInChildren<NetworkedAnimatorBoolController>();
                playerScript.SetBoolParameterServerRpc("IsPanicing", true);
            }
            
        }
    }

    [ClientRpc]
    private void DeactivatePanicClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            var playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
            if (playerObject != null)
            {
                NetworkedAnimatorBoolController playerScript = playerObject.GetComponentInChildren<NetworkedAnimatorBoolController>();
                playerScript.SetBoolParameterServerRpc("IsPanicing", false);
            }

        }
    }

    [ClientRpc]
    private void ScreamMeterFillClientRpc()
    {
        if (!IsServer) return;

        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            if (client.Value.PlayerObject.TryGetComponent(out ScaleFromMicrophone playerScript))
            {
                float temp = playerScript.TotalScreamScore.Value;
                Debug.Log($"Client {client.Key} Temp Value: {temp}");

                if (client.Key == player1)
                {
                    Player1Total += temp;
                    Debug.Log($"Player1Total: {Player1Total}");
                    UpdatePlayerMeterClientRpc(player1, temp/100);
                }
                else if (client.Key == player2)
                {
                    Player2Total += temp;
                    Debug.Log($"Player2Total: {Player2Total}");
                    UpdatePlayerMeterClientRpc(player2, temp/100);
                }
            }
        }
    }

    [ClientRpc]
    private void UpdatePlayerMeterClientRpc(ulong playerId, float fillAmount)
    {
        if (playerId == player1)
        {
            Player1Meter.fillAmount = fillAmount;
            Debug.Log($"Player1Meter updated to: {fillAmount}");
        }
        else if (playerId == player2)
        {
            Player2Meter.fillAmount = fillAmount;
            Debug.Log($"Player2Meter updated to: {fillAmount}");
        }
    }
}




