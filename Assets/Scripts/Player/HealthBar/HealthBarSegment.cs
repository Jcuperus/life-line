using UnityEngine;
/// <summary>
/// Behaviour for the gameObjects that form the player's health bar
/// </summary>
public class HealthBarSegment : MonoBehaviour, IHealthBarNode, IProjectileHit
{
    /**************** VARIABLES *******************/
    [SerializeField][Range(0,50)] private float moveSpeed = 10f;
    private IHealthBarNode previousNode;
    public IHealthBarNode PreviousNode
    {
        get => previousNode;
        set
        {
            previousNode = value;
            if (previousNode != null) SetPositionAtNode(previousNode);
        }
    }
    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }
    public IHealthBarNode NextNode { get; set; }
    public float headOffset = 3f;
    public float offset = 1.5f;
    /**********************************************/
    
    /******************* LOOP *********************/
    private void FixedUpdate()
    {
        if (PreviousNode == null) return;

        Vector3 moveDirection = PreviousNode.Position - transform.position;
        Vector3 desiredPosition = PreviousNode.Position - moveDirection.normalized * GetOffset();
        transform.SetPositionAndRotation(Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * moveSpeed), Quaternion.Euler(0f, 0f, VectorHelper.GetAngleFromDirection(moveDirection)));
    }
    /**********************************************/
    
    /****************** METHODS *******************/
    private void SetPositionAtNode(IHealthBarNode node)
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
    
    public void AddTail(IHealthBarNode tail)
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
        // do nothing
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
    /**********************************************/
}
