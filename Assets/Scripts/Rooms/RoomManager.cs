using System.Collections;
using Enemies;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rooms
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField] private Tilemap[] roomLayouts;
        [SerializeField] private PlayerTrigger roomTrigger;
        [SerializeField] private GameObject[] doors;
        [SerializeField] private EnemySpawner spawnerPrefab;
        [SerializeField] private float waveDelay = 2f;

        private int enemyAmount;

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

            SetDoorsClosed(true);
            StartCoroutine(SpawnEnemies());
        }

        private IEnumerator SpawnEnemies()
        {
            AbstractEnemy.OnEnemyIsDestroyed += DecrementEnemyCounter;

            foreach (Tilemap layout in roomLayouts)
            {
                yield return new WaitForSeconds(waveDelay);
                
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

                yield return new WaitUntil(() => enemyAmount == 0);
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

        private void DecrementEnemyCounter() => enemyAmount--;
    }
}