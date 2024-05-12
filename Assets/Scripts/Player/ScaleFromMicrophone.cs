using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ScaleFromMicrophone : NetworkBehaviour
{
    [SerializeField] AudioLoudnessDetector detector;
    Rigidbody2D rb;

    public float LoudnessSensiblity = 100f;
    public float threshold = 0.1f;

    private void Start()
    {
        if (!IsOwner) return;
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        float loudness = detector.GetLoudnessFromMicrophone() * LoudnessSensiblity *threshold;
        //Debug.Log(loudness);
        Vector2 playerVelocity = new Vector2(rb.velocity.x, rb.velocity.y + loudness);
        rb.velocity = playerVelocity;
        

    }
}
