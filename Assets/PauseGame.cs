using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    private bool isPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (isPaused) EndPause();
            else StartPause();
        }
    }

    private void StartPause()
    {
        Time.timeScale = 0;
        transform.GetChild(0).gameObject.SetActive(true);
        isPaused = !isPaused;
    }

    public void EndPause()
    {
        Time.timeScale = 1;
        transform.GetChild(0).gameObject.SetActive(false);
        isPaused = !isPaused;
    }
}
