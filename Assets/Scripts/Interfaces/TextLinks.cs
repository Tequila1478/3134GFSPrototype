using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextLinks : MonoBehaviour
{
    public void OpenURL(string URL)
    {
        Debug.Log("OpenURL: " + URL);
        Application.OpenURL(URL);
    }
    public void OpenURL(TMP_Text text)
    {
        Debug.Log("OpenURL: " + text.text);
        Application.OpenURL(text.text);
    }
}
