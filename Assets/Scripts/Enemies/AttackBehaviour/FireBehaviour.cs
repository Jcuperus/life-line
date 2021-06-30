using System.Collections;
using UnityEngine;
/// <summary>
/// Base class for enemy attack patterns, more easily allowing for complex and varied behaviours.
/// </summary>
public abstract class FireBehaviour
{
    /**************** VARIABLES *******************/
    protected Projectile projectile;
    public Transform position;
    /**********************************************/
    /****************** METHODS *******************/
    public FireBehaviour(Transform position, Projectile projectile)
    {
        this.position = position;
        this.projectile = projectile;
    }
    public abstract IEnumerator Execute();
    /**********************************************/
}
