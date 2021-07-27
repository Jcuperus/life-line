using System.Collections;
using Gameplay.Projectile;
using UnityEngine;

namespace Gameplay.AttackBehaviour
{
    /// <summary>
    /// Base class for enemy attack patterns, more easily allowing for complex and varied behaviours.
    /// </summary>
    public abstract class FireBehaviour
    {
        /**************** VARIABLES *******************/
        protected readonly Transform origin;
        protected readonly Transform target;
        /**********************************************/
    
        /****************** METHODS *******************/
        protected FireBehaviour(Transform origin, Transform target = null)
        {
            this.origin = origin;
            this.target = target;
        }
    
        public abstract IEnumerator Execute(ProjectileFactory.ProjectileTypes projectileType);
        /**********************************************/
    }
}
