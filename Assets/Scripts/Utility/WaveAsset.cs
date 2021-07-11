using System;
using Enemies;
using UnityEngine;

namespace Utility
{
    /// <summary>
    /// ScriptableObject containing data to populate one room.
    /// </summary>
    [CreateAssetMenu(fileName = "WaveAsset",menuName = "ScriptableObjects/WaveAsset")]
    public class WaveAsset : ScriptableObject
    {
        [SerializeField] private Wave[] waves;
        public Wave[] Waves => waves;
    }
    [Serializable]
    public class Wave
    {
        public SubWave[] subWaves;
    }
    [Serializable]
    public class SubWave
    {
        public AbstractEnemy type;
        public int amount;
        public float delay;
    }
}

