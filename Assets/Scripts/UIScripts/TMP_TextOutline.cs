using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TMP_TextOutline : MonoBehaviour
{
    public TMP_Text text;

    public float outlineWidth = 0.2f;
    public Color32 textColor = new Color32(255, 128, 0, 255);

    private void OnValidate()
    {
        if (text == null)
        {
            text = GetComponent<TMP_Text>(); //Grab text component automatically
        }
        if (outlineWidth < 0) outlineWidth = 0; // Prevent outline width from going too low
        UpdateOutline();
    }

    void Awake()
    {
        UpdateOutline();
    }

    public void UpdateOutline()
    {
        if (text)
        {
            text.outlineWidth = outlineWidth;
            text.outlineColor = textColor;
        }
    }
}
