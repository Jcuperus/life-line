using System.Collections;
using Gameplay.Projectile;
using UnityEngine;

namespace Enemies.AttackBehaviour
{
    public class BulletStream : FireBehaviour
    {
        /**************** VARIABLES *******************/
        private readonly int bulletAmount = 6;
        private readonly float fireDelay = 0.5f;
        private readonly Transform target;
        /**********************************************/
    
        /******************* INIT *********************/
        public BulletStream(ProjectileFactory.ProjectileTypes projectileType, Transform transform, Transform target) : base(projectileType, transform)
        {
            this.target = target;
        }
        /**********************************************/
    
        /***************** METHODS ********************/
        public override IEnumerator Execute()
        {
            Vector3 currentPosition = transform.position;
        
            for (int i = 0; i < bulletAmount; i++)
            {
                Vector2 direction = (target.position - currentPosition).normalized;
                projectileFactory.Instantiate(projectileType, currentPosition, direction);

                yield return new WaitForSeconds(fireDelay);
            }
        }
        /**********************************************/
    }
}