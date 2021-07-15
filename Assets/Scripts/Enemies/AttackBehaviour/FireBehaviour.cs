using System.Collections;
using Gameplay.Projectile;
using UnityEngine;

namespace Enemies.AttackBehaviour
{
    /// <summary>
    /// Base class for enemy attack patterns, more easily allowing for complex and varied behaviours.
    /// </summary>
    public abstract class FireBehaviour
    {
        /**************** VARIABLES *******************/
        protected readonly ProjectileFactory.ProjectileTypes projectileType;
        protected readonly Transform transform;
        /**********************************************/
    
        /****************** METHODS *******************/
        protected FireBehaviour(ProjectileFactory.ProjectileTypes projectileType, Transform transform)
        {
            this.projectileType = projectileType;
            this.transform = transform;
        }
    
        public abstract IEnumerator Execute();
        /**********************************************/
    }
}
