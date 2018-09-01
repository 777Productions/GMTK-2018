using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashManager : MonoBehaviour {

    [Range(0.1f, 10.0f)]
    public float duration = 5.0f;

    [Range(0, 5)]
    public float fadeDuration = 2.0f;

    public Image fadeImage;

    private float startTime;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
        StartCoroutine(FadeHelper.FadeToZeroAlpha(fadeImage, fadeDuration));
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime >= duration)
        {
            StartCoroutine(FadeHelper.FadeToFullAlpha(fadeImage, fadeDuration, true));
        }
    }
}

public static class FadeHelper
{
    public static IEnumerator FadeToFullAlpha(Image fadeImage, float fadeDuration, bool loadNextScene = false)
    {
        fadeImage.enabled = true;
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);

        while (fadeImage.color.a < 1.0f)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeImage.color.a + (Time.deltaTime / fadeDuration));
            yield return null;
        }

        if (loadNextScene)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public static IEnumerator FadeToZeroAlpha(Image fadeImage, float fadeDuration)
    {
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);

        while (fadeImage.color.a > 0.0f)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeImage.color.a - (Time.deltaTime / fadeDuration));
            yield return null;
        }

        fadeImage.enabled = false;
    }
}
