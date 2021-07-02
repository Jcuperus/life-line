namespace Gameplay.Projectile
{
    /// <summary>
    /// Inherit from this interface to use the <see cref="OnProjectileHit(Projectile)"/> event.
    /// </summary>
    public interface IProjectileHit
    {
        public void OnProjectileHit(Projectile projectile);
    }
}