using System.Collections;
using UnityEngine;

/// <summary>
/// Base class for enemy attack patterns, more easily allowing for complex and varied behaviours.
/// </summary>
public abstract class FireBehaviour
{
    /**************** VARIABLES *******************/
    protected readonly Projectile projectile;
    protected readonly Transform position;
    /**********************************************/
    
    /****************** METHODS *******************/
    protected FireBehaviour(Transform position, Projectile projectile)
    {
        this.position = position;
        this.projectile = projectile;
    }
    
    public abstract IEnumerator Execute();
    /**********************************************/
}
