using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeUI : MonoBehaviour
{
    public GameObject settingsUI;
    public GameObject howToPlayUI;

    public void DisplayHowToPlayUI()
    {
        howToPlayUI.SetActive(true);
    }

    public void DisplaySettingUI()
    {
        settingsUI.SetActive(true);
    }

    public void RemoveHowToPlayUI()
    {
        howToPlayUI.SetActive(false);
    }

    public void RemoveSettingUI()
    {
        settingsUI.SetActive(false);
    }
}
