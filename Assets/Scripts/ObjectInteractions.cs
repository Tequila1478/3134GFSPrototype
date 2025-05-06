using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class ObjectInteractions : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed = 5;
    private PlayerInteraction playerStatus_PI;
    private CharacterController characterController;
    private Vector3 playerInput;
    private Vector3 upInput;



    // Start is called before the first frame update
    void Start()
    {
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


    //Can we make it based on the camera? That way we can add basic camera controls if we want later? 
    //Done
    public void Move()
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
    }




}
