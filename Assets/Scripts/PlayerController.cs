using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

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

    public bool holdingOn = true;
    private bool playerReady = false;

    private Rigidbody2D body;
    private string horizontal_axis;
    private string vertical_axis;
    private string grab_button;

    private DistanceJoint2D joint;
    private GameManager gameManager;

    public Text readyText;

    private float min_x;
    private float max_x;

    public GameObject leftWall;
    public GameObject rightWall;

    private Animator animator;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        joint = GetComponent<DistanceJoint2D>();
        gameManager = FindObjectOfType<GameManager>();

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

        body.centerOfMass = new Vector2(0, -0.05f);
        body.angularDrag = drag;
        body.drag = drag/20;

        min_x = leftWall.transform.position.x + (leftWall.GetComponent<BoxCollider2D>().size.x / 2);
        max_x = rightWall.transform.position.x - (rightWall.GetComponent<BoxCollider2D>().size.x / 2);

        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        HandleInput();
    }

    void HandleInput()
    {
        animator.SetBool("isClimbing", false);
        animator.SetBool("isSwinging", false);

        if (CrossPlatformInputManager.GetButtonDown(grab_button))
        {
            if (!FallingTooFast())
            {
                playerReady = true;
                readyText.enabled = true;
                body.isKinematic = true;
                body.velocity = Vector2.zero;
                body.angularVelocity = 0;
                holdingOn = true;
                
                transform.rotation = Quaternion.identity;
            }
        }

        if (playerReady && otherPlayer.GetComponent<PlayerController>().playerReady)
        {
            gameManager.HideInstructionPanel();

            if (CrossPlatformInputManager.GetButtonUp(grab_button))
            {
                body.isKinematic = false;
                holdingOn = false;
            }

            float iHorizontal, iVertical;

            iHorizontal = CrossPlatformInputManager.GetAxis(horizontal_axis);
            iVertical = CrossPlatformInputManager.GetAxis(vertical_axis);

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

                if (iHorizontal != 0 || iVertical != 0)
                {
                    animator.SetBool("isClimbing", true);
                }
                
                float hor = hSpeed * iHorizontal;
                float ver = vSpeed * iVertical;

                if (transform.position.x < min_x)
                {
                    hor = Mathf.Max(hor, 0);
                }
                else if (transform.position.x > max_x)
                {
                    hor = Mathf.Min(hor, 0);
                }

                Vector2 newVelocity = new Vector2(hor, ver);
                body.velocity = newVelocity;
            }
            else if (holdingOn && !otherPlayer.GetComponent<PlayerController>().holdingOn)
            {
                body.velocity = Vector2.zero;
            }
            else //we're not holding on - apply swing force!
            {
                animator.SetBool("isSwinging", true);
                ApplyForce(iHorizontal, iVertical);
            }
        }
        else
        {
            if (CrossPlatformInputManager.GetButtonUp(grab_button))
            {
                playerReady = false;
                readyText.enabled = false;
            }
        }
    }   

    private void ApplyForce(float iHor, float iVert)
    {
        body.AddForce(transform.right * iHor * swingForce);
    }

    public bool FallingTooFast()
    {
        return (body.velocity.magnitude > grabVelocityLimit) && !otherPlayer.GetComponent<PlayerController>().holdingOn;
    }

    public void SetSpeed(float newSpeed)
    {
        vSpeed = newSpeed;
    }
}

public enum PlayerNumber
{
    One, Two
}
