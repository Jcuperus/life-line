using System.Collections;
using UnityEngine;

public abstract class FireBehaviour
{
    protected Projectile projectile;
    public Transform position;
    
    public FireBehaviour(Transform position, Projectile projectile)
    {
        this.position = position;
        this.projectile = projectile;
    }
    
    public abstract IEnumerator Execute();
}
