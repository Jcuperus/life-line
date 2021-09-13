using System;
using Gameplay.Projectile;
using UnityEngine;

namespace Gameplay.AttackBehaviour
{
    /// <summary>
    /// Base class for enemy attack patterns, more easily allowing for complex and varied behaviours.
    /// </summary>
    public abstract class FireBehaviour : ScriptableObject
    {
        public delegate Vector3 CalculateDirectionAction();
        
        public abstract void Execute(ProjectileFactory.ProjectileTypes projectileType, MonoBehaviour source, Vector3 direction);

        public virtual void Execute(ProjectileFactory.ProjectileTypes projectileType, MonoBehaviour source, CalculateDirectionAction directionAction, Action finishedCallback)
        {
            Execute(projectileType, source, directionAction());
            finishedCallback();
        }
    }
}
