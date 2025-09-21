using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class CameraNumberTracker : MonoBehaviour
{
    private CameraCinemaSwitch css;
    private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
       css = FindObjectOfType<CameraCinemaSwitch>();
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (css.currentSpecialCamera != -1)
        {
            text.text = "?";
        }
        else
        {
            text.text = ((int)(css.currentCamera) + 1).ToString();
        }
        
        if (Input.GetMouseButtonDown(0)) // or just run every frame if you like
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            Debug.Log("UI raycast hit:");
            foreach (RaycastResult r in results)
            {
                Debug.Log(r.gameObject);
            }
        }
        
    }
}
