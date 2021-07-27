using System.Collections;
using Gameplay.Projectile;
using UnityEngine;

namespace Gameplay.AttackBehaviour
{
    public class BulletStream : FireBehaviour
    {
        /**************** VARIABLES *******************/
        private readonly int bulletAmount = 6;
        private readonly float fireDelay = 0.5f;
        /**********************************************/

        public BulletStream(Transform origin, Transform target = null) : base(origin, target) {}

        /***************** METHODS ********************/
        public override IEnumerator Execute(ProjectileFactory.ProjectileTypes projectileType)
        {
            Vector3 currentPosition = origin.position;
        
            for (int i = 0; i < bulletAmount; i++)
            {
                Vector2 direction = (target.position - currentPosition).normalized;
                ProjectileFactory.Instance.Instantiate(projectileType, currentPosition, direction);
                yield return new WaitForSeconds(fireDelay);
            }
        }
        /**********************************************/
    }
}