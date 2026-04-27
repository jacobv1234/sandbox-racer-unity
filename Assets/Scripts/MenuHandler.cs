using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPlayClick()
    {
        SceneManager.LoadScene("LevelEditor");
    }

    private void OnHowToPlayClick()
    {
        SceneManager.LoadScene("HowToPlay");
    }

    private void OnBackClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
