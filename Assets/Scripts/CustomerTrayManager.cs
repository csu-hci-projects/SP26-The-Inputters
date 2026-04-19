using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using TMPro;

public class CustomerTrayManager : MonoBehaviour
{
    int numObjects = 0;
    GameObject persistentGO;
    public CustomerManager cManager;
    public GameObject records;
    private RecordingManager dataRecorder;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] globalRecords = GameObject.FindGameObjectsWithTag("Global Records");
        if (globalRecords.Length > 0)
        {
            records = globalRecords[0];
        }
        else
        {
            Debug.LogError("Global Records object not found in the scene!");
        }
        // records = GameObject.FindGameObjectsWithTag("Global Records")[0];
        dataRecorder = FindObjectOfType<RecordingManager>();
    }

    // Update is called once per frame

    private void OnTriggerEnter(Collider other)
    {
        persistentGO = GameObject.FindGameObjectsWithTag("PersistentGO")[0];
        numObjects++;
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("OnTriggerStay called for: " + other.gameObject.name);
        if (other.CompareTag("Coffee_Cup") || other.CompareTag("Ingredient_Base"))
        {
            if (!other.GetComponent<ObjectManager>().isGrabbed)
            {
                List<string> preparedFood = CreateIngredientsList(other.gameObject);
                if (transform.parent.GetComponent<CustomerManager>().CheckIndredients(preparedFood, other.gameObject))
                {
                    float serveDuration = Time.time - cManager.spawnTime;
                    string logData = $"Correct Food: {transform.GetComponentInParent<CustomerManager>().GetObjectID()}, Ingredients: {CreateIngredientsString(other.gameObject)}, Serve Duration: {serveDuration:F2}";
                    dataRecorder?.RecordEvent(logData);
                    

                    transform.parent.transform.GetComponentInParent<ServingStationManager>().RemoveCustomer(transform.parent.gameObject);


                    Destroy(other.gameObject);
                    Destroy(transform.parent.gameObject);
                }
                else
                {
                    if (!transform.GetChild(0).gameObject.activeSelf)
                    {
                        float serveDuration = Time.time - cManager.spawnTime;
                        string logData = $"Wrong Food: {transform.GetComponentInParent<CustomerManager>().GetObjectID()}, Ingredients: {CreateIngredientsString(other.gameObject)}, Serve Duration: {serveDuration:F2}";
                        dataRecorder?.RecordEvent(logData);

                       /* persistentGO.GetComponent<PersistentGOManager>().AddData(
                            "Food Served",
                            "Wrong Food:" + transform.GetComponentInParent<CustomerManager>().GetObjectID(),
                            2,
                            CreateIngredientsString(other.gameObject)
                        );*/
                        transform.GetChild(0).gameObject.SetActive(true);
                    }
                }
            }
        }
        else
        {
            if (!transform.GetChild(0).gameObject.activeSelf)
            {
               /* persistentGO.GetComponent<PersistentGOManager>().AddData(
                    "Food Served",
                    "Not Food:" + transform.GetComponentInParent<CustomerManager>().GetObjectID(),
                    2
                );*/
                transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        numObjects--;
        if (numObjects == 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    List<string> CreateIngredientsList(GameObject foodItem)
    {

        IngredientProperties ingredient = foodItem.GetComponent<IngredientProperties>();
        if (ingredient == null)
        {
            Debug.LogError("IngredientProperties component missing on " + foodItem.name);
            return new List<string>(); // Return an empty list to avoid further errors
        }

        List<string> preparedFood = new List<string>() { foodItem.GetComponent<IngredientProperties>().GetPrefabName() };
       
        if (preparedFood[0] == "CoffeeCup")
            return preparedFood;

        if (preparedFood[0] == "Dough Ketchup")
        {
            for (int i = 3; i < foodItem.transform.childCount; i++)
            {
                Transform child = foodItem.transform.GetChild(i);
                IngredientProperties childIngredient = child.GetComponentInChildren<IngredientProperties>();

                if (childIngredient != null)
                {
                    preparedFood.Add(childIngredient.GetPrefabName());
                }
                else
                {
                    Debug.LogWarning("IngredientProperties missing in child: " + child.name);
                }
            }
        }

        if (preparedFood[0] == "Burger Bread Down")
        {
            for (int i = 2; i < foodItem.transform.childCount; i++)
            {
                Transform child = foodItem.transform.GetChild(i);
                IngredientProperties childIngredient = child.GetComponentInChildren<IngredientProperties>();

                if (childIngredient != null)
                {
                    preparedFood.Add(childIngredient.GetPrefabName());
                }
                else
                {
                    Debug.LogWarning("IngredientProperties missing in child: " + child.name);
                }
            }
        }

        return preparedFood;
    }

    string CreateIngredientsString(GameObject foodItem)
    {
        string preparedFood = foodItem.GetComponent<IngredientProperties>().GetPrefabName();
        if (preparedFood == "CoffeeCup")
            return "[" + preparedFood + "]";

        if (preparedFood == "Dough Ketchup")
        {
            for (int i = 3; i < foodItem.transform.childCount; i++)
            {
                preparedFood = preparedFood + ";" + foodItem.transform.GetChild(i).GetComponentInChildren<IngredientProperties>().GetPrefabName();
            }
        }

        if (preparedFood == "Burger Bread Down")
        {
            for (int i = 2; i < foodItem.transform.childCount; i++)
            {
                preparedFood = preparedFood + ";" + foodItem.transform.GetChild(i).GetComponentInChildren<IngredientProperties>().GetPrefabName();
            }
        }
        return "[" + preparedFood + "]";
    }
}
