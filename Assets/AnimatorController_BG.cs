using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController_BG : MonoBehaviour
{
    [Header("All animators to control")]
    [SerializeField] private Animator[] animators;
    
    public bool isSetting = false;
    private bool _isSetting = false;

    private const string transitionParam = "TransitionToSettings";

    private void Update()
    {
        if(_isSetting != isSetting)
        {
            TransitionToSettings(isSetting);
        }
        _isSetting = isSetting;
    }

    public void ToSettingsMenu()
    {
        isSetting = true;
    }

    public void FromSettingsMenu()
    {
        isSetting = false;
    }


    public void TransitionToSettings(bool condition)
    {
        foreach (Animator anim in animators)
        {
            if (anim != null)
                anim.SetBool(transitionParam, condition);
        }
    }

}
