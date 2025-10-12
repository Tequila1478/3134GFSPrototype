using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatorController_BG : MonoBehaviour
{
    [Header("All animators to control")]
    [SerializeField] private Animator[] animators;

    public enum InMenu
    {
        isNone,
        isSetting,
        outSetting,
        isHowToPlay,
        outHowToPlay,
        isCredits,
        outCredits
    }
    public InMenu inMenu = InMenu.isNone;
    [NonSerialized] public InMenu _inMenu = InMenu.isNone;

    /*public bool isSetting = false;
    private bool _isSetting = false;
    public bool isHowToPlay = false;
    public bool _isHowToPlay = false;*/


    private const string transitionParamSettings = "TransitionToSettings";
    private const string transitionParamHowToPlay = "TransitionToHowToPlay";
    private const string transitionParamCredits = "TransitionToCredits";

    private void Start()
    {
        _inMenu = inMenu;
    }

    private void Update()
    {
        if(_inMenu != inMenu)
        {
            TransitionToMenu(inMenu);
        }
        _inMenu = inMenu;
    }

    public void OpenMenu(InMenu newMenu)
    {
        inMenu = newMenu;
    }
    public void OpenMenu(string newMenu)
    {
        if (System.Enum.TryParse(newMenu, out InMenu parsed_enum))
        {
            inMenu = parsed_enum;
        }
    }
    public void ToSettingsMenu()
    {
        inMenu = InMenu.isSetting;
    }

    public void FromSettingsMenu()
    {
        inMenu = InMenu.outSetting;
    }

    public void ToHowToPlayMenu()
    {
        inMenu = InMenu.isHowToPlay;
    }


    public void TransitionToMenu(InMenu newMenu)
    {
        switch (newMenu)
        {
            case (InMenu.isSetting):
                TransitionToMenu(transitionParamSettings, true);
                break;
            case (InMenu.outSetting):
                TransitionToMenu(transitionParamSettings, false);
                inMenu = InMenu.isNone;
                _inMenu = InMenu.isNone;
                break;
            case (InMenu.isHowToPlay):
                TransitionToMenu(transitionParamHowToPlay, true);
                break;
            case (InMenu.outHowToPlay):
                TransitionToMenu(transitionParamHowToPlay, false);
                inMenu = InMenu.isNone;
                _inMenu = InMenu.isNone;
                break;
            case (InMenu.isCredits):
                TransitionToMenu(transitionParamCredits, true);
                break;
            case (InMenu.outCredits):
                TransitionToMenu(transitionParamCredits, false);
                inMenu = InMenu.isNone;
                _inMenu = InMenu.isNone;
                break;
            default: //InMenu.None
                Debug.Log("No transition performed.");
                break;
        }
    }
    public void TransitionToMenu(string nextTransitionParam, bool entering)
    {
        foreach (Animator anim in animators)
        {
            if (anim != null)
            {
                anim.SetBool(nextTransitionParam, entering);
            }
        }
    }
}
