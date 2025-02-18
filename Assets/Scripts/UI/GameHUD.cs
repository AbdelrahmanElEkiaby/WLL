using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    public void LeaveGame()
    {
        if(NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.GameManger.Shutdown();
        }

        ClientSingleton.Instace.GameManager.Disconnect();
    }
}
