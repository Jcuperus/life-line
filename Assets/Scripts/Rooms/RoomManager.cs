using System.Collections;
using Enemies;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rooms
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField] private Tilemap[] enemyLayouts;
        [SerializeField] private PlayerTrigger roomTrigger;
        [SerializeField] private GameObject entranceDoor, exitDoor;
        [SerializeField] private EnemySpawner spawnerPrefab;
        [SerializeField] private float waveDelay = 2f;

        private int enemyAmount;

        private void OnEnable()
        {
            roomTrigger.onPlayerEnter += StartRoom;
            entranceDoor.SetActive(false);
        }

        private void OnDisable()
        {
            roomTrigger.onPlayerEnter -= StartRoom;
        }

        private void StartRoom()
        {
            if (enemyLayouts == null || enemyLayouts.Length == 0) return;

            entranceDoor.SetActive(true);

            StartCoroutine(SpawnEnemies());
        }

        private IEnumerator SpawnEnemies()
        {
            AbstractEnemy.OnEnemyIsDestroyed += DecrementEnemyCounter;

            foreach (Tilemap layout in enemyLayouts)
            {
                yield return new WaitForSeconds(waveDelay);
                
                BoundsInt bounds = layout.cellBounds;

                foreach (Vector3Int tilePosition in bounds.allPositionsWithin)
                {
                    var tile = layout.GetTile<AssetTile>(tilePosition);
                    
                    if (tile == null) continue;

                    EnemySpawner spawner = Instantiate(spawnerPrefab, layout.CellToWorld(tilePosition),
                        tile.tileAsset.transform.rotation);
                    spawner.enemyPrefab = tile.tileAsset;
                    enemyAmount++;
                }

                yield return new WaitUntil(() => enemyAmount == 0);
            }
            
            AbstractEnemy.OnEnemyIsDestroyed -= DecrementEnemyCounter;
            
            if (exitDoor != null) exitDoor.SetActive(false);
        }

        private void DecrementEnemyCounter() => enemyAmount--;
    }
}