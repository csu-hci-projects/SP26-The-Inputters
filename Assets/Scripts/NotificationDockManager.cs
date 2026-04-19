using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationDockManager : MonoBehaviour
{
    [SerializeField] GameObject notificationBtnText;
    [SerializeField] GameObject notificationCountGO;
    [SerializeField] GameObject notificationButton;
    [SerializeField] GameObject notificationParent;
    [SerializeField] GameObject notificationBtnBackplate;
    [SerializeField] Material[] notificationBtnMaterial;

    List<GameObject> notificationsList = new List<GameObject>();
    List<string> notificationText = new List<string>();
    List<string> stationText = new List<string>();
    List<int> gameObjectId = new List<int>();

    // Start is called before the first frame update
    void Start()
    {

    }
    private void AdjustNotificationPanelSize()
    {
       //float newHeight = Mathf.Max(1.0f, notificationsList.Count * 0.5f); // Adjust height based on notifications
       // notificationParent.transform.localScale = new Vector3(1, newHeight, 1);
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
            //notificationBtnBackplate.GetComponent<Renderer>().material = notificationBtnMaterial[0];
            for (int i = 0; i < notificationText.Count; i++)
            {
                notificationsList.Add(Instantiate(notificationButton, new Vector3(0, 0, 0), Quaternion.identity));
                float y = -1 * (float)i / 10;
                notificationsList[i].GetComponent<NotificationManager>().SetNotificationProperties(stationText[i], notificationText[i], notificationParent, new Vector3(0, y, 0), Quaternion.identity, new Vector3(3, 3, 1));
            }
        }
       // AdjustNotificationPanelSize();
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
        //ShowNotifications();

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
            Debug.Log($"[Dock] Removing Dock Notification: {notificationText[index]} (Object ID: {gameObjectId[index]})");
            GazeNotificationManager gazeNotificationManager = FindObjectOfType<GazeNotificationManager>();
            gazeNotificationManager.RemoveNotificationGaze(gameObjectId[index]);
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
