using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject winPanel;

    public GameObject instructionPanel;

    public bool gameStarted = false;

    public Image fadeImage;

    [Range(0, 5)]
    public float fadeDuration = 2.0f;
    
    private bool waitingOnNewGame = false;

    private Vector3 winPos;
    private Vector3 losePos;
    private Vector3 winCameraPos;

    public SpriteRenderer flag;
    public GameObject HUD;
    
    // Use this for initialization
    void Start ()
    {
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
        StartCoroutine(FadeHelper.FadeToZeroAlpha(fadeImage, fadeDuration));

        winPos = new Vector3(-2.4f, 50.2f, 0);
        losePos = new Vector3(-0.46f, 47.83f, 0);
        winCameraPos = new Vector3(0f, 52.7f, -10);
    }

    private void Update()
    {
        if (waitingOnNewGame)
        {
            if (CrossPlatformInputManager.GetButtonDown("Player1_Grab"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            if (CrossPlatformInputManager.GetButtonDown("Cancel"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            }
        }
    }

    public void Win(GameObject winner)
    {
        //fade to black
        FadeHelper.FadeToFullAlpha(fadeImage, 2.0f);
        
        //get both player controllers
        PlayerController winningPlayer = winner.GetComponent<PlayerController>();
        GameObject loser = winningPlayer.otherPlayer.gameObject;
        PlayerController losingPlayer = loser.GetComponent<PlayerController>();
                
        //set sprite
        winner.GetComponent<SpriteRenderer>().sprite = winningPlayer.winSprite;
        loser.GetComponent<SpriteRenderer>().sprite = losingPlayer.loseSprite;

        //change flag colour
        flag.color = winningPlayer.colour;

        //disable both player scripts
        winningPlayer.Disable();
        losingPlayer.Disable();

        //set pos
        winner.transform.position = winPos;
        loser.transform.position = losePos;
       
        //move camera
        Camera.main.GetComponent<CameraController>().enabled = false;
        Camera.main.transform.position = winCameraPos;
        
        //set active text
        winPanel.SetActive(true);
        waitingOnNewGame = true;

        //hide the HUD
        HUD.SetActive(false);

        //fade from black
        FadeHelper.FadeToZeroAlpha(fadeImage, 2.0f);
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        waitingOnNewGame = true;
    }

    public void StartGame()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            Animator fadePanel = instructionPanel.GetComponent<Animator>();
            fadePanel.SetTrigger("FadeOut");
        }
    }
}
