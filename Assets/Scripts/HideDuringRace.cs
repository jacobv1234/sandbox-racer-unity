using UnityEngine;

public class HideDuringRace : MonoBehaviour
{
    private StateTracker state;
    private Renderer renderer;
    private Canvas canvas;
    // mode == 0: use renderer, mode == 1: use canvas
    private int mode = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = GameObject.FindGameObjectWithTag("State").GetComponent<StateTracker>();
    
        renderer = GetComponent<Renderer>();
        canvas = GetComponent<Canvas>();
        if (renderer == null)
        {
            mode = 1;
        } else
        {
            mode = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (state.wasChangedThisFrame())
        {
            switch (state.getState())
            {
                case 0:
                    switch (mode)
                    {
                        case 0:
                            renderer.enabled = true; break;
                        case 1:
                            canvas.enabled = true; break;
                    }

                    // enable other components
                    if (GetComponent<TrackSpawning>() != null)
                    {
                        GetComponent<TrackSpawning>().enabled = true;
                    }
                    if (GetComponent<ObjectPreview>() != null)
                    {
                        GetComponent<ObjectPreview>().enabled = true;
                    }
                    if (GetComponent<Collider>() != null)
                    {
                        GetComponent<Collider>().enabled = true;
                    }

                    break;
                case 1:
                    switch (mode)
                    {
                        case 0:
                            renderer.enabled = false; break;
                        case 1:
                            canvas.enabled = false; break;
                    }

                    // disable other components
                    if (GetComponent<TrackSpawning>() != null)
                    {
                        GetComponent<TrackSpawning>().enabled = false;
                    }
                    if (GetComponent<ObjectPreview>() != null)
                    {
                        GetComponent<ObjectPreview>().enabled = false;
                    }
                    if (GetComponent<Collider>() != null)
                    {
                        GetComponent<Collider>().enabled = false;
                    }

                    break;
            }
        }
    }
}
