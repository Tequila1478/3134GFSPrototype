using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public string menuName = "MainMenu";


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            switch (menuName)
            {
                case "SettingsScreen":
                    FindObjectOfType<AnimatorController_BG>().FromSettingsMenu();
                    break;
                case "HowToPlayScreen":
                    FindObjectOfType<AnimatorController_BG>().FromSettingsMenu();
                    break;
                default:
                    break;
            }
        }
    }

    public void UpdateMenu(string name)
    {
        menuName = name;
    }
}
