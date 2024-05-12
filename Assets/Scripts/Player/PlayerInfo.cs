using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using Unity.Collections;

public class PlayerInfo : NetworkBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [SerializeField] private int ownerPriority = 15;

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            UserData userData =
                HostSingleton.Instance.GameManger.networkServer.GetUserDataByClientId(OwnerClientId);

            PlayerName.Value = userData.userName;

        }
        if (IsOwner)
        {
            virtualCamera.Priority = ownerPriority;
        }
    }
}
