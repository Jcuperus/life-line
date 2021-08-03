using System.Collections.Generic;
using UnityEngine;
using Utility.Extensions;

namespace Player
{
    /// <summary>
    /// Behaviour for the gameObjects that form the player's health bar
    /// </summary>
    public class HealthBarSegment : MonoBehaviour
    {
        /**************** VARIABLES *******************/
        [SerializeField, Range(0,100)] private float moveSpeed = 50f;

        public LinkedListNode<GameObject> Node { get; private set; }

        public HealthBar Parent { get; set; }
        /**********************************************/

        private void Awake()
        {
            Node = new LinkedListNode<GameObject>(gameObject);
        }

        private void FixedUpdate()
        {
            if (Node.Previous == null) return;

            Vector3 currentPosition = transform.position;
            Vector3 previousPosition = Node.Previous.Value.transform.position;
            Vector3 moveDirection = previousPosition - currentPosition;
            Vector3 desiredPosition = previousPosition - moveDirection.normalized * HealthBar.GetOffset(Node);
            transform.SetPositionAndRotation(
                Vector3.MoveTowards(currentPosition, desiredPosition, Time.deltaTime * moveSpeed),
                Quaternion.Euler(0f, 0f, moveDirection.GetAngle()));
        }
    }
}



