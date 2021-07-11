using System.Collections;
using Enemies;
using UnityEngine;

namespace Utility
{
    public class WaveManager : Singleton<WaveManager>
    {
        [SerializeField] private WaveAsset[] wavesPerRoom;
        [SerializeField] private AbstractEnemy bossEnemy;

        [SerializeField] private Vector3 bossPosition = new Vector3(-30f, 200f, 0f);

        private bool waveIsInProgress;
        private int spawnedEnemyAmount;

        public delegate void SubWaveStartAction(int roomID, SubWave subWave);

        public event SubWaveStartAction OnSubWaveStartAction;

        public delegate void RoomFinishedAction(int roomID);

        public event RoomFinishedAction OnRoomIsFinished;

        public delegate void PickupSpawnAction(int roomID);

        public event PickupSpawnAction OnPickupSpawned;

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
            OnPickupSpawned?.Invoke(roomID);

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

            foreach (Wave wave in wavesPerRoom[roomID].Waves)
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
}