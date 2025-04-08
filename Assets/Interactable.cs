using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    public Material outlineMat;
    public float speed = 2f;
    public float height = 0.001f;
    public float rotation = 0.1f;

    private bool moveComplete = false;
    private Vector3 pos;

    private PlayerInteraction playerStatus_PI;

    public bool floating = false;

    private Material originalMat;
    // Start is called before the first frame update
    void Start()
    {
        if(outlineMat == null)
        {
            Debug.LogError("Error: No Outline Material Set for object " + this.gameObject);
        }

        if(playerStatus_PI == null)
        {
            playerStatus_PI = FindObjectOfType<PlayerInteraction>();
        }


        outlineMat.SetTexture("_MainTex" , gameObject.GetComponent<Renderer>().material.mainTexture);
    }

    // Update is called once per frame
    void Update()
    {
        if (floating)
        {
            if (moveComplete)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Sin(Time.time * speed) * height, transform.position.z);
                transform.Rotate(0, 6.0f * 1f * Time.deltaTime, 0);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, (transform.position.y + height), transform.position.z), speed * 50 * Time.deltaTime);
            }
        }
    }

    /* Things to do:
     * Apply outline on mouse hover
     * On mouse click apply hover
     * Throw object
     * Move object
     * Spring lock into place
    */

    private void OnMouseEnter()
    {
        if (!playerStatus_PI.isHolding)
        {
            ApplyMaterial();
            Debug.Log("Item Highlighted");
        }
    }

    private void OnMouseDown()
    {
        if (!floating && playerStatus_PI.itemHeld == null)
        {
            Debug.Log("Object floating");
            //move object up to position
            //gameObject.GetComponent<Rigidbody>().useGravity = true;
            //start bobbing
            floating = true;
            playerStatus_PI.isHolding = true;
            playerStatus_PI.itemHeld = this;
            //float
            gameObject.GetComponent<Rigidbody>().useGravity = false;

        }
        else if(floating)
        {
            moveComplete = false;
            floating = false;
            playerStatus_PI.isHolding = false;
            playerStatus_PI.itemHeld = null;
            Debug.Log("Object dropped");

            gameObject.GetComponent<Rigidbody>().useGravity = true;

        }
        
    }

    private void OnMouseUp()
    {
        Vector3 pos = gameObject.transform.position;

        if(floating) moveComplete = true;

        //gameObject.GetComponent<Rigidbody>().useGravity = true;
        //return to place - try using spring
    }

    private void OnMouseExit()
    {
        if (!playerStatus_PI.isHolding)
        {
            Debug.Log("Item Highlight removed");

            RemoveMaterial();
        }
    }

    void RemoveMaterial()
    {
        Material[] matsArr = gameObject.GetComponent<Renderer>().materials;

        Material[] newMatArr = new Material[matsArr.Length - 1];
        for (int i = 0; i < matsArr.Length - 1; i++) 
        {
            newMatArr[i] = matsArr[i];
        }
        gameObject.GetComponent<Renderer>().materials = newMatArr;
    }

    void ApplyMaterial()
    {
        outlineMat.SetTexture("_MainTex", gameObject.GetComponent<Renderer>().material.mainTexture);
        //find material array
        Material[] matsArr = gameObject.GetComponent<Renderer>().materials;
        //Apply Mat
        Material[] newMatToAdd = {outlineMat};
        Material[] newMatArr = matsArr.Concat(newMatToAdd).ToArray();
        gameObject.GetComponent<Renderer>().materials = newMatArr;
    }
}
