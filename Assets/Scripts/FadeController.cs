using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeController : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private Image panel;
    [SerializeField] private TMP_Text text; // Text to fade in

    [Header("Fade Settings")]
    [SerializeField] private float fadeInDuration = 3f;   // Fade from black to transparent
    [SerializeField] private float fadeOutDuration = 10f; // Fade from transparent to black
    [SerializeField] private float blackHoldDuration = 2f; // Hold black screen before/after fade
    [SerializeField] private float autoFadeOutAfter = 60f; // Time before fade-out starts automatically


    private void Awake()
    {
        // Start scene with fade in
        StartCoroutine(FadeOut());
        // Start automatic fade out after delay
        StartCoroutine(FadeIn(fadeOutDuration));
    }

    // Fade from black to transparent (scene start)
    private IEnumerator FadeOut()
    {
        Color color = panel.color;
        color.a = 1f; // Start fully black
        panel.color = color;

        yield return new WaitForSeconds(blackHoldDuration);

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeInDuration);
            color.a = Mathf.SmoothStep(1f, 0f, t);
            panel.color = color;
            yield return null;
        }

        color.a = 0f;
        panel.color = color;
    }

    // Fade from transparent to black AND fade in text
    public IEnumerator FadeIn(float duration)
    {
        yield return new WaitForSeconds(autoFadeOutAfter);

        Color panelColor = panel.color;
        Color textColor = text.color;

        float elapsed = 0f;

        // Ensure panel starts transparent and text invisible
        panelColor.a = 0f;
        textColor.a = 0f;
        panel.color = panelColor;
        text.color = textColor;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Fade panel to black
            panelColor.a = Mathf.SmoothStep(0f, 1f, t);
            panel.color = panelColor;

            // Fade in text at the same speed
            textColor.a = Mathf.SmoothStep(0f, 1f, t);
            text.color = textColor;

            yield return null;
        }

        panelColor.a = 1f;
        textColor.a = 1f;
        panel.color = panelColor;
        text.color = textColor;

        yield return new WaitForSeconds(blackHoldDuration);
    }
}
