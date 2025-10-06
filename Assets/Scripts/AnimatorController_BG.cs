using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatorController_BG : MonoBehaviour
{
    [Header("All animators to control")]
    [SerializeField] private Animator[] animators;

    public bool isSetting = false;
    private bool _isSetting = false;
    public bool isHowToPlay = false;
    public bool _isHowToPlay = false;


    private const string transitionParam = "TransitionToSettings";
    private const string transitionParam2 = "TransitionToHowToPlay";


    private void Update()
    {
        if(_isSetting != isSetting)
        {
            TransitionToSettings(transitionParam, isSetting);
        }
        _isSetting = isSetting;

        if (_isHowToPlay != isHowToPlay)
        {
            TransitionToSettings(transitionParam2, isHowToPlay);
        }
        _isHowToPlay = isHowToPlay;
    }

    public void ToSettingsMenu()
    {
        isSetting = true;
    }

    public void FromSettingsMenu()
    {
        isSetting = false;
        isHowToPlay = false;
    }

    public void ToHowToPlayMenu()
    {
        isHowToPlay = true;
    }

    public void TransitionToSettings(string transitionParam, bool condition)
    {
        foreach (Animator anim in animators)
        {
            if (anim != null)
                anim.SetBool(transitionParam, condition);
        }
    }
}
