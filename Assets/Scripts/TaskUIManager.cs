using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskUIManager : MonoBehaviour
{
    public TaskManager taskManager;               // Reference to TaskManager
    public TextMeshProUGUI headerText;            // e.g. "Today's Task List! Day: X"
    public Transform taskListParent;              // Parent object to hold task items
    public GameObject taskEntryPrefab;            // Prefab with a TextMeshProUGUI component
    public Slider progressBar;                    // Completion percentage bar
    public TextMeshProUGUI progressText;          // Text to show percent complete
    public GameObject endDayButton;               // Button to end the day

    private Dictionary<string, GameObject> taskEntries = new();
    private bool dayEnded = false;

    private float overallPercent = 0f;

    void Start()
    {
        if (taskManager == null)
            taskManager = FindObjectOfType<TaskManager>();

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
            entry.GetComponent<TextMeshProUGUI>().text = "";
            taskEntries[req.taskType] = entry;
        }
    }

    void UpdateUI()
    {
        headerText.text = "Today's Task List! Day: " + GetDayNumber();

        float totalRequired = 0f;
        float totalRequiredCompleted = 0f;
        float totalAllTasks = 0f;
        float totalAllTasksCompleted = 0f;

        foreach (var req in taskManager.taskRequirements)
        {
            string type = req.taskType;

            // Use TaskManager helper to get required & optional counts
            taskManager.GetTaskCounts(type, out int requiredCompleted, out int optionalCompleted);

            int totalTasksOfType = taskManager.GetTotalTasksOfType(type);
            int optionalTotal = Mathf.Max(0, totalTasksOfType - req.minimumRequired);

            // Update totals for progress bars
            totalRequired += req.minimumRequired;
            totalRequiredCompleted += requiredCompleted;
            totalAllTasks += req.minimumRequired + optionalTotal;
            totalAllTasksCompleted += requiredCompleted + optionalCompleted;

            // Build display string
            string taskLine = $"{type}: {requiredCompleted} / {req.minimumRequired} (required)";
            /*if (optionalTotal > 0)
            {
                taskLine += $"\n({optionalCompleted} / {optionalTotal} optional)";
            }*/

            taskEntries[type].GetComponent<TextMeshProUGUI>().text = taskLine;
        }

        // Calculate percentages
        float requiredPercent = totalRequired > 0 ? totalRequiredCompleted / totalRequired : 1f;
        overallPercent = totalAllTasks > 0 ? totalAllTasksCompleted / totalAllTasks : 1f;

        // Update UI
        progressBar.value = requiredPercent;
        progressText.text = $"{Mathf.RoundToInt(requiredPercent * 100)}%";

        // Show end day button if all required tasks are complete
        if (!dayEnded && Mathf.RoundToInt(requiredPercent * 100f) >= 100)
        {
            dayEnded = true;
            FindObjectOfType<DialogueScript>().houseClean = overallPercent >= 0.8f;
            endDayButton.SetActive(true);
            Debug.Log("All required tasks complete");
        }
        else if (!dayEnded)
        {
            endDayButton.SetActive(false);
        }
    }

    int GetDayNumber()
    {
        // Replace with your actual day logic
        return 1;
    }
}
