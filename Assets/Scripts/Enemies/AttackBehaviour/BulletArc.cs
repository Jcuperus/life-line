using System.Collections;
using Gameplay.Projectile;
using UnityEngine;
using Utility;

namespace Enemies.AttackBehaviour
{
    public class BulletArc : FireBehaviour
    {
        /**************** VARIABLES *******************/
        private readonly Transform target;
        private const int BulletAmount = 5;
        private const int SalvoAmount = 4;
        private const float FireDelay = 0.5f;
        private const float BulletDistance = 15f;
        private const float CircleRadius = 9.5f;
        /**********************************************/
    
        /******************* INIT *********************/
        public BulletArc(ProjectileFactory.ProjectileTypes projectileType, Transform transform, Transform target) : base(projectileType, transform)
        {
            this.target = target;
        }
        /**********************************************/
    
        /***************** METHODS ********************/
        public override IEnumerator Execute()
        {
            Vector3 currentPosition = transform.position;
            Vector3 direction = (target.position - currentPosition).normalized;
            float arcCenterAngle = VectorHelper.GetAngleFromDirection(direction) + 90f; //TODO: need to fix this function
            float arcLength = BulletDistance * BulletAmount;
            float arcStartAngle = arcCenterAngle - arcLength * 0.5f + BulletDistance * 0.5f;

            for (int j = 0; j < SalvoAmount; j++)
            {
                yield return new WaitForSeconds(FireDelay);
            
                for (int i = 0; i < BulletAmount; i++)
                {
                    float angle = (arcStartAngle + BulletDistance * i) * Mathf.Deg2Rad;
                    var circlePosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
                    Vector3 projectilePosition = currentPosition + circlePosition * CircleRadius;
                    Vector2 projectileDirection = (projectilePosition - currentPosition).normalized;
                    projectileFactory.Instantiate(projectileType, projectilePosition,
                        projectileDirection);
                }
            }
        }
        /**********************************************/
    }
}
