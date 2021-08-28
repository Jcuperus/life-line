using System;
using Enemies;
using UnityEngine;

namespace Utility
{
    /// <summary>
    /// ScriptableObject containing data to populate one room.
    /// </summary>
    [CreateAssetMenu(fileName = "SpawnerArray",menuName = "ScriptableObjects/SpawnerArray")]
    public class SpawnConfigAsset : ScriptableObject
    {
        [SerializeField] private EnemySpawnConfig[] spawnerConfigs;
        public EnemySpawnConfig[] SpawnerConfigs => spawnerConfigs;
    }
    [Serializable]
    public class EnemySpawnConfig
    {
        public AbstractEnemy prefab;
        public int amount;
        public float spawnDelay = 5f;
    }
}

