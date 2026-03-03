using UnityEngine;
using UnityEngine.UI;

public class ModeSwitchSpriteSwap : MonoBehaviour
{
    [SerializeField]
    private Sprite build;
    [SerializeField]
    private Sprite race;

    private StateTracker state;

    private Image image;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = gameObject.GetComponent<Image>();
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
                    image.sprite = race; break;
                case 1:
                    image.sprite = build; break;
            }
        }
    }
}
