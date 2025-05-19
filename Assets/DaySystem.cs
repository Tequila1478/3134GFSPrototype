using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaySystem : MonoBehaviour
{
    public int dayStage = 0;
    public DialogueScript dailyDialogue;
    // Start is called before the first frame update
    void Start()
    {
        dailyDialogue = FindObjectOfType<DialogueScript>();
        dailyDialogue.StartDay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
