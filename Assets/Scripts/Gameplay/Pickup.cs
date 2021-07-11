using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// MonoBehaviour for gameObject containing simply the enum dictating which powerUp is activated on pickup. 
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Pickup : MonoBehaviour
    {
        /**************** VARIABLES *******************/
        [SerializeField] private PickupType type = PickupType.NotSet;
        public PickupType Type => type;
        /**********************************************/
    }
    
    public enum PickupType
    {
        NotSet,
        DamageUp,
        FireRateUp,
        ShotSpeedUp,
        SpeedUp,
        HealthUp,
        SpikeShield,
        BigShield,
        PiercingShot,
        GhostShot,
        Ricochet,
        KnockBack,
        SpreadShot,
        BullCharge,
        BulletTime
    }
}