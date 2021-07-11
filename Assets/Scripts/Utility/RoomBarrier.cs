using UnityEngine;

namespace Utility
{
    public class RoomBarrier : MonoBehaviour
    {
        [SerializeField] private int roomID;
    
        private void Start()
        {
            WaveManager.Instance.OnRoomIsFinished += id =>
            {
                if (id == roomID) Destroy(gameObject);
            };
        }
    }
}