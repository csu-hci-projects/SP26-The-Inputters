using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoffeeMakerManager : MonoBehaviour
{

    [SerializeField] Vector3 coffeePotPos;
    [SerializeField] GameObject playArea;
    [SerializeField] GameObject coffeePotGlass;
    [SerializeField] GameObject coffeeMakerBody;
    [SerializeField] GameObject coffeeLevel_GO;
    [SerializeField] GameObject coffeeLevelMeter;
    [SerializeField] float coffeeFillRate = 2;
    [SerializeField] float maxCoffeeLevel = 0.05f;
    [SerializeField] GameObject globalRecords_GO;
    [SerializeField] GameObject cup;
    [SerializeField] private TextMeshPro coffeeStatusText;

    bool coffeeMakerOn = false;
    bool cupFull = false;
    Renderer rend;
    float coffeeLevel = -0.05f;
    int coffeeCupCnt = 0;
    GameObject notification_GO;

    // Start is called before the first frame update
    void Start()
    {
        globalRecords_GO = GameObject.FindWithTag("Global Records");
        Physics.IgnoreCollision(coffeePotGlass.GetComponent<MeshCollider>(), coffeeMakerBody.GetComponent<MeshCollider>(), true);
        rend = coffeeLevel_GO.GetComponent<Renderer>();
        if (coffeeStatusText != null)
            coffeeStatusText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (coffeeMakerOn && coffeeLevel < maxCoffeeLevel && transform.GetChild(1).GetComponent<CoffeePotManager>().GetPlaced())
        {
            coffeeLevel += coffeeFillRate * 0.001f * Time.deltaTime;
            rend.material.SetFloat("FillLevel", coffeeLevel);
        }
        else if (coffeeMakerOn && coffeeLevel >= maxCoffeeLevel && transform.GetChild(1).GetComponent<CoffeePotManager>().GetPlaced())
        {
            TurnOnCoffeeMaker();
        }

        if (((coffeeLevel + maxCoffeeLevel) * 3) / (maxCoffeeLevel * 2) > coffeeCupCnt + 1)
        {
            coffeeCupCnt++;
            if (globalRecords_GO.GetComponent<Records>().GetPersistentGO().GetComponent<PersistentGOManager>().GetShowNotification())
            {
                globalRecords_GO.GetComponent<Records>().AddNotificationOnDock("Coffee", "Coffee cup added", transform.GetInstanceID());
            }
           /* if (globalRecords_GO.GetComponent<Records>().GetNotificationType() == 3)
            {
                PersistentGOManager.instance.GetComponent<PersistentGOManager>().AddData("Notification", "Coffee cup added" + ":" + transform.GetInstanceID().ToString(), 1);
                Camera.main.transform.GetComponent<AudioSource>().Play();
            }*/
        }
    }

    public Vector3 GetCoffeePotPos()
    {
        return coffeePotPos;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Coffee_Pot") && !other.GetComponent<CoffeePotManager>().GetPlaced())
        {
            if (!other.GetComponent<CoffeePotManager>().GetIsGrabbed())
            {
                // other.transform.parent = transform;
                other.transform.localPosition = coffeePotPos;
                other.transform.localRotation = Quaternion.identity;
                other.GetComponent<CoffeePotManager>().SetPlaced(true);
                coffeeLevel = other.GetComponent<CoffeePotManager>().GetCoffeeLevel();
            }
        }
        if (other.CompareTag("Coffee_Pot") && other.GetComponent<CoffeePotManager>().GetPlaced() && !coffeeLevelMeter.activeSelf)
        {
            coffeeLevelMeter.SetActive(true);
            coffeeLevel_GO.SetActive(true);
        }
    }

    public void TurnOnCoffeeMaker()
    {
        if (coffeeMakerOn)
        {
            coffeeMakerOn = false;
            if (coffeeStatusText != null)
                coffeeStatusText.gameObject.SetActive(false);
        }
        else
        {
            coffeeMakerOn = true;
            coffeeLevel = rend.material.GetFloat("FillLevel");
            if (coffeeStatusText != null)
            {
                coffeeStatusText.text = "Coffee Maker: ON";
                coffeeStatusText.gameObject.SetActive(true);
            }
        }
    }

    public bool TakeFilledCup()
    {
        if (!cupFull)
        {
            Debug.Log("[Voice] Cup is not filled yet.");
            return false;
        }
        if (cup == null || !cup.activeSelf)
        {
            Debug.Log("[Voice] No cup available.");
            return false;
        }
        cup.SetActive(false);
        cupFull = false;
        Debug.Log("[Voice] Cup taken for serving.");
        return true;
    }

    public float GetCoffeeLevel()
    {
        return coffeeLevel;
    }

    public void SpawnCup()
    {
        if (cup == null)
        {
            Debug.LogWarning("[Voice] Cup not assigned.");
            return;
        }
        if (cup.activeSelf)
        {
            Debug.Log("[Voice] Cup already out.");
            return;
        }
        cup.SetActive(true);
        cupFull = false;
        Debug.Log("[Voice] Cup revealed.");
    }

    public void PourCoffeeIntoLastCup()
    {
        if (cupFull)
        {
            Debug.Log("[Voice] Cup is already full.");
            return;
        }
        if (coffeeCupCnt <= 0)
        {
            Debug.Log("[Voice] Not enough coffee brewed yet.");
            return;
        }
        if (cup == null || !cup.activeSelf)
        {
            Debug.Log("[Voice] No cup to pour into.");
            return;
        }
        CoffeeCupManager cupManager = cup.GetComponentInChildren<CoffeeCupManager>();
        if (cupManager == null)
        {
            Debug.LogWarning("[Voice] CoffeeCupManager not found on cup.");
            return;
        }
        cupManager.FillToFull();
        cupFull = true;
        coffeeCupCnt--;
        coffeeLevel -= (maxCoffeeLevel * 2f) / 3f;
        rend.material.SetFloat("FillLevel", coffeeLevel);
        Debug.Log("[Voice] Coffee poured.");
    }
}
