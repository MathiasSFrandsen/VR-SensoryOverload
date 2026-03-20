using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HyperFocus : MonoBehaviour
{
    [Header("Visual Focus")]
    [SerializeField] private Volume globalVolume;
    private DepthOfField dof;

    [Header("Focus Rendering")]
    [SerializeField] private string focusLayerName = "FocusObject";
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera focusCamera;

    private int focusLayer;
    private int defaultLayer;

    [Header("Raycast Settings")]
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private string tagName = "TargetObject";

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

    [Header("Audio Settings")]
    [SerializeField] private float audioTransitionSpeed = 2f;
    [SerializeField] private string focusedVolumeParam = "VolumeFocused";
    [SerializeField] private string unfocusedVolumeParam = "VolumeUnfocused";
    [SerializeField] private string lowpassParam = "UnfocusedLowpass";

    private float currentFocusedVolume;
    private float currentUnfocusedVolume;
    private float currentLowpass;

    private GameObject currentTarget;
    private AudioSource[] allAudioSources;

    private AudioMixerGroup focusedGroup;
    private AudioMixerGroup unfocusedGroup;

    void Start()
    {
        // Layers
        focusLayer = LayerMask.NameToLayer(focusLayerName);
        defaultLayer = LayerMask.NameToLayer("Water");

        // Audio setup
        currentFocusedVolume = normalFocusedVolume;
        currentUnfocusedVolume = normalUnfocusedVolume;
        currentLowpass = normalLowpass;

        allAudioSources = FindObjectsOfType<AudioSource>();

        focusedGroup = mixer.FindMatchingGroups("Focused")[0];
        unfocusedGroup = mixer.FindMatchingGroups("Unfocused")[0];

        // DOF setup (safe)
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out dof);
        }
    }

    void Update()
    {
        HandleRaycast();
        UpdateAudio();
        UpdateDOF();
    }

    void LateUpdate()
    {
        SyncCameras();
    }

    #region Audio

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

        foreach (AudioSource src in allAudioSources)
        {
            src.outputAudioMixerGroup =
                (currentTarget != null && src.gameObject == currentTarget)
                ? focusedGroup
                : unfocusedGroup;
        }
    }

    #endregion

    #region Visual DOF

    void UpdateDOF()
    {
        if (dof == null) return;

        if (currentTarget == null)
        {
            // Normal VR (ingen blur)
            dof.focalLength.value = 1f;
            dof.focusDistance.value = 1f;
        }
        else
        {
            // Fokus på target → blur baggrund
            dof.focalLength.value = 300f;
        }
    }

    void SyncCameras()
    {
        if (focusCamera != null && mainCamera != null)
        {
            focusCamera.transform.position = mainCamera.transform.position;
            focusCamera.transform.rotation = mainCamera.transform.rotation;
        }
    }

    #endregion

    #region Raycast

    void HandleRaycast()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            // Tjek kun tag, ikke layer
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

        if (currentTarget != null)
            SetLayerRecursively(currentTarget, defaultLayer);

        if (newTarget != null)
            SetLayerRecursively(newTarget, focusLayer);

        currentTarget = newTarget;
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    #endregion
}