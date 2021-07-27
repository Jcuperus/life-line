using System.Collections;
using Gameplay.Projectile;
using UnityEngine;

namespace Gameplay.AttackBehaviour
{
    public class BulletSingle : FireBehaviour
    {
        public BulletSingle(Transform origin, Transform target = null) : base(origin, target) {}

        public override IEnumerator Execute(ProjectileFactory.ProjectileTypes projectileType)
        {
            Vector3 currentPosition = origin.position;
            Vector2 direction = (target.position - currentPosition).normalized;
            ProjectileFactory.Instance.Instantiate(projectileType, currentPosition, direction);
            
            yield return null;
        }
    }
}