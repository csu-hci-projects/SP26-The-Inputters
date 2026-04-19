using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown participantDropdown;
    [SerializeField] private TMP_Dropdown sceneDropdown;
    private string selectedParticipant;
    private string selectedScene;

    void Start()
    {
        participantDropdown.onValueChanged.AddListener(delegate { UpdateParticipant(); });
        sceneDropdown.onValueChanged.AddListener(delegate { UpdateScene(); });
    }

    void UpdateParticipant()
    {
        selectedParticipant = participantDropdown.options[participantDropdown.value].text;
    }

    void UpdateScene()
    {
        selectedScene = sceneDropdown.options[sceneDropdown.value].text;
    }

    public void StartExperiment()
    {
        if (string.IsNullOrEmpty(selectedParticipant) || string.IsNullOrEmpty(selectedScene))
        {
            Debug.LogWarning("Lütfen hem participant hem de scene seçin!");
            return;
        }

        PlayerPrefs.SetString("ParticipantNo", selectedParticipant);
        PlayerPrefs.SetString("NotificationType", selectedScene);

        Debug.Log($"Starting scene: {selectedScene} for Participant {selectedParticipant}");
        SceneManager.LoadScene(selectedScene);
    }
}
