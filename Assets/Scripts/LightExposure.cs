using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LightExposure : MonoBehaviour
{
     public Volume globalVolume;
    private ColorAdjustments colorAdjustments;

    [Header("Exposure Settings")]
    public float targetExposureUp = 3f;       // Spike når man kigger på lys
    public float targetContrastUp = 30f;

    public float targetExposureDown = 0f;     // Base exposure starter her
    public float targetContrastDown = 0f;

    public float smoothSpeed = 2f;

    [Header("Gradual Overexposure")]
    public float exposureIncreaseRate = 0.05f; // Hvor hurtigt base stiger pr. sekund
    public float maxBaseExposure = 5f;         // Maks base exposure
    private float baseExposure = 0f;

    [Header("Spike Settings")]
    private float spikeExposure = 0f;          // Midlertidig spike når man kigger på lys
    public float spikeLerpSpeed = 5f;          // Hvor hurtigt spike falder tilbage

    private float currentExposure;
    private float currentContrast;

    void Start()
    {
        if (globalVolume.profile.TryGet(out colorAdjustments))
        {
            currentExposure = colorAdjustments.postExposure.value;
            currentContrast = colorAdjustments.contrast.value;

            baseExposure = currentExposure; // Start base exposure
        }
    }

    void Update()
    {
        // 1. Gradually increase baseExposure over time
        baseExposure += exposureIncreaseRate * Time.deltaTime;
        baseExposure = Mathf.Min(baseExposure, maxBaseExposure);

        // 2. Raycast for lys
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

        // 3. Spike logic
        if (lookingAtLight)
        {
            spikeExposure = targetExposureUp; // Spike når man kigger på lys
        }
        else
        {
            // Spike falder glidende til 0
            spikeExposure = Mathf.Lerp(spikeExposure, 0f, Time.deltaTime * spikeLerpSpeed);
        }

        // 4. Beregn target exposure: base + spike
        float targetExposure = targetExposureDown + baseExposure + spikeExposure;
        float targetContrast = lookingAtLight ? targetContrastUp : targetContrastDown;

        // 5. Smooth overgang
        currentExposure = Mathf.Lerp(currentExposure, targetExposure, Time.deltaTime * smoothSpeed);
        currentContrast = Mathf.Lerp(currentContrast, targetContrast, Time.deltaTime * smoothSpeed);

        // 6. Apply til volume
        colorAdjustments.postExposure.value = currentExposure;
        colorAdjustments.contrast.value = currentContrast;
    }
}
