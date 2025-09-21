using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDrawer : MonoBehaviour, IClickable, IHoverable
{
    public Animator animator;
    public string animatorBoolName;
    public bool animatorState = false;
    public void OnClick()
    {
        animatorState = !animatorState;
        animator.SetBool(animatorBoolName, animatorState);
    }

    public void OnHoverEnter()
    {
        //throw new System.NotImplementedException();
    }

    public void OnHoverExit()
    {
        //throw new System.NotImplementedException();
    }

    public void OnRelease()
    {
        //throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
