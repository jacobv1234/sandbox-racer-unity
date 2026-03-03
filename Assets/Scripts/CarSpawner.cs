using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    private StateTracker state;

    [SerializeField]
    private GameObject playerCarPrefab;

    private GameObject playerCar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = GameObject.FindGameObjectWithTag("State").GetComponent<StateTracker>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state.wasChangedThisFrame())
        {
            switch (state.getState())
            {
                case 0:
                    Destroy(playerCar); break;
                case 1:
                    playerCar = Instantiate(playerCarPrefab, transform.position + Vector3.up, transform.rotation); break;
            }
        }
    }
}
