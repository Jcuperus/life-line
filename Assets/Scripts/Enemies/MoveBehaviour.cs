using System;
using Player;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MoveBehaviour : MonoBehaviour
    {
        [SerializeField] protected float moveSpeed = 1f;
        
        protected new Rigidbody2D rigidbody;
        protected PlayerController player;

        [NonSerialized] public Vector2 moveDirection;
        
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            player = FindObjectOfType<PlayerController>();
        }
    }
}