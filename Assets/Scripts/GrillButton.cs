using UnityEngine;

public class GrillButton : MonoBehaviour
{
    [SerializeField] private GrillManager grillManager;

    public void Press()
    {
        grillManager.StartCooking();
    }
}
