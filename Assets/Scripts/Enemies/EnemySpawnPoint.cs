using System.Collections;
using Spine;
using Spine.Unity;
using UnityEngine;

/// <summary>
/// Behaviour for glitched walls, functioning as enemy spawn points. 
/// </summary>
public class EnemySpawnPoint : MonoBehaviour
{
    /**************** VARIABLES *******************/
    [Header("Animation")]
    [SerializeField] private SkeletonAnimation animator;
    [SerializeField] private AnimationReferenceAsset spawnAnimation;
    
    [Header("Config")]
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;
    [SerializeField] private int roomID;
    /**********************************************/
    
    /******************* INIT *********************/
    public void Start()
    {
        WaveManager.Instance.OnSubWaveStartAction += SpawnSubWave;
    }
    /**********************************************/
    
    /***************** METHODS ********************/
    private void SpawnSubWave(int roomID, SubWave subWave)
    {
        if (roomID == this.roomID) StartCoroutine(SpawnEnemies(subWave));
    }

    private IEnumerator SpawnEnemies(SubWave subWave)
    {
        for (int i = 0; i < subWave.amount; i++)
        {
            yield return new WaitForSeconds(subWave.delay);
            SetAnimation(spawnAnimation, false, 0.5f);
            AbstractEnemy spawnedEnemy = Instantiate(subWave.type);
            spawnedEnemy.transform.position = transform.position + spawnOffset;
        }
    }
    
    private void SetAnimation(AnimationReferenceAsset animation, bool loop, float timeScale)
    {
        TrackEntry trackEntry = animator.state.SetAnimation(0, animation, loop);
        trackEntry.TimeScale = timeScale;
    }
    /**********************************************/
}
