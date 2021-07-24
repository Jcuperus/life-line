using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(fileName = "PickupConfiguration", menuName = "ScriptableObjects/PickupScriptableObject", order = 2)]
    public class PickupScriptableObject : ScriptableObject
    {
        public Pickup[] pickups;

        public Pickup GetRandomPickup()
        {

            for (int i = 0; i < pickups.Length+1; i++)
            {
                Pickup attemptPickup = pickups[Random.Range(0, pickups.Length)];
                if (attemptPickup.Type != PickupType.HealthUp) return attemptPickup;
            }
            return null;
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