using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Rope rope;
    public PlayerColour playerColour;
    public PlayerController otherPlayer;

    [Range(0.1f, 3.0f)]
    public float vSpeed = 1.0f;
    [Range(0.1f, 3.0f)]
    public float hSpeed = 1.0f;

    public float grabVelocityLimit = 5;

    public AudioClip climbingSound;
    public AudioClip swingingSound;
    public AudioClip deathSound;

    private AudioSource audioSource;

    private Rigidbody2D body;

    private string horizontal_axis;
    private string vertical_axis;
    private string grab_button;

    private DistanceJoint2D joint;

    private GameManager gameManager;

    public Text readyText;

    private Animator animator;

    public float swingStrength = 5.0f;

    public bool supportingPlayer;

    public Sprite winSprite;
    public Sprite loseSprite;

    public Color colour;

    public bool gameOver;

    private bool deathFallingSoundPlayed = false;

    #region States

    private bool isReady;
    public bool IsReady
    {
        get { return isReady; }
        set
        {
            isReady = value;
            readyText.enabled = value;
        }
    }

    private bool isDead;
    public bool IsDead
    {
        get { return isDead; }
        set
        {
            if (!isDead && value == true)
            {
                PlayClip(deathSound);
            }

            isDead = value;

            if (isDead)
            {
                animator.SetTrigger("dead");
            }
        }
    }

    private bool isStationary;
    public bool IsStationary
    {
        get { return isStationary; }
        set
        {
            isStationary = value;
            animator.SetBool("isStationary", value);
        }
    }

    private bool isClimbing;
    public bool IsClimbing
    {
        get { return isClimbing; }
        set
        {
            isClimbing = value;
            animator.SetBool("isClimbing", value);
        }
    }

    private bool isSwinging;
    public bool IsSwinging
    {
        get { return isSwinging; }
        set
        {
            isSwinging = value;
            animator.SetBool("isSwinging", value);
        }
    }

    private bool isFalling;
    public bool IsFalling
    {
        get { return isFalling; }
        set
        {
            isFalling = value;
            animator.SetBool("isFalling", value);
        }
    }

    private bool isHoldingOn;
    public bool IsHoldingOn
    {
        get { return isHoldingOn; }
        set
        {
            isHoldingOn = value;

            if (value)
            {
                //body.velocity = Vector2.zero;
                body.rotation = 0;
                body.constraints = RigidbodyConstraints2D.FreezeRotation;
                body.gravityScale = 0;
                body.mass = 100;
                otherPlayer.SetSupporting(false);
            }
            else
            {
                body.constraints = RigidbodyConstraints2D.None;
                body.gravityScale = 1;
                body.mass = 1;

                otherPlayer.SetSupporting(true);
            }
        }
    }

    #endregion


    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        joint = GetComponent<DistanceJoint2D>();
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (playerColour == PlayerColour.Blue)
        {
            horizontal_axis = "Player1_Horizontal";
            vertical_axis = "Player1_Vertical";
            grab_button = "Player1_Grab";
            colour = new Color(26, 152, 238);
        }
        else
        {
            horizontal_axis = "Player2_Horizontal";
            vertical_axis = "Player2_Vertical";
            grab_button = "Player2_Grab";
            colour = new Color(209, 15, 15);
        }

        joint.distance = rope.maxLength;

        body.centerOfMass = new Vector2(0, -1f);

        IsHoldingOn = true;
        IsReady = false;
        IsDead = false;
        IsStationary = true;
        IsClimbing = false;
        IsSwinging = false;
        IsFalling = false;

        gameOver = false;
        
    }

    void Update()
    {
        if (gameManager.gameStarted && !IsDead && !gameOver)
        {
            HandleInput();
        }
        else
        {
            HandleGameStart();
        }

        if (FallingTooFast() && otherPlayer.FallingTooFast() && ! deathFallingSoundPlayed && !IsDead)
        {
            deathFallingSoundPlayed = true;
            Debug.Log("You dead");
            PlayClip(deathSound);
        }
    }

    private void HandleGameStart()
    {
        if (CrossPlatformInputManager.GetButtonDown(grab_button))
        {
            IsReady = true;
        }

        if (CrossPlatformInputManager.GetButtonUp(grab_button))
        {
            IsReady = false;
        }

        if (BothReady())
        {
            gameManager.StartGame();
        }
    }

    private bool BothReady()
    {
        return IsReady && otherPlayer.IsReady;
    }

    void HandleInput()
    {
        IsSwinging = false;
        IsStationary = false;
        IsClimbing = false;
        IsFalling = false;

        float iHorizontal = CrossPlatformInputManager.GetAxis(horizontal_axis);
        float iVertical = CrossPlatformInputManager.GetAxis(vertical_axis);

        if (CrossPlatformInputManager.GetButtonDown(grab_button))
        {
            if (!FallingTooFast())
            {
                IsHoldingOn = true;
            }
        }
        else if (CrossPlatformInputManager.GetButtonUp(grab_button))
        {
            IsHoldingOn = false;
        }

        // Climbing or stationary
        if (IsHoldingOn)
        {
            // Only allow movement if both players are holding on.
            if (!supportingPlayer)
            {
                Vector2 newVelocity = new Vector2(iHorizontal * hSpeed, iVertical * vSpeed);

                IsClimbing = newVelocity.magnitude > 0.0f;
                IsStationary = newVelocity.magnitude == 0.0f;

                body.velocity = newVelocity;
            }
            else
            {
                IsStationary = true;

                body.velocity = Vector2.zero;
            }
        }

        // Falling or swinging
        else
        {
            if ((transform.position - otherPlayer.transform.position).magnitude > joint.distance - 0.05)
            {
                IsSwinging = true;

            }
            else
            {
                IsFalling = true;
            }

            body.AddForce(transform.right * iHorizontal * swingStrength);
        }
    }

    public bool FallingTooFast()
    {
        return body.velocity.magnitude > grabVelocityLimit && !otherPlayer.IsHoldingOn;
    }

    public void SetSupporting(bool isSupporting)
    {
        supportingPlayer = isSupporting;

        if (supportingPlayer && IsHoldingOn)
        {
            body.velocity = Vector2.zero;
        }
    }

    public void SetSpeed(float newSpeed)
    {
        vSpeed = newSpeed;
    }

    public void Die()
    {
        IsDead = true;
        IsHoldingOn = false;
    }

    private void PlayClip(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void DisableMovement()
    {
        body.isKinematic = true;
        body.velocity = Vector3.zero;
        animator.enabled = false;
        gameOver = true;
    }
}

public enum PlayerColour
{
    Blue, Red
}
