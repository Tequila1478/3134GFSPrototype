using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleLight : MonoBehaviour, IHoverable, IClickable
{
    public Light light;
    public bool isOn = true;
    private Renderer[] renderers;
    private MaterialPropertyBlock mpb;
    private Color[] originalColors;
    private Color hoverColor = Color.white;
    public ParticleSystem sparks;

    private void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend == null)
        {
            renderers = GetComponentsInChildren<Renderer>();
        }
        else
        {
            renderers = new Renderer[] { rend };
        }

        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            Material mat = renderers[i].sharedMaterial; // or .material if you want a unique instance
            if (mat != null && mat.HasProperty("_OutlineColour"))
            {
                originalColors[i] = mat.GetColor("_OutlineColour");
            }
            else
            {
                originalColors[i] = Color.clear; // or some default value
            }
        }

    }
    public void OnClick()
    {
        ToggleLightSwitch();
    }

    public void OnHoverEnter()
    {
        HighlightObject();
    }

    public void OnHoverExit()
    {
        UnhighlightObject();
    }

    public void OnRelease()
    {

    }

    private void ToggleLightSwitch()
    {
        isOn = !isOn;
        light.enabled = isOn;
        sparks.Play();
    }

    private void HighlightObject()
    {
        foreach (var rend in renderers)
        {
            var mpb = new MaterialPropertyBlock();
            rend.GetPropertyBlock(mpb);
            mpb.SetColor("_OutlineColour", hoverColor);
            rend.SetPropertyBlock(mpb);
        }
    }

    private void UnhighlightObject()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            var mpb = new MaterialPropertyBlock();
            var rend = renderers[i];
            rend.GetPropertyBlock(mpb);
            mpb.SetColor("_OutlineColour", originalColors[i]);
            rend.SetPropertyBlock(mpb);
        }
    }
}
