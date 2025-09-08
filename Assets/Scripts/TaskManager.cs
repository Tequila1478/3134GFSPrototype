using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [System.Serializable]
    public class Task
    {
        public GameObject interactable;
        public string taskType;
        public bool isCompleted;
    }

    [System.Serializable]
    public class TaskRequirements
    {
        public string taskType;
        public int minimumRequired;
    }

    public List<Task> allTasks = new List<Task>();
    public TaskRequirements[] taskRequirements;

    private Dictionary<string, int> completedByType = new Dictionary<string, int>();

    void Start()
    {
        PopulateTasksFromScene();
        UpdateProgress();
    }

    void Update()
    {
        CheckForCompletedTasks();
        UpdateProgress();
    }

    void PopulateTasksFromScene()
    {
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
        foreach (GameObject obj in interactables)
        {
            Interactable interactable = obj.GetComponent<Interactable>();
            if (interactable != null)
            {
                Task newTask = new Task
                {
                    interactable = obj,
                    taskType = interactable.taskType,
                    isCompleted = interactable.isAtSetSpot
                };
                allTasks.Add(newTask);
            }
        }
    }

    void CheckForCompletedTasks()
    {
        foreach (Task task in allTasks)
        {
            bool atSpot = task.interactable.GetComponent<Interactable>().isAtSetSpot;
            task.isCompleted = atSpot;
        }
    }

    void UpdateProgress()
    {
        completedByType.Clear();

        foreach (Task task in allTasks)
        {
            if (task.isCompleted)
            {
                if (!completedByType.ContainsKey(task.taskType))
                    completedByType[task.taskType] = 0;

                completedByType[task.taskType]++;
            }
        }
    }

    public int GetCompletedCount(string taskType)
    {
        return completedByType.ContainsKey(taskType) ? completedByType[taskType] : 0;
    }

    // New helper: total tasks of a type
    public int GetTotalTasksOfType(string taskType)
    {
        int count = 0;
        foreach (var task in allTasks)
        {
            if (task.taskType == taskType)
                count++;
        }
        return count;
    }

    // Optional: returns required & optional counts for a type
    public void GetTaskCounts(string taskType, out int required, out int optional)
    {
        int completed = GetCompletedCount(taskType);
        int minimumRequired = 0;

        // Find minimumRequired for this type
        foreach (var req in taskRequirements)
        {
            if (req.taskType == taskType)
            {
                minimumRequired = req.minimumRequired;
                break;
            }
        }

        required = Mathf.Min(completed, minimumRequired);
        optional = Mathf.Max(0, completed - minimumRequired);
    }

}