using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private Pickup[] listofPickupTypes;
    public Pickup[] GetPickups => listofPickupTypes;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject pauseScreen;
    public bool waveBusy = false;
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
    public AbstractEnemy BossEnemy => bossEnemy;
    [SerializeField] private AbstractEnemy bossEnemy;
    public Room[] Rooms => rooms;
    public float bulletTime { get; private set; }
    public float ricochet { get; private set; }

    public readonly GMNullState nullState = new GMNullState { };
    public readonly GMFailState failState = new GMFailState { };
    public readonly GMFailState winState = new GMFailState { };
    public GameObject[] wall;
    public int roomCount = 0;
    private bool paused = false;
    [SerializeField] private AudioClip[] music;
    private AudioSource audioSource;
    public int enemiesAlive
    {
        get { return enemiesAlive_; }
        set
        {
            enemiesAlive_ = value;
            if (enemiesAlive_ == 0)
            {
                Destroy(wall[roomCount]);
                waveBusy = false;
            };
        }
    }
    private int enemiesAlive_;
    public GMBaseState gameState { get; private set; }
    private void TransitionToState(GMBaseState state)
    {
        if (gameState != state)
        {
            gameState = state;
            gameState.EnterState(this);
        }
        else
        {
            Debug.LogWarning("Redundant State Transition");
        }
    }
    private PlayerMovement player;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        TransitionToState(nullState);
        pauseScreen?.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
    }
    private void Start()
    {
        SetMusic(0);
        EventBroker.SpawnEnemyEvent += SetRoom;
    }
    private void SetRoom(int index)
    {
        roomCount = index;
    }

    public void StartLevel(string scene)
    {
        SceneManager.LoadScene(scene);
        StartCoroutine(OnSceneStart(scene));
    }
    public void SetMusic(int id)
    {
        if (id == -1)
        {
            audioSource.clip = null;
            return;
        }
        audioSource.clip = music[id];
        audioSource.Play();
    }
    private IEnumerator OnSceneStart(string scene)
    {
        yield return new WaitUntil(()=>SceneManager.GetActiveScene().name==scene);
        player = FindObjectOfType<PlayerSpawner>().SpawnPlayer();
        SetMusic(1);
        EventBroker.LevelReadyTrigger();
    }

    private void Update()
    {
        if (gameState == failState)
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
            if (bulletTime > 0)
            {
                bulletTime -= Time.deltaTime;
            }
            else if (bulletTime < 0)
            {
                bulletTime = 0;
            }
            if (ricochet > 0)
            {
                ricochet -= Time.deltaTime;
            }
            else if (ricochet < 0)
            {
                ricochet = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Application.Quit();
        }
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
                bulletTime += 5;
                break;
            case PickupType.Ricochet:
                ricochet += 5;
                break;
        }
    }
    public void Death()
    {
        TransitionToState(failState);
        SetMusic(-1);
        Instantiate(gameOverScreen, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0), Quaternion.identity);
    }

    public void Victory()
    {
        //TODO: play win sound/song
        TransitionToState(winState);
        Instantiate(winScreen, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0), Quaternion.identity);
    }
}

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