using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ObstacleMovement : NetworkBehaviour
{
    private Vector3 scaleChange, positionChange;
    public bool isMove;
    public int MoveDir;

    void Start()
    {
        // Define the amount of change in scale
        scaleChange = new Vector3(0f, 0.05f, 0f);
        // Define the amount of change in position
        //positionChange = new Vector3(0f, 0.001f, 0f);
    }

    void Update()
    {
        if (!IsServer) return;

        // Update the position so that it appears to grow only from the top
        if (isMove && MoveDir == 1)
        {
            if (this.transform.position.y <= -2.5f)
            {
                this.transform.position += new Vector3(0, scaleChange.y / 2, 0);
                this.transform.localScale += scaleChange;
            }
            else
            {
                isMove = false;
                MoveDir = 0;
            }
        }
        else if(isMove && MoveDir == 2)
        {
            if (this.transform.position.y >= -4.5f)
            {
                this.transform.position -= new Vector3(0, scaleChange.y / 2, 0);
                this.transform.localScale -= scaleChange;
            }
            else
            {
                isMove = false;
                MoveDir = 0;
            }
        }
    }

    public void Move(bool move, int dir)
    {
        isMove = move;
        MoveDir = dir;
    }
}
