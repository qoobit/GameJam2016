using UnityEngine;
using System.Collections;

public class StateEntity : MonoBehaviour
{
    public int currentState = 0;
    public int nextState = 0;
    public float nextStateTime = float.MaxValue;

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        if (Time.time >= nextStateTime)
            currentState = nextState;
    }

    public void setCurrentState(int state)
    {
        currentState = state;
        nextState = state;
        nextStateTime = float.MaxValue;
    }

    public void setNextState(int state, float delay, bool forceUpdate = false)
    {
        if (nextState != state || forceUpdate)
        {
            nextState = state;
            nextStateTime = Time.time + delay;
        }
    }
}
