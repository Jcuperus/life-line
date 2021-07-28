using System.Collections;
using Gameplay.Projectile;
using UnityEngine;

namespace Gameplay.AttackBehaviour
{
    [CreateAssetMenu(fileName = "RepeatingFireBehaviour", menuName = "ScriptableObjects/AttackBehaviours/RepeatingFireBehaviour", order = 3)]
    public class RepeatingFireBehaviour : FireBehaviour
    {
        [SerializeField] private FireBehaviour attackBehaviour;
        [SerializeField] private float fireDelay = 0.5f;
        [SerializeField] private int repeatAmount = 6;
        
        private bool isFinished;

        public override void Execute(ProjectileFactory.ProjectileTypes projectileType, MonoBehaviour source, Vector3 direction)
        {
            source.StartCoroutine(ExecuteCoroutine(projectileType, source, () => direction));
        }

        public override void Execute(ProjectileFactory.ProjectileTypes projectileType, MonoBehaviour source, CalculateDirectionAction directionAction)
        {
            source.StartCoroutine(ExecuteCoroutine(projectileType, source, directionAction));
        }

        public override bool IsFinished()
        {
            return isFinished;
        }

        private IEnumerator ExecuteCoroutine(ProjectileFactory.ProjectileTypes projectileType, MonoBehaviour source, CalculateDirectionAction directionAction)
        {
            for (int i = 0; i < repeatAmount; i++)
            {
                yield return new WaitForSeconds(fireDelay);
                attackBehaviour.Execute(projectileType, source, directionAction());
            }

            isFinished = true;
        }
    }
}