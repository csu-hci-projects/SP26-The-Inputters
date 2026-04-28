using Meta.WitAi;
using Meta.WitAi.Json;
using Oculus.Voice;
using UnityEngine;

public class CoffeeVoiceController : MonoBehaviour
{
    [SerializeField] private AppVoiceExperience voiceExperience;
    [SerializeField] private CoffeeMakerManager coffeeMaker;

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
    }

    private void OnError(string error, string message)
    {
        Debug.LogError($"[Voice] Error: {error} — {message}");
    }

    private void OnResponse(WitResponseNode response)
    {
        Debug.Log($"[Voice] Raw response: {response}");
        string intent = response.GetIntentName();
        Debug.Log($"[Voice] Response received. Intent: '{intent}'");
        if (string.Equals(intent, "start_coffee", System.StringComparison.OrdinalIgnoreCase))
        {
            coffeeMaker.TurnOnCoffeeMaker();
            Debug.Log("[Voice] Coffee maker toggled via voice command.");
        }
    }
}
