/// <summary>
/// Class which allows various objects in the project to communicate with one another without needing many references back and forth.
/// </summary>
public static class EventBroker
{
    public delegate void SpawnEnemyAction(int room);
    public static event SpawnEnemyAction SpawnEnemyEvent;
    
    public static void SpawnEnemyTrigger(int room)
    {
        SpawnEnemyEvent?.Invoke(room);
    }
    
    public delegate void SpawnPickupAction(int room, int index = -1);
    public static event SpawnPickupAction SpawnPickupEvent;
    
    public static void SpawnPickupTrigger(int room, int index = -1)
    {
        SpawnPickupEvent?.Invoke(room, index);
    }
    
    public delegate void LevelReadyAction();
    public static event LevelReadyAction LevelReadyEvent;
    
    public static void LevelReadyTrigger()
    {
        LevelReadyEvent?.Invoke();
    }
}
