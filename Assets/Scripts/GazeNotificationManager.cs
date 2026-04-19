using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;
using System;

public class GazeNotificationManager : MonoBehaviour
{
    [SerializeField] GameObject notificationButton;
    [SerializeField] GameObject notificationParent;
    List<GameObject> notificationsList = new List<GameObject>();
    List<string> notificationText = new List<string>();
    List<string> stationText = new List<string>();
    List<int> gameObjectId = new List<int>();
    public GazeTracking gazeTracking;
    private float notificationSpacing = 0.3f;
    //GameObject newNotification;
    private float verticalOffset = 0.1f;

    void Update()
    {

      foreach (var notification in notificationsList)
         {
             if (notification != null)
             {
                 Vector3 gazeDirection = gazeTracking.GetGazeDirection();
                 Vector3 newPosition = Camera.main.transform.position + gazeDirection * 1.01f;

                int index = notificationsList.IndexOf(notification);
                newPosition += new Vector3(0, index * verticalOffset, 0);

                notification.transform.position = Vector3.Lerp(notification.transform.position, newPosition, Time.deltaTime * 5f);
                 notification.transform.rotation = Quaternion.LookRotation(gazeDirection);

                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, gazeDirection, out hit, 2.0f))
                {
                    if (hit.collider.CompareTag("Table") || hit.collider.CompareTag("grill")) 

                    {
                        Debug.Log("table collision");
                        notification.transform.position = hit.point + new Vector3(0, 1.5f, 0); 
                    }
                }
            }
         }
        
    }

    public void ShowNotificationsGaze(string stationTxt, string notificationTxt, int objectId)
    {

        Vector3 gazeDirection = gazeTracking.GetGazeDirection();
        //float notificationDistance = 0.0001f;
        Vector3 gazePosition = Camera.main.transform.position + gazeDirection;
       // GameObject newNotification = Instantiate(notificationButton, gazePosition, Quaternion.LookRotation(gazeDirection));
        Vector3 adjustedPosition = gazePosition + new Vector3(0, notificationsList.Count * verticalOffset, 0);

        GameObject newNotification = Instantiate(notificationButton, adjustedPosition, Quaternion.LookRotation(gazeDirection));

        notificationsList.Add(newNotification);
         gameObjectId.Add(objectId);

        newNotification.GetComponent<NotificationManager>().SetNotificationProperties(
        stationTxt, notificationTxt, notificationParent, new Vector3(0, -notificationsList.Count * 0.2f, 0), Quaternion.identity, new Vector3(3, 3, 1));

        
        StartCoroutine(DestroyNotificationAfterTime(newNotification, objectId, 3f));



    }

    public void ShowNotificationsGazeDock(string stationTxt, string notificationTxt, int objectId)
    {

        Vector3 gazeDirection = gazeTracking.GetGazeDirection();
        //float notificationDistance = 0.0001f;
        Vector3 gazePosition = Camera.main.transform.position + gazeDirection;
        // GameObject newNotification = Instantiate(notificationButton, gazePosition, Quaternion.LookRotation(gazeDirection));
        Vector3 adjustedPosition = gazePosition + new Vector3(0, notificationsList.Count * verticalOffset, 0);

        GameObject newNotification = Instantiate(notificationButton, adjustedPosition, Quaternion.LookRotation(gazeDirection));

        notificationsList.Add(newNotification);
        gameObjectId.Add(objectId);

        newNotification.GetComponent<NotificationManager>().SetNotificationProperties(
        stationTxt, notificationTxt, notificationParent, new Vector3(0, -notificationsList.Count * 0.2f, 0), Quaternion.identity, new Vector3(3, 3, 1));


    }

    public void AddNotification(string stationTxt, string notificationTxt, int objectId)
    {
        
        if (gameObjectId.Contains(objectId))
            {

            int index = gameObjectId.IndexOf(objectId);

            gameObjectId.RemoveAt(index);

                notificationText.RemoveAt(index);
                stationText.RemoveAt(index);
           
        }
        
        notificationText.Insert(0, notificationTxt);
        stationText.Insert(0, stationTxt);
        gameObjectId.Insert(0, objectId);
        ShowNotificationsGaze(stationTxt, notificationTxt, objectId);
    }

    public void AddNotificationGazeDock(string stationTxt, string notificationTxt, int objectId)
    {
        //Debug.Log($"AddNotification called with stationTxt: {stationTxt}, notificationTxt: {notificationTxt}, objectId: {objectId}");
        if (gameObjectId.Contains(objectId))
        {
            int index = gameObjectId.IndexOf(objectId);
           // Debug.Log($"Found objectId: {objectId} at index: {index}");
            gameObjectId.RemoveAt(index);

            notificationText.RemoveAt(index);
            stationText.RemoveAt(index);
            //Debug.Log($"Removed notification at index {index}: {stationText[index]}, {notificationText[index]}");
        }
       // Debug.Log($"Inserted new notification at the start: {stationTxt}, {notificationTxt}, {objectId}");
        notificationText.Insert(0, notificationTxt);
        stationText.Insert(0, stationTxt);
        gameObjectId.Insert(0, objectId);
        ShowNotificationsGazeDock(stationTxt, notificationTxt, objectId);
       // Debug.Log($"ShowNotificationsGaze called with: {stationTxt}, {notificationTxt}, {objectId}");
    }
   

    public void RemoveNotificationGaze(int objectId)
    {
        int index = gameObjectId.IndexOf(objectId);
        if (index != -1)
        {
            Debug.Log($"[Gaze] Removing Gaze Notification: {notificationText[index]} (Object ID: {objectId})");
            Destroy(notificationsList[index]);
            notificationsList.RemoveAt(index);
            gameObjectId.RemoveAt(index);
           
        }
    }
    public void RemoveNotiGazeDock(GameObject notificationGO)
    {
        
        
    }
    IEnumerator DestroyNotificationAfterTime(GameObject notification, int objectId, float duration)
    {
        yield return new WaitForSeconds(duration);
        RemoveNotificationGaze(objectId);
    }


}