using UnityEngine;

public class StateTracker : MonoBehaviour
{
    // 0: build, 1: race
    [SerializeField]
    private int state;
    private int nextstate;
    [SerializeField]
    private bool changedThisFrame;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = 0;
        nextstate = 0;
    }

    private void Update()
    {
        if (state != nextstate)
        {
            changedThisFrame = true;
            state = nextstate;
        }
        else
        {
            changedThisFrame = false;
        }
    }

    public int getState()
    {
        return state;
    }

    public void setState(int state)
    {
        this.nextstate = state;
    }

    public void toggleState()
    {
        nextstate = Mathf.Abs(state - 1);
    }

    public bool wasChangedThisFrame()
    {
        return changedThisFrame;
    }
}


