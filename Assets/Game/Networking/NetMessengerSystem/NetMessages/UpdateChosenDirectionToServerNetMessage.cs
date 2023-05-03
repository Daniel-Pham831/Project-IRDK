using System;
using Game.Enums;
using MemoryPack;

namespace Game.Networking.NetMessengerSystem.NetMessages
{
    [Serializable]
    [MemoryPackable]
    public partial class UpdateChosenDirectionToServerNetMessage : NetMessage
    {
        public Direction ChosenDirection;
        
        public UpdateChosenDirectionToServerNetMessage(Direction chosenDirection = Direction.None)
        {
            ChosenDirection = chosenDirection;
        }
    }
}