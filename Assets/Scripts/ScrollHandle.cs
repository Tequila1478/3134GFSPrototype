using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollHandle : MonoBehaviour
{
    public Scrollbar scrollbar;
    public float value = 0.5f;
    public void UpdateHandlePosition()
    {
        if (scrollbar != null)
            value = scrollbar.value;
    }
}
