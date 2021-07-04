﻿using UnityEngine;

namespace Gameplay.Projectile
{
    [CreateAssetMenu(fileName = "ProjectileConfiguration", menuName = "ScriptableObjects/ProjectileScriptableObject", order = 1)]
    public class ProjectileScriptableObject : ScriptableObject
    {
        public Sprite projectileSprite;
        public float projectileSpeed;
        public bool playerIsOwner;
        public bool canRicochet;
    }
}