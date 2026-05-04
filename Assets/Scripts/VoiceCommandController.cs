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
    [SerializeField] private OvenManager oven;
    [SerializeField] private PizzaAssemblyManager pizzaAssembly;
    [SerializeField] private BurgerAssemblyManager burgerAssembly;
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

        if (Contains(transcription, "pour coffee"))
        {
            coffeeMaker.PourCoffeeIntoLastCup();
            ShowFeedback("Pouring coffee");
        }
        else if (Contains(transcription, "grab cup") || Contains(transcription, "get cup"))
        {
            coffeeMaker.SpawnCup();
            ShowFeedback("Grabbing cup");
        }
        else if (Contains(transcription, "coffee"))
        {
            coffeeMaker.TurnOnCoffeeMaker();
            ShowFeedback("Starting coffee machine");
        }
        else if (Contains(transcription, "add patty") || Contains(transcription, "patty done") || Contains(transcription, "take patty"))
        {
            if (grill.TransferPattyToBurger(burgerAssembly))
                ShowFeedback("Adding patty to burger");
            else
                ShowFeedback("No patty on grill");
        }
        else if (Contains(transcription, "place patty") || Contains(transcription, "put patty"))
        {
            grill.PlacePattyOnGrill();
            ShowFeedback("Placing patty on grill");
        }
        else if (Contains(transcription, "grill"))
        {
            grill.StartCooking();
            ShowFeedback("Starting grill");
        }
        else if (Contains(transcription, "bottom bun") || Contains(transcription, "place bun") || Contains(transcription, "start burger"))
        {
            burgerAssembly.PlaceBottomBun();
            ShowFeedback("Placing bottom bun");
        }
        else if (Contains(transcription, "top bun") || Contains(transcription, "finish burger"))
        {
            burgerAssembly.AddTopBun();
            ShowFeedback("Adding top bun");
        }
        else if (Contains(transcription, "cheese"))
        {
            burgerAssembly.AddCheese();
            ShowFeedback("Adding cheese");
        }
        else if (Contains(transcription, "tomato"))
        {
            burgerAssembly.AddTomato();
            ShowFeedback("Adding tomato");
        }
        else if (Contains(transcription, "onion"))
        {
            burgerAssembly.AddOnion();
            ShowFeedback("Adding onion");
        }
        else if (Contains(transcription, "lettuce") || Contains(transcription, "salad"))
        {
            burgerAssembly.AddSalad();
            ShowFeedback("Adding lettuce");
        }
        else if (Contains(transcription, "bacon"))
        {
            burgerAssembly.AddBacon();
            ShowFeedback("Adding bacon");
        }
        else if (Contains(transcription, "bake pizza") || Contains(transcription, "put pizza in oven") || Contains(transcription, "pizza in oven"))
        {
            GameObject pizza = pizzaAssembly.HandOffActivePizza();
            if (pizza != null)
            {
                oven.LoadPizza(pizza);
                ShowFeedback("Putting pizza in oven");
            }
            else
            {
                ShowFeedback("No pizza to put in oven");
            }
        }
        else if (Contains(transcription, "oven"))
        {
            oven.StartCooking();
            ShowFeedback("Starting oven");
        }
        else if (Contains(transcription, "place pizza") || Contains(transcription, "set pizza"))
        {
            pizzaAssembly.PlacePizza();
            ShowFeedback("Placing pizza");
        }
        else if (Contains(transcription, "pepperoni"))
        {
            pizzaAssembly.AddPepperoni();
            ShowFeedback("Adding pepperoni");
        }
        else if (Contains(transcription, "mushroom"))
        {
            pizzaAssembly.AddMushroom();
            ShowFeedback("Adding mushrooms");
        }
        else if (Contains(transcription, "olive"))
        {
            pizzaAssembly.AddOlive();
            ShowFeedback("Adding olives");
        }
        else if (Contains(transcription, "pepper"))
        {
            pizzaAssembly.AddPepper();
            ShowFeedback("Adding peppers");
        }
        else if (Contains(transcription, "basil"))
        {
            pizzaAssembly.AddBasil();
            ShowFeedback("Adding basil");
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
