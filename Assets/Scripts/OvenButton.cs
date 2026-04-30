using UnityEngine;

public class OvenButton : MonoBehaviour
{
    [SerializeField] private OvenManager ovenManager;

    public void Press()
    {
        ovenManager.StartCooking();
    }
}
