using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class RecordApp : MonoBehaviour
{
    private string participantNo;
    private string notificationType;
    private string filePath;
    private bool isTestMode = false; 

    float time_s = 0;

    void Start()
    {
        participantNo = PlayerPrefs.GetString("ParticipantNo", "Unknown");
        notificationType = PlayerPrefs.GetString("NotificationType", "Unknown");

        if (participantNo == "Test")
        {
            isTestMode = true;
            Debug.Log("Test mode aktif - veri kaydedilmeyecek.");
            return;
        }

       
        int notificationId = GetNotificationId(notificationType);

       
        string folderPath = Application.persistentDataPath + "/Results";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        filePath = folderPath + $"/Participant{participantNo}_Notification{notificationId}.csv";

        if (!File.Exists(filePath))
        {
            using (StreamWriter sw = new StreamWriter(filePath, false))
            {
                sw.WriteLine("ParticipantNo,NotificationType,TimeStamp, Duration, EventData"); // Headers
            }
        }
    }

    private void Update()
    {
        time_s += Time.deltaTime;
    }

    public void RecordEventon(string eventData)
    {
        if (isTestMode) return; 

        string timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        using (StreamWriter sw = new StreamWriter(filePath, true))
        {
            sw.WriteLine($"{participantNo},{notificationType},{timeStamp}, {Time.time}, {eventData}");
        }
    }

    private int GetNotificationId(string sceneName)
    {
        switch (sceneName)
        {
            case "NoO": return 0;
            case "NoD": return 1;
            case "NoG": return 2;
            case "NoGD": return 3;
            default: return -1; 
        }
    }
}
