using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

// very similar to TrackSpawning.cs
// though keeps an object permanently in view

public class ObjectPreview : MonoBehaviour
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

    private float rotation = 0.0f;
    private InputSystem_Actions actions;
    private GameObject child;
    private string selected = "straight";
    private int selectedIndex = 1;
    private Vector3 position;

    private string[] tools = { "blank","straight","corner","checkpoint","finish"};
    private GameObject[] objects;

    void rotateRight()
    {
        rotation += 90;
        if (rotation >= 360)
        {
            rotation = 0.0f;
        }
        updatePreview();
    }
    void rotateLeft()
    {
        rotation -= 90;
        if (rotation < 0)
        {
            rotation = 270.0f;
        }
        updatePreview();
    }

    void updatePreview()
    {
        if (child != null)
        {
            Destroy(child); // remove existing object if there is one
        }
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

        // disables click-to-remove on the preview object
        child.SendMessage("DoNotDestroy", SendMessageOptions.DontRequireReceiver);

        // disable spawning a car on the preview
        if (child.GetComponent<CarSpawner>() != null)
        {
            Destroy(child.GetComponent<CarSpawner>());
        }

        child.AddComponent<HideDuringRace>();

        // also update the editor tiles with the new settings
        updateTiles();
    }

    void scrollToolsDown()
    {
        if (selectedIndex < tools.Length - 1)
        {
            selectedIndex += 1;
        }
        else
        {
            selectedIndex = 0;
        }
        selected = tools[selectedIndex];
        updatePreview();
    }

    void scrollToolsUp()
    {
        if (selectedIndex > 0)
        {
            selectedIndex -= 1;
        }
        else
        {
            selectedIndex = tools.Length - 1;
        }
        selected = tools[selectedIndex];
        updatePreview();
    }

    void updateTiles()
    {
        foreach(GameObject obj in objects)
        {
            obj.SendMessage("OnUpdateSelected",selected,SendMessageOptions.DontRequireReceiver);
            obj.SendMessage("OnUpdateRotation", rotation, SendMessageOptions.DontRequireReceiver);
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        position = transform.position + (Vector3.up);
        actions = new InputSystem_Actions();
        actions.Enable();
        objects = GameObject.FindGameObjectsWithTag("EditorTile");
        Debug.Log(objects.Length);
        updatePreview();
    }

    // Update is called once per frame
    void Update()
    {
        if (actions.Player.Accelerate.WasPressedThisFrame())
        {
            scrollToolsUp();
        }
        if (actions.Player.Brake.WasPressedThisFrame())
        {
            scrollToolsDown();
        }
        if (actions.Player.Left.WasPressedThisFrame())
        {
            rotateLeft();
        }
        if (actions.Player.Right.WasPressedThisFrame())
        {
            rotateRight();
        }
    }
}
