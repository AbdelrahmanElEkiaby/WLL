using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class Health : NetworkBehaviour
{

    [field : SerializeField] public int MaxHealth { get; private set; }  = 100;
    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();
    private bool isDead;
    public Action<Health> OnDie;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }
        CurrentHealth.Value = MaxHealth;
    }

    public void TakeDamage(int damageValue)
    {
        //ModifyHealth(-damageValue);
    }

    private void ModifyHealth(int value)
    {
        if (isDead) { return; }
        CurrentHealth.Value += value;
        CurrentHealth.Value = Mathf.Clamp(CurrentHealth.Value, 0, MaxHealth);

        if(CurrentHealth.Value == 0)
        {
            OnDie?.Invoke(this);
            isDead = true;
            Debug.Log(isDead);
        }
    }
}
