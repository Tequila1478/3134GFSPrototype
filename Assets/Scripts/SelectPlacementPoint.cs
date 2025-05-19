using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEditor;
using UnityEngine;

public class SelectPlacementPoint : MonoBehaviour
{
    public PlayerInteraction player;
    public PlacementSpot place;
    public 

    // Start is called before the first frame update
    void Start()
    {
        if(place == null)
        {
            place = gameObject.GetComponentInParent<PlacementSpot>();
        }
        if(player == null)
        {
            player = FindObjectOfType<PlayerInteraction>();
        }
        //GetComponent<MeshCollider>().isTrigger = true;
        //GetComponent<MeshCollider>().sharedMesh = gameObject.GetComponentInParent<MeshCollider>().sharedMesh;
    }

    private void OnMouseEnter()
    {
        Debug.Log("Hi");
        if(player.itemHeld != null)
        {
            Debug.Log("I see what you're holding");
        }
        /*if(GetComponent<MeshCollider>().sharedMesh == null)
        {
            if (place.player.itemHeld.GetComponent<MeshCollider>().sharedMesh)
            {
                GetComponent<MeshCollider>().sharedMesh = place.player.itemHeld.GetComponent<MeshCollider>().sharedMesh;
            }
            else
            {
                Debug.Log("No Item held");
                return;
            }
        }
        place.SelectObject();)*/
    }

    private void OnMouseExit()
    {
        //place.DeselectObject();
        //GetComponent<MeshCollider>().sharedMesh = null;
    }
}
