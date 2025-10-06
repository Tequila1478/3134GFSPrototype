using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InreaseTextOnHover : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float hoverSize = 36f;
    [SerializeField] private float speed = 10f;

    private float originalSize;
    private float targetSize;
    private float currentSize;

    private WavyText wavy;

    private void Awake()
    {
        if (text == null)
            text = GetComponentInChildren<TMP_Text>();

        originalSize = text.fontSize;
        targetSize = originalSize;
        currentSize = originalSize;
        wavy = GetComponent<WavyText>();
    }

    private void Update()
    {
        // Smooth interpolation stored in our own float
        currentSize = Mathf.Lerp(currentSize, targetSize, Time.unscaledDeltaTime * speed);
        text.fontSize = currentSize;
    }

    public void IncreaseText()
    {
        Debug.Log("Hovering started");
        targetSize = hoverSize;
    }

    public void DecreaseText()
    {
        targetSize = originalSize;
    }
}
