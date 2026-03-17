using UnityEngine;
using UnityEngine.Audio;

public class HyperFocus : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float maxDistance = 10f;
    public LayerMask interactLayer;

    [Header("Audio Mixer")]
    public AudioMixer mixer;

    // Smooth values
    private float focusedVol = 0f;
    private float unfocusedVol = 0f;
    private float lowpass = 22000f;

    private GameObject currentTarget;
    [SerializeField] private AudioSource[] allAudioSources;

    void Start()
    {
        // Find alle lydkilder på scenen
        allAudioSources = FindObjectsOfType<AudioSource>();
    }

    void Update()
    {
        // Raycast frem fra kamera
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, interactLayer))
        {
            // Hvis objektet har AudioSource
            if (hit.collider.gameObject != currentTarget && hit.collider.GetComponent<AudioSource>() != null)
            {
                currentTarget = hit.collider.gameObject;
                Debug.Log("Hello");
            }
        }
        else
        {
            currentTarget = null;
        }

        // Target values
        float targetFocused = currentTarget != null ? 0f : 0f;      // dB for focused lyd
        float targetUnfocused = currentTarget != null ? -20f : 0f;  // dB for unfocused
        float targetLowpass = currentTarget != null ? 800f : 22000f;

        // Smooth transitions
        focusedVol = Mathf.Lerp(focusedVol, targetFocused, Time.deltaTime * 3f);
        unfocusedVol = Mathf.Lerp(unfocusedVol, targetUnfocused, Time.deltaTime * 3f);
        lowpass = Mathf.Lerp(lowpass, targetLowpass, Time.deltaTime * 3f);

        // Sæt mixer parametre
        mixer.SetFloat("VolumeFocused", focusedVol);
        mixer.SetFloat("VolumeUnfocused", unfocusedVol);
        mixer.SetFloat("UnfocusedLowpass", lowpass);

        // Opdater AudioSource output (alternativ metode)
        foreach (AudioSource src in allAudioSources)
        {
            if (currentTarget != null && src.gameObject == currentTarget)
            {
                // Sørg for den går til Focused gruppen
                src.outputAudioMixerGroup = mixer.FindMatchingGroups("Focused")[0];
            }
            else
            {
                // Alle andre til Unfocused
                src.outputAudioMixerGroup = mixer.FindMatchingGroups("Unfocused")[0];
            }
        }
    }
}
