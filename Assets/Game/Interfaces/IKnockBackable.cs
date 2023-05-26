using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Interfaces
{
    public interface IKnockBackable
    {
        UniTask KnockBack(Vector2 direction, float distance);
    }
}