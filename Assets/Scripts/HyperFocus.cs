using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class HyperFocus : MonoBehaviour
{
    [Header("Mode")]
    [SerializeField] private bool audioOnlyMode = false;
    [SerializeField] private string tagName = "TargetObject";

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
    private FocusTarget currentTargetData;

    void Start()
    {
        focusLayer = LayerMask.NameToLayer(focusLayerName);
        defaultLayer = LayerMask.NameToLayer("Water");

        currentFocusedVolume = normalFocusedVolume;
        currentUnfocusedVolume = normalUnfocusedVolume;
        currentLowpass = normalLowpass;

        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out dof);
        }

        Invoke(nameof(DisableSelf), disableAfterSeconds);
    }

    void Update()
    {
        HandleRaycast();
        UpdateAudio();

        if (!audioOnlyMode)
        {
            UpdateDOF();
        }
    }

    void LateUpdate()
    {
        if (!audioOnlyMode)
        {
            SyncCameras();
        }
    }

    #region Raycast

    void HandleRaycast()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
        {
            // 1. Must have correct tag (eligibility check)
            if (!hit.collider.CompareTag(tagName))
            {
                SetNewTarget(null, null);
                return;
            }

            // 2. Must have FocusTarget component (behavior config)
            FocusTarget target = hit.collider.GetComponentInParent<FocusTarget>();

            if (target != null)
            {
                SetNewTarget(hit.collider.gameObject, target);
                return;
            }
        }

        SetNewTarget(null, null);
    }

    void SetNewTarget(GameObject newTarget, FocusTarget data)
    {
        if (newTarget == currentTarget && data == currentTargetData)
            return;

        if (!audioOnlyMode)
        {
            if (currentTarget != null)
                SetLayerRecursively(currentTarget, defaultLayer);

            if (newTarget != null)
                SetLayerRecursively(newTarget, focusLayer);
        }

        currentTarget = newTarget;
        currentTargetData = data;
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
    }

    #endregion

    #region Visual

    void UpdateDOF()
    {
        if (dof == null) return;

        bool audioOnlyTarget = currentTargetData != null && currentTargetData.audioOnly;

        if (currentTarget == null || audioOnlyTarget)
        {
            float targetFocalLength = 1f;

            dof.focalLength.value = Mathf.Lerp(
                dof.focalLength.value,
                targetFocalLength,
                1 - Mathf.Exp(-2f * Time.deltaTime)
            );

            dof.focusDistance.value = 0.1f;
        }
        else
        {
            float targetFocalLength = 80f;

            dof.focalLength.value = Mathf.Lerp(
                dof.focalLength.value,
                targetFocalLength,
                1 - Mathf.Exp(-0.1f * Time.deltaTime)
            );

            dof.focusDistance.value = 0.1f;
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

    #region Disable

    void DisableSelf()
    {
        currentTarget = null;
        currentTargetData = null;

        currentFocusedVolume = normalFocusedVolume;
        currentUnfocusedVolume = normalUnfocusedVolume;
        currentLowpass = normalLowpass;

        mixer.SetFloat(focusedVolumeParam, currentFocusedVolume);
        mixer.SetFloat(unfocusedVolumeParam, currentUnfocusedVolume);
        mixer.SetFloat(lowpassParam, currentLowpass);

        this.enabled = false;
    }

    #endregion
}