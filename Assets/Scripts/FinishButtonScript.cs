using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishButtonScript : MonoBehaviour
{
    [SerializeField] private string startSceneName = "StartScene";

    public void ExitApplication()
    {
        Debug.Log("Returning to start scene");
        SceneManager.LoadScene(startSceneName);
    }
}