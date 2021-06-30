using System.Collections;
using Spine.Unity;
using Spine;
using UnityEngine;

/// <summary>
/// Behaviour for glitched walls, functioning as enemy spawn points. 
/// </summary>
public class EnemySpawnPoint : MonoBehaviour
{
    /**************** VARIABLES *******************/
    [Header("Animation")]
    [SerializeField] private SkeletonAnimation animator;
    [SerializeField] private AnimationReferenceAsset spawnAnimation;
    [Header("Config")]
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;
    [SerializeField] private int roomID;
    public int RoomID => roomID;

    private bool canInterruptAnimation = true;
    /**********************************************/
    /******************* INIT *********************/
    public void Start()
    {
        EventBroker.SpawnEnemyEvent += StartSpawningWaves;
    }
    /**********************************************/
    /***************** METHODS ********************/
    private void StartSpawningWaves(int room)
    {
        if (room == roomID)
        {
            if (roomID == -1)
            {
                AbstractEnemy spawnedEnemy = Instantiate(GameManager.Instance.BossEnemy);
                spawnedEnemy.transform.position = this.transform.position + spawnOffset;
            }
            else
            {
                StartCoroutine(SpawnWaves());
            }
            
        }
    }
    private IEnumerator SpawnWaves()
    {
        Debug.Log(GameManager.Instance.Rooms[roomID].waves.Length + " waves");
        foreach(GameManager.Wave wave in GameManager.Instance.Rooms[roomID].waves)
        {
            yield return new WaitWhile(() => GameManager.Instance.waveBusy);
            Debug.Log(wave.subWaves.Length + " subwaves");
                foreach (GameManager.SubWave subWave in wave.subWaves)
                {
                Debug.Log("Spawning " + subWave.type.name);
                    AbstractEnemy prefab = subWave.type;
                    float delay = subWave.delay;
                    int amount = subWave.amount;
                    StartCoroutine(SpawnEnemies(prefab, delay, amount));
                }
            GameManager.Instance.waveBusy = true;

        }
    }
    private IEnumerator SpawnEnemies(AbstractEnemy enemy, float freq, int n)
    {
        for (int i = 0; i < n; i++)
        {
            yield return new WaitForSeconds(freq);
            SetAnimation(spawnAnimation, false, .5f);
            AbstractEnemy spawnedEnemy = Instantiate(enemy);
            GameManager.Instance.EnemiesAlive--;
            spawnedEnemy.transform.position = this.transform.position + spawnOffset;
        }
    }
    protected void SetAnimation(AnimationReferenceAsset animation, bool loop, float timeScale)
    {
        if (!canInterruptAnimation) return;

        TrackEntry trackEntry = animator.state.SetAnimation(0, animation, loop);
        trackEntry.TimeScale = timeScale;
        trackEntry.Complete += OnAnimationComplete;
    }
    protected virtual void OnAnimationComplete(TrackEntry trackEntry) { }
    /**********************************************/
}
