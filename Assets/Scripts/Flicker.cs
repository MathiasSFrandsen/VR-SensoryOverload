using UnityEngine;
using System.Collections;

public class Flicker : MonoBehaviour
{
    [Header("Light & Renderer")]
    public Light tubeLight;
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
        if (tubeLight == null)
            tubeLight = GetComponentInChildren<Light>();

        if (tubeRenderer == null)
            tubeRenderer = GetComponent<MeshRenderer>();

        StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // Lyset tænder + materialeskift
            tubeLight.enabled = true;
            tubeRenderer.material = litMaterial;
            float onTime = Random.Range(minOnTime, maxOnTime);
            yield return new WaitForSeconds(onTime);

            // Lyset slukker + materialeskift
            tubeLight.enabled = false;
            tubeRenderer.material = unlitMaterial;
            float offTime = Random.Range(minOffTime, maxOffTime);
            yield return new WaitForSeconds(offTime);
        }
    }
}
