using UnityEngine;

public class ModeManager : MonoBehaviour
{
    // --- OVR Controller objects ---
    [Header("Controllers")]
    [SerializeField] private GameObject ovrControllers;

    // --- OVR Hand objects ---
    [Header("Hands")]
    [SerializeField] private GameObject ovrHands;
    [SerializeField] private GameObject leftHandGrabUseSynthetic;
    [SerializeField] private GameObject rightHandGrabUseSynthetic;
    [SerializeField] private GameObject leftHandTouchGrabSynthetic;
    [SerializeField] private GameObject rightHandTouchGrabSynthetic;
    [SerializeField] private GameObject leftHandSynthetic;
    [SerializeField] private GameObject rightHandSynthetic;

    // --- Voice ---
    [Header("Voice")]
    [SerializeField] private VoiceCommandController voiceCommandController;

    // --- Locomotion (thumbstick) ---
    [Header("Locomotion")]
    [SerializeField] private PlayerLocomotion playerLocomotion;

    private void Awake()
    {
        int modeIndex = PlayerPrefs.GetInt("InputMode", (int)SceneManagement.InputMode.Test);
        ApplyMode((SceneManagement.InputMode)modeIndex);
    }

    private void ApplyMode(SceneManagement.InputMode mode)
    {
        bool useControllers = mode == SceneManagement.InputMode.Controller || mode == SceneManagement.InputMode.Test;
        bool useHands       = mode == SceneManagement.InputMode.Hands      || mode == SceneManagement.InputMode.Test;
        bool useVoice       = mode == SceneManagement.InputMode.Voice       || mode == SceneManagement.InputMode.Test;

        SetActive(ovrControllers, useControllers);

        SetActive(ovrHands, useHands);
        SetActive(leftHandGrabUseSynthetic,   useHands);
        SetActive(rightHandGrabUseSynthetic,  useHands);
        SetActive(leftHandTouchGrabSynthetic, useHands);
        SetActive(rightHandTouchGrabSynthetic,useHands);
        SetActive(leftHandSynthetic,          useHands);
        SetActive(rightHandSynthetic,         useHands);

        if (voiceCommandController != null)
            voiceCommandController.enabled = useVoice;

        // Locomotion only active for controller and test modes
        if (playerLocomotion != null)
            playerLocomotion.enabled = useControllers && !useVoice || mode == SceneManagement.InputMode.Test;

        Debug.Log($"[ModeManager] Mode: {mode} — Controllers:{useControllers} Hands:{useHands} Voice:{useVoice}");
    }

    private static void SetActive(GameObject go, bool active)
    {
        if (go != null) go.SetActive(active);
    }
}
