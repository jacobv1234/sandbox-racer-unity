using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip buildMusic;
    [SerializeField]
    private AudioClip raceMusic;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<GameObject> players = new List<GameObject> { };
        GameObject.FindGameObjectsWithTag("Music", players);

        if (players.Count > 1)
        {
            Destroy(gameObject);
            Destroy(this);
        }

        DontDestroyOnLoad(gameObject); // seamless music when switching between scenes
        audioSource = GetComponent<AudioSource>();
        StartBuildMusic();
    }

    private void StartBuildMusic()
    {
        audioSource.Stop();
        audioSource.clip = buildMusic;
        audioSource.Play();
    }

    private void StartRaceMusic()
    {
        audioSource.Stop();
        audioSource.clip = raceMusic;
        audioSource.Play();
    }
}
