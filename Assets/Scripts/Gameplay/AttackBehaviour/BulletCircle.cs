using Gameplay.Projectile;
using UnityEngine;

namespace Gameplay.AttackBehaviour
{
    [CreateAssetMenu(fileName = "BulletCircle", menuName = "ScriptableObjects/AttackBehaviours/BulletCircle", order = 2)]
    public class BulletCircle : FireBehaviour
    {
        [SerializeField] private float circleRadius = 9.5f;
        [SerializeField] private int bulletAmount = 20;

        public override void Execute(ProjectileFactory.ProjectileTypes projectileType, MonoBehaviour source, Vector3 direction)
        {
            Vector3 currentPosition = source.transform.position;
            float segmentOffset = 360f * Mathf.Deg2Rad / bulletAmount;
        
            for (int i = 0; i < bulletAmount; i++)
            {
                var circlePosition = new Vector3(Mathf.Cos(segmentOffset * i), Mathf.Sin(segmentOffset * i));
                Vector3 projectilePosition = currentPosition + circlePosition * circleRadius;
                Vector2 projectileDirection = (projectilePosition - currentPosition).normalized;
                ProjectileFactory.Instance.Instantiate(projectileType, projectilePosition, projectileDirection);
            }
        }
    }
}