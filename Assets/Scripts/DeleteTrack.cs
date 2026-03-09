// remove this object when clicked

using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeleteTrack : MonoBehaviour
{
    [SerializeField]
    private bool willDestroy = true;

    private bool canDestroy;

    private StateTracker state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canDestroy = willDestroy;
        state = GameObject.FindGameObjectWithTag("State").GetComponent<StateTracker>();
    }

    // Update is called once per frame
    void Update()
    {
        // detect if object was clicked
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform.name == transform.name && canDestroy)
            {
                Destroy(gameObject);
            }
        }

        // disable deletion if in race mode
        if (state.wasChangedThisFrame())
        {
            switch (state.getState())
            {
                case 0:
                    canDestroy = willDestroy; break;
                case 1:
                    canDestroy = false; break;
            }
        }
    }

    // called by ObjectPreview to disable this effect
    void DoNotDestroy()
    {
        willDestroy = false;
        canDestroy = false;
    }
}
