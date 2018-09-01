using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float x_move_factor = 10f;

    private float y_min;

    private PlayerController[] playerControllers;
    
    private void Start()
    {
        playerControllers = FindObjectsOfType<PlayerController>();
        y_min = transform.position.y;
    }

    void Update ()
    {
        if (!AllPlayersDead())
        {
            transform.position = GetNewCameraPos();
        }
    }

    private Vector3 GetNewCameraPos()
    {
        float sum_x = 0f, sum_y = 0f;

        foreach(PlayerController player in playerControllers)
        {
            sum_x += player.transform.position.x;
            sum_y += player.transform.position.y;
        }

        float average_x = sum_x / playerControllers.Length;
        float average_y = sum_y / playerControllers.Length;

        average_y = Mathf.Max(y_min, average_y);
        average_x = Mathf.Clamp(average_x / x_move_factor, -0.7f, 0.7f);
        
        return new Vector3(average_x, average_y, transform.position.z);
    }

    private bool AllPlayersDead()
    {
        foreach(PlayerController player in playerControllers)
        {
            if (!player.FallingTooFast())
            {
                return false;
            }
        }

        return true;
    }
}
