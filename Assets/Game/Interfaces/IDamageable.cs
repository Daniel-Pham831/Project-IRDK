using Cysharp.Threading.Tasks;

namespace Game.Interfaces
{
    public interface IDamageable
    {
        UniTask TakeDamage(float amount);
    }
}