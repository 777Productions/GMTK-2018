using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class MenuManager : MonoBehaviour {

    public Image fadeImage;

    [Range(0, 5)]
    public float fadeDuration = 2.0f;

    // Use this for initialization
    void Start () {
        StartCoroutine(FadeHelper.FadeToZeroAlpha(fadeImage, fadeDuration));
    }

    private void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Player1_Grab"))
        {
            OnPlay();
        }
    }

    public void OnPlay()
    {
        StartCoroutine(FadeHelper.FadeToFullAlpha(fadeImage, fadeDuration, true));
    }
}
