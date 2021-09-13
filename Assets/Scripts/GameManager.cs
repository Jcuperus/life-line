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
    [SerializeField] private GameObject gameOverScreenPrefab;
    [SerializeField] private GameObject winScreenPrefab;
    [SerializeField] private Image pauseScreenPrefab;
    [SerializeField] private Slider powerupTimer;
    private Image pauseScreenObject;
    
    [Header("Music")]
    [SerializeField] private AudioClip restartSound;
    private AudioSource audioSource;

    private IEnumerator powerupTimerCoroutine;
    
    private float BulletTime { get; set; }

    public static State CurrentState { get; private set; }
    
    public enum State
    {
        Running,
        Paused,
        Ended
    }

    public static event Action OnHealthPickup;

    public delegate void RicochetActivatedAction(float duration);
    public static event RicochetActivatedAction OnRicochetActivated;
    
    public delegate void SpeedMultiplierAction(float multiplier, float duration);
    public static event SpeedMultiplierAction OnSpeedMultiplierApplied;

    public delegate void SpreadShotActivatedAction(float duration);
    public static event SpreadShotActivatedAction OnSpreadShotActivated;

    public delegate void SpeedShotActivatedAction(float duration);
    public static event SpeedShotActivatedAction OnSpeedShotActivated;

    public delegate void StateChangedAction(State state);
    public static event StateChangedAction OnStateChanged;
    /**********************************************/

    /****************** INIT **********************/
    private void Awake()
    {
        DontDestroyOnLoad(this);
        TransitionToState(State.Running);
        audioSource = GetComponent<AudioSource>();
        
        EnemySun.OnSunDefeated += Victory;
    }
    /**********************************************/
    
    /****************** LOOP **********************/
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && CurrentState == State.Ended)
        {
            Restart();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Application.Quit();
        }

        if (CurrentState == State.Running)
        {
            if (BulletTime > 0)
            {
                BulletTime -= Time.deltaTime;
            }
            else if (BulletTime < 0)
            {
                BulletTime = 0;
            }
        }
    }
    /**********************************************/
    
    /***************** METHODS ********************/
    private void TransitionToState(State state)
    {
        if (CurrentState == state) return;
        
        switch (state)
        {
            case State.Running:
                Time.timeScale = 1f;
                break;
            case State.Ended:
            case State.Paused:
                Time.timeScale = 0f;
                break;
            default:
                Debug.LogWarning("Warning: Invalid state transition.");
                break;
        }

        CurrentState = state;
        OnStateChanged?.Invoke(state);
    }
    
    private void TogglePause()
    {
        if (CurrentState == State.Paused)
        {
            TransitionToState(State.Running);
            Time.timeScale = 1f;
            pauseScreenObject.gameObject.SetActive(false);
        }
        else if (CurrentState == State.Running)
        {
            TransitionToState(State.Paused);
            Time.timeScale = 0f;
            
            if (pauseScreenObject == null)
            {
                pauseScreenObject = Instantiate(pauseScreenPrefab, FindObjectOfType<Canvas>().transform);
            }
            
            pauseScreenObject.gameObject.SetActive(true);
        }
    }
    private void Restart()
    {
        if (powerupTimerCoroutine != null) StopCoroutine(powerupTimerCoroutine);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        TransitionToState(State.Running);
        audioSource.PlayOneShot(restartSound);
    }
    
    public void StartLevel(string scene)
    {
        SceneManager.LoadScene(scene);
        MusicManager.Instance.Stop();
        StartCoroutine(OnSceneStart(scene));
    }
    
    private IEnumerator OnSceneStart(string scene)
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == scene);
        MusicManager.Instance.Stop();
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
            ShowPowerup(pickup.gameObject.GetComponent<SpriteRenderer>().sprite, duration);
        }
    }

    private void ShowPowerup(Sprite sprite, float duration)
    {
        powerupTimerCoroutine = ShowPowerupCoroutine(sprite, duration);
        StartCoroutine(powerupTimerCoroutine);
    }
    
    private IEnumerator ShowPowerupCoroutine(Sprite sprite, float duration)
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
        powerupTimerCoroutine = null;
    }

    public void Death()
    {
        TransitionToState(State.Ended);
        MusicManager.Instance.Stop();
        Instantiate(gameOverScreenPrefab, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0), Quaternion.identity);
    }
    
    private void Victory()
    {
        TransitionToState(State.Ended);
        MusicManager.Instance.Stop();
        Instantiate(winScreenPrefab, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0), Quaternion.identity);
    }
    /**********************************************/
}