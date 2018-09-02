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

    public GameObject flag;
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

    public IEnumerator Win(GameObject winner)
    {
        //get both player controllers
        PlayerController winningPlayer = winner.GetComponent<PlayerController>();
        GameObject loser = winningPlayer.otherPlayer.gameObject;
        PlayerController losingPlayer = loser.GetComponent<PlayerController>();

        winningPlayer.DisableMovement();
        losingPlayer.DisableMovement();

        //fade to black
        fadeImage.enabled = true;
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);

        while (fadeImage.color.a < 1.0f)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeImage.color.a + (Time.deltaTime / 0.5f));
            yield return null;
        }
                

                
        //set sprite
        winner.GetComponent<SpriteRenderer>().sprite = winningPlayer.winSprite;
        loser.GetComponent<SpriteRenderer>().sprite = losingPlayer.loseSprite;

        //disable both player scripts
        winningPlayer.enabled = false;
        losingPlayer.enabled = false;

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
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);

        while (fadeImage.color.a > 0.0f)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeImage.color.a - (Time.deltaTime / 0.5f));
            yield return null;
        }

        fadeImage.enabled = false;
        
        yield return null;
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
