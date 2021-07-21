using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;

namespace Tiles
{
    [CreateAssetMenu(fileName = "RoomBarrierTile", menuName = "Tiles/RoomBarrierTile", order = 1)]
    public class RoomBarrierTile : Tile
    {
        [SerializeField] private int roomID;

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
            gameObject = go;
            
            if (Application.isPlaying)
            {
                WaveManager.Instance.OnRoomIsFinished += id =>
                {
                    Debug.Log("destroy tile");
                    if (id == roomID) Destroy(gameObject);
                };
            }

            return base.StartUp(position, tilemap, go);
        }
    }
}