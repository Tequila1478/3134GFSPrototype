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
    public int totalRequiredTasks = 0;
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
                    isCompleted = interactable.isAtSetSpot,
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
        foreach (var task in taskRequirements)
        {
            totalRequiredTasks += task.minimumRequired;
        }
    }

    void CheckForCompletedTasks()
    {
        foreach (Task task in allTasks)
        {
            if (!task.isCompleted && task.interactable.GetComponent<Interactable>().isAtSetSpot)
            {
                task.isCompleted = true;
            }
            if (task.isCompleted && !task.interactable.GetComponent<Interactable>().isAtSetSpot)
            {
                task.isCompleted = false;
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

}