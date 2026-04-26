using Unity.VisualScripting;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField]
    private AudioSource music;
    [SerializeField]
    private AudioClip buildMusic;
    [SerializeField]
    private AudioClip raceMusic;

    private StateTracker state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject stateObj = GameObject.FindGameObjectWithTag("State");
        if (stateObj != null)
        {
            state = stateObj.GetComponent<StateTracker>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (state.wasChangedThisFrame())
        {
            music.Stop();
            switch (state.getState())
            {
                case 0:
                    music.clip = buildMusic; break;
                case 1:
                    music.clip = raceMusic; break;
            }
            music.Play();
        }
    }
}
