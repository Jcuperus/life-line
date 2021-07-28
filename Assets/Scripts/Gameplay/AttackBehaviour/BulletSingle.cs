using Gameplay.Projectile;
using UnityEngine;

namespace Gameplay.AttackBehaviour
{
    [CreateAssetMenu(fileName = "BulletSingle", menuName = "ScriptableObjects/AttackBehaviours/BulletSingle", order = 0)]
    public class BulletSingle : FireBehaviour
    {
        [SerializeField] private float offset = 2f;
        
        public override void Execute(ProjectileFactory.ProjectileTypes projectileType, MonoBehaviour source, Vector3 direction)
        {
            Vector3 projectilePosition = source.transform.position + direction * offset;
            ProjectileFactory.Instance.Instantiate(projectileType, projectilePosition, direction);
        }
    }
}