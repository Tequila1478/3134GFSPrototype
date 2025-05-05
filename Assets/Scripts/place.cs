using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class place : MonoBehaviour
{
    private PlayerInteraction pi;
    private Vector3 placementPoint;
    private Vector3 direction;

    private bool withinRange;

    public bool claimed = false;
    private GameObject otherObject = null;

    void Start()
    {
        if(pi == null) pi = GetComponent<PlayerInteraction>();
        placementPoint = gameObject.transform.position;
        direction = gameObject.transform.up;
    }

    // Update is called once per frame
    void Update()
    {
        if(otherObject != null)
        {

        }
    }

    private void OnMouseEnter()
    {
        Debug.Log("Mouse over object");
    }

}
