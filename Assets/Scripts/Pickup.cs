using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class Pickup : MonoBehaviour
{
    public PickupType type = PickupType.NOTSET;
}
public enum PickupType
{
    NOTSET,
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