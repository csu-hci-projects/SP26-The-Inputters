using UnityEngine;

public class FinishButtonScript : MonoBehaviour
{
    public void ExitApplication()
    {
        Debug.Log("quitting app");
        Application.Quit(); 
    }
}