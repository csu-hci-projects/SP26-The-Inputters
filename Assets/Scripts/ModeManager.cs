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

    // --- Machine buttons (disabled in Hands+Voice so voice handles them) ---
    [Header("Machine Buttons")]
    [SerializeField] private GrillButton grillButton;
    [SerializeField] private OvenButton ovenButton;

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
        bool useControllers = mode == SceneManagement.InputMode.Controller
                           || mode == SceneManagement.InputMode.ControllersAndVoice
                           || mode == SceneManagement.InputMode.Test;

        bool useHands       = mode == SceneManagement.InputMode.Hands
                           || mode == SceneManagement.InputMode.HandsAndVoice
                           || mode == SceneManagement.InputMode.Test;

        bool useVoice       = mode == SceneManagement.InputMode.Voice
                           || mode == SceneManagement.InputMode.ControllersAndVoice
                           || mode == SceneManagement.InputMode.HandsAndVoice
                           || mode == SceneManagement.InputMode.Test;

        // --- Input devices ---
        SetActive(ovrControllers, useControllers);

        SetActive(ovrHands, useHands);
        SetActive(leftHandGrabUseSynthetic,    useHands);
        SetActive(rightHandGrabUseSynthetic,   useHands);
        SetActive(leftHandTouchGrabSynthetic,  useHands);
        SetActive(rightHandTouchGrabSynthetic, useHands);
        SetActive(leftHandSynthetic,           useHands);
        SetActive(rightHandSynthetic,          useHands);

        // --- Voice command scope ---
        if (voiceCommandController != null)
        {
            voiceCommandController.enabled = useVoice;
            if (useVoice)
            {
                voiceCommandController.commandMode = mode switch
                {
                    SceneManagement.InputMode.ControllersAndVoice => VoiceCommandController.VoiceCommandMode.ServeOnly,
                    SceneManagement.InputMode.HandsAndVoice       => VoiceCommandController.VoiceCommandMode.MacroAndServe,
                    _                                             => VoiceCommandController.VoiceCommandMode.Full
                };
                voiceCommandController.ShowIntroHelp();
            }
        }

        // --- Physical serving ---
        // Disabled for multimodal modes — voice is the only serve path there
        CustomerTrayManager.PhysicalServingEnabled =
            mode == SceneManagement.InputMode.Controller ||
            mode == SceneManagement.InputMode.Hands      ||
            mode == SceneManagement.InputMode.Test;

        // --- Machine buttons ---
        // Disabled for Hands+Voice — voice handles grill/oven triggers in that mode
        bool useMachineButtons = mode != SceneManagement.InputMode.HandsAndVoice;
        if (grillButton != null) grillButton.enabled = useMachineButtons;
        if (ovenButton  != null) ovenButton.enabled  = useMachineButtons;

        // --- Locomotion ---
        bool useLocomotion = mode == SceneManagement.InputMode.Controller
                          || mode == SceneManagement.InputMode.ControllersAndVoice
                          || mode == SceneManagement.InputMode.Test;
        if (playerLocomotion != null)
            playerLocomotion.enabled = useLocomotion;

        Debug.Log($"[ModeManager] Mode:{mode} Controllers:{useControllers} Hands:{useHands} Voice:{useVoice} " +
                  $"PhysicalServe:{CustomerTrayManager.PhysicalServingEnabled} MachineButtons:{useMachineButtons} Locomotion:{useLocomotion}");
    }

    private static void SetActive(GameObject go, bool active)
    {
        if (go != null) go.SetActive(active);
    }
}
