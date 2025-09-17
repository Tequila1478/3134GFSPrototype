using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class WavyAndIncreaseSize : MonoBehaviour
{
    [Header("Wave settings")]
    [SerializeField] private float amplitude = 5f;
    [SerializeField] private float frequency = 2f;
    [SerializeField] private float waveSpacing = 0.5f;

    [Header("Hover settings")]
    [SerializeField] private float hoverSize = 36f;
    [SerializeField] private float speed = 10f;

    public TMP_Text tmp;
    private float originalSize;
    private float targetSize;
    private float currentSize;

    private bool waveActive = false;

    private void Awake()
    {
        if (tmp == null) tmp = GetComponent<TMP_Text>();

        originalSize = tmp.fontSize;
        targetSize = originalSize;
        currentSize = originalSize;
    }

    private void Update()
    {
        // Smoothly grow/shrink font size
        currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * speed);
        tmp.fontSize = currentSize;

        // Start waving only after fully grown
        if (!waveActive && Mathf.Abs(currentSize - targetSize) < 0.01f && currentSize > originalSize)
        {
            waveActive = true;
        }
        else if (currentSize <= originalSize + 0.01f)
        {
            waveActive = false;
        }

        if (waveActive)
        {
            ApplyWave();
        }
    }

    private void ApplyWave()
    {
        tmp.ForceMeshUpdate();
        TMP_TextInfo textInfo = tmp.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;

            // Calculate wave offset
            float wave = Mathf.Sin(Time.time * frequency + i * waveSpacing) * amplitude;
            Vector3 offset = new Vector3(0, wave, 0);

            // Apply to all four vertices of the character
            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] += offset;
            }
        }

        // Push updated vertices back to the mesh
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            tmp.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }


    public void StartEffect()
    {
        targetSize = hoverSize;
    }

    public void EndEffect()
    {
        targetSize = originalSize;
    }

}
