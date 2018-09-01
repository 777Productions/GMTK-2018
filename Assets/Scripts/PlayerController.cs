using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class PlayerController : MonoBehaviour
{
    public Rope rope;
    public PlayerNumber playerNumber;
    public Transform otherPlayer;

    [Range(0.1f, 3.0f)]
    public float vSpeed = 1.0f;
    [Range(0.1f, 3.0f)]
    public float hSpeed = 1.0f;

    public float drag = 0.5f;
    public float grabVelocityLimit = 5;
    public float swingForce = 5;

    public bool holdingOn = false;

    private Rigidbody2D body;
    private string horizontal_axis;
    private string vertical_axis;
    private string grab_button;

    private DistanceJoint2D joint;
        
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        joint = GetComponent<DistanceJoint2D>();
        joint.distance = rope.maxLength;

        if (playerNumber == PlayerNumber.One)
        {
            horizontal_axis = "Player1_Horizontal";
            vertical_axis = "Player1_Vertical";
            grab_button = "Player1_Grab";
        }
        else
        {
            horizontal_axis = "Player2_Horizontal";
            vertical_axis = "Player2_Vertical";
            grab_button = "Player2_Grab";
        }

        body.centerOfMass = new Vector2(0, -0.1f);
        body.angularDrag = drag;
        body.drag = drag/20;
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        float iHorizontal, iVertical;
                
        iHorizontal = CrossPlatformInputManager.GetAxis(horizontal_axis);
        iVertical = CrossPlatformInputManager.GetAxis(vertical_axis);

        if (CrossPlatformInputManager.GetButtonDown(grab_button))
        {
            if (!FallingTooFast())
            {
                body.isKinematic = true;
                body.velocity = Vector2.zero;
                body.angularVelocity = 0;
                holdingOn = true;

                transform.rotation = Quaternion.identity;
            }
        }

        if (CrossPlatformInputManager.GetButtonUp(grab_button))
        {
            body.isKinematic = false;
            holdingOn = false;
        } 
        
        if (holdingOn && otherPlayer.GetComponent<PlayerController>().holdingOn)
        {
            if (rope.CalculateCurrentLength() >= rope.maxLength)
            {
                //limit climbing
                if (otherPlayer.position.y > transform.position.y)
                {
                    iVertical = Mathf.Clamp(iVertical, 0, 1);
                }
                else
                {
                    iVertical = Mathf.Clamp(iVertical, -1, 0);
                }

                if (otherPlayer.position.x > transform.position.x)
                {
                    iHorizontal = Mathf.Clamp(iHorizontal, 0, 1);
                }
                else
                {
                    iHorizontal = Mathf.Clamp(iHorizontal, -1, 0);
                }
            }

            body.velocity = new Vector2(hSpeed * iHorizontal, vSpeed * iVertical);
        }
        else if (holdingOn && !otherPlayer.GetComponent<PlayerController>().holdingOn)
        {
            body.velocity = Vector2.zero;
        }
        else //we're not holding on - apply swing force!
        {
            ApplyForce(iHorizontal, iVertical);
        }

    }   

    private void ApplyForce(float iHor, float iVert)
    {
        body.AddForce(Vector2.right * iHor * swingForce);
    }

    public bool FallingTooFast()
    {
        return body.velocity.magnitude > grabVelocityLimit;
    }
}

public enum PlayerNumber
{
    One, Two
}
