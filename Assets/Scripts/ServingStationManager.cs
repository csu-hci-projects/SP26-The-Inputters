using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServingStationManager : MonoBehaviour
{
    [SerializeField] GameObject customerPrefab;
    [SerializeField] GameObject[] customerPositionGO;
    [SerializeField] GameObject globalRecords_GO;
    float customerDuration = 999999f;

    Dictionary<int, GameObject> customers = new Dictionary<int, GameObject>();
    Vector3[] customerPositions = new Vector3[3];
    int numCustomers = 0;
    int totalCustomers = 0;
    string[] currCustomerNames;


    // Start is called before the first frame update
    private static readonly string[] fixedFoodItems = { "Coffee", "Burger", "Pizza" };

    void Start()
    {
        globalRecords_GO = GameObject.FindWithTag("Global Records");
        for (int i = 0; i < customerPositionGO.Length; i++)
        {
            customers.Add(i, null);
            customerPositions[i] = customerPositionGO[i].transform.localPosition;
        }

        currCustomerNames = new string[0];
        for (int i = 0; i < 3; i++)
        {
            GameObject tempCustomer = Instantiate(customerPrefab);
            customers[i] = tempCustomer;
            tempCustomer.transform.parent = transform;
            tempCustomer.transform.localPosition = customerPositions[i];
            tempCustomer.transform.localRotation = Quaternion.identity;
            tempCustomer.GetComponent<CustomerManager>().CreateCustomer(customerDuration, fixedFoodItems[i], i, currCustomerNames, i);
            tempCustomer.GetComponent<CustomerManager>().SetObjectID(tempCustomer.GetInstanceID().ToString());
            numCustomers++;
            totalCustomers++;

            GameObject labelGO = new GameObject($"CustomerLabel_{i + 1}");
            labelGO.transform.SetParent(tempCustomer.transform);
            labelGO.transform.localPosition = new Vector3(0f, 0.3f, 0f);
            labelGO.transform.localRotation = Quaternion.identity;
            TextMeshPro tmp = labelGO.AddComponent<TextMeshPro>();
            tmp.text = $"Customer {i + 1}";
            tmp.fontSize = 0.6f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.rectTransform.sizeDelta = new UnityEngine.Vector2(0.6f, 0.2f);
        }
    }




    public void RemoveCustomer(GameObject cust)
    {
        for (int i = 0; i < numCustomers; i++)
        {
            if (customers[i] != null)
            {
                if (customers[i].Equals(cust))
                {
                    customers[i] = null;
                    break;
                }
            }
        }
        numCustomers--;
       // PersistentGOManager.instance.AddData("Customer", cust.name + ":" + cust.GetInstanceID().ToString(), 2);
    }

    string[] CustomerNames()
    {
        string[] names = new string[transform.childCount - 3];

        for (int i = 3; i < transform.childCount; i++)
        {
            names[i - 3] = GetPrefabName(transform.GetChild(i).GetChild(4).gameObject);
        }
        return names;
    }

    string GetPrefabName(GameObject temp_GO)
    {
        string objectName = temp_GO.transform.name;
        for (int i = 0; i < objectName.Length; i++)
        {
            if (((objectName[i] - '0') <= 9 && (objectName[i] - '0') >= 0) || objectName[i] == '(')     // Prefab names are only made of alphabets. Number or Brackets signify copies
            {
                return objectName.Substring(0, i);
            }
        }
        return objectName;
    }

    public int GetCustomerCount()
    {
        return totalCustomers;
    }

    public CustomerManager GetCustomer(int index)
    {
        if (!customers.ContainsKey(index) || customers[index] == null) return null;
        return customers[index].GetComponent<CustomerManager>();
    }

    public void ServeCustomer(int index)
    {
        if (!customers.ContainsKey(index) || customers[index] == null) return;
        GameObject cust = customers[index];
        customers[index] = null;
        numCustomers--;
        Destroy(cust);
        Debug.Log($"[Voice] Customer {index + 1} served and removed.");
    }
}
