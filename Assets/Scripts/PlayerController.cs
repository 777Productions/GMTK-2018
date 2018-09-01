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

    private Rigidbody2D body;
    private bool holdingOn;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        float iHorizontal, iVertical;

        if (playerNumber == PlayerNumber.One)
        {
            iHorizontal = CrossPlatformInputManager.GetAxis("Player1_Horizontal");
            iVertical = CrossPlatformInputManager.GetAxis("Player1_Vertical");
            if (CrossPlatformInputManager.GetButtonDown("Player1_Grab"))
            {
                body.isKinematic = true;
                holdingOn = true;
            }
            if (CrossPlatformInputManager.GetButtonUp("Player1_Grab"))
            {
                body.isKinematic = false;
                holdingOn = false;
            }
        }
        else
        {
            iHorizontal = CrossPlatformInputManager.GetAxis("Player2_Horizontal");
            iVertical = CrossPlatformInputManager.GetAxis("Player2_Vertical");
            if (CrossPlatformInputManager.GetButtonDown("Player2_Grab"))
            {
                body.isKinematic = true;
                holdingOn = true;
            }
            if (CrossPlatformInputManager.GetButtonUp("Player2_Grab"))
            {
                body.isKinematic = false;
                holdingOn = false;
            }
        }

        if (rope.CalculateCurrentLength() >= rope.maxLength)
        {
            if (holdingOn)
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
            else
            {
                //swing
                Debug.Log("I am swinging");
            }
        }

        if (body.isKinematic == true)
        {
            body.velocity = new Vector2(hSpeed * iHorizontal, vSpeed * iVertical);
        }
    }
}

public enum PlayerNumber
{
    One, Two
}
