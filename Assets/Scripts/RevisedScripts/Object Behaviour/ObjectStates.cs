using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectStates
{
    public void Enter(ObjectBehaviour ob, ObjectData obj);
    public void Execute();
    public void Exit();

    public void HoverEnter();
    public void HoverExit();
    public void Click();
    public void Release();
}
