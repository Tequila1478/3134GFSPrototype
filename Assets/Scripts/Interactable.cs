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

    public bool hasSetSpot = false;
    public bool movingToSetSpot = false;
    public Vector3 newDirection;
    public Vector3 edgeOfObject;

    //Other variables
    private PlayerInteraction playerStatus_PI;

    //Movement variables
    public ObjectInteractions oi;


    // Start is called before the first frame update
    void Start()
    {
        edgeOfObject = new Vector3(this.GetComponent<MeshRenderer>().localBounds.extents.x * this.transform.localScale.x, this.GetComponent<MeshRenderer>().localBounds.extents.y * this.transform.localScale.y, this.GetComponent<MeshRenderer>().localBounds.extents.z * this.transform.localScale.z) ;


        //Set Down Parameters
        routeToGo = 0;
        tParam = 0f;
        speedModifier = 0.5f;
        coroutineAllowed = false;

        //Checks to see if the outline material is applies
        if (outlineMat == null)
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

        if(oi == null)
        {
            oi = gameObject.GetComponent<ObjectInteractions>();
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
            else if (!isMoving)
            {
                //Controls the upwards movement when clicked
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, (transform.position.y + height * 10), transform.position.z), speed * 50 * Time.deltaTime);
            }
        }

        if (playerStatus_PI.itemHeld == this && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E)))
        {
            isMoving = true;
        }

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.E))
        {
            isMoving = false;
            gameObject.GetComponent<CharacterController>().enabled = false;
        }

        if (isMoving)
        {
            gameObject.GetComponent<CharacterController>().enabled = true;
            oi.Move();
            //Add control logic to this
        }

        if (coroutineAllowed)
        {
            StartCoroutine(GoByTheRoute(routeToGo));
        }

        if (movingToSetSpot)
        {
            float singleStep = speed * 0.1f * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(this.transform.forward, newDirection, singleStep, 0));
            if (Vector3.Angle(transform.forward, newDirection) < 0.1f) movingToSetSpot = false;
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
            this.tag = "Held Item";
            //float
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            gameObject.GetComponent<Rigidbody>().drag = 4;
            gameObject.GetComponent<Rigidbody>().isKinematic = false;


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
            this.tag = "Interactable";
            Debug.Log("Object dropped");
            
            if (hasSetSpot)
            {
                coroutineAllowed = true;
                movingToSetSpot = true;
            }
            else
            {
                gameObject.GetComponent<Rigidbody>().useGravity = true;
                gameObject.GetComponent<Rigidbody>().drag = 0;
            }

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


    //Set Down stuff
    [SerializeField]

    public Vector3[] routes;
    private int routeToGo;
    private float tParam;
    private Vector3 objectPosition;
    private float speedModifier;
    private bool coroutineAllowed;

    private IEnumerator GoByTheRoute(int routeNum)
    {

        coroutineAllowed = false;

        Vector3 p0 = routes[0];
        Vector3 p1 = routes[1];
        Vector3 p2 = routes[2];
        Vector3 p3 = routes[3];

        while (tParam < 1)
        {
            tParam += Time.deltaTime * speedModifier;
            objectPosition = Mathf.Pow(1 - tParam, 3) * p0 + 3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 + 3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 + Mathf.Pow(tParam, 3) * p3;
            transform.position = objectPosition;
            yield return new WaitForEndOfFrame();
        }

        tParam = 0;
        speedModifier = speedModifier * 0.90f;
        routeToGo += 1;

        if (routeToGo >= 1)
        {
            coroutineAllowed = false;
            routeToGo = 0;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            
        }
        else coroutineAllowed = true;
    }
}
