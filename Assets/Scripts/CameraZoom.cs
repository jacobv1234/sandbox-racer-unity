using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private StateTracker state;
    private Camera camera;
    private float target;

    [SerializeField]
    private float zoomRate = 1.0f;
    [SerializeField]
    private float buildZoom = 48.0f;
    [SerializeField]
    private float raceZoom = 35.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = GameObject.FindGameObjectWithTag("State").GetComponent<StateTracker>();
        camera = Camera.main;
        target = buildZoom;
    }

    // Update is called once per frame
    void Update()
    {
        if (state.wasChangedThisFrame())
        {
            switch (state.getState())
            {
                case 0:
                    target = buildZoom; break;
                case 1:
                    target = raceZoom; break;
            }
        }

        if (camera.orthographicSize > target)
        {
            camera.orthographicSize -= zoomRate * Time.deltaTime;
        }
        if (camera.orthographicSize < target)
        {
            camera.orthographicSize += zoomRate * Time.deltaTime;
        }
        if (camera.orthographicSize < raceZoom)
        {
            camera.orthographicSize = raceZoom;
        }
        if (camera.orthographicSize > buildZoom)
        {
            camera.orthographicSize = buildZoom;
        }
    }
}
