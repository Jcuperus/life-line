using System.Collections;
using UnityEngine;
public class BulletCircle : FireBehaviour
{    
    /**************** VARIABLES *******************/
    private readonly int bulletAmount = 20;
    private readonly float circleRadius = 9.5f;
    /**********************************************/
    /******************* INIT *********************/
    public BulletCircle(Transform position, Projectile projectile) : base(position, projectile)
    {
    }
    /**********************************************/
    /****************** METHODS *******************/
    public override IEnumerator Execute()
    {
        yield return null;

        float segmentOffset = 360f * Mathf.Deg2Rad / bulletAmount;
        
        for (int i = 0; i < bulletAmount; i++)
        {
            var circlePosition = new Vector3(Mathf.Cos(segmentOffset * i), Mathf.Sin(segmentOffset * i));
            Vector3 projectilePosition = position.position + circlePosition * circleRadius;
            Projectile newProjectile = Object.Instantiate(projectile, projectilePosition, Quaternion.Euler(0f, 0f, 0f));
            newProjectile.direction = (projectilePosition - position.position).normalized;
            newProjectile.ricochet = true;
        }
    }
    /**********************************************/
}