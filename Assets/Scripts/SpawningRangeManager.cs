using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningRangeManager : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;          // GameObject to spawn
    int numObjectsSpawnned = 0;
    //GameObject globalRecords_GO;                    // Reference to global records
    public GameObject globalRecords_GO;

    // Start is called before the first frame update
    /* IEnumerator Start()
     {
         yield return new WaitForEndOfFrame(); 
         globalRecords_GO = GameObject.FindWithTag("Global Records");

         if (globalRecords_GO == null)
             Debug.LogError("Global Records cant find");
         else
             Debug.Log("Global Records found: " + globalRecords_GO.name);
     }*/

    // Update is called once per frame


    private void OnTriggerEnter(Collider other)
    {
        // Ignore collision between box collision (collision to prevent objects entering in teh range) and spawned object
        if (other.name.Contains(objectToSpawn.name))
        {
            if (!other.gameObject.GetComponent<ObjectManager>().isGrabbed && other.GetComponent<ObjectManager>().justSpawned)
            {
                Physics.IgnoreCollision(other.GetComponent<BoxCollider>(), transform.parent.GetChild(1).GetComponent<BoxCollider>(), true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Set parent of spawned object to null, variable justSpawned to false and change collision back on
        // Spawn new object (initialize new object)
        // Set transformation and prefab name
        if (other.name.Contains(objectToSpawn.name) && other.gameObject.GetComponent<ObjectManager>().isGrabbed && other.gameObject.GetComponent<ObjectManager>().justSpawned)
        {
            numObjectsSpawnned += 1;
            //other.transform.SetParent(globalRecords_GO.GetComponent<Records>().GetPlayArea().transform);
            other.GetComponent<ObjectManager>().justSpawned = false;
            GameObject objectSpawned = Instantiate(objectToSpawn);
            objectSpawned.transform.SetParent(transform.parent);
            objectSpawned.transform.localPosition = objectSpawned.GetComponent<IngredientProperties>().GetLocation();
            objectSpawned.transform.localRotation = objectSpawned.GetComponent<IngredientProperties>().GetRotation();
            // objectSpawned.transform.localScale = objectSpawned.GetComponent<IngredientProperties>().getScale();
            objectSpawned.GetComponent<IngredientProperties>().SetPrefabName();
            objectSpawned.name = objectSpawned.GetComponent<IngredientProperties>().GetPrefabName() + " " + numObjectsSpawnned;
            Physics.IgnoreCollision(other.GetComponent<BoxCollider>(), transform.parent.GetChild(1).GetComponent<BoxCollider>(), false);
            //Debug.Log("fuck this shit!" + other.name.Contains(objectToSpawn.name) + " " + other.gameObject.GetComponent<ObjectManager>().isGrabbed + " " + other.gameObject.GetComponent<ObjectManager>().justSpawned + " this fucking num: " + numObjectsSpawnned); ;
        }
    }
}
