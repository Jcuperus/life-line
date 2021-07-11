using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(fileName = "PickupConfiguration", menuName = "ScriptableObjects/PickupScriptableObject", order = 2)]
    public class PickupScriptableObject : ScriptableObject
    {
        public Pickup[] pickups;

        public Pickup GetRandomPickup()
        {
            return pickups[Random.Range(0, pickups.Length)];
        }

        public Pickup GetPickup(PickupType type)
        {
            foreach (Pickup pickup in pickups)
            {
                if (pickup.Type == type) return pickup;
            }

            return null;
        }
    }
}