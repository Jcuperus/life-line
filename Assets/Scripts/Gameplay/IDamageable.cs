namespace Gameplay
{
    /// <summary>
    /// Interface for objects that need to respond to being damaged
    /// </summary>
    public interface IDamageable
    {
        public void OnDamaged(int damage);
    }
}