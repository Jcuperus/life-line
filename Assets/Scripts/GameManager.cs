using System;
using System.Collections;
using Enemies;
using Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;

/// <summary>
/// General Utility class which handles many of the runtime activity and data containment. Needs preset configurations in a scene to function properly.
/// </summary>
public class GameManager : MonoSingleton<GameManager>
{
    /**************** VARIABLES *******************/
    [Header("Assets & Prefabs")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private Slider powerupTimer;
    [Space]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip restartSound;
    private AudioSource audioSource;

    // player power up status:
    public float BulletTime { get; private set; }

    // game state:
    private readonly GMNullState nullState = new GMNullState();
    private readonly GMFailState failState = new GMFailState();
    private readonly GMFailState winState = new GMFailState();
    private GMBaseState GameState { get; set; }
    private bool paused = false;
    
    public static event Action OnHealthPickup;

    public delegate void RicochetActivatedAction(float duration);
    public static event RicochetActivatedAction OnRicochetActivated;
    
    public delegate void SpeedMultiplierAction(float duration, float multiplier);
    public static event SpeedMultiplierAction OnSpeedMultiplierApplied;

    public delegate void SpreadShotActivatedAction(float duration);
    public static event SpreadShotActivatedAction OnSpreadShotActivated;

    public delegate void SpeedShotActivatedAction(float duration);
    public static event SpeedShotActivatedAction OnSpeedShotActivated;
    /**********************************************/

    /****************** INIT **********************/
    private void Awake()
    {
        DontDestroyOnLoad(this);
        TransitionToState(nullState);
        pauseScreen.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        
        EnemySun.OnSunDefeated += Victory;
        pauseScreen.SetActive(false);
    }
    
    private void Start()
    {
        PlayMusic(menuMusic);
    }
    /**********************************************/
    
    /****************** LOOP **********************/
    private void Update()
    {
        if (GameState == failState || GameState == winState)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (paused)
                {
                    paused = false;
                    Time.timeScale = 1;
                    pauseScreen.SetActive(false);
                }
                else
                {
                    paused = true;
                    Time.timeScale = 0;
                    pauseScreen.SetActive(true);
                    pauseScreen.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
                }
            }
            if (BulletTime > 0)
            {
                BulletTime -= Time.deltaTime;
            }
            else if (BulletTime < 0)
            {
                BulletTime = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Application.Quit();
        }
    }
    /**********************************************/
    
    /***************** METHODS ********************/
    private void TransitionToState(GMBaseState state)
    {
        if (GameState != state)
        {
            GameState = state;
            GameState.EnterState(this);
        }
        else
        {
            Debug.LogWarning("Redundant State Transition");
        }
    }
    
    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        TransitionToState(nullState);
        audioSource.PlayOneShot(restartSound);
    }
    
    public void StartLevel(string scene)
    {
        SceneManager.LoadScene(scene);
        StartCoroutine(OnSceneStart(scene));
    }
    
    private IEnumerator OnSceneStart(string scene)
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == scene);
        PlayMusic(null);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == audioSource.clip) return;
        
        audioSource.clip = clip;
        audioSource.Play();
    }
    
    public void ResolvePickup(Pickup pickup)
    {
        float duration = 0;
        switch (pickup.Type)
        {
            case PickupType.HealthUp:
                OnHealthPickup?.Invoke();
                break;
            case PickupType.SpeedUp:
                duration = 5;
                OnSpeedMultiplierApplied?.Invoke(duration, 1.5f);
                break;
            case PickupType.BulletTime:
                duration = 5;
                BulletTime += duration;
                break;
            case PickupType.Ricochet:
                duration = 5;
                OnRicochetActivated?.Invoke(duration);
                break;
            case PickupType.SpreadShot:
                duration = 5;
                OnSpreadShotActivated?.Invoke(duration);
                break;
            case PickupType.ShotSpeedUp:
                duration = 5;
                OnSpeedShotActivated?.Invoke(duration);
                break;
        }
        if (duration > 0)
        {
            StartCoroutine(ShowPowerup(pickup.gameObject.GetComponent<SpriteRenderer>().sprite, duration));
        }
    }
    private IEnumerator ShowPowerup(Sprite sprite, float duration)
    {
        Slider slider = Instantiate(powerupTimer, FindObjectOfType<Canvas>().transform);
        slider.handleRect.GetComponent<Image>().sprite = sprite;
        slider.maxValue = duration;
        while (duration > 0)
        {
            slider.value = duration;
            yield return new WaitForFixedUpdate();
            duration -= Time.deltaTime;
        }
        Destroy(slider.gameObject);
    }

    public void Death()
    {
        TransitionToState(failState);
        PlayMusic(null);
        Instantiate(gameOverScreen, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0), Quaternion.identity);
    }
    
    public void Victory()
    {
        TransitionToState(winState);
        PlayMusic(null);
        Instantiate(winScreen, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0), Quaternion.identity);
    }
    /**********************************************/
}

/************** MACHINE STATES ****************/
public abstract class GMBaseState
{
    public abstract void EnterState(GameManager gm);
}

public class GMNullState : GMBaseState
{
    public override void EnterState(GameManager gm)
    {
        Time.timeScale = 1;
    }
}

public class GMFailState : GMBaseState
{
    public override void EnterState(GameManager gm)
    {
        Time.timeScale = 0;
    }
}
/**********************************************/