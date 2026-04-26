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
    // used to ensure each part of updateSpriteAndTarget only runs once
    private int countdown_stage;

    private StateTracker state;

    private GameObject player;
    private GameObject opponent;

    private Image image;
    private RectTransform transform2D;

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip sound_321;
    [SerializeField]
    private AudioClip sound_GO;

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
                    opponent = null;
                    countdown_running = false;
                    changeSprite(sprite_ready);
                    break;

                case 1:
                    countdown_running = true;
                    timer = 0;
                    changeSprite(sprite_ready);
                    move_target = onscreenY;
                    countdown_stage = 0;
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
        if (timer >= 6 && countdown_stage == 4)
        {
            move_target = offscreenY;
            countdown_stage = 5;
        }
        else if (timer >= 5 && countdown_stage == 3)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            player.SendMessage("startControls");

            opponent = GameObject.FindGameObjectWithTag("Opponent");
            opponent.SendMessage("startDriving");

            changeSprite(sprite_go);
            audioSource.PlayOneShot(sound_GO);
            countdown_stage++;
        }
        else if (timer >= 4 && countdown_stage == 2)
        {
            changeSprite(sprite_1);
            audioSource.PlayOneShot(sound_321);
            countdown_stage++;
        }
        else if (timer >= 3 && countdown_stage == 1)
        {
            changeSprite(sprite_2);
            audioSource.PlayOneShot(sound_321);
            countdown_stage++;
        }
        else if (timer >= 2 && countdown_stage == 0)
        {
            changeSprite(sprite_3);
            audioSource.PlayOneShot(sound_321);
            countdown_stage++;
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

        audioSource = GetComponent<AudioSource>();
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
