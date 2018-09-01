using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public Image fadeImage;

    [Range(0, 5)]
    public float fadeDuration = 2.0f;

    // Use this for initialization
    void Start () {
        StartCoroutine(FadeHelper.FadeToZeroAlpha(fadeImage, fadeDuration));
    }

    public void OnStartClicked()
    {
        StartCoroutine(FadeHelper.FadeToFullAlpha(fadeImage, fadeDuration, true));
    }
}
