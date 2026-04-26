using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip build1;
    [SerializeField]
    private AudioClip build2;

    [SerializeField]
    private AudioClip remove1;
    [SerializeField]
    private AudioClip remove2;

    [SerializeField]
    private float buildVolume = 1.5f;
    [SerializeField]
    private float removeVolume = 2.2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void PlayBuildSFX()
    {
        int soundChoice = Random.Range(0, 2);
        AudioClip chosenSound;
        if (soundChoice == 0)
        {
            chosenSound = build1;
        }
        else
        {
            chosenSound = build2;
        }

        audioSource.pitch = Random.Range(0.6f, 1.5f);
        audioSource.volume = buildVolume;
        audioSource.PlayOneShot(chosenSound);
    }

    private void PlayRemoveSFX()
    {
        int soundChoice = Random.Range(0, 2);
        AudioClip chosenSound;
        if (soundChoice == 0)
        {
            chosenSound = remove1;
        }
        else
        {
            chosenSound = remove2;
        }

        audioSource.pitch = Random.Range(0.6f, 1.5f);
        audioSource.volume = removeVolume;
        audioSource.PlayOneShot(chosenSound);
    }
}
