using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeUI : MonoBehaviour
{
    public GameObject settingsUI;
    public GameObject howToPlayUI;
    public GameObject mainMenuUI;

    public void DisplayHowToPlayUI()
    {
        howToPlayUI.SetActive(true);
    }

    public void DisplaySettingUI()
    {
        settingsUI.SetActive(true);
    }

    public void DisplayMainMenuUI()
    {
        mainMenuUI.SetActive(true);
    }

    public void RemoveMainMenuUI()
    {
        mainMenuUI.SetActive(false);
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
