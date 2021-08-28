using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

namespace Utility
{
    public class WaveManager : Singleton<WaveManager>
    {
        [SerializeField] protected float initialDelay = 7f;
        public float InitialDelay => initialDelay;
        [SerializeField] protected float waveDelay = 0f;
        public float WaveDelay => waveDelay;

        [Serializable]
        public class SpawnerArray
        {
            [Tooltip("Int to match the room the enemies should spawn in. Use multiple with the same roomID to spawn them in waves")] public int roomID;
            public SpawnConfigAsset config;
        }
        [SerializeField] private SpawnerArray[] spawnerArrays;
        public SpawnerArray[] SpawnerArrays => spawnerArrays;
        public List<SpawnerArray> SpawnerArraysByRoomID(int value)
        {
            List<SpawnerArray> waves = new List<SpawnerArray> { };

            foreach (SpawnerArray spawnerArray in spawnerArrays)
            {
                if (spawnerArray.roomID == value)
                {
                    waves.Add(spawnerArray);
                }
            }
            return waves;
        }


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

        private void OnDestroy()
        {
            SpawnTrigger.OnWaveTriggered -= OnWaveTriggered;
        }

        private void OnWaveTriggered(int roomID)
        {
            StartCoroutine(SpawnRoomWaves(roomID));
        }

        private IEnumerator SpawnRoomWaves(int roomID)
        {
            yield return new WaitUntil(() => !waveIsInProgress);
            waveIsInProgress = true;
            yield return new WaitForSeconds(initialDelay);
            foreach (SpawnerArray wave in SpawnerArraysByRoomID(roomID))
            {
                OnPickupWaveTriggered?.Invoke(roomID);
                foreach (EnemySpawnConfig spawnConfig in wave.config.SpawnerConfigs)
                {
                    SpawnEnemyType(roomID, spawnConfig);
                    yield return new WaitForSeconds(spawnConfig.spawnDelay);
                }
                yield return new WaitUntil(() => spawnedEnemyAmount <= 0);
                yield return new WaitForSeconds(waveDelay);
            }
            waveIsInProgress = false;
            OnRoomIsFinished?.Invoke(roomID);
        }

        private void SpawnEnemyType(int roomID, EnemySpawnConfig spawnConfig)
        {
            List<EnemySpawnPoint> possibleSpawnPoints = new List<EnemySpawnPoint> { };
            foreach (EnemySpawnPoint spawnPoint in spawnPoints)
            {
                if (spawnPoint.roomID == roomID)
                {
                    possibleSpawnPoints.Add(spawnPoint);
                }
            }
            for (int i = 0; i < spawnConfig.amount; i++)
            {
                spawnedEnemyAmount++;
                StartCoroutine(possibleSpawnPoints[UnityEngine.Random.Range(0,possibleSpawnPoints.Count)].SpawnEnemy(spawnConfig.prefab));
            }

        }

        public void RegisterSpawnPoint(EnemySpawnPoint spawnPoint)
        {
            spawnPoints.Add(spawnPoint);
        }
    }
}