using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VoiceCommandController : MonoBehaviour
{
    // Full     = all commands (Voice-only, Test)
    // MacroAndServe = machine triggers + serve only (Hands+Voice)
    // ServeOnly     = serve commands only (Controllers+Voice)
    public enum VoiceCommandMode { Full, MacroAndServe, ServeOnly }
    [HideInInspector] public VoiceCommandMode commandMode = VoiceCommandMode.Full;

    [SerializeField] private string openAIApiKey = "";
    [SerializeField] private CoffeeMakerManager coffeeMaker;
    [SerializeField] private GrillManager grill;
    [SerializeField] private OvenManager oven;
    [SerializeField] private PizzaAssemblyManager pizzaAssembly;
    [SerializeField] private BurgerAssemblyManager burgerAssembly;
    [SerializeField] private ServingStationManager servingStation;
    [SerializeField] private FinishButtonScript finishButton;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Help Panel")]
    [SerializeField] private GameObject helpPanel;
    [SerializeField] private TextMeshProUGUI helpPanelText;
    private Coroutine _helpCoroutine;
    private bool _helpVisible;

    private AudioClip _recording;
    private bool _isRecording;
    private Coroutine _feedbackCoroutine;
    private const int SampleRate = 16000;

    private void Awake()
    {
        ForceOverlayTMP(feedbackText);
        if (helpPanel != null)
        {
            ForceOverlayTMP(helpPanelText);
            foreach (Image img in helpPanel.GetComponentsInChildren<Image>(true))
                ForceOverlayImage(img);
        }
    }

    private static void ForceOverlayTMP(TextMeshProUGUI tmp)
    {
        if (tmp == null) return;
        Material m = new Material(tmp.fontSharedMaterial);
        m.SetInt("unity_GUIZTestMode", (int)CompareFunction.Always);
        tmp.fontMaterial = m;
    }

    private static void ForceOverlayImage(Image img)
    {
        if (img == null) return;
        Material m = new Material(img.material);
        m.SetInt("unity_GUIZTestMode", (int)CompareFunction.Always);
        img.material = m;
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One) && !_isRecording)
        {
            StartRecording();
        }
        else if (OVRInput.GetUp(OVRInput.Button.One) && _isRecording)
        {
            StopAndTranscribe();
        }
    }

    private void StartRecording()
    {
        _recording = Microphone.Start(null, false, 10, SampleRate);
        _isRecording = true;
        Debug.Log("[Voice] Recording...");
        ShowFeedback("Listening...");
    }

    private void StopAndTranscribe()
    {
        int position = Microphone.GetPosition(null);
        Microphone.End(null);
        _isRecording = false;

        if (position <= 0)
        {
            Debug.Log("[Voice] No audio captured.");
            return;
        }

        float[] data = new float[position * _recording.channels];
        _recording.GetData(data, 0);
        AudioClip trimmed = AudioClip.Create("recording", position, _recording.channels, SampleRate, false);
        trimmed.SetData(data, 0);

        StartCoroutine(SendToWhisper(trimmed));
    }

    private IEnumerator SendToWhisper(AudioClip clip)
    {
        byte[] wav = EncodeWav(clip);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", wav, "audio.wav", "audio/wav");
        form.AddField("model", "whisper-1");

        using UnityWebRequest request = UnityWebRequest.Post("https://api.openai.com/v1/audio/transcriptions", form);
        request.SetRequestHeader("Authorization", $"Bearer {openAIApiKey}");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[Voice] Whisper error: {request.error} — {request.downloadHandler.text}");
            ShowFeedback("Voice error");
            yield break;
        }

        string transcription = ParseWhisperText(request.downloadHandler.text);
        Debug.Log($"[Voice] Heard: {transcription}");
        OnTranscription(transcription);
    }

    private static string ParseWhisperText(string json)
    {
        int start = json.IndexOf("\"text\"");
        if (start < 0) return "";
        start = json.IndexOf('"', start + 6) + 1;
        int end = json.IndexOf('"', start);
        return end < 0 ? "" : json.Substring(start, end - start);
    }

    private static byte[] EncodeWav(AudioClip clip)
    {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        short[] intData = new short[samples.Length];
        for (int i = 0; i < samples.Length; i++)
            intData[i] = (short)(Mathf.Clamp(samples[i], -1f, 1f) * 32767);

        using MemoryStream stream = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(stream);

        int dataSize = intData.Length * 2;
        writer.Write(new[] { 'R', 'I', 'F', 'F' });
        writer.Write(36 + dataSize);
        writer.Write(new[] { 'W', 'A', 'V', 'E' });
        writer.Write(new[] { 'f', 'm', 't', ' ' });
        writer.Write(16);
        writer.Write((short)1);
        writer.Write((short)clip.channels);
        writer.Write(clip.frequency);
        writer.Write(clip.frequency * clip.channels * 2);
        writer.Write((short)(clip.channels * 2));
        writer.Write((short)16);
        writer.Write(new[] { 'd', 'a', 't', 'a' });
        writer.Write(dataSize);
        foreach (short s in intData)
            writer.Write(s);

        return stream.ToArray();
    }

    private void HandleServeCommand(string food, int customerIndex)
    {
        if (servingStation == null) { ShowFeedback("No serving station assigned"); return; }

        CustomerManager customer = servingStation.GetCustomer(customerIndex);
        if (customer == null)
        {
            ShowFeedback($"No customer {customerIndex + 1}");
            return;
        }

        string wanted = customer.GetFoodType();
        if (!string.Equals(wanted, food, System.StringComparison.OrdinalIgnoreCase))
        {
            ShowFeedback($"Customer {customerIndex + 1} wants {wanted}");
            return;
        }

        bool success = false;
        switch (food)
        {
            case "Coffee":
                success = coffeeMaker.TakeFilledCup();
                break;
            case "Burger":
                GameObject burger = burgerAssembly.TakeCompletedBurger();
                if (burger != null) { Destroy(burger); success = true; }
                break;
            case "Pizza":
                GameObject pizza = oven.TakeCookedPizza();
                if (pizza != null) { Destroy(pizza); success = true; }
                break;
        }

        if (success)
        {
            servingStation.ServeCustomer(customerIndex);
            ShowFeedback($"Served {food} to customer {customerIndex + 1}!");
        }
        else
        {
            ShowFeedback($"{food} not ready");
        }
    }

    private void OnTranscription(string transcription)
    {
        bool canMachine  = commandMode == VoiceCommandMode.Full || commandMode == VoiceCommandMode.MacroAndServe;
        bool canAssemble = commandMode == VoiceCommandMode.Full;

        // --- Close help panel ---
        if (_helpVisible && Contains(transcription, "close"))
        {
            CloseHelp();
            return;
        }

        // --- Finish (Voice-only / Full mode) ---
        if (commandMode == VoiceCommandMode.Full &&
            (Contains(transcription, "finish") || Contains(transcription, "end session") || Contains(transcription, "i'm done") || Contains(transcription, "im done")))
        {
            ShowFeedback("Ending session...");
            if (finishButton != null)
                finishButton.ExitApplication();
            else
                SceneManager.LoadScene("StartScene");
            return;
        }

        // --- Help (available in all voice modes) ---
        if (Contains(transcription, "help"))
        {
            if (Contains(transcription, "coffee"))      { ShowHelp("coffee"); return; }
            if (Contains(transcription, "burger"))      { ShowHelp("burger"); return; }
            if (Contains(transcription, "pizza"))       { ShowHelp("pizza");  return; }
        }

        // --- Serve (always available when voice is active) ---
        if (Contains(transcription, "serve") && Contains(transcription, "customer"))
        {
            string food = "";
            if (Contains(transcription, "coffee"))      food = "Coffee";
            else if (Contains(transcription, "burger")) food = "Burger";
            else if (Contains(transcription, "pizza"))  food = "Pizza";

            int customerNum = -1;
            if (Contains(transcription, " 1") || Contains(transcription, "one"))        customerNum = 1;
            else if (Contains(transcription, " 2") || Contains(transcription, "two"))   customerNum = 2;
            else if (Contains(transcription, " 3") || Contains(transcription, "three")) customerNum = 3;

            if (food != "" && customerNum > 0)
            {
                HandleServeCommand(food, customerNum - 1);
                return;
            }
        }

        // --- Machine triggers (Full + MacroAndServe) ---
        if (canMachine && Contains(transcription, "pour coffee"))
        {
            coffeeMaker.PourCoffeeIntoLastCup();
            ShowFeedback("Pouring coffee");
        }
        else if (canMachine && (Contains(transcription, "grab cup") || Contains(transcription, "get cup")))
        {
            coffeeMaker.SpawnCup();
            ShowFeedback("Grabbing cup");
        }
        else if (canMachine && Contains(transcription, "coffee"))
        {
            coffeeMaker.TurnOnCoffeeMaker();
            ShowFeedback("Starting coffee machine");
        }
        else if (canMachine && (Contains(transcription, "bake pizza") || Contains(transcription, "put pizza in oven") || Contains(transcription, "pizza in oven")))
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
        else if (canMachine && Contains(transcription, "grill"))
        {
            grill.StartCooking();
            ShowFeedback("Starting grill");
        }
        else if (canMachine && Contains(transcription, "oven"))
        {
            oven.StartCooking();
            ShowFeedback("Starting oven");
        }

        // --- Assembly (Full only) ---
        else if (canAssemble && (Contains(transcription, "add patty") || Contains(transcription, "patty done") || Contains(transcription, "take patty")
                              || Contains(transcription, "add paddy") || Contains(transcription, "paddy done") || Contains(transcription, "take paddy")))
        {
            if (grill.TransferPattyToBurger(burgerAssembly))
                ShowFeedback("Adding patty to burger");
            else
                ShowFeedback("No patty on grill");
        }
        else if (canAssemble && (Contains(transcription, "place patty") || Contains(transcription, "put patty")
                              || Contains(transcription, "place paddy") || Contains(transcription, "put paddy")))
        {
            grill.PlacePattyOnGrill();
            ShowFeedback("Placing patty on grill");
        }
        else if (canAssemble && (Contains(transcription, "bottom bun") || Contains(transcription, "place bun") || Contains(transcription, "start burger")))
        {
            burgerAssembly.PlaceBottomBun();
            ShowFeedback("Placing bottom bun");
        }
        else if (canAssemble && (Contains(transcription, "top bun") || Contains(transcription, "finish burger")))
        {
            burgerAssembly.AddTopBun();
            ShowFeedback("Adding top bun");
        }
        else if (canAssemble && Contains(transcription, "cheese"))
        {
            burgerAssembly.AddCheese();
            ShowFeedback("Adding cheese");
        }
        else if (canAssemble && Contains(transcription, "tomato"))
        {
            burgerAssembly.AddTomato();
            ShowFeedback("Adding tomato");
        }
        else if (canAssemble && Contains(transcription, "onion"))
        {
            burgerAssembly.AddOnion();
            ShowFeedback("Adding onion");
        }
        else if (canAssemble && (Contains(transcription, "lettuce") || Contains(transcription, "salad")))
        {
            burgerAssembly.AddSalad();
            ShowFeedback("Adding lettuce");
        }
        else if (canAssemble && Contains(transcription, "bacon"))
        {
            burgerAssembly.AddBacon();
            ShowFeedback("Adding bacon");
        }
        else if (canAssemble && (Contains(transcription, "place pizza") || Contains(transcription, "set pizza")))
        {
            pizzaAssembly.PlacePizza();
            ShowFeedback("Placing pizza");
        }
        else if (canAssemble && Contains(transcription, "pepperoni"))
        {
            pizzaAssembly.AddPepperoni();
            ShowFeedback("Adding pepperoni");
        }
        else if (canAssemble && Contains(transcription, "mushroom"))
        {
            pizzaAssembly.AddMushroom();
            ShowFeedback("Adding mushrooms");
        }
        else if (canAssemble && Contains(transcription, "olive"))
        {
            pizzaAssembly.AddOlive();
            ShowFeedback("Adding olives");
        }
        else if (canAssemble && Contains(transcription, "pepper"))
        {
            pizzaAssembly.AddPepper();
            ShowFeedback("Adding peppers");
        }
        else if (canAssemble && Contains(transcription, "basil"))
        {
            pizzaAssembly.AddBasil();
            ShowFeedback("Adding basil");
        }
        else
        {
            Debug.Log($"[Voice] No command matched (mode: {commandMode}): {transcription}");
        }
    }

    public void ShowIntroHelp()
    {
        if (helpPanel == null || helpPanelText == null) return;
        if (_helpCoroutine != null) StopCoroutine(_helpCoroutine);
        helpPanelText.text = GetIntroHelpText();
        helpPanel.SetActive(true);
        _helpVisible = true;
        _helpCoroutine = StartCoroutine(AutoCloseHelp());
    }

    private string GetIntroHelpText()
    {
        bool canAssemble = commandMode == VoiceCommandMode.Full;
        bool canMachine  = commandMode == VoiceCommandMode.Full || commandMode == VoiceCommandMode.MacroAndServe;

        string header = "── VOICE COMMANDS ──\n\n";
        string footer = "\n\nSay 'help coffee/burger/pizza' for details\nSay 'close' to dismiss";
        string body   = "";

        if (canAssemble)
        {
            body += "COFFEE: \"coffee\" · \"grab cup\" · \"pour coffee\"\n";
            body += "BURGER: \"place patty\" · \"grill\" · \"add patty\"\n";
            body += "         \"bottom/top bun\" · \"cheese\" · \"tomato\" ...\n";
            body += "PIZZA:  \"place pizza\" · \"pepperoni\" · \"mushroom\" ...\n";
            body += "         \"bake pizza\" · \"oven\"\n";
            body += "END:    \"finish\" or \"end session\"\n";
        }
        else if (canMachine)
        {
            body += "COFFEE: \"coffee\" · \"grab cup\" · \"pour coffee\"\n";
            body += "BURGER: \"grill\"\n";
            body += "PIZZA:  \"oven\" · \"bake pizza\"\n";
        }

        body += "SERVE:  \"serve [food] to customer [1/2/3]\"";

        return header + body + footer;
    }

    private void ShowHelp(string topic)
    {
        if (helpPanel == null || helpPanelText == null) return;
        if (_helpCoroutine != null) StopCoroutine(_helpCoroutine);
        helpPanelText.text = GetHelpText(topic);
        helpPanel.SetActive(true);
        _helpVisible = true;
        _helpCoroutine = StartCoroutine(AutoCloseHelp());
    }

    private void CloseHelp()
    {
        if (_helpCoroutine != null) StopCoroutine(_helpCoroutine);
        if (helpPanel != null) helpPanel.SetActive(false);
        _helpVisible = false;
    }

    private IEnumerator AutoCloseHelp()
    {
        yield return new WaitForSeconds(15f);
        CloseHelp();
    }

    private string GetHelpText(string topic)
    {
        string header = $"── {topic.ToUpper()} COMMANDS ──\n\n";
        string footer = "\n\nSay 'close' to dismiss";
        string body = "";

        bool canAssemble = commandMode == VoiceCommandMode.Full;
        bool canMachine  = commandMode == VoiceCommandMode.Full || commandMode == VoiceCommandMode.MacroAndServe;

        switch (topic.ToLower())
        {
            case "coffee":
                if (canMachine)
                {
                    body += "\"coffee\"  →  start coffee maker\n";
                    body += "\"grab cup\"  →  get a cup\n";
                    body += "\"pour coffee\"  →  fill the cup\n";
                }
                body += "\"serve coffee to customer 1 / 2 / 3\"";
                break;

            case "burger":
                if (canAssemble)
                {
                    body += "\"place patty\"  →  place patty on grill\n";
                    body += "\"grill\"  →  start grill\n";
                    body += "\"add patty\"  →  move patty to burger\n";
                    body += "\"bottom bun\"  →  place bottom bun\n";
                    body += "\"cheese / tomato / onion / lettuce / bacon\"  →  add toppings\n";
                    body += "\"top bun\"  →  finish burger\n";
                }
                else if (canMachine)
                {
                    body += "\"grill\"  →  start grill\n";
                }
                body += "\"serve burger to customer 1 / 2 / 3\"";
                break;

            case "pizza":
                if (canAssemble)
                {
                    body += "\"place pizza\"  →  place pizza dough\n";
                    body += "\"pepperoni / mushroom / olive / pepper / basil\"  →  add toppings\n";
                    body += "\"bake pizza\"  →  put pizza in oven\n";
                    body += "\"oven\"  →  start oven\n";
                }
                else if (canMachine)
                {
                    body += "\"oven\"  →  start oven\n";
                }
                body += "\"serve pizza to customer 1 / 2 / 3\"";
                break;
        }

        return header + body + footer;
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
}
