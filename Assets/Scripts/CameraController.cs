using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    public bool isTracking = true;
	
	void Update ()
    {
        if (isTracking)
        {       
            float averageYpos = GetAveragePlayerYpos();
            transform.position = new Vector3(transform.position.x, averageYpos, transform.position.z);
        }
    }

    private float GetAveragePlayerYpos()
    {
        return (player1.transform.position.y + player2.transform.position.y) / 2;
    }
}
