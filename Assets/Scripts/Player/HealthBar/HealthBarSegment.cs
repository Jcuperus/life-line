using System.Collections.Generic;
using UnityEngine;
using Gameplay.Projectile;

namespace Player.HealthBar
{
    /// <summary>
    /// Behaviour for the gameObjects that form the player's health bar
    /// </summary>
    public class HealthBarSegment : MonoBehaviour, IProjectileHit
    {
        /**************** VARIABLES *******************/
        [SerializeField, Range(0,100)] private float moveSpeed = 50f;

        public LinkedListNode<GameObject> Node { get; private set; }
        /**********************************************/

        private void Awake()
        {
            Node = new LinkedListNode<GameObject>(gameObject);
        }

        /******************* LOOP *********************/
        private void FixedUpdate()
        {
            if (Node.Previous == null) return;

            Vector3 currentPosition = transform.position;
            Vector3 previousPosition = Node.Previous.Value.transform.position;
            Vector3 moveDirection = previousPosition - currentPosition;
            Vector3 desiredPosition = previousPosition - moveDirection.normalized * HealthBar.GetOffset(Node);
            transform.SetPositionAndRotation(
                Vector3.MoveTowards(currentPosition, desiredPosition, Time.deltaTime * moveSpeed),
                Quaternion.Euler(0f, 0f, VectorHelper.GetAngleFromDirection(moveDirection)));
        }
        /**********************************************/
        
        /****************** METHODS *******************/
        public void OnProjectileHit(Projectile projectile)
        {
            // do nothing
        }
        /**********************************************/
    }
}



