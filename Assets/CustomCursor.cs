using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomCursor : MonoBehaviour
{
    public Sprite cursorVisualIdle;
    public Sprite cursorVisualInteract;

    public Vector3 cursorAdjustment;

    public 
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Transform>().position = Input.mousePosition + cursorAdjustment;
    }

    public void ChangeVisual(int newVisual)
    {
        if (newVisual == 0)
        {
            this.GetComponent<Image>().sprite = cursorVisualIdle;
        }
        if(newVisual == 1)
        {
            this.GetComponent<Image>().sprite = cursorVisualInteract;
        }
    }
}
