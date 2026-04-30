using System.Collections;
using Meta.WitAi;
using Meta.WitAi.Json;
using Oculus.Voice;
using TMPro;
using UnityEngine;

public class VoiceCommandController : MonoBehaviour
{
    [SerializeField] private AppVoiceExperience voiceExperience;
    [SerializeField] private CoffeeMakerManager coffeeMaker;
    [SerializeField] private GrillManager grill;
    [SerializeField] private TextMeshProUGUI feedbackText;

    private Coroutine _feedbackCoroutine;

    private void OnEnable()
    {
        voiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnTranscription);
        voiceExperience.VoiceEvents.OnError.AddListener(OnError);
        voiceExperience.VoiceEvents.OnResponse.AddListener(OnResponse);
    }

    private void OnDisable()
    {
        voiceExperience.VoiceEvents.OnFullTranscription.RemoveListener(OnTranscription);
        voiceExperience.VoiceEvents.OnError.RemoveListener(OnError);
        voiceExperience.VoiceEvents.OnResponse.RemoveListener(OnResponse);
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            voiceExperience.Activate();
            Debug.Log("[Voice] Listening...");
        }
    }

    private void OnTranscription(string transcription)
    {
        Debug.Log($"[Voice] Heard: {transcription}");

        if (Contains(transcription, "coffee"))
        {
            coffeeMaker.TurnOnCoffeeMaker();
            ShowFeedback("Starting coffee machine");
        }
        else if (Contains(transcription, "grill"))
        {
            grill.StartCooking();
            ShowFeedback("Starting grill");
        }
    }

    private void ShowFeedback(string message)
    {
        if (feedbackText == null) return;

        if (_feedbackCoroutine != null)
            StopCoroutine(_feedbackCoroutine);

        _feedbackCoroutine = StartCoroutine(DisplayFeedback(message));
    }

    private IEnumerator DisplayFeedback(string message)
    {
        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        feedbackText.gameObject.SetActive(false);
    }

    private static bool Contains(string text, string keyword)
        => text.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0;

    private void OnError(string error, string message)
        => Debug.LogError($"[Voice] Error: {error} — {message}");

    private void OnResponse(WitResponseNode response)
        => Debug.Log($"[Voice] Response: {response}");
}
