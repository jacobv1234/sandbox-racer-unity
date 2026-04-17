using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    private StateTracker state;

    [SerializeField]
    private GameObject playerCarPrefab;
    [SerializeField]
    private GameObject enemyCarPrefab;

    private GameObject playerCar;
    private GameObject enemyCar;

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
                    Destroy(playerCar);
                    Destroy(enemyCar);
                    break;
                case 1:
                    Vector3 playerOffset = transform.rotation * new Vector3(1, 1, 0);
                    Vector3 enemyOffset = transform.rotation * new Vector3(-1, 1, 0);

                    playerCar = Instantiate(playerCarPrefab, transform.position + playerOffset, transform.rotation);
                    enemyCar = Instantiate(enemyCarPrefab, transform.position + enemyOffset, transform.rotation);
                    
                    break;
            }
        }
    }
}
