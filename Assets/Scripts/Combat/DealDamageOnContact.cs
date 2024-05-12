using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int damage = 25;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Health health = this.GetComponent<Health>();

            health.TakeDamage(damage);
        }
        if (collision.gameObject.tag == "Fan")
        {
            Destroy(this.gameObject);
        }

    }
}
