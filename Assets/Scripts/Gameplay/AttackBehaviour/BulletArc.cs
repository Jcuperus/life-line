using Gameplay.Projectile;
using UnityEngine;
using Utility.Extensions;

namespace Gameplay.AttackBehaviour
{
    [CreateAssetMenu(fileName = "BulletArc", menuName = "ScriptableObjects/AttackBehaviours/BulletArc", order = 1)]
    public class BulletArc : FireBehaviour
    {
        [SerializeField] private int bulletAmount = 5;
        [SerializeField] private float bulletDistance = 15f, circleRadius = 9.5f;
        
        public override void Execute(ProjectileFactory.ProjectileTypes projectileType, MonoBehaviour source, Vector3 direction)
        {
            Vector3 origin = source.transform.position;
            float arcCenterAngle = direction.GetAngle() + 90f;
            float arcLength = bulletDistance * bulletAmount;
            float arcStartAngle = arcCenterAngle - arcLength * 0.5f + bulletDistance * 0.5f;

            for (int i = 0; i < bulletAmount; i++)
            {
                float angle = (arcStartAngle + bulletDistance * i) * Mathf.Deg2Rad;
                var circlePosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector3 projectilePosition = origin + circlePosition * circleRadius;
                Vector2 projectileDirection = (projectilePosition - origin).normalized;
                ProjectileFactory.Instance.Instantiate(projectileType, projectilePosition,
                    projectileDirection);
            }
        }
    }
}
