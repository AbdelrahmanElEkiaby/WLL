using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] GameObject micVolume;
    private float moveSpeed;
    Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    
    void FixedUpdate()
    {
        if (!IsOwner) { return; }

        Move();

    }

    void Move()
    {
        moveSpeed = micVolume.GetComponent<MicrophoneInput>().loudness * 0.01f;
        Vector2 playerVelocity = new Vector2(rb.velocity.x, rb.velocity.y + moveSpeed);
        rb.velocity = playerVelocity;
    }
}
