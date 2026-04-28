using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown participantDropdown;
    [SerializeField] private string gameSceneName = "NoD";
    private string selectedParticipant;

    void Start()
    {
        participantDropdown.onValueChanged.AddListener(delegate { UpdateParticipant(); });
    }

    void UpdateParticipant()
    {
        selectedParticipant = participantDropdown.options[participantDropdown.value].text;
    }

    public void StartExperiment()
    {
        if (string.IsNullOrEmpty(selectedParticipant))
        {
            Debug.LogWarning("Please select a participant number.");
            return;
        }

        PlayerPrefs.SetString("ParticipantNo", selectedParticipant);
        PlayerPrefs.SetString("NotificationType", gameSceneName);

        Debug.Log($"Starting scene: {gameSceneName} for Participant {selectedParticipant}");
        SceneManager.LoadScene(gameSceneName);
    }
}
