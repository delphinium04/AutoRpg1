namespace Interfaces
{
    public interface IPoolable
    {
        public void Initialize(params object[] param);
        public void OnSpawn();
        public void OnDespawn();
    }
}