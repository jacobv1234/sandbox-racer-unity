using UnityEngine;
using TMPro;

public class FinishText : MonoBehaviour
{
    [SerializeField]
    private float maxFontSize = 250;
    [SerializeField]
    private float speed = 250;
    private float targetSize;

    private TMP_Text text;

    private float timer;
    private bool animation_running;

    private StateTracker state;

    private void updateFontSize()
    {
        float size = text.fontSize;
        if (size < targetSize) { text.fontSize += speed * Time.deltaTime; }
        if (size > targetSize) { text.fontSize -= speed * Time.deltaTime; }
        if (size > maxFontSize) {  text.fontSize = maxFontSize; }
        if (size < 0) {  text.fontSize = 0; }
    }

    private void updateTimer()
    {
        if (animation_running)
        {
            timer += Time.deltaTime;
        }

        if (timer >= 3)
        {
            state.setState(0); // go back to builder
            animation_running = false;
            timer = 0;
        }
        else if (timer >= 2)
        {
            targetSize = 0;
        }
    }

    private void doAnimation()
    {
        animation_running = true;
        timer = 0;
        targetSize = maxFontSize;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0;
        text = GetComponent<TMP_Text>();
        targetSize = 0;
        state = GameObject.FindGameObjectWithTag("State").GetComponent<StateTracker>();
        animation_running = false;
    }

    // Update is called once per frame
    void Update()
    {
        updateFontSize();
        updateTimer();
    }
}
