using UnityEngine;

public interface HealthBarNode
{
    // public Transform transform;
    public HealthBarNode PreviousNode { get; set; }
    
    public HealthBarNode NextNode { get; set; }
    
    public Vector3 Position { get; set; }

    public void AddTail(HealthBarNode tail);
    public void PassHit();
}