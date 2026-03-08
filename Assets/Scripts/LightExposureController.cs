using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LightExposureController : MonoBehaviour
{
    public Volume globalVolume;
    private ColorAdjustments colorAdjustments;

    public float targetExposureUp = 3f;
    public float targetContrastUp = 30f;

    public float targetExposureDown = 0f;
    public float targetContrastDown = 0f;

    public float smoothSpeed = 2f;

    private float currentExposure;
    private float currentContrast;

    void Start()
    {
        if (globalVolume.profile.TryGet(out colorAdjustments))
        {
            currentExposure = colorAdjustments.postExposure.value;
            currentContrast = colorAdjustments.contrast.value;
        }
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        bool lookingAtLight = false;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider.CompareTag("LightTrigger"))
            {
                lookingAtLight = true;
            }
        }

        float targetExposure = lookingAtLight ? targetExposureUp : targetExposureDown;
        float targetContrast = lookingAtLight ? targetContrastUp : targetContrastDown;

        currentExposure = Mathf.Lerp(currentExposure, targetExposure, Time.deltaTime * smoothSpeed);
        currentContrast = Mathf.Lerp(currentContrast, targetContrast, Time.deltaTime * smoothSpeed);

        colorAdjustments.postExposure.value = currentExposure;
        colorAdjustments.contrast.value = currentContrast;
    }
}
