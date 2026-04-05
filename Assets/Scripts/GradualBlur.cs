using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Brug HDRP hvis du er i HDRP
using System.Collections;

public class GradualBlur : MonoBehaviour
{
    public Volume globalVolume;           // Global volume med DOF override
    public float targetFocalLength = 300f; // Den endelige fokallængde
    public float smoothSpeed = 1f;        // Hvor hurtigt focal length stiger
     public float delayBeforeBlur = 20f;     // Seconds to wait before starting blur

    private DepthOfField dof;
    private float currentFocalLength;
    public bool shouldBlur = false;

    void Start()
    {
        if (globalVolume.profile.TryGet(out dof))
        {
            currentFocalLength = dof.focalLength.value; // Startværdi
        }
        else
        {
            Debug.LogWarning("DepthOfField override not found in volume!");
        }
        // Start coroutine to enable blur after delay
        StartCoroutine(StartBlurAfterDelay());
    }

    void Update()
    {
        if (!shouldBlur || dof == null) return;

        // Lerp focal length gradvist mod target
        currentFocalLength = Mathf.Lerp(currentFocalLength, targetFocalLength, Time.deltaTime * smoothSpeed);

        // Hvis tæt på target, snap til target
        if (Mathf.Abs(currentFocalLength - targetFocalLength) < 0.01f)
            currentFocalLength = targetFocalLength;

        dof.focalLength.value = currentFocalLength;
    }

    IEnumerator StartBlurAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeBlur);
        shouldBlur = true;
    }
}
