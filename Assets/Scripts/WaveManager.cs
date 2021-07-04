using System;
using System.Collections;
using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
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

    [SerializeField] private Vector3 bossPosition = new Vector3(-30f, 200f, 0f);
    
    private bool waveIsInProgress;
    private int spawnedEnemyAmount;

    public delegate void SubWaveStartAction(int roomID, SubWave subWave);
    public event SubWaveStartAction OnSubWaveStartAction;

    public delegate void RoomFinishedAction(int roomID);
    public event RoomFinishedAction OnRoomIsFinished;

    protected override void Awake()
    {
        base.Awake();
        
        SpawnTrigger.OnWaveTriggered += OnWaveTriggered;
        AbstractEnemy.OnEnemyIsDestroyed += () => spawnedEnemyAmount--;
    }

    private void OnDestroy()
    {
        SpawnTrigger.OnWaveTriggered -= OnWaveTriggered;
    }

    private void OnWaveTriggered(int roomID)
    {
        //TODO: refactor old wave code
        EventBroker.SpawnPickupTrigger(roomID);

        //TODO: change room music selection
        int musicIndex = roomID switch
        {
            0 => 1,
            1 => 2,
            -1 => 3,
            _ => 0
        };
        
        GameManager.Instance.PlayMusic(musicIndex);
        
        //TODO: change boss spawn workaround
        if (roomID == -1)
        {
            AbstractEnemy spawnedEnemy = Instantiate(bossEnemy);
            spawnedEnemy.transform.position = bossPosition;
        }
        else
        {
            StartCoroutine(SpawnRoomWaves(roomID));
        }
    }

    private IEnumerator SpawnRoomWaves(int roomID)
    {
        yield return new WaitUntil(() => !waveIsInProgress);
        waveIsInProgress = true;
        
        foreach (Wave wave in rooms[roomID].waves)
        {
            foreach (SubWave subWave in wave.subWaves)
            {
                OnSubWaveStartAction?.Invoke(roomID, subWave);
                spawnedEnemyAmount = subWave.amount;

                yield return new WaitUntil(() => spawnedEnemyAmount <= 0);
            }
        }

        waveIsInProgress = false;
        OnRoomIsFinished?.Invoke(roomID);
    }
}