using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Interfaces
{
    public interface IInteractableMono
    {
        public MonoBehaviour Mono { get; } 
        public int InteractPriority { get; }
        UniTask Interact(object interactor = null);
    }
}