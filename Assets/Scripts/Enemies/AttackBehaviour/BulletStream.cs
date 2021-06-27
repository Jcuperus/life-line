using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletStream : FireBehaviour
{
    private int bulletAmount = 6;
    private float fireDelay = 0.5f;
    private Transform target;
    
    public BulletStream(Transform position, Projectile projectile, Transform target) : base(position, projectile)
    {
        this.target = target;
    }

    public override IEnumerator Execute()
    {
        for (int i = 0; i < bulletAmount; i++)
        {
            Projectile newProjectile = Object.Instantiate(projectile, position.position, Quaternion.Euler(0f, 0f, 0f));
            newProjectile.direction = (target.position - position.position).normalized;
            newProjectile.ricochet = true;

            yield return new WaitForSeconds(fireDelay);
        }
    }
}