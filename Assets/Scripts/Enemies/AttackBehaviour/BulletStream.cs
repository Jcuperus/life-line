using System.Collections;
using UnityEngine;

public class BulletStream : FireBehaviour
{
    /**************** VARIABLES *******************/
    private readonly int bulletAmount = 6;
    private readonly float fireDelay = 0.5f;
    private readonly Transform target;
    /**********************************************/
    
    /******************* INIT *********************/
    public BulletStream(Transform position, Projectile projectile, Transform target) : base(position, projectile)
    {
        this.target = target;
    }
    /**********************************************/
    
    /***************** METHODS ********************/
    public override IEnumerator Execute()
    {
        for (int i = 0; i < bulletAmount; i++)
        {
            Projectile newProjectile = Object.Instantiate(projectile, position.position, Quaternion.Euler(0f, 0f, 0f));
            newProjectile.direction = (target.position - position.position).normalized;
            newProjectile.Ricochet = true;

            yield return new WaitForSeconds(fireDelay);
        }
    }
    /**********************************************/
}