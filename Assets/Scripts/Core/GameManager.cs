//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Unity.Netcode;
//using TMPro;
//using Unity.Services.Lobbies.Models;
//using System;

//public class GameManager : NetworkBehaviour
//{
//    public static GameManager Instance { get; private set; }

//    public NetworkVariable<float> timer = new NetworkVariable<float>();
//    [SerializeField] private TMP_Text timerText;
//    [SerializeField] GameObject EndingHUD;
//    [SerializeField] private TMP_Text EndingText;
//    private bool OnStart;

//    private int playersConnected;

//    private void Awake()
//    {
//        // Singleton pattern to ensure only one instance of GameManager
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    void Start()
//    {
//        if (IsServer)
//        {
//            timer.Value = 60;
//        }
//        if (IsServer)
//        {
//            if (NetworkManager.Singleton != null)
//            {
//                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
//                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
//            }
//        }

//    }

//    private void OnClientConnected(ulong clientId)
//    {
//        if (!IsServer) return;

//        // Increment the number of connected players
//        playersConnected++;

//        //Debug.Log(playersConnected);
//        // Check if two players are connected
//        if (playersConnected == 1)
//        {
//            OnStart = true;
//            StartGame();
//        }
//    }

//    private void OnClientDisconnected(ulong clientId)
//    {
//        if (!IsServer) return;

//        // Decrement the number of connected players
//        playersConnected--;
//        if (playersConnected == 0)
//        {
//            OnStart = false;
//            StopGame();
//        }
//    }

//    void Update()
//    {
//        if (!IsServer) { return; }
//        if (OnStart)
//        {

//            if ((int)timer.Value == 55)
//            {
//                ActivateMeter();
//            }
//            //else if ((int)timer.Value == 50)
//            //{
//            //    DeactivateMeter();
//            //}
//            timer.Value -= Time.deltaTime;
//            UpdateTimerDisplay();
//            UpdateTimerClientRpc();
//        }

//    }

//    public override void OnNetworkSpawn()
//    {
//        if (!IsServer) { return; }
//    }

//    public override void OnNetworkDespawn()
//    {
//        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
//        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
//    }

//    public void OnPlayerDied(ulong playerId)
//    {
//        if (!IsServer) return;
//        ulong winnerId = GetWinner(playerId);
//        UserData userData =
//                HostSingleton.Instance.GameManger.networkServer.GetUserDataByClientId(winnerId);
//        string winner = userData.userName;
//        DisplayWinnerClientRpc(winner);
//        StopGame();
//        OnStart = false;
//    }

//    private ulong GetWinner(ulong deadPlayerId)
//    {
//        foreach (var client in NetworkManager.Singleton.ConnectedClients)
//        {
//            if (client.Key != deadPlayerId)
//            {
//                return client.Key;
//            }
//        }
//        return ulong.MaxValue; // No winner found
//    }

//    [ClientRpc]
//    private void DisplayWinnerClientRpc(string winner)
//    {
//        EndingHUD.SetActive(true);
//        EndingText.text = $"{winner} won";

//    }

//    [ClientRpc]
//    private void UpdateTimerClientRpc()
//    {
//        UpdateTimerDisplay();
//    }

//    private void UpdateTimerDisplay()
//    {
//        float minutes = Mathf.FloorToInt(timer.Value / 60);
//        float seconds = Mathf.FloorToInt(timer.Value % 60);
//        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
//    }

//    private void StartGame()
//    {
//        foreach (var client in NetworkManager.Singleton.ConnectedClients)
//        {
//            StartGameClientRpc(client.Key);
//        }
//    }

//    private void StopGame()
//    {
//        foreach (var client in NetworkManager.Singleton.ConnectedClients)
//        {
//            StopGameClientRpc(client.Key);
//        }
//    }

//    private void ActivateMeter()
//    {
//        foreach (var client in NetworkManager.Singleton.ConnectedClients)
//        {
//            ActivateMeterClientRpc(client.Key);
//        }
//    }

