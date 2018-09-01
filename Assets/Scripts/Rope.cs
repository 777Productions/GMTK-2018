using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    private LineRenderer line;

    public float maxLength;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
    }

    void Update ()
    {
        RedrawLine();
    }

    public void RedrawLine()
    {
        //only want to do this when a player moves
        Vector3[] wireEnds = { player1.position, player2.position };
        line.SetPositions(wireEnds);
    }
    
    public float CalculateCurrentLength()
    {
        Vector2 p1pos = player1.position;
        Vector2 p2pos = player2.position;

        Vector2 distanceVector = p2pos - p1pos;

        return distanceVector.magnitude;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(player1.position, player2.position);
    }
}
