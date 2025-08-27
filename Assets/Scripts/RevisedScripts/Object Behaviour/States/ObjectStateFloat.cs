using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStateFloat : IObjectStates
{
    ObjectData _objectData;
    ObjectBehaviour _objectBehaviour;
    public void Enter(ObjectBehaviour ob, ObjectData obj)
    {
        _objectData = obj;
        _objectBehaviour = ob;
    }

    public void Execute()
    {
        float floatY = Mathf.Sin(Time.time * _objectData.speed) * _objectData.height;
        _objectData.transform.position += new Vector3(0, floatY, 0);
        _objectBehaviour.transform.Rotate(0, _objectData.hoverRotation * Time.deltaTime, 0);
    }

    public void Exit()
    {
        //Do nothing
    }

    public void HoverEnter()
    {

    }

    public void HoverExit()
    {

    }

    public void Click()
    {

    }

    public void Release()
    {
        _objectBehaviour.ChangeState(new ObjectStateIdle());
    }
}
