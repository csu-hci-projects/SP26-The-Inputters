using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OvenManager : MonoBehaviour
{
    [SerializeField] GameObject pizzaPos;
    [SerializeField] float cookingSpeed;
    [SerializeField] GameObject progressText_GO;
    [SerializeField] GameObject globalRecords_GO;
    [SerializeField] private TextMeshPro ovenStatusText;

    bool cooking = false;
    float cookingProgress = 0;
    int prevQuotient = 0;
    GameObject food;
    GameObject notification_GO;


    // Start is called before the first frame update
    void Start()
    {
        globalRecords_GO = GameObject.FindWithTag("Global Records");
        if (ovenStatusText != null)
            ovenStatusText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (cooking && food.GetComponent<IngredientProperties>().GetCookingStatus() != "Burnt")
        {
            cookingProgress += Time.deltaTime;
            if (cookingProgress / cookingSpeed > prevQuotient + 1)
            {
                prevQuotient = (int)Math.Floor(cookingProgress / cookingSpeed);
                food.GetComponent<IngredientProperties>().SetCookingStatus(prevQuotient, "Pizza");
                switch (prevQuotient)
                {
                    case 0:
                        progressText_GO.GetComponent<TextMesh>().text = "Uncooked";
                        break;
                    case 1:
                        progressText_GO.GetComponent<TextMesh>().text = "Cooked";
                        break;
                    case 2:
                        progressText_GO.GetComponent<TextMesh>().text = "Burnt";
                        cooking = false;
                        if (ovenStatusText != null)
                            ovenStatusText.gameObject.SetActive(false);
                        break;
                }

                if (globalRecords_GO.GetComponent<Records>().GetPersistentGO().GetComponent<PersistentGOManager>().GetShowNotification())
                {
                    globalRecords_GO.GetComponent<Records>().AddNotificationOnDock("Pizza", progressText_GO.GetComponent<TextMesh>().text, transform.GetInstanceID());
                }
            }
        }
    }

    public void StartCooking()
    {
        cooking = !cooking;

        if (ovenStatusText != null)
        {
            if (cooking)
            {
                ovenStatusText.text = "Oven: ON";
                ovenStatusText.gameObject.SetActive(true);
            }
            else
            {
                ovenStatusText.gameObject.SetActive(false);
            }
        }
        Debug.Log($"[Voice] Oven toggled {(cooking ? "ON" : "OFF")}.");
    }

    public void LoadPizza(GameObject pizza)
    {
        if (food != null)
        {
            Debug.Log("[Voice] Oven already has food.");
            return;
        }

        food = pizza;
        pizza.transform.position = pizzaPos.transform.position;
        pizza.transform.rotation = Quaternion.identity;

        string status = pizza.GetComponent<IngredientProperties>().GetCookingStatus();
        progressText_GO.GetComponent<TextMesh>().text = status;
        switch (status)
        {
            case "Uncooked":
                cookingProgress = 0;
                prevQuotient = 0;
                break;
            case "Cooked":
                cookingProgress = cookingSpeed * 1;
                break;
            case "Burnt":
                cookingProgress = cookingSpeed * 2;
                break;
        }

        Debug.Log("[Voice] Pizza loaded into oven.");
    }

    public GameObject TakeCookedPizza()
    {
        // Check pizza currently sitting in the oven
        if (food != null)
        {
            if (food.GetComponent<IngredientProperties>().GetCookingStatus() != "Cooked")
            {
                Debug.Log("[Voice] Pizza not cooked yet.");
                return null;
            }
            GameObject pizza = food;
            food = null;
            cooking = false;
            progressText_GO.GetComponent<TextMesh>().text = "Place Food";
            if (ovenStatusText != null) ovenStatusText.gameObject.SetActive(false);
            return pizza;
        }

        // Fallback: pizza was grabbed out of the oven and is being held or placed nearby
        foreach (IngredientProperties ip in FindObjectsOfType<IngredientProperties>())
        {
            if (ip.GetPrefabName() == "Dough Ketchup" && ip.GetCookingStatus() == "Cooked")
            {
                Debug.Log("[Voice] Found cooked pizza outside oven.");
                return ip.gameObject;
            }
        }

        Debug.Log("[Voice] No cooked pizza found.");
        return null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ingredient_Base") && !other.gameObject.GetComponent<ObjectManager>().isGrabbed)
        {
            if (food == null)
            {
                food = other.gameObject;
                other.transform.position = pizzaPos.transform.position;
                other.transform.rotation = Quaternion.identity;
                progressText_GO.GetComponent<TextMesh>().text = other.GetComponent<IngredientProperties>().GetCookingStatus();
                switch (other.GetComponent<IngredientProperties>().GetCookingStatus())
                {
                    case "Uncooked":
                        cookingProgress = 0;
                        prevQuotient = 0;
                        break;
                    case "Cooked":
                        cookingProgress = cookingSpeed * 1;
                        break;
                    case "Burnt":
                        cookingProgress = cookingSpeed * 2;
                        RecordingManager recorder = FindObjectOfType<RecordingManager>();
                        recorder.RecordEvent($"Pizza burnt");
                        break;
                }
            }
        }
        else if (other.CompareTag("Ingredient_Base") && other.gameObject.GetComponent<ObjectManager>().isGrabbed)
        {
            cooking = false;
            if (ovenStatusText != null)
                ovenStatusText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ingredient_Base") && other.gameObject.GetComponent<ObjectManager>().isGrabbed)
        {
            other.transform.GetChild(1).gameObject.SetActive(false);
            progressText_GO.GetComponent<TextMesh>().text = "Place Food";
            cooking = false;
            food = null;
            if (ovenStatusText != null)
                ovenStatusText.gameObject.SetActive(false);
            if (notification_GO != null)
                Destroy(notification_GO);
        }
    }
}