//    [ClientRpc]
//    private void ActivateMeterClientRpc(ulong clientId)
//    {
//        if (NetworkManager.Singleton.LocalClientId == clientId)
//        {
//            if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent(out ScaleFromMicrophone playerScript))
//            {
//                playerScript.ActivateMeter();
//            }
//        }
//    }

//    private void DeactivateMeter()
//    {
//        foreach (var client in NetworkManager.Singleton.ConnectedClients)
//        {
//            DeactivateMeterClientRpc(client.Key);
//        }
//    }

//    [ClientRpc]
//    private void DeactivateMeterClientRpc(ulong clientId)
//    {
//        if (NetworkManager.Singleton.LocalClientId == clientId)
//        {
//            if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent(out ScaleFromMicrophone playerScript))
//            {
//                playerScript.DeactivateMeter();
//            }
//        }
//    }

//    [ClientRpc]
//    private void StopGameClientRpc(ulong clientId)
//    {
//        if (NetworkManager.Singleton.LocalClientId == clientId)
//        {
//            if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent(out ScaleFromMicrophone playerScript))
//            {
//                playerScript.StopMovement();
//            }
//        }
//    }

//    [ClientRpc]
//    private void StartGameClientRpc(ulong clientId)
//    {
//        if (NetworkManager.Singleton.LocalClientId == clientId)
//        {
//            if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent(out ScaleFromMicrophone playerScript))
//            {
//                playerScript.StartMovement();
//            }
//        }
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Services.Lobbies.Models;
using System;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public NetworkVariable<float> timer = new NetworkVariable<float>();
    [SerializeField] private TMP_Text timerText;
    [SerializeField] GameObject EndingHUD;
    [SerializeField] private TMP_Text EndingText;
    private bool OnStart;

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
            timer.Value = 60;
        }
        if (IsServer)
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            }
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;

        // Increment the number of connected players
        playersConnected++;

        // Check if two players are connected
        if (playersConnected == 1)
        {
            OnStart = true;
            StartGame();
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (!IsServer) return;

        // Decrement the number of connected players
        playersConnected--;
        if (playersConnected == 0)
        {
            OnStart = false;
            StopGame();
        }
    }

    void Update()
    {
        if (!IsServer) { return; }
        if (OnStart)
        {
            if ((int)timer.Value == 55)
            {
                ActivateMeter();
            }
            //else if ((int)timer.Value == 50)
            //{
            //    DeactivateMeter();
            //}
            timer.Value -= Time.deltaTime;
            UpdateTimerDisplay();
            UpdateTimerClientRpc();
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public void OnPlayerDied(ulong playerId)
    {
        if (!IsServer) return;
        ulong winnerId = GetWinner(playerId);
        UserData userData = HostSingleton.Instance.GameManger.networkServer.GetUserDataByClientId(winnerId);
        string winner = userData.userName;
        DisplayWinnerClientRpc(winner);
        StopGame();
        OnStart = false;
    }

    private ulong GetWinner(ulong deadPlayerId)
    {
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

    private void StartGame()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            StartGameClientRpc(client.Key);
        }
    }

    private void StopGame()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            StopGameClientRpc(client.Key);
        }
    }

    private void ActivateMeter()
    {
        ActivateMeterClientRpc();
    }

    private void DeactivateMeter()
    {
        DeactivateMeterClientRpc();
    }

    [ClientRpc]
    private void ActivateMeterClientRpc()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            if (client.Value.PlayerObject.TryGetComponent(out ScaleFromMicrophone playerScript))
            {
                playerScript.ActivateMeter();
            }
        }
    }

    [ClientRpc]
    private void DeactivateMeterClientRpc()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            if (client.Value.PlayerObject.TryGetComponent(out ScaleFromMicrophone playerScript))
            {
                playerScript.DeactivateMeter();
            }
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
}





