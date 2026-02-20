// remove this object when clicked

using UnityEngine;
using UnityEngine.InputSystem;

public class DeleteTrack : MonoBehaviour
{
    private bool willDestroy = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform.name == transform.name && willDestroy)
            {
                Destroy(gameObject);
            }
        }
    }

    // called by ObjectPreview to disable this effect
    void DoNotDestroy()
    {
        willDestroy = false;
    }
}
