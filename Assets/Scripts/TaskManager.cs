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
        public bool isRequired;
    }

    public List<Task> allTasks = new List<Task>();

    [System.Serializable]
    public class TaskRequirements
    {
        public string taskType;
        public int minimumRequired;
    }

    public TaskRequirements[] taskRequirements;

    private Dictionary<string, int> completedByType = new Dictionary<string, int>();
    private int totalRequiredTasks = 0;
    private int totalRequiredCompleted = 0;

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
                    isCompleted = interactable.hasSetSpot,
                    isRequired = interactable.isRequired
                };

                allTasks.Add(newTask);
            }
        }

        // Count required tasks
        foreach (var task in allTasks)
        {
            if (task.isRequired)
                totalRequiredTasks++;
        }
    }

    void CheckForCompletedTasks()
    {
        foreach (Task task in allTasks)
        {
            if (!task.isCompleted && task.interactable.GetComponent<Interactable>().hasSetSpot)
            {
                task.isCompleted = true;
            }
        }
    }

    void UpdateProgress()
    {
        completedByType.Clear();
        totalRequiredCompleted = 0;

        foreach (Task task in allTasks)
        {
            if (task.isCompleted)
            {
                if (!completedByType.ContainsKey(task.taskType))
                    completedByType[task.taskType] = 0;

                completedByType[task.taskType]++;

                if (task.isRequired)
                    totalRequiredCompleted++;
            }
        }
    }

    public int GetCompletedCount(string taskType)
    {
        return completedByType.ContainsKey(taskType) ? completedByType[taskType] : 0;
    }

    public float GetCompletionPercentage()
    {
        if (totalRequiredTasks == 0)
        {
            Debug.Log("No required Tasks");
            return 1f;
        }
        return (float)totalRequiredCompleted / totalRequiredTasks;
    }

    public bool HasMetMinimum(string taskType)
    {
        foreach (var req in taskRequirements)
        {
            if (req.taskType == taskType)
            {
                return GetCompletedCount(taskType) >= req.minimumRequired;
            }
        }
        return false;
    }
}