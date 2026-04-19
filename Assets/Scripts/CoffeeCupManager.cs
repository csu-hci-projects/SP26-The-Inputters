using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeCupManager : MonoBehaviour
{
    [SerializeField] GameObject coffeeCupCap;

    float coffeeCupFillRate = 4.2f;
    public float coffeeLevel;
    Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = transform.GetComponent<Renderer>();
        coffeeLevel = rend.material.GetFloat("FillLevel");
    }


    private void OnTriggerStay(Collider other)
    {
       // Debug.Log("Something is inside the trigger: " + other.gameObject.name);

        if (other.CompareTag("Coffee_Pour"))
        {
            //Debug.Log("Cup is under pouring coffee!");

            if (other.transform.parent != null)
            {
                CoffeePotManager coffeePot = other.transform.parent.GetComponent<CoffeePotManager>();

                if (coffeePot != null && coffeePot.GetIsPouring())
                {
                   // Debug.Log("Coffee is actively pouring!");

                    if (coffeeLevel < 0.03f)
                    {
                        coffeeLevel += coffeeCupFillRate * 0.001f * Time.deltaTime;
                        rend.material.SetFloat("FillLevel", coffeeLevel);
                       // Debug.Log("Coffee level: " + coffeeLevel);
                    }
                    else if (!coffeeCupCap.activeSelf)
                    {
                        coffeeCupCap.SetActive(true);
                        //Debug.Log("Coffee cap activated!");
                    }
                }

            }

        }
    }

}
