using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;

public class OpenDrawer : MonoBehaviour, IClickable, IHoverable
{
    public Animator animator;
    public string animatorBoolName;
    public bool animatorState = false;
    public GameObject[] toggledObjects = new GameObject[0];

    public CinemachineVirtualCamera specialCamera;

    public bool enablePlacementPointsOnOpen = true;

    private PlacementSpot[] childPlacementSpots;
    private Coroutine triggerPlacementSpots;

    public AudioClip InteractNoise;

    public AudioManager audio_AM;

    // Update is called once per frame
    void Update()
    {
        if (animatorState && (CameraCinemaSwitch.FindIndexOfSpecialCamera(specialCamera) != CameraCinemaSwitch.instance.currentSpecialCamera))
        {
            OnClick(false);
        }
    }

    public void OnClick()
    {
        OnClick(!animatorState);

        audio_AM.PlaySFX(InteractNoise);
    }
    public void OnClick(bool active)
    {
        animatorState = active;
        animator.SetBool(animatorBoolName, animatorState);
        if(triggerPlacementSpots != null) StopCoroutine(triggerPlacementSpots);
        ToggleObjects(animatorState);
        triggerPlacementSpots = StartCoroutine(TogglePlacementSpots(active));

    }

    // Start is called before the first frame update
    void Start()
    {
        ToggleObjects(animatorState);

        childPlacementSpots = GetComponentsInChildren<PlacementSpot>();

        triggerPlacementSpots = StartCoroutine(TogglePlacementSpots(false));


        audio_AM = FindObjectOfType<AudioManager>();
    }

    public void ToggleObjects(bool boolean)
    {
        // Set active of toggled objects
        foreach (GameObject obj in toggledObjects)
        {
            obj.SetActive(boolean);
        }
    }

    public void OnHoverEnter()
    {
        //throw new System.NotImplementedException();
    }

    public void OnHoverExit()
    {
        //throw new System.NotImplementedException();
    }

    public void OnRelease()
    {
        //throw new System.NotImplementedException();
    }

    public IEnumerator TogglePlacementSpots(bool active)
    {
        if (!active)
        {
            yield return new WaitForSecondsRealtime(2);


            foreach (PlacementSpot ps in childPlacementSpots)
            {
                ps.transform.GetChild(1).gameObject.GetComponent<MeshFilter>().mesh = null;
            }

            foreach (PlacementSpot ps in childPlacementSpots)
            {
                ps.gameObject.SetActive(active);
            }
        }

        else
        {
            foreach (PlacementSpot ps in childPlacementSpots)
            {
                ps.gameObject.SetActive(active);
            }
        }


    }
}
