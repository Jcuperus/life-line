using UnityEngine;

public class RoomBarrier : MonoBehaviour
{
    [SerializeField] private int roomID;
    
    private void Awake()
    {
        WaveManager.OnRoomIsFinished += id =>
        {
            if (id == roomID) Destroy(gameObject);
        };
    }
}
