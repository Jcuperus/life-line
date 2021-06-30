using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// General Utility class which handles many of the runtime activiy and data containment. Needs preset configurations in a scene to function properly.
/// </summary>
public class GameManager : MonoSingleton<GameManager>
{
    /**************** VARIABLES *******************/
    #region VARIABLES
    [Header("Assets & Prefabs")]
    [SerializeField] private Pickup[] pickupTypes;
    public Pickup[] GetPickups => pickupTypes;
    [Space]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject pauseScreen;
    [Space]    
    [SerializeField] private AudioClip[] music;
    private AudioSource audioSource;
    [Space]
    #region Temporary Wave Construction
    [Header("Temp Wave Construction")]
    public bool waveBusy = false; // change this when changing spawn workflow
    [Serializable]
    public class SubWave
    {
        public AbstractEnemy type;
        public int amount;
        public float delay;
    }
    [Serializable]
    public class Wave
    {
        public SubWave[] subWaves;
    }
    [Serializable]
    public class Room
    {
        public Wave[] waves;
    }
    [SerializeField] private Room[] rooms;
    [SerializeField] private AbstractEnemy bossEnemy;
    public AbstractEnemy BossEnemy => bossEnemy;
    public Room[] Rooms => rooms;
    public GameObject[] barriers;
    #endregion

    // player powerup status:
    public float BulletTime { get; private set; }
    public float Ricochet { get; private set; }

    // gamestate:
    public readonly GMNullState nullState = new GMNullState { };
    public readonly GMFailState failState = new GMFailState { };
    public readonly GMFailState winState = new GMFailState { };
    public GMBaseState GameState { get; private set; }
    private bool paused = false;
    // tracking rooms & waves
    [HideInInspector] public int roomCount = 0;
    private void SetRoom(int index)
    {
        roomCount = index;
    }
    public int EnemiesAlive
    {
        get { return enemiesAlive; }
        set
        {
            enemiesAlive = value;
            if (enemiesAlive == 0)
            {
                Destroy(barriers[roomCount]);
                waveBusy = false;
            };
        }
    }
    private int enemiesAlive;
    // inscene references
    private PlayerMovement player;
    #endregion
    /**********************************************/
    /****************** INIT **********************/
    private void Awake()
    {
        DontDestroyOnLoad(this);
        TransitionToState(nullState);
        if(pauseScreen==null)pauseScreen.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
    }
    private void Start()
    {
        PlayMusic(0);
        EventBroker.SpawnEnemyEvent += SetRoom;
    }
    /**********************************************/
    /****************** LOOP **********************/
    private void Update()
    {
        if (GameState == failState)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("Implement quick reset");
                //StartLevel(SceneManager.GetActiveScene().name);
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
            if (Ricochet > 0)
            {
                Ricochet -= Time.deltaTime;
            }
            else if (Ricochet < 0)
            {
                Ricochet = 0;
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
    public void StartLevel(string scene)
    {
        SceneManager.LoadScene(scene);
        StartCoroutine(OnSceneStart(scene));
    }
    private IEnumerator OnSceneStart(string scene)
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == scene);
        player = FindObjectOfType<PlayerSpawner>().SpawnPlayer();
        PlayMusic(1);
        EventBroker.LevelReadyTrigger();
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
                player.SpawnSegment();
                break;
            case PickupType.SpeedUp:
                //player.ApplySpeedMultiplier(1.5f, 5);
                break;
            case PickupType.BulletTime:
                BulletTime += 5;
                break;
            case PickupType.Ricochet:
                Ricochet += 5;
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
        //TODO: play win sound/song
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