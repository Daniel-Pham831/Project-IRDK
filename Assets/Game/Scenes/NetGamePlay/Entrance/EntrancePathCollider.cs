using Game.Enums;
using ToolBox.Tags;
using UnityEngine;

namespace Game.Scenes.NetGamePlay.Entrance
{
    public class EntrancePathCollider : MonoBehaviour
    {
        [SerializeField] private Direction entranceDirection;
        private EntranceController _entranceController;
        
        public void Init(EntranceController entranceController)
        {
            _entranceController = entranceController;
        }
        
        private async void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.HasTag(_entranceController.LocalPlayerTag))
            {
                await _entranceController.OnPlayerEnter(entranceDirection);
            }
        }

        private async void OnTriggerExit2D(Collider2D col)
        {
            if (col.gameObject.HasTag(_entranceController.LocalPlayerTag))
            {
                await _entranceController.OnPlayerExit(entranceDirection);
            }
        }
    }
}