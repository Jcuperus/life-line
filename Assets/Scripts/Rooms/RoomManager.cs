using System.Collections;
using Enemies;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rooms
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField] private Tilemap[] enemyLayouts;
        [SerializeField] private TileDictionaryScriptableObject tileDictionary;
        [SerializeField] private PlayerTrigger roomTrigger;
        [SerializeField] private float waveDelay = 2f;

        private int enemyAmount;

        private void OnEnable()
        {
            roomTrigger.onPlayerEnter += StartRoom;
        }

        private void OnDisable()
        {
            roomTrigger.onPlayerEnter -= StartRoom;
        }

        private void StartRoom()
        {
            if (enemyLayouts == null || enemyLayouts.Length == 0) return;

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
                    var tile = layout.GetTile<Tile>(tilePosition);
                    
                    if (tile == null || !tileDictionary.ContainsKey(tile)) continue;
                    
                    GameObject tileAsset = tileDictionary[tile];
                    Instantiate(tileAsset, layout.CellToWorld(tilePosition), tileAsset.transform.rotation);
                    enemyAmount++;
                }

                yield return new WaitUntil(() => enemyAmount == 0);
            }
            
            AbstractEnemy.OnEnemyIsDestroyed -= DecrementEnemyCounter;
        }

        private void DecrementEnemyCounter() => enemyAmount--;
    }
}