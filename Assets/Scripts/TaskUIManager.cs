using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskUIManager : MonoBehaviour
{
    public TaskManager taskManager;                    // Reference to your TaskManager script
    public TextMeshProUGUI headerText;                 // e.g. "Today's Task List! Day: X"
    public Transform taskListParent;                   // Parent object to hold task items
    public GameObject taskEntryPrefab;                 // A prefab with a TextMeshProUGUI component
    public Slider progressBar;                         // Completion percentage bar
    public TextMeshProUGUI progressText;               // Text to show percent complete

    private Dictionary<string, GameObject> taskEntries = new();


    void Start()
    {
        if (taskManager == null)
        {
            taskManager = FindObjectOfType<TaskManager>();
        }

        PopulateTaskList();
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    void PopulateTaskList()
    {
        foreach (var req in taskManager.taskRequirements)
        {
            GameObject entry = Instantiate(taskEntryPrefab, taskListParent);
            entry.GetComponent<TextMeshProUGUI>().text = ""; // Set later
            taskEntries[req.taskType] = entry;
        }
    }

    void UpdateUI()
    {
        headerText.text = "Today's Task List! Day: " + GetDayNumber();
        float totalRequired = 0;
        float totalRequiredCompleted = 0;
        float percent;

        foreach (var req in taskManager.taskRequirements)
        {
            string type = req.taskType;
            int completed = taskManager.GetCompletedCount(type);

            int required = req.minimumRequired;
            totalRequired += required;
            int optional = CountOptionalOfType(type) - req.minimumRequired;
            int optionalCompleted = Mathf.Max(0, completed - required);

            int completedReq = (completed > required) ? required : completed;
            totalRequiredCompleted += completed;

            string taskLine = "";

            
          
            taskLine = $"{type}: {completedReq} / {required} (required)";            

            if (optional > 0)
            {
                taskLine += $"\n   {type}: {optionalCompleted} / {optional} (optional)";
            }

            taskEntries[type].GetComponent<TextMeshProUGUI>().text = taskLine;
        }

        totalRequiredCompleted = (totalRequiredCompleted > totalRequired) ? totalRequired : totalRequiredCompleted;
        percent = totalRequiredCompleted / totalRequired;
        progressBar.value = percent;
        progressText.text = Mathf.RoundToInt(percent * 100) + "%";
    }

    int CountOptionalOfType(string type)
    {
        int count = 0;
        foreach (var task in taskManager.allTasks)
        {
            if (task.taskType == type && !task.isRequired)
            {
                count++;
            }
        }
        return count;
    }

    int GetDayNumber()
    {
        // Replace with actual day logic

        return 1;
    }
}
