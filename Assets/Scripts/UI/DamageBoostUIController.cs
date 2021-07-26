using Player;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class DamageBoostUIController : MonoBehaviour
    {
        private TextMeshProUGUI boostCounterText;
        
        private void Awake()
        {
            boostCounterText = GetComponent<TextMeshProUGUI>();
        }

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
            boostCounterText.text = "Damage Boost: " + amount;
        }
    }
}