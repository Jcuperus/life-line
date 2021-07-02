using System;
using UnityEngine;

namespace Gameplay.Projectile
{
    public class ProjectileFactory : MonoSingleton<ProjectileFactory>
    {
        [SerializeField] private Projectile projectilePrefab;
        
        [SerializeField] private ProjectileScriptableObject playerConfiguration;
        [SerializeField] private ProjectileScriptableObject playerRicochetConfiguration;
        [SerializeField] private ProjectileScriptableObject enemyConfiguration;
        [SerializeField] private ProjectileScriptableObject enemyRicochetConfiguration;

        public enum ProjectileTypes
        {
            Player,
            Enemy,
            PlayerRicochet,
            EnemyRicochet
        }

        private const string PlayerLayer = "FriendlyProjectile";
        private const string EnemyLayer = "EnemyProjectile";

        public Projectile Instantiate(ProjectileTypes type, Vector3 position, Vector2 direction)
        {
            Projectile projectile = Instantiate(projectilePrefab);

            projectile.projectileConfiguration = type switch
            {
                ProjectileTypes.Player => playerConfiguration,
                ProjectileTypes.Enemy => enemyConfiguration,
                ProjectileTypes.PlayerRicochet => playerRicochetConfiguration,
                ProjectileTypes.EnemyRicochet => enemyRicochetConfiguration,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            
            projectile.gameObject.layer = LayerMask.NameToLayer(projectile.projectileConfiguration.playerIsOwner ? PlayerLayer : EnemyLayer);
            projectile.transform.position = position;
            projectile.direction = direction;
            projectile.gameObject.SetActive(true);
            return projectile;
        }
    }
}