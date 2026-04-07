using UnityEngine;

public class LightBrightness : MonoBehaviour
{
    public Light targetLight;        // Lyset der skal ændres
    public float targetIntensity = 5f; // Hvor stærkt lyset skal blive
    public float speed = 1f;         // Hvor hurtigt det går

    void Update()
    {
        if (targetLight == null) return;

        targetLight.intensity = Mathf.Lerp(
            targetLight.intensity,
            targetIntensity,
            Time.deltaTime * speed
        );
    }
}
