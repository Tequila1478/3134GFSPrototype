using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a data class which ObjectBehaviour references to get values

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class ObjectData : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string taskType;
    public bool isRequired;
    public string objectName;

    [Header("Movement Settings")]
    public float speed = 2f;
    public float height = 0.01f;
    public float hoverRotation = 0.1f;

    [Header("Object Information (autosets)")]
    public CustomCursor cursor;
    public Rigidbody rb;
    public CharacterController charController;
    public AudioSource audioSource;
    public AudioClip[] soundEffects;

    [Header("Required to be seperate object please set")]
    public ParticleSystem ghostParticles;



    private void Start()
    {
        CacheComponents();
    }
    //Sets all the object information
    private void CacheComponents()
    {
        rb = GetComponent<Rigidbody>();
        charController = GetComponent<CharacterController>();
        cursor = FindObjectOfType<CustomCursor>();
        audioSource = GetComponent<AudioSource>();
        //ghostParticles = GetComponent<ParticleSystem>();
    }
}
