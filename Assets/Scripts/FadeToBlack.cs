using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    [SerializeField] public Image panel;
    [SerializeField] public float fadeDuration = 10f;
    [SerializeField] public float blackHoldDuration = 2f;
    [SerializeField] public float startFadeAfter = 60f; // Time before fade begins


    private void Awake()
    {
        StartCoroutine(AutoStartFade());
    }

    private IEnumerator AutoStartFade()
    {
        // Wait until the experience ends
        yield return new WaitForSeconds(startFadeAfter);

        // Begin fade to black
        yield return StartCoroutine(FadeToBlackRoutine());
    }

    private IEnumerator FadeToBlackRoutine()
    {
        Color color = panel.color;
        float elapsed = 0f;

        // Smoothly fade from transparent to black
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            color.a = Mathf.SmoothStep(0f, 1f, t);
            panel.color = color;
            yield return null;
        }

        // Ensure it?s fully black
        color.a = 1f;
        panel.color = color;

        // Hold the black screen briefly
        yield return new WaitForSeconds(blackHoldDuration);

    }
}
