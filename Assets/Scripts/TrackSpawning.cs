using UnityEngine;
using UnityEngine.InputSystem;

public class TrackSpawning : MonoBehaviour
{
    // prefabs for each tile type
    [SerializeField]
    private GameObject blank;
    [SerializeField]
    private GameObject straight;
    [SerializeField]
    private GameObject corner;
    [SerializeField]
    private GameObject checkpoint;
    [SerializeField]
    private GameObject finish;

    [SerializeField]
    private string selected;

    [SerializeField]
    private float rotation = 0.0f;

    [SerializeField]
    private GameObject child;

    [SerializeField]
    private Vector3 position;

    private InputSystem_Actions actions;


    // create a tile
    void create_object()
    {
        GameObject selectedObject;
        switch (selected)
        {
            case "blank":
                selectedObject = blank;
                break;
            case "straight":
                selectedObject = straight;
                break;
            case "corner":
                selectedObject = corner;
                break;
            case "checkpoint":
                selectedObject = checkpoint;
                break;
            case "finish":
                selectedObject = finish;
                break;
            default:
                selectedObject = blank;
                break;
        }

        child = Instantiate(selectedObject, position, Quaternion.identity);
        Vector3 rotateVector = new Vector3(0, rotation, 0);
        child.transform.Rotate(rotateVector);
    }
    
    void rotate()
    {
        rotation += 90;
        if (rotation >= 360){
            rotation = 0.0f;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        position = transform.position;
        actions = new InputSystem_Actions();
        actions.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        // OnMouseDown doesn't work with InputSystem
        // after a lot of trial and error and documentation
        // I found a solution on StackOverflow at
        // https://stackoverflow.com/questions/63872772/how-replace-onmousedown-in-unitys-new-input-system
        // the next 4 lines come almost directly from there
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform.name == transform.name)
            {
                create_object();
            }
        }

        // rotate command
        if (actions.Player.Respawn.WasPressedThisFrame())
        {
            rotate();
        }
    }
}
