using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStateRise : IObjectStates
{
    ObjectData _objectData;
    ObjectBehaviour _objectBehaviour;

    public void Click()
    {
        //Nothing
    }

    public void Enter(ObjectBehaviour ob, ObjectData obj)
    {
        _objectBehaviour = ob;
        _objectData = obj;
        //turn the gravity off
        _objectData.rb.useGravity = false;
        _objectData.rb.drag = 4f;
        _objectData.rb.isKinematic = false;

        //set reference from player
        PlayerData.instance.heldObject = _objectData;
        _objectData.tag = "Held Item";
        _objectData.ghostParticles.Play();
    }

    //Moves object upward
    public void Execute()
    {
        float targetY = _objectBehaviour.transform.position.y + _objectData.height * 10;
        _objectBehaviour.transform.position = Vector3.MoveTowards(_objectBehaviour.transform.position, new Vector3(_objectBehaviour.transform.position.x, targetY, _objectBehaviour.transform.position.z), _objectData.speed * 50 * Time.deltaTime);
    }

    public void Exit()
    {
        //Do nothing
    }

    public void HoverEnter()
    {
        throw new System.NotImplementedException();
    }

    public void HoverExit()
    {
        throw new System.NotImplementedException();
    }

    public void Release()
    {
        _objectBehaviour.ChangeState(new ObjectStateFloat());
    }

    private bool IsAnyMovementKeyPressed()
    {
        return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) ||
               Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E);
    }
}
