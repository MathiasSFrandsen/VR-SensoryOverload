using UnityEngine;
using UnityEngine.Audio;

public class HyperFocus : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask interactLayer;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mixer;

    [Header("Normal State (No Focus)")]
    [SerializeField] private float normalFocusedVolume = 0f;
    [SerializeField] private float normalUnfocusedVolume = 0f;
    [SerializeField] private float normalLowpass = 22000f;

    [Header("Focus State")]
    [SerializeField] private float focusFocusedVolume = 8f;
    [SerializeField] private float focusUnfocusedVolume = -20f;
    [SerializeField] private float focusLowpass = 800f;

    [Header("Transition Settings")]
    [SerializeField] private float transitionSpeed = 1f;

    [Header("Audio Mixer Parameters")]
    [SerializeField] private string focusedVolumeParam = "VolumeFocused";
    [SerializeField] private string unfocusedVolumeParam = "VolumeUnfocused";
    [SerializeField] private string lowpassParam = "UnfocusedLowpass";

    // Current (smoothed runtime values)
    private float currentFocusedVolume;
    private float currentUnfocusedVolume;
    private float currentLowpass;

    private GameObject currentTarget;
    private AudioSource[] allAudioSources;

    private AudioMixerGroup focusedGroup;
    private AudioMixerGroup unfocusedGroup;

    void Start()
    {
        // Initialize current values to normal state
        currentFocusedVolume = normalFocusedVolume;
        currentUnfocusedVolume = normalUnfocusedVolume;
        currentLowpass = normalLowpass;

        // Find all audio sources
        allAudioSources = FindObjectsOfType<AudioSource>();

        // Cache mixer groups (avoid doing this every frame!)
        focusedGroup = mixer.FindMatchingGroups("Focused")[0];
        unfocusedGroup = mixer.FindMatchingGroups("Unfocused")[0];
    }

    void Update()
    {
        HandleRaycast();
        UpdateAudioTargets();
        ApplyAudio();
        UpdateAudioSourceGroups();
    }

    void HandleRaycast()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, interactLayer))
        {
            if (hit.collider.GetComponent<AudioSource>() != null)
            {
                currentTarget = hit.collider.gameObject;
                return;
            }
        }

        currentTarget = null;
    }

    void UpdateAudioTargets()
    {
        bool isFocusing = currentTarget != null;

        float targetFocused = isFocusing ? focusFocusedVolume : normalFocusedVolume;
        float targetUnfocused = isFocusing ? focusUnfocusedVolume : normalUnfocusedVolume;
        float targetLowpass = isFocusing ? focusLowpass : normalLowpass;

        currentFocusedVolume = Mathf.Lerp(currentFocusedVolume, targetFocused, Time.deltaTime * transitionSpeed);
        currentUnfocusedVolume = Mathf.Lerp(currentUnfocusedVolume, targetUnfocused, Time.deltaTime * transitionSpeed);
        currentLowpass = Mathf.Lerp(currentLowpass, targetLowpass, Time.deltaTime * transitionSpeed);
    }

    void ApplyAudio()
    {
        mixer.SetFloat(focusedVolumeParam, currentFocusedVolume);
        mixer.SetFloat(unfocusedVolumeParam, currentUnfocusedVolume);
        mixer.SetFloat(lowpassParam, currentLowpass);
    }

    void UpdateAudioSourceGroups()
    {
        foreach (AudioSource src in allAudioSources)
        {
            if (currentTarget != null && src.gameObject == currentTarget)
            {
                src.outputAudioMixerGroup = focusedGroup;
            }
            else
            {
                src.outputAudioMixerGroup = unfocusedGroup;
            }
        }
    }
}
