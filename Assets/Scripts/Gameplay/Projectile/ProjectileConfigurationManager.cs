using System.Collections;
using Player;
using UnityEngine;

namespace Gameplay.Projectile
{
    public class ProjectileConfigurationManager : MonoBehaviour
    {
        public ProjectileScriptableObject PlayerConfiguration { get; private set; }
        public ProjectileScriptableObject PlayerRicochetConfiguration { get; private set; }
        public ProjectileScriptableObject EnemyConfiguration { get; private set; }
        public ProjectileScriptableObject EnemyRicochetConfiguration { get; private set; }
        
        [SerializeField] private ProjectileScriptableObject playerConfigurationPrefab, playerRicochetConfigurationPrefab;
        [SerializeField] private ProjectileScriptableObject enemyConfigurationPrefab, enemyRicochetConfigurationPrefab;

        private bool speedShotIsActive;

        private void Awake()
        {
            PlayerConfiguration = Instantiate(playerConfigurationPrefab);
            PlayerRicochetConfiguration = Instantiate(playerRicochetConfigurationPrefab);
            EnemyConfiguration = Instantiate(enemyConfigurationPrefab);
            EnemyRicochetConfiguration = Instantiate(enemyRicochetConfigurationPrefab);
        }

        private void OnEnable()
        {
            GameManager.OnSpeedShotActivated += ApplySpeedShot;
            PlayerController.OnDamageBoostChanged += ApplyDamageMultiplier;
        }

        private void OnDisable()
        {
            GameManager.OnSpeedShotActivated -= ApplySpeedShot;
            PlayerController.OnDamageBoostChanged -= ApplyDamageMultiplier;
        }

        private void ApplyDamageMultiplier(int amount)
        {
            PlayerConfiguration.damage = playerConfigurationPrefab.damage + amount;
            PlayerRicochetConfiguration.damage = playerRicochetConfigurationPrefab.damage + amount;
        }

        private void ApplySpeedShot(float duration)
        {
            if (speedShotIsActive) return;

            StartCoroutine(SpeedShotCoroutine(duration));
        }
        
        private IEnumerator SpeedShotCoroutine(float duration)
        {
            speedShotIsActive = true;

            float initialSpeed = PlayerConfiguration.projectileSpeed;
            float initialRicochetSpeed = PlayerRicochetConfiguration.projectileSpeed;

            PlayerConfiguration.projectileSpeed *= 3f;
            PlayerRicochetConfiguration.projectileSpeed *= 3f;
            
            yield return new WaitForSeconds(duration);

            PlayerConfiguration.projectileSpeed = initialSpeed;
            PlayerRicochetConfiguration.projectileSpeed = initialRicochetSpeed;
            
            speedShotIsActive = false;
        }
    }
}