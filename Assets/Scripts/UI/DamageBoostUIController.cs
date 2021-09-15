using Player;
using UnityEngine;

namespace UI
{
    public class DamageBoostUIController : MonoBehaviour
    {
        [SerializeField] private GameObject tickContainer;
        [SerializeField] private GameObject tickPrefab;
        
        private void OnEnable()
        {
            PlayerController.OnDamageBoostChanged += UpdateCounter;
        }

        private void OnDisable()
        {
            PlayerController.OnDamageBoostChanged -= UpdateCounter;
        }

        private void UpdateCounter(int amount)
        {
            for (int i = 0; i < tickContainer.transform.childCount; i++)
            {
                Destroy(tickContainer.transform.GetChild(i).gameObject);
            }
            
            for (int i = 0; i < amount; i++)
            {
                Instantiate(tickPrefab, tickContainer.transform, false);
            }
        }
    }
}