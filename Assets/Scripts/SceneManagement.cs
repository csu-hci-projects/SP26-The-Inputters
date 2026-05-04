using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public enum InputMode { Controller = 0, Hands = 1, Voice = 2, Test = 3 }

    [SerializeField] private TMP_Dropdown modeDropdown;
    [SerializeField] private string gameSceneName = "NoD";

    void Start()
    {
        modeDropdown.ClearOptions();
        modeDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Controller",
            "Hands",
            "Voice",
            "Test (All)"
        });
        modeDropdown.value = (int)InputMode.Test;
        modeDropdown.RefreshShownValue();
    }

    public void StartExperiment()
    {
        PlayerPrefs.SetInt("InputMode", modeDropdown.value);
        PlayerPrefs.Save();
        Debug.Log($"Starting {gameSceneName} in mode: {(InputMode)modeDropdown.value}");
        SceneManager.LoadScene(gameSceneName);
    }
}
