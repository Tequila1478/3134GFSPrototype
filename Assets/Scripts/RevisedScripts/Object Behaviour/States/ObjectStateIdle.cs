using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStateIdle : IObjectStates
{
    ObjectData _objectData;
    ObjectBehaviour _objectBehaviour;

    public void Click()
    {
        _objectBehaviour.ChangeState(new ObjectStateRise());
    }

    public void Enter(ObjectBehaviour ob, ObjectData obj)
    {
        _objectBehaviour = ob;
        _objectData = obj;
        //turn the gravity back on
        _objectData.rb.useGravity = true;
        _objectData.rb.drag = 0;
        _objectData.tag = "Interactable";
        //Play sound
        _objectData.audioSource.clip = _objectData.soundEffects[1];
        _objectData.audioSource.PlayOneShot(_objectData.audioSource.clip);
        //Stop particles
        _objectData.ghostParticles.Stop();

        //remove reference from player
        PlayerData.instance.heldObject = null;
    }



    public void Execute()
    {
        //Do nothing
    }

    public void Exit()
    {
        //Do nothing
    }

    public void HoverEnter()
    {
        PlayerData.instance.cursor.ChangeVisual(1);
        _objectBehaviour.HighlightObject();
    }

    public void HoverExit()
    {
        PlayerData.instance.cursor.ChangeVisual(0);
        _objectBehaviour.UnHighlightObject();
    }

    public void Release()
    {
        throw new System.NotImplementedException();
    }
}
