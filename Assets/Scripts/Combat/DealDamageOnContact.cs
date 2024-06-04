using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class DealDamageOnContact : NetworkBehaviour
{
    [SerializeField] private int damage = 25;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer) return;


        if (collision.gameObject.tag == "Wall")
        {
            Health health = this.GetComponent<Health>();

            health.TakeDamage(damage);
        }
        if (collision.gameObject.tag == "Fan")
        {
            //Destroy(this.gameObject);
            Die();
        }



    }

    private void Die()
    {
        GameManager.Instance.OnPlayerDied(OwnerClientId);
        //NetworkObject.Despawn();
    }
}
