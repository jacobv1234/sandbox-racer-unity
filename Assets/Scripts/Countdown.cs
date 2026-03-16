using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    [SerializeField]
    private Sprite sprite_ready;
    [SerializeField]
    private Sprite sprite_3;
    [SerializeField]
    private Sprite sprite_2;
    [SerializeField]
    private Sprite sprite_1;
    [SerializeField]
    private Sprite sprite_go;

    [SerializeField]
    private float offscreenY = 640;
    [SerializeField]
    private float onscreenY = 440;
    [SerializeField]
    private float movespeed = 10;

    [SerializeField]
    private float timer;
    private float move_target;
    private bool countdown_running;

    private StateTracker state;

    private GameObject player;

    private Image image;
    private RectTransform transform2D;

    private void changeSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }


    private void checkState()
    {
        if (state.wasChangedThisFrame())
        {
            switch (state.getState())
            {
                case 0:
                    move_target = offscreenY;
                    timer = 0;
                    player = null;
                    countdown_running = false;
                    changeSprite(sprite_ready);
                    break;

                case 1:
                    countdown_running = true;
                    timer = 0;
                    changeSprite(sprite_ready);
                    move_target = onscreenY;
                    break;
            }
        }
    }

    private void updatePosition()
    {
        float currentY = transform2D.anchoredPosition.y;
        float currentX = transform2D.anchoredPosition.x;
        if (currentY > move_target) { transform2D.anchoredPosition += Vector2.down * movespeed * Time.deltaTime; }
        if (currentY < move_target) { transform2D.anchoredPosition += Vector2.up * movespeed * Time.deltaTime; }
        if (currentY < onscreenY) { transform2D.anchoredPosition = new Vector2(currentX, onscreenY); }
        if (currentY > offscreenY) { transform2D.anchoredPosition = new Vector2(currentX, offscreenY); }
    }

    private void updateTimer()
    {
        if (countdown_running)
        {
            timer += Time.deltaTime;
        }
    }

    private void updateSpriteAndTarget()
    {
        if (timer >= 6)
        {
            move_target = offscreenY;
        }
        else if (timer >= 5)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            player.SendMessage("startControls");
            changeSprite(sprite_go);
        }
        else if (timer >= 4)
        {
            changeSprite(sprite_1);
        }
        else if (timer >= 3)
        {
            changeSprite(sprite_2);
        }
        else if (timer >= 2)
        {
            changeSprite(sprite_3);
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject stateObj = GameObject.FindGameObjectWithTag("State");
        if (stateObj != null)
        {
            state = stateObj.GetComponent<StateTracker>();
        }

        image = gameObject.GetComponent<Image>();
        transform2D = gameObject.GetComponent<RectTransform>();

        timer = 0;
        move_target = offscreenY;
        countdown_running = false;
    }

    // Update is called once per frame
    void Update()
    {
        checkState();
        updatePosition();
        updateTimer();
        updateSpriteAndTarget();
    }
}
