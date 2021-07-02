using System.Collections;
using UnityEngine;
using Gameplay.Projectile;

/// <summary>
/// Base class for enemy attack patterns, more easily allowing for complex and varied behaviours.
/// </summary>
public abstract class FireBehaviour
{
    /**************** VARIABLES *******************/
    protected readonly ProjectileFactory.ProjectileTypes projectileType;
    protected readonly Transform transform;
    protected readonly ProjectileFactory projectileFactory;
    /**********************************************/
    
    /****************** METHODS *******************/
    protected FireBehaviour(ProjectileFactory.ProjectileTypes projectileType, Transform transform)
    {
        this.projectileType = projectileType;
        this.transform = transform;
        projectileFactory = ProjectileFactory.Instance;
    }
    
    public abstract IEnumerator Execute();
    /**********************************************/
}
