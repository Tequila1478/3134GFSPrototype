using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;

    //Reference to held item
    public ObjectData heldObject;
    //Reference to selected movement point
    public ObjectLocation objectLocation;
    //Reference to cursor Object
    public CustomCursor cursor;

    void Start()
    {
        instance = this;
        cursor = FindObjectOfType<CustomCursor>();
    }
}
