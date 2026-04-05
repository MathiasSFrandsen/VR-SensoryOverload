using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeController : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private Image panel;

    [Header("Fade Settings")]
    [SerializeField] private float fadeInDuration = 3f;   // Fade from black to transparent
    [SerializeField] private float fadeOutDuration = 3f; // Fade from transparent to black
    [SerializeField] private float blackHoldDuration = 2f; // Hold black screen before/after fade
    [SerializeField] private float autoFadeOutAfter = 10f; // Time before fade-out starts automatically
    [SerializeField] private int nextSceneIndex = 1;

    [Header("Reference to Script")]
    [SerializeField] private HyperFocus hyperFocus;


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

        hyperFocus.enabled = true;
    }

    // Fade from transparent to black AND fade in text
    public IEnumerator FadeIn(float duration)
    {
        yield return new WaitForSeconds(autoFadeOutAfter);

        Color panelColor = panel.color;

        float elapsed = 0f;

        // Ensure panel starts transparent and text invisible
        panelColor.a = 0f;
        panel.color = panelColor;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Fade panel to black
            panelColor.a = Mathf.SmoothStep(0f, 1f, t);
            panel.color = panelColor;

            yield return null;
        }

        panelColor.a = 1f;
        panel.color = panelColor;
        yield return new WaitForSeconds(blackHoldDuration);

        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneIndex);
    }
}
