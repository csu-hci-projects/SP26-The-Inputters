using Meta.WitAi;
using Meta.WitAi.Json;
using Oculus.Voice;
using UnityEngine;

public class OvenVoiceController : MonoBehaviour
{
    [SerializeField] private AppVoiceExperience voiceExperience;
    [SerializeField] private OvenManager oven;

    private void OnEnable()
    {
        voiceExperience.VoiceEvents.OnResponse.AddListener(OnResponse);
        voiceExperience.VoiceEvents.OnError.AddListener(OnError);
        voiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnTranscription);
    }

    private void OnDisable()
    {
        voiceExperience.VoiceEvents.OnResponse.RemoveListener(OnResponse);
        voiceExperience.VoiceEvents.OnError.RemoveListener(OnError);
        voiceExperience.VoiceEvents.OnFullTranscription.RemoveListener(OnTranscription);
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            Debug.Log("[Voice] A button pressed — activating voice.");
            StartListening();
        }
    }

    public void StartListening()
    {
        voiceExperience.Activate();
        Debug.Log("[Voice] Activate() called.");
    }

    private void OnTranscription(string transcription)
    {
        Debug.Log($"[Voice] Heard: {transcription}");

        if (transcription.IndexOf("oven", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
            transcription.IndexOf("pizza", System.StringComparison.OrdinalIgnoreCase) >= 0)
        {
            oven.ToggleOven();
            Debug.Log("[Voice] Oven triggered via transcription.");
        }
    }

    private void OnError(string error, string message)
    {
        Debug.LogError($"[Voice] Error: {error} — {message}");
    }

    private void OnResponse(WitResponseNode response)
    {
        Debug.Log($"[Voice] Raw response: {response}");
    }
}
