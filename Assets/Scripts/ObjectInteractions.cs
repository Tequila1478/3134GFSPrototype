using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Rendering.Universal.Internal;

public class ObjectInteractions : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed = 5;
    private PlayerInteraction playerStatus_PI;
    private CharacterController characterController;
    private Vector3 playerInput;
    private Vector3 upInput;


    [SerializeField] private float placementRadius = 2f;
    [SerializeField] private LayerMask glowLayer = 14;

    private List<GameObject> activeGlows = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        glowLayer = 1 << 8;
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();


        if (playerStatus_PI == null)
        {
            playerStatus_PI = FindObjectOfType<PlayerInteraction>();
            //To be used when implementing movement into the interactable script
        }

    }

    // Update is called once per frame
    void Update()
    {
        //Move();
    }


    public void Move()
    {
        if (characterController && Input.GetKey(KeyCode.LeftShift))
        {
            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");
            float upInput = Input.GetAxis("Jump");

            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;
            Vector3 up = Camera.main.transform.up;
            forward.y = 0;
            right.y = 0;
            forward = forward.normalized;
            right = right.normalized;
            up = up.normalized;

            Vector3 forwardRelativeInput = verticalInput * forward;
            Vector3 rightRelativeInput = horizontalInput * right;
            Vector3 upRelativeInput = upInput * up;

            rb.constraints = RigidbodyConstraints.FreezeRotation;
            //when rotation is not frozen the cube spins all over the place - I
            Vector3 playerInput = forwardRelativeInput + rightRelativeInput + upRelativeInput;
            characterController.Move(playerInput * Time.deltaTime * moveSpeed);

            //Add a way for 

            ShowNearbyPlacementSpots();
        }
    }
    public void ClearPlacementSpots()
    {
        foreach(GameObject glow in activeGlows)
        {
            glow.SetActive(false);
        }

        activeGlows.Clear();
    }
    private void ShowNearbyPlacementSpots()
    {
        foreach (GameObject glow in activeGlows)
        {
            glow.SetActive(false);
        }

        activeGlows.Clear();

        Collider[] hits = Physics.OverlapSphere(transform.position, placementRadius, glowLayer);

        foreach (Collider hit in hits)
        {
            PlacementSpot spot = hit.GetComponent<PlacementSpot>();
            if (spot != null && !spot.claimed)
            {
                Debug.Log(spot);
                //GameObject glow = Instantiate(glowPrefab, hit.transform.position + Vector3.up * 0.1f, Quaternion.identity);
                GameObject glow = spot.highlightVisualisation;
                glow.SetActive(true);
                activeGlows.Add(glow);
            }
        }
    }

    public void FreezeObject()
    {
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        characterController = null;
    }

    public void ReEnableObject()
    {
        characterController = GetComponent<CharacterController>();

        if (rb)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }


}
