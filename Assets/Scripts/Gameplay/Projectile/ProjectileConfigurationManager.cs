using System;
using System.Collections;
using Player;
using UnityEngine;

namespace Gameplay.Projectile
{
    public class ProjectileConfigurationManager : MonoBehaviour
    {
        public ProjectileConfiguration PlayerConfiguration { get; private set; }
        public ProjectileConfiguration PlayerRicochetConfiguration { get; private set; }
        public ProjectileConfiguration EnemyConfiguration { get; private set; }
        public ProjectileConfiguration EnemyRicochetConfiguration { get; private set; }
        
        [SerializeField] private ProjectileConfiguration playerConfigurationPrefab, playerRicochetConfigurationPrefab;
        [SerializeField] private ProjectileConfiguration enemyConfigurationPrefab, enemyRicochetConfigurationPrefab;

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