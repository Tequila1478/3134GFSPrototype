using UnityEngine;

public class StateController : MonoBehaviour
{
    State currentState;

    public DefaultState defaultState = new DefaultState();
    public HoverState hoverState = new HoverState();
    public PushedState pushedState = new PushedState();
    public PoppedState poppedState = new PoppedState();
    public HoopedState hoopedState = new HoopedState();

    private void Start()
    {
        ChangeState(defaultState);
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.OnStateUpdate();
        }
    }

    public void ChangeState(State newState)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
        }
        currentState = newState;
        currentState.OnStateEnter(this);
    }
}

public abstract class State
{
    public StateController sc;

    public void OnStateEnter(StateController stateController)
    {
        // Code placed here will always run
        sc = stateController;
        OnEnter();
    }

    protected virtual void OnEnter()
    {
        // Code placed here can be overridden
    }

    public void OnStateUpdate()
    {
        // Code placed here will always run
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
        // Code placed here can be overridden
    }

    public void OnStateHurt()
    {
        // Code placed here will always run
        OnHurt();
    }

    protected virtual void OnHurt()
    {
        // Code placed here can be overridden
    }

    public void OnStateExit()
    {
        // Code placed here will always run
        OnExit();
    }

    protected virtual void OnExit()
    {
        // Code placed here can be overridden
    }
}