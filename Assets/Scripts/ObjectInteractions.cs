using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            //To be used when implementing movement into the interactable script - I
        }

    }

    // Update is called once per frame
    void Update()
    {
        //Move();
        //not really sure if I needed to make this its own function but I did it anyway - I 
    }


    //Can we make it based on the camera? That way we can add basic camera controls if we want later?
    public void Move()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        //when rotation is not frozen the cube spins all over the place - I
        playerInput = new Vector3(Input.GetAxis("Horizontal") * -1, Input.GetAxis("Jump"), Input.GetAxis("Vertical") * -1);
        characterController.Move(playerInput * Time.deltaTime * moveSpeed);
        //The horizontal and vertical axes are multiplied by -1, otherwise the movement is inverted and it feels weird - I
    }


    //The character controller seems to mess with an object with the interactable script while floating so if we want to use this movement method we'll have to do something about that - I

}
