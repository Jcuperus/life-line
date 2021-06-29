using System;
using UnityEngine;

public class HealthBarSegment : MonoBehaviour, HealthBarNode, ProjectileHit
{
    [SerializeField] private float moveSpeed = 10f;

    private HealthBarNode _previousNode;
    
    public HealthBarNode PreviousNode
    {
        get => _previousNode;
        set
        {
            _previousNode = value;
            if (_previousNode != null) SetPositionAtNode(_previousNode);
        }
    }

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public HealthBarNode NextNode { get; set; }

    public float headOffset = 3f;
    public float offset = 1.5f;

    private void FixedUpdate()
    {
        if (PreviousNode == null) return;

        Vector3 moveDirection = PreviousNode.Position - transform.position;
        Vector3 desiredPosition = PreviousNode.Position - moveDirection.normalized * GetOffset();
        transform.rotation = Quaternion.Euler(0f, 0f, VectorHelper.GetAngleFromDirection(moveDirection));
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * moveSpeed);
    }

    private void SetPositionAtNode(HealthBarNode node)
    {
        transform.position = GetOffsetPosition(node.Position);
    }

    private float GetOffset()
    {
        return PreviousNode is PlayerMovement ? headOffset : offset;
    }

    private Vector3 GetOffsetPosition(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        return position - direction.normalized * GetOffset();
    }

    public void AddTail(HealthBarNode tail)
    {
        if (NextNode == null)
        {
            NextNode = tail;
            tail.PreviousNode = this;
        }
        else
        {
            NextNode.AddTail(tail);
        }
    }

    public void OnProjectileHit(Projectile projectile)
    {

    }

    public void PassHit()
    {
        if (NextNode == null)
        {
            PreviousNode.NextNode = null;
            Destroy(gameObject);
        }
        else
        {
            NextNode.PassHit();
        }
    }
}
