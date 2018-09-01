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

    public float drag = 5.0f;

    private Rigidbody2D body;
    public bool holdingOn = false;
    private bool isSwinging = false;

    private string horizontal_axis;
    private string vertical_axis;
    private string grab_button;

    private float angularVelocity;
        
    void Start()
    {
        body = GetComponent<Rigidbody2D>();

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
    }

    void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        if (isSwinging)
        {
            Swing();
        }
    }

    void HandleInput()
    {
        float iHorizontal, iVertical;
                
        iHorizontal = CrossPlatformInputManager.GetAxis(horizontal_axis);
        iVertical = CrossPlatformInputManager.GetAxis(vertical_axis);

        if (CrossPlatformInputManager.GetButtonDown(grab_button))
        {
            body.isKinematic = true;
            holdingOn = true;
            isSwinging = false;

            transform.rotation = Quaternion.identity;
        }

        if (CrossPlatformInputManager.GetButtonUp(grab_button))
        {
            body.isKinematic = false;
            holdingOn = false;
        } 
        
        if (holdingOn)
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
        else //i.e. falling/swinging
        {
            if (!otherPlayer.GetComponent<PlayerController>().holdingOn)
            {
                body.isKinematic = false;
            }

            if (!isSwinging && (rope.CalculateCurrentLength() >= rope.maxLength) && transform.position.y < otherPlayer.position.y)
            {
                isSwinging = true;
                body.velocity = Vector2.zero;
                body.isKinematic = true;
                angularVelocity = CalculateAngularVelocity(body.velocity.y);
            }
        }
    }

    public void Swing()
    {
        angularVelocity += CalculateAngularVelocity(Physics.gravity.y);

        if (angularVelocity > 0)
        {
            angularVelocity -= drag;
        }
        else
        {
            angularVelocity += drag;
        }
                
        transform.RotateAround(otherPlayer.transform.position, Vector3.forward, angularVelocity * Time.deltaTime);
    }

    public float CalculateAngularVelocity(float downAcceleration)
    {
        float angle_toOtherPlayer = Vector2.Angle(transform.up, otherPlayer.position - transform.position);
        float alpha = 90 - angle_toOtherPlayer;
        float angularVelocity = downAcceleration / Mathf.Cos(Mathf.Deg2Rad * alpha);

        if (transform.position.x < otherPlayer.position.x)
        {
            angularVelocity *= -1;
        }
        
        return angularVelocity;
    }
}

public enum PlayerNumber
{
    One, Two
}
