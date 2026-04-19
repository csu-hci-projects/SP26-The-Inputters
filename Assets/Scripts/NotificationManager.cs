
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] GameObject notificationTxt_GO;
    [SerializeField] GameObject stationTxt_GO;
    [SerializeField] float duration;
    GameObject cutletGO;
    public GameObject globalRecords_GO;     // Reference to global records
    int gameObjectId = 0;
    bool burgerNotification = false;
    Vector3 relativePos = new Vector3(0, 0, 0);


    public float spawnNotiTime { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        if (duration > 0)
            StartCoroutine(NotificationDuration(duration));
        globalRecords_GO = GameObject.FindWithTag("Global Records");
    }

    private void Update()
    {
        if (globalRecords_GO.GetComponent<Records>().GetNotificationType().Equals(0))
        {
            if (burgerNotification)
            {
                if (cutletGO == null)
                    Destroy(transform.gameObject);
                else
                    transform.localPosition = cutletGO.transform.localPosition + relativePos;
                //Debug.Log("CutletGO: " + cutletGO);
            }
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
       // Debug.Log($"{gameObject.name} (Cutlet) was destroyed.");
    }

    public void setText(string stationTxt, string notificationTxt)
    {
        notificationTxt_GO.GetComponent<TextMeshPro>().text = notificationTxt;
        stationTxt_GO.GetComponent<TextMeshPro>().text = stationTxt;
    }

    public string getText(string txtType)
    {
        if (txtType == "Station")
            return stationTxt_GO.GetComponent<TextMeshPro>().text;
        else
            return notificationTxt_GO.GetComponent<TextMeshPro>().text;
    }

    IEnumerator PauseForSound()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(transform.gameObject);
    }

    public void OnNotificationClick()
    {
        float reactionTime = Time.time - spawnNotiTime;
        //int objectId = GetObjectId();
        RecordingManager recorder = FindObjectOfType<RecordingManager>();
        GazeNotificationManager gazeNotificationManager = FindObjectOfType<GazeNotificationManager>();
        recorder.RecordEvent($"Notification Clicked: {notificationTxt_GO.GetComponent<TextMeshPro>().text}, ReactionTime: {reactionTime}");
        if (globalRecords_GO.GetComponent<Records>().GetNotificationType() == 1)
            transform.parent.parent.GetComponent<NotificationDockManager>().ManageNotificationLayout(gameObject);

        if (globalRecords_GO.GetComponent<Records>().GetNotificationType() == 3)
        {
            
           // Debug.Log($"[Click] Removing Dock Notification: {notificationTxt_GO.GetComponent<TextMeshPro>().text}");
            transform.parent.parent.GetComponent<NotificationDockManager>().ManageNotificationLayout(gameObject);

            
            if (gazeNotificationManager != null)
            {
                gazeNotificationManager.RemoveNotiGazeDock(gameObject);
               // Debug.Log($"[Click] Also Removing Gaze Notification with Object ID: {notificationTxt_GO.GetComponent<TextMeshPro>().text}");
            }
            else
            {
                Debug.LogError("[Click] ERROR: GazeNotificationManager not found!");
            }
            //globalRecords_GO.GetComponent<Records>().RemoveNotificationViewport(gameObject);
        }
        if (transform.parent.name.Equals("Customer(Clone)"))
            globalRecords_GO.GetComponent<Records>().GetPersistentGO().GetComponent<PersistentGOManager>().AddData("Notification", stationTxt_GO.GetComponent<TextMeshPro>().text + ":" + notificationTxt_GO.GetComponent<TextMeshPro>().text + ":" + transform.parent.GetInstanceID().ToString(), 2);
        else
            globalRecords_GO.GetComponent<Records>().GetPersistentGO().GetComponent<PersistentGOManager>().AddData("Notification", stationTxt_GO.GetComponent<TextMeshPro>().text + ":" + notificationTxt_GO.GetComponent<TextMeshPro>().text + ":" + transform.parent.GetInstanceID().ToString(), 2);
        StartCoroutine(PauseForSound());
    }

    public void SetNotificationProperties(string stationTxt, string notificationTxt, GameObject parent, Vector3? pos = null, Quaternion? rot = null, Vector3? scale = null, GameObject cutletGameObject = null)
    {
        spawnNotiTime = Time.time;
        if (pos == null)
            pos = Vector3.zero;
        if (rot == null)
            rot = Quaternion.identity;
        if (scale == null)
            scale = transform.localScale;
        setText(stationTxt, notificationTxt);
        transform.SetParent(parent.transform);
       // Debug.Log("Parent Position: " + parent.transform.position +parent.name);
        //Debug.Log("Parent Rotation: " + parent.transform.rotation);
        //Debug.Log("Parent Scale: " + parent.transform.localScale);
        if (stationTxt == "Burger")
        {
            relativePos = pos.Value;
            cutletGO = cutletGameObject;
            burgerNotification = true;
        }
        transform.localPosition = pos.Value;
        transform.localRotation = rot.Value;
        transform.localScale = scale.Value;
        //gameObjectId = objectId;
      
    }

    IEnumerator NotificationDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(transform.gameObject);
    }

    public void SetObjectId(int objectId)
    {
        gameObjectId = objectId;
    }

    public int GetObjectId()
    {
        return gameObjectId;
    }
}
