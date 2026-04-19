using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutletManager : MonoBehaviour
{
    [SerializeField] float cookingSpeed;
    [SerializeField] GameObject globalRecords_GO;

    int prevQuotient;
    bool cookingState = false;
    float cookingProgress = 0;
    GameObject notification_GO;

    // Start is called before the first frame update
    void Start()
    {
        globalRecords_GO = GameObject.FindWithTag("Global Records");
    }

    private void OnDestroy()
    {
        if (globalRecords_GO != null)
        {
            if (globalRecords_GO.GetComponent<Records>().GetNotificationType().Equals(1))
                globalRecords_GO.GetComponent<Records>().RemoveNotificationOnDock(transform.gameObject);
           // if (globalRecords_GO.GetComponent<Records>().GetNotificationType().Equals(2))
               // globalRecords_GO.GetComponent<Records>().RemoveNotificationViewport(transform.gameObject);
        }

      //  Debug.Log("Destroying Notification: " + notification_GO.name);
    }

    // Update is called once per frame
    void Update()
    {
       
        if (transform.GetComponent<IngredientProperties>().GetCookingStatus() != "Burnt" && cookingState)
        {
           // Debug.Log("Cooking State: " + cookingState);
           // Debug.Log("Cooking Status: " + transform.GetComponent<IngredientProperties>().GetCookingStatus());
            cookingProgress += Time.deltaTime;
            if (cookingProgress / cookingSpeed > prevQuotient + 1)
            {
                prevQuotient = (int)Math.Floor(cookingProgress / cookingSpeed);
                transform.GetComponent<IngredientProperties>().SetCookingStatus(prevQuotient, "Burger");
                string notifiText = "";
                switch (prevQuotient)
                {
                    case 0:
                        notifiText = "Uncooked";
                        break;
                    case 1:
                        notifiText = "Cooked";
                        break;
                    case 2:
                        notifiText = "Burnt";
                        break;
                }

                if (globalRecords_GO.GetComponent<Records>().GetPersistentGO().GetComponent<PersistentGOManager>().GetShowNotification())
                {
                    switch (globalRecords_GO.GetComponent<Records>().GetNotificationType())
                    {

                        case 0:
                            if (notification_GO != null)
                            Destroy(notification_GO); // Destroy any previous notification
                            notification_GO = globalRecords_GO.GetComponent<Records>().AddNotificationOnObject("Burger", notifiText, transform.GetInstanceID());
                            notification_GO.GetComponent<NotificationManager>().SetNotificationProperties("Burger", notifiText, transform.parent.gameObject,
                                new Vector3(0, 5f, 0), Quaternion.identity, new Vector3(121, 121f, 40), cutletGameObject: transform.gameObject);
                            break;

                        case 1:
                            globalRecords_GO.GetComponent<Records>().AddNotificationOnDock("Burger", notifiText, transform.GetInstanceID());
                            break;

                        case 2:
                            globalRecords_GO.GetComponent<Records>().AddNotificationOnViewport("Burger", notifiText, transform.GetInstanceID());
                            break;
                        case 3:
                            globalRecords_GO.GetComponent<Records>().AddNotificationBoth("Burger", notifiText, transform.GetInstanceID());
                            break;
                    }
                }
          
            }
        }
    }

    public bool GetCookingState()
    {
        return cookingState;
    }

    public void SetCookingState(bool status)
    {
        cookingState = status;
        //if (status)
           // this.GetComponent<AudioSource>().Play();
    }
}
