using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Point : NetworkBehaviour
{
    [SerializeField] GameObject positionHost;
    void Start()
    {
        if(IsHost)
        {

            this.transform.position = positionHost.transform.position;
        }
    }

    
}
