using System.Collections;
using Enemies;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;

namespace Rooms
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField] private Tilemap[] roomLayouts;
        [SerializeField] private PlayerTrigger roomTrigger;
        [SerializeField] private GameObject[] doors;
        [SerializeField] private EnemySpawner spawnerPrefab;
        [SerializeField, Min(0)] private int waveAdvanceThreshold = 2;
        [SerializeField, Min(0)] private float waveDelay = 2f;

        private MusicSource musicSource;
        private int enemyAmount;

        private void Awake()
        {
            musicSource = GetComponent<MusicSource>();

            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<MusicSource>();
            }
        }

        private void OnEnable()
        {
            roomTrigger.onPlayerEnter += StartRoom;
            SetDoorsClosed(false);
        }

        private void OnDisable()
        {
            roomTrigger.onPlayerEnter -= StartRoom;
        }

        private void StartRoom()
        {
            if (roomLayouts == null || roomLayouts.Length == 0) return;

            musicSource.Play();
            SetDoorsClosed(true);
            StartCoroutine(SpawnEnemies());
        }

        private IEnumerator SpawnEnemies()
        {
            AbstractEnemy.OnEnemyIsDestroyed += DecrementEnemyCounter;

            for (int i = 0; i < roomLayouts.Length; i++)
            {
                yield return new WaitForSeconds(waveDelay);
                
                Tilemap layout = roomLayouts[i];
                BoundsInt bounds = layout.cellBounds;

                foreach (Vector3Int tilePosition in bounds.allPositionsWithin)
                {
                    var tile = layout.GetTile<AssetTile>(tilePosition);
                    
                    if (tile == null) continue;

                    Quaternion assetRotation = tile.tileAsset.transform.rotation;

                    if (tile.tileAsset.CompareTag("Enemy"))
                    {
                        EnemySpawner spawner =
                            Instantiate(spawnerPrefab, layout.CellToWorld(tilePosition), assetRotation);
                        spawner.enemyPrefab = tile.tileAsset;
                        enemyAmount++;
                    }
                    else
                    {
                        Instantiate(tile.tileAsset, layout.CellToWorld(tilePosition), assetRotation);
                    }
                }

                yield return new WaitUntil(() => CanAdvanceWave(i));
            }
            
            AbstractEnemy.OnEnemyIsDestroyed -= DecrementEnemyCounter;
            
            SetDoorsClosed(false);
        }

        private void SetDoorsClosed(bool isClosed)
        {
            foreach (GameObject door in doors)
            {
                door.SetActive(isClosed);
            }
        }

        private bool CanAdvanceWave(int waveIndex)
        {
            return waveIndex < roomLayouts.Length - 1 && enemyAmount <= waveAdvanceThreshold || enemyAmount == 0;
        }

        private void DecrementEnemyCounter() => enemyAmount--;
    }
}