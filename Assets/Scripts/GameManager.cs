using System;
using System.Collections;
using Enemies;
using Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [Space]    
    [SerializeField] private AudioClip[] music;
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
    /**********************************************/
    
    /****************** INIT **********************/
    private void Awake()
    {
        DontDestroyOnLoad(this);
        TransitionToState(nullState);
        if (pauseScreen == null) pauseScreen.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        
        EnemySun.OnSunDefeated += Victory;
    }
    
    private void Start()
    {
        PlayMusic(0);
    }
    /**********************************************/
    
    /****************** LOOP **********************/
    private void Update()
    {
        if (GameState == failState)
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
    }
    
    public void StartLevel(string scene)
    {
        SceneManager.LoadScene(scene);
        StartCoroutine(OnSceneStart(scene));
    }
    
    private IEnumerator OnSceneStart(string scene)
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == scene);
        PlayMusic(1);
    }
    
    public void PlayMusic(int id)
    {
        if (id == -1)
        {
            audioSource.clip = null;
            return;
        }
        audioSource.clip = music[id];
        audioSource.Play();
    }
    
    public void ResolvePickup(PickupType pickup)
    {
        switch (pickup)
        {
            case PickupType.HealthUp:
                OnHealthPickup?.Invoke();
                break;
            case PickupType.SpeedUp:
                OnSpeedMultiplierApplied?.Invoke(5, 1.5f);
                break;
            case PickupType.BulletTime:
                BulletTime += 5;
                break;
            case PickupType.Ricochet:
                OnRicochetActivated?.Invoke(5f);
                break;
        }
    }

    public void Death()
    {
        TransitionToState(failState);
        PlayMusic(-1);
        Instantiate(gameOverScreen, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0), Quaternion.identity);
    }
    
    public void Victory()
    {
        TransitionToState(winState);
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