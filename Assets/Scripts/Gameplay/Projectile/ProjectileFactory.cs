using UnityEngine;

namespace Gameplay.Projectile
{
    public class ProjectileFactory
    {
        [SerializeField] private Projectile prefab;
        
        [SerializeField] private Sprite friendlySprite;
        [SerializeField] private Sprite enemySprite;
        [SerializeField] private Sprite bouncySprite;
    }
}