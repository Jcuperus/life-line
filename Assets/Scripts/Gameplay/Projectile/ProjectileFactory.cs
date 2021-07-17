using System;
using UnityEngine;
using Utility;

namespace Gameplay.Projectile
{
    public class ProjectileFactory : Singleton<ProjectileFactory>
    {
        [SerializeField] private Projectile projectilePrefab;
        
        [SerializeField] private ProjectileScriptableObject playerConfiguration;
        [SerializeField] private ProjectileScriptableObject playerRicochetConfiguration;
        [SerializeField] private ProjectileScriptableObject enemyConfiguration;
        [SerializeField] private ProjectileScriptableObject enemyRicochetConfiguration;

        private ObjectPool<Projectile> projectilePool;
        
        public enum ProjectileTypes
        {
            Player,
            Enemy,
            PlayerRicochet,
            EnemyRicochet
        }

        private const string PlayerLayer = "FriendlyProjectile";
        private const string EnemyLayer = "EnemyProjectile";

        protected override void Awake()
        {
            base.Awake();

            projectilePool = new ObjectPool<Projectile>(projectilePrefab);
        }

        public Projectile Instantiate(ProjectileTypes type, Vector3 position, Vector2 direction)
        {
            Projectile projectile = projectilePool.GetObject();

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