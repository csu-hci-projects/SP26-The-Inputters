using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GazeDockNotificationManager : MonoBehaviour
{
    [SerializeField] GameObject notificationBtnText;
    [SerializeField] GameObject notificationCountGO;
    [SerializeField] GameObject notificationButton;
    [SerializeField] GameObject notificationParent;

    public GazeTracking gazeTracking;
    private float verticalOffset = 0.2f;
    List<GameObject> notificationsList = new List<GameObject>();
    List<string> notificationText = new List<string>();
    List<string> stationText = new List<string>();
    List<int> gameObjectId = new List<int>();

    // Start is called before the first frame update
    void Start()
    {

    }
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
                        notification.transform.position = hit.point + new Vector3(0, 1f, 0);
                    }
                }
            }
        }

    }
    public void ShowNotifications()
    {
        if (notificationBtnText.GetComponent<TextMeshPro>().text == "Hide Notifications")
        {
            notificationBtnText.GetComponent<TextMeshPro>().text = "Show Notifications";
            for (int i = 0; i < notificationsList.Count; i++)
            {
                Destroy(notificationsList[i]);
            }
            notificationsList.Clear();
        }
        else
        {
            notificationBtnText.GetComponent<TextMeshPro>().text = "Hide Notifications";

            for (int i = 0; i < notificationText.Count; i++)
            {
                notificationsList.Add(Instantiate(notificationButton, new Vector3(0, 0, 0), Quaternion.identity));
                float y = -1 * (float)i / 10;
                notificationsList[i].GetComponent<NotificationManager>().SetNotificationProperties(stationText[i], notificationText[i], notificationParent, new Vector3(0, y, 0), Quaternion.identity, new Vector3(3, 3, 1));
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


        //StartCoroutine(DestroyNotificationAfterTime(newNotification, objectId, 3f));



    }
    public void AddNotification(string stationTxt, string notificationTxt, int objectId)
    {
        if (gameObjectId.Contains(objectId))
        {
            if (notificationBtnText.GetComponent<TextMeshPro>().text == "Hide Notifications")
            {
                ManageNotificationLayout(notificationsList[gameObjectId.IndexOf(objectId)]);
            }
            else
            {
                int index = gameObjectId.IndexOf(objectId);
                gameObjectId.RemoveAt(index);
                notificationText.RemoveAt(index);
                stationText.RemoveAt(index);
            }
        }

        notificationCountGO.GetComponentInChildren<TextMeshPro>().text = (int.Parse(notificationCountGO.GetComponentInChildren<TextMeshPro>().text) + 1).ToString();
        notificationText.Insert(0, notificationTxt);
        stationText.Insert(0, stationTxt);
        gameObjectId.Insert(0, objectId);
        ShowNotifications();
        ShowNotificationsGaze(stationTxt, notificationTxt, objectId);


    }

    public void RemoveNotification(GameObject cutletGO)
    {
        if (gameObjectId.Contains(cutletGO.GetInstanceID()))
        {
            if (notificationBtnText.GetComponent<TextMeshPro>().text == "Hide Notifications")
            {
                ManageNotificationLayout(notificationsList[gameObjectId.IndexOf(cutletGO.GetInstanceID())]);
            }
            else
            {
                int index = gameObjectId.IndexOf(cutletGO.GetInstanceID());
                gameObjectId.RemoveAt(index);
                notificationText.RemoveAt(index);
                stationText.RemoveAt(index);
            }
        }
    }

    public void ManageNotificationLayout(GameObject notificationGO)
    {
        int index = notificationsList.IndexOf(notificationGO);
        if (index != -1)
        {
            GameObject GOtoDestroy = notificationsList[index];
            gameObjectId.RemoveAt(index);
            notificationText.RemoveAt(index);
            stationText.RemoveAt(index);
            notificationsList.RemoveAt(index);
            Destroy(GOtoDestroy);
            // AdjustNotificationPanelSize();
            if (notificationBtnText.GetComponent<TextMeshPro>().text == "Hide Notifications")
            {
                for (int i = 0; i < notificationText.Count; i++)
                {
                    float y = -1 * (float)i / 10;
                    notificationsList[i].GetComponent<NotificationManager>().SetNotificationProperties(stationText[i], notificationText[i], notificationParent, new Vector3(0, y, 0), Quaternion.identity, new Vector3(3, 3, 1));
                }
            }
            notificationCountGO.GetComponentInChildren<TextMeshPro>().text = (int.Parse(notificationCountGO.GetComponentInChildren<TextMeshPro>().text) - 1).ToString();
        }
    }

    public int GetNotificationCountGO()
    {
        return int.Parse(notificationCountGO.GetComponentInChildren<TextMeshPro>().text);
    }

    public void ResetRotation()
    {
        PersistentGOManager.instance.AddData("Dock", "Released [" + transform.position.x.ToString() + ";" + transform.position.y.ToString() + ";" + transform.position.z.ToString() + "]", 2);
        transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
    }

    public void DockGrabbed()
    {
        PersistentGOManager.instance.AddData("Dock", "Grabbed [" + transform.position.x.ToString() + ";" + transform.position.y.ToString() + ";" + transform.position.z.ToString() + "]", 1);
    }
}
