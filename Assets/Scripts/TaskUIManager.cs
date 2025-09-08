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

    public GameObject endDayButton;
    private bool dayEnded = false;

    public float overallPercent = 0f;


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
        float totalAllTasks = 0;
        float totalAllTasksCompleted = 0;

        foreach (var req in taskManager.taskRequirements)
        {
            string type = req.taskType;

            int completed = taskManager.GetCompletedCount(type);
            int required = req.minimumRequired;

            // Required progress
            int completedReq = Mathf.Min(completed, required);
            totalRequired += required;
            totalRequiredCompleted += completedReq;

            // Optional progress (extra completions beyond required)
            int optionalTotal = CountOptionalOfType(type);
            int optionalCompleted = Mathf.Max(0, completed - required);

            // Track overall totals
            totalAllTasks += required + optionalTotal;
            totalAllTasksCompleted += Mathf.Min(completed, required + optionalTotal);

            // Build display string
            string taskLine = $"{type}: {completedReq} / {required} (required)";
            if (optionalTotal > 0)
            {
                taskLine += $"\n({type}: {optionalCompleted} / {optionalTotal} (optional))";
            }

            taskEntries[type].GetComponent<TextMeshProUGUI>().text = taskLine;
        }

        // Required-only percent
        float requiredPercent = (totalRequired > 0) ? totalRequiredCompleted / totalRequired : 1f;

        // Overall percent (required + optional)
        overallPercent = (totalAllTasks > 0) ? totalAllTasksCompleted / totalAllTasks : 1f;

        // Update required progress bar
        progressBar.value = requiredPercent;
        progressText.text = $"{Mathf.RoundToInt(requiredPercent * 100)}%";

        if (!dayEnded)
        {
            if (Mathf.RoundToInt(requiredPercent * 100f) >= 100)
            {
                dayEnded = true;
                               
                FindObjectOfType<DialogueScript>().houseClean = (Mathf.RoundToInt(overallPercent * 100) >= 80f) ? true : false;
                Debug.Log(Mathf.RoundToInt(requiredPercent * 100));
                
                endDayButton.SetActive(true);
                Debug.Log("All required tasks complete");
            }
            else
            {
                endDayButton.SetActive(false);
            }
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
}
