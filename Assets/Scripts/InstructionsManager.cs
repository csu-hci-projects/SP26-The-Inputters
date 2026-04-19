using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InstructionsManager : MonoBehaviour
{
    [SerializeField] GameObject[] instructionPos;  // Array to store positions for text
    [SerializeField] string[] instructionText;     // Array to store the instruction texts
    [SerializeField] GameObject textGO;            // Text object to update the instruction text

    private int instructionStage = 0;  // Current stage/index for the instruction
    private GameObject globalRecords_GO;

    // Start is called before the first frame update
    void Start()
    {
        globalRecords_GO = GameObject.FindWithTag("Global Records");

        // Set initial instruction text and position (Stage 0)
        UpdateInstruction();
    }

    // Method to handle the "Next" button press
    public void NextInstruction()
    {
        if (instructionStage < instructionPos.Length - 1)  // Check if we are not at the last stage
        {
            instructionStage++;  // Move to the next stage
            UpdateInstruction();  // Update the text and position for the new stage
        }
        else
        {
            // If this is the last instruction, perform final actions
            globalRecords_GO.GetComponent<Records>().GetPersistentGO().GetComponent<PersistentGOManager>().SetShowNotification(true);

            // Optionally load the next scene or take some other action
            // LoadNextLevel();
        }
    }

    // Method to update instruction text and position
    private void UpdateInstruction()
    {
        textGO.transform.position = instructionPos[instructionStage].transform.position;
        textGO.transform.rotation = instructionPos[instructionStage].transform.rotation;

        // Update the instruction text
        textGO.GetComponent<TextMeshPro>().text = instructionText[instructionStage];
    }

    public void LoadNextLevel(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Unload current scene if needed
        if (SceneManager.GetActiveScene().name == "Instructions Scene")
        {
            yield return SceneManager.UnloadSceneAsync("Instructions Scene");
        }

        // Load new scene
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }
}
