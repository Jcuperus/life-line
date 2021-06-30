using System.Collections;
using UnityEngine;

public class BulletArc : FireBehaviour
{
    /**************** VARIABLES *******************/
    private readonly Transform target;
    private readonly int bulletAmount = 5;
    private readonly int salvoAmount = 4;
    private readonly float fireDelay = 0.5f;
    private readonly float bulletDistance = 15f;
    private readonly float circleRadius = 9.5f;
    /**********************************************/
    /******************* INIT *********************/
    public BulletArc(Transform position, Projectile projectile, Transform target) : base(position, projectile)
    {
        this.target = target;
    }
    /**********************************************/
    /***************** METHODS ********************/
    public override IEnumerator Execute()
    {
        Vector3 direction = (target.position - position.position).normalized;
        float arcCenterAngle = VectorHelper.GetAngleFromDirection(direction) + 90f; // need to fix this function
        float arcLength = bulletDistance * bulletAmount;
        float arcStartAngle = arcCenterAngle - arcLength * 0.5f + bulletDistance * 0.5f;

        for (int j = 0; j < salvoAmount; j++)
        {
            yield return new WaitForSeconds(fireDelay);
            
            for (int i = 0; i < bulletAmount; i++)
            {
                float angle = (arcStartAngle + bulletDistance * i) * Mathf.Deg2Rad;
                Debug.Log(angle);
                var circlePosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector3 projectilePosition = position.position + circlePosition * circleRadius;
                Projectile newProjectile = Object.Instantiate(projectile, projectilePosition, Quaternion.Euler(0f, 0f, 0f));
                newProjectile.direction = (projectilePosition - position.position).normalized;
                newProjectile.Ricochet = true;
            }
        }
    }
    /**********************************************/
}
