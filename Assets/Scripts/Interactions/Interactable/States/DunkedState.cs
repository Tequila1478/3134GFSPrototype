using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/* DunkedState - State used by InteractableStateController when trash is thrown away, i.e. "dunked".
 */
public class DunkedState : State
{
    private Vector3 scaleShrink = new Vector3(0.95f, 0.98f, 0.95f); //y value is larger than x & z value for a more stylised shrink effect.

    protected override void OnEnter()
    {
        // "What was that!?"
        Debug.Log("Ran OnEnter in DunkedState");

        // Update physics
        sc.rb.useGravity = true;
        sc.rb.drag = 0;
        sc.rb.isKinematic = false;
        sc.SetCollidersAsTrigger(true);
        sc.rb.constraints = RigidbodyConstraints.None;
        //sc.ps.SetLayer(8);

        // Update particles
        sc.ToggleParticles();

        // Update held item
        if (sc.playerInteraction.itemHeld == sc)
        {
            sc.playerInteraction.itemHeld = null;
        }

        // Update layer
        sc.SetNewLayer(sc.layerWhenUnselected);

        // Set parent
        sc.transform.SetParent(sc.ps.transform, true);

        // Update progress counter
        sc.ps.IncrementTrash();
    }

    protected override void OnUpdate()
    {
        // Shrink
        sc.transform.localScale = new Vector3(sc.transform.localScale.x * scaleShrink.x, sc.transform.localScale.y * scaleShrink.y, sc.transform.localScale.z * scaleShrink.z);
        
        // Destroy
        if (sc.transform.localScale.magnitude < 0.01f)
        {
            //sc.DoDestroy(sc.gameObject);
        }
    }
    protected override void OnHurt()
    {
        // Transition to Hurt State
    }
    protected override void OnRightClick()
    {
        // Do nothing
    }
    protected override void OnExit()
    {
        // "Must've been the wind"
    }
    public override void OnHoverEnter()
    {
        // Do nothing
    }
    public override void OnHoverExit()
    {
        // Do nothing
    }
}