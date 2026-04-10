using UnityEngine;
using UnityEngine.Audio;

public class HyperFocusAudioOnly : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private string tagName = "TargetObjectAudio";

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mixer;

    [Header("Normal State")]
    [SerializeField] private float normalFocusedVolume = 0f;
    [SerializeField] private float normalUnfocusedVolume = 0f;
    [SerializeField] private float normalLowpass = 22000f;

    [Header("Focus State")]
    [SerializeField] private float focusFocusedVolume = 8f;
    [SerializeField] private float focusUnfocusedVolume = -20f;
    [SerializeField] private float focusLowpass = 800f;

    [Header("Audio Settings")]
    [SerializeField] private float audioTransitionSpeed = 2f;
    [SerializeField] private string focusedVolumeParam = "VolumeFocused";
    [SerializeField] private string unfocusedVolumeParam = "VolumeUnfocused";
    [SerializeField] private string lowpassParam = "UnfocusedLowpass";

    [SerializeField] private float disableAfterSeconds = 30f;

    private float currentFocusedVolume;
    private float currentUnfocusedVolume;
    private float currentLowpass;

    private GameObject currentTarget;
    private AudioSource targetAudio;

    private AudioMixerGroup focusedGroup;
    private AudioMixerGroup unfocusedGroup;

    void Start()
    {
        currentFocusedVolume = normalFocusedVolume;
        currentUnfocusedVolume = normalUnfocusedVolume;
        currentLowpass = normalLowpass;

        focusedGroup = mixer.FindMatchingGroups("Focused")[0];
        unfocusedGroup = mixer.FindMatchingGroups("Unfocused")[0];

        Invoke(nameof(DisableSelf), disableAfterSeconds);
    }

    void Update()
    {
        HandleRaycast();
        UpdateAudio();
    }

    void HandleRaycast()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.CompareTag(tagName))
            {
                SetNewTarget(hit.collider.gameObject);
                return;
            }
        }

        SetNewTarget(null);
    }

    void SetNewTarget(GameObject newTarget)
    {
        if (newTarget == currentTarget) return;

        currentTarget = newTarget;

        if (currentTarget != null)
        {
            targetAudio = currentTarget.GetComponent<AudioSource>();
        }
        else
        {
            targetAudio = null;
        }
    }

    void UpdateAudio()
    {
        bool isFocusing = currentTarget != null;

        float targetFocused = isFocusing ? focusFocusedVolume : normalFocusedVolume;
        float targetUnfocused = isFocusing ? focusUnfocusedVolume : normalUnfocusedVolume;
        float targetLowpass = isFocusing ? focusLowpass : normalLowpass;

        currentFocusedVolume = Mathf.Lerp(currentFocusedVolume, targetFocused, Time.deltaTime * audioTransitionSpeed);
        currentUnfocusedVolume = Mathf.Lerp(currentUnfocusedVolume, targetUnfocused, Time.deltaTime * audioTransitionSpeed);
        currentLowpass = Mathf.Lerp(currentLowpass, targetLowpass, Time.deltaTime * audioTransitionSpeed);

        mixer.SetFloat(focusedVolumeParam, currentFocusedVolume);
        mixer.SetFloat(unfocusedVolumeParam, currentUnfocusedVolume);
        mixer.SetFloat(lowpassParam, currentLowpass);

        // ONLY affect the target object
        if (targetAudio != null)
        {
            targetAudio.outputAudioMixerGroup = focusedGroup;
        }
    }

    void DisableSelf()
    {
        currentTarget = null;
        targetAudio = null;

        currentFocusedVolume = normalFocusedVolume;
        currentUnfocusedVolume = normalUnfocusedVolume;
        currentLowpass = normalLowpass;

        mixer.SetFloat(focusedVolumeParam, currentFocusedVolume);
        mixer.SetFloat(unfocusedVolumeParam, currentUnfocusedVolume);
        mixer.SetFloat(lowpassParam, currentLowpass);

        this.enabled = false;
    }
}
