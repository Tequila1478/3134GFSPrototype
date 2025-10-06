using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnterHowToPlayMenu : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //FindObjectOfType<PauseGame>().SetPauseMenuActive(false);
        FindObjectOfType<MainMenu>().UpdateMenu("HowToPlayScreen");

        //Do nothing
    }

    private bool hasTriggered = false;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // NormalizedTime goes from 0 (start) to 1 (end) for non-looping animations
        if (!hasTriggered && stateInfo.normalizedTime >= 1.0f)
        {
            hasTriggered = true;
            
        }
    }


    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasTriggered = false;
    }

}
