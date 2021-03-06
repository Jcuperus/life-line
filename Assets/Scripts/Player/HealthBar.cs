using System.Collections.Generic;
using UnityEngine;
using Utility.Extensions;

namespace Player
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private HealthBarSegment segmentPrefab;
        [SerializeField] private float attachmentDelay = 1.5f;
        
        private const float HeadOffset = 2.5f, Offset = 2f;

        private readonly LinkedList<GameObject> segments = new LinkedList<GameObject>();
        private bool canAttach = true;

        public int Count => segments.Count;

        public void AddFirst(LinkedListNode<GameObject> node)
        {
            if (!canAttach) return;
            
            segments.AddFirst(node);
        }

        public void RemoveFirst()
        {
            segments.RemoveFirst();

            canAttach = false;
            this.DelayedAction(() => canAttach = true, attachmentDelay);
        }

        public bool IsFirst(LinkedListNode<GameObject> node)
        {
            return segments.First == node;
        }

        public void AddLast(LinkedListNode<GameObject> node)
        {
            segments.AddLast(node);
            SetInitialNodePosition(node);
        }

        public void RemoveLast()
        {
            Destroy(segments.Last.Value);
            segments.RemoveLast();
        }

        public void SpawnSegment()
        {
            HealthBarSegment newSegment = Instantiate(segmentPrefab, transform, true);
            newSegment.Parent = this;
            AddLast(newSegment.Node);
        }
        
        public static float GetOffset(LinkedListNode<GameObject> node)
        {
            return node.Previous != null && node.Previous.Value.CompareTag("Player") ? HeadOffset : Offset;
        }

        private static void SetInitialNodePosition(LinkedListNode<GameObject> node)
        {
            if (node.Previous == null) return;

            Vector3 previousPosition = node.Previous.Value.transform.position;
            Vector3 direction = node.Value.transform.eulerAngles.y.GetDirectionVector().normalized;
            Vector3 offsetPosition = previousPosition - direction.normalized * GetOffset(node);
            node.Value.transform.position = offsetPosition;
        }
    }
}