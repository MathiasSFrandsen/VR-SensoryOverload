using UnityEngine;
using System.Collections;

public class Flicker : MonoBehaviour
{
    [Header("Light & Renderer")]
    public Light[] tubeLights; // Flere lys
    public MeshRenderer tubeRenderer;
    public Material litMaterial;
    public Material unlitMaterial;

    [Header("Flicker Settings")]
    public float minOnTime = 0.05f;
    public float maxOnTime = 0.2f;
    public float minOffTime = 0.02f;
    public float maxOffTime = 0.1f;

    private void Start()
    {
        if (tubeLights == null || tubeLights.Length == 0)
            tubeLights = GetComponentsInChildren<Light>();

        if (tubeRenderer == null)
            tubeRenderer = GetComponent<MeshRenderer>();

        StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // Tænd alle lys + skift materiale
            SetLights(true);
            tubeRenderer.material = litMaterial;
            float onTime = Random.Range(minOnTime, maxOnTime);
            yield return new WaitForSeconds(onTime);

            // Sluk alle lys + skift materiale
            SetLights(false);
            tubeRenderer.material = unlitMaterial;
            float offTime = Random.Range(minOffTime, maxOffTime);
            yield return new WaitForSeconds(offTime);
        }
    }

    private void SetLights(bool state)
    {
        foreach (Light l in tubeLights)
        {
            l.enabled = state;
        }
    }
}
