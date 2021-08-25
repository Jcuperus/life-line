using System;
using UnityEngine;

namespace Rooms
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerTrigger : MonoBehaviour
    {
        public Action onPlayerEnter;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                onPlayerEnter.Invoke();
                Destroy(gameObject);
            }
        }
    }
}