using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

namespace Utility
{
    public class WaveManager : Singleton<WaveManager>
    {
        [SerializeField] private WaveAsset[] waveConfig;

        private readonly List<EnemySpawnPoint> spawnPoints = new List<EnemySpawnPoint>();
        
        private bool waveIsInProgress;
        private int spawnedEnemyAmount;

        public delegate void RoomFinishedAction(int roomID);

        public event RoomFinishedAction OnRoomIsFinished;

        public delegate void PickupSpawnAction(int roomID);

        public event PickupSpawnAction OnPickupWaveTriggered;

        protected override void Awake()
        {
            base.Awake();

            SpawnTrigger.OnWaveTriggered += OnWaveTriggered;
            AbstractEnemy.OnEnemyIsDestroyed += () => spawnedEnemyAmount--;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                OnRoomIsFinished?.Invoke(0);
            }
        }

        private void OnDestroy()
        {
            SpawnTrigger.OnWaveTriggered -= OnWaveTriggered;
        }

        private void OnWaveTriggered(int roomID)
        {
            OnPickupWaveTriggered?.Invoke(roomID);
            StartCoroutine(SpawnRoomWaves(roomID));
        }

        private IEnumerator SpawnRoomWaves(int roomID)
        {
            yield return new WaitUntil(() => !waveIsInProgress);
            waveIsInProgress = true;

            foreach (Wave wave in waveConfig[roomID].Waves)
            {
                foreach (SubWave subWave in wave.subWaves)
                {
                    StartSubWave(roomID, subWave);
                    yield return new WaitUntil(() => subWave.continuesWave || spawnedEnemyAmount <= 0);
                }
            }

            waveIsInProgress = false;
            OnRoomIsFinished?.Invoke(roomID);
        }

        private void StartSubWave(int roomID, SubWave subWave)
        {
            foreach (EnemySpawnPoint spawnPoint in spawnPoints)
            {
                if (spawnPoint.roomID == roomID)
                {
                    spawnedEnemyAmount += subWave.amount;
                    spawnPoint.SpawnSubWave(subWave);
                }
            }
        }

        public void RegisterSpawnPoint(EnemySpawnPoint spawnPoint)
        {
            spawnPoints.Add(spawnPoint);
        }
    }
}