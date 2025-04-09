using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    //Material Variables
    public Material outlineMat;
    public Material originalMat;

    //Floating variables
    public float speed = 2f;
    public float height = 0.01f;
    public float rotation = 0.1f;

    private bool isMoving = false;
    private bool moveComplete = false;
    private Vector3 pos;
    public bool floating = false;

    //Other variables
    private PlayerInteraction playerStatus_PI;


    // Start is called before the first frame update
    void Start()
    {
        //Checks to see if the outline material is applies
        if(outlineMat == null)
        {
            Debug.LogError("Error: No Outline Material Set for object " + this.gameObject);
        }
        if (originalMat == null)
        {
            originalMat = this.gameObject.GetComponent<Renderer>().material;
        }
        //Checks to see if it can reference the player interaction script
        if (playerStatus_PI == null)
        {
            playerStatus_PI = FindObjectOfType<PlayerInteraction>();
        }


        //redundant but I'm scared to delete
        outlineMat.SetTexture("_MainTex" , gameObject.GetComponent<Renderer>().material.mainTexture);
    }

    // Update is called once per frame
    void Update()
    {
        if (floating)
        {
            if (moveComplete && !isMoving)
            {
                //Controls the floating logic
                transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Sin(Time.time * speed) * height, transform.position.z);
                transform.Rotate(0, 6.0f * 1f * Time.deltaTime, 0);
                //Temporarily disable it
            }
            else
            {
                //Controls the upwards movement when clicked
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, (transform.position.y + height * 10), transform.position.z), speed * 50 * Time.deltaTime);
            }
        }

        if (isMoving)
        {
            //Add control logic to this
        }
    }

    /* Things to do:
     * Apply outline on mouse hover
     * On mouse click apply hover
     * Throw object
     * Move object
     * Spring lock into place
    */

    //Highlights the object when the mouose hovers, and no other object is selected
    private void OnMouseEnter()
    {
        if (!playerStatus_PI.isHolding)
        {
            ApplyMaterial();
            Debug.Log("Item Highlighted");
        }
    }

    //Starts the floating logic --> update doesn't trigger until this is called
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
        
    }


    //Removes highlight and stops it moving up
    private void OnMouseUp()
    {
        if (moveComplete)
        {
            moveComplete = false;
            floating = false;
            playerStatus_PI.isHolding = false;
            playerStatus_PI.itemHeld = null;
            Debug.Log("Object dropped");

            gameObject.GetComponent<Rigidbody>().useGravity = true;

        }
        else
        {
            Vector3 pos = gameObject.transform.position;

            if (floating) moveComplete = true;

            //gameObject.GetComponent<Rigidbody>().useGravity = true;
            //return to place - try using spring
        }
    }

    //Only triggers if no object is selected
    private void OnMouseExit()
    {
        if (!playerStatus_PI.isHolding)
        {
            Debug.Log("Item Highlight removed");

            RemoveMaterial();
        }
    }

    //Material stuff
    void RemoveMaterial()
    {
        //Material[] matsArr = gameObject.GetComponent<Renderer>().materials;

        //Material[] newMatArr = new Material[matsArr.Length - 1];
        //for (int i = 0; i < matsArr.Length - 1; i++) 
        //{
        //    newMatArr[i] = matsArr[i];
        //}
        //gameObject.GetComponent<Renderer>().materials = newMatArr;
        gameObject.GetComponent<Renderer>().material = originalMat;
    }

    //Material stuff
    void ApplyMaterial()
    {
        outlineMat.SetTexture("_MainTex", gameObject.GetComponent<Renderer>().material.mainTexture);
        //find material array
        //Material[] matsArr = gameObject.GetComponent<Renderer>().materials;
        //Apply Mat
        //Material[] newMatToAdd = {outlineMat};
        //Material[] newMatArr = matsArr.Concat(newMatToAdd).ToArray();
        //gameObject.GetComponent<Renderer>().materials = newMatArr;
        gameObject.GetComponent<Renderer>().material = outlineMat;
    }
}
