using UnityEngine;
/// <summary>
/// Interface for the behaviour of the individual components the player's health bar consists of.
/// </summary>
public interface IHealthBarNode
{
    // public Transform transform;
    public IHealthBarNode PreviousNode { get; set; }
    
    public IHealthBarNode NextNode { get; set; }
    
    public Vector3 Position { get; set; }

    public void AddTail(IHealthBarNode tail);
    public void PassHit();
}