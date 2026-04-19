using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using UnityEngine.Profiling;

public class PersistentGOManager : MonoBehaviour
{
    public static PersistentGOManager instance;

    #region Consts to modify
    private const int FlushAfter = 40;
    #endregion

    [SerializeField] bool showNotification = false;
    [SerializeField] GameObject StudyBillboard;

    Vector3 position = new Vector3(1000, 1000, 1000);
    GameObject currGlobalRecordsGO;
   
    string unloadSceneName;
    bool notificationSound = false;
    bool sceneChanged = false;
    List<string> sceneNames;
    int sceneIndex = 0;
    GameObject Records;
    public int participantNumber = 0;
    string filePath;
    StreamWriter writer;
    float time_s = 0;
    List<string> independentCSVData = new List<string>();
    private StringBuilder csvData;


    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] globalRecords = GameObject.FindGameObjectsWithTag("Global Records");
      /*  if (globalRecords.Length > 0)
        {
            Records = globalRecords[0];
        }*/

        // Make sure the directory exists:
       // if (!Directory.Exists(filePath))
            //Directory.CreateDirectory(filePath);

        // You can log the path to check it:
       // Debug.Log("Data will be saved to: " + filePath);
    }

    // Update is called once per frame
    void Update()
    {
        //time_s += Time.deltaTime;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public void SetPosition(Vector3 pos)
    {
        position = pos;
    }

    public bool GetShowNotification()
    {
        return showNotification;
    }

    public void SetShowNotification(bool sNotifi)
    {
        showNotification = sNotifi;
    }

    public void SetCurrGlobalRecordsGO(GameObject currObject)
    {
        currGlobalRecordsGO = currObject;
        foreach (var independentData in independentCSVData)
        {
            csvData.AppendLine(participantNumber + "," + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + "," + time_s + "," + currGlobalRecordsGO.GetComponent<Records>().GetNotificationType() + "," + independentData);
        }
        independentCSVData.Clear();
    }

  

    //============================== Study ==============================//

    public void SetParticipantNumber(int pNum)
    {
       /* participantNumber = pNum;

        // Using persistentDataPath for writing data
        filePath = Application.persistentDataPath + "/Records";

        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath); // Ensure directory exists

        // Generate file name with participant number and notification type
        string fileName = $"/Participant{participantNumber}_{currGlobalRecordsGO.GetComponent<Records>().GetNotificationType()}.csv";
        filePath = filePath + fileName;

        try
        {
            using (writer = File.CreateText(filePath))
            {
                writer.WriteLine("Participant_Number,Timestamp,Time_s,Notification_Type,Notification_Sound,Category,Action,Status,Ingredients");
            }
            csvData = new StringBuilder(); // Reset CSV data builder
            Debug.Log($"Data file created: {filePath}");
        }
        catch (UnauthorizedAccessException)
        {
            Debug.LogError($"UnauthorizedAccessException: Cannot write to file {filePath}. Check permissions.");
        }
        catch (IOException ex)
        {
            Debug.LogError($"IOException: {ex.Message}");
        }*/
    }

    public int GetParticipantNumber()
    {
        return participantNumber;
    }

    public void AddData(string category = "n/a", string action = "n/a", int status = 0, string ingredients = "n/a")
    {

       /* string notificationType;
        try
        {
            notificationType = currGlobalRecordsGO.GetComponent<Records>().GetNotificationType().ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error getting Notification Type: " + ex.Message);
            return;
        }

        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("Error: filePath is not set.");
            return;
        }

        try
        {
            bool fileExists = File.Exists(filePath);

            using (var writer = new StreamWriter(filePath, true))
            {
                // Write headers only if file does not exist
                if (!fileExists)
                {
                    writer.WriteLine("Participant_Number,Timestamp,Time_s,Notification_Type,Category,Action,Status,Ingredients");
                }

                writer.WriteLine($"{participantNumber},{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff},{Time.time},{notificationType},{category},{action},{status},{ingredients}");
            }
        }
        catch (UnauthorizedAccessException)
        {
            Debug.LogError($"UnauthorizedAccessException: Cannot write to file {filePath}. Check permissions.");
        }
        catch (IOException ex)
        {
            Debug.LogError($"IOException: {ex.Message}");
        }*/
    }



    void FlushData()
    {
        if (csvData.Length > 0)
        {
            using (var csvWriter = new StreamWriter(filePath, true))
            {
                csvWriter.Write(csvData.ToString());
            }
            csvData.Clear();
        }
    }

    public void EndCSV()
    {
        if (csvData == null)
        {
            return;
        }
        using (var csvWriter = new StreamWriter(filePath, true))
        {
            csvWriter.Write(csvData.ToString());
        }
        csvData = null;
    }

    private void OnDestroy()
    {
        EndCSV();
    }

}
