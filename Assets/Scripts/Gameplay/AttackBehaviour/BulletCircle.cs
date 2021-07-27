using System.Collections;
using Gameplay.Projectile;
using UnityEngine;

namespace Gameplay.AttackBehaviour
{
    public class BulletCircle : FireBehaviour
    {    
        /**************** VARIABLES *******************/
        private readonly int bulletAmount = 20;
        private readonly float circleRadius = 9.5f;
        /**********************************************/

        public BulletCircle(Transform origin, Transform target = null) : base(origin, target) {}

        /****************** METHODS *******************/
        public override IEnumerator Execute(ProjectileFactory.ProjectileTypes projectileType)
        {
            yield return null;

            Vector3 currentPosition = origin.position;
            float segmentOffset = 360f * Mathf.Deg2Rad / bulletAmount;
        
            for (int i = 0; i < bulletAmount; i++)
            {
                var circlePosition = new Vector3(Mathf.Cos(segmentOffset * i), Mathf.Sin(segmentOffset * i));
                Vector3 projectilePosition = currentPosition + circlePosition * circleRadius;
                Vector2 direction = (projectilePosition - currentPosition).normalized;
                ProjectileFactory.Instance.Instantiate(projectileType, projectilePosition, direction);
            }
        }
        /**********************************************/
    }
}