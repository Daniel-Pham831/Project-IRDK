using System;
using Game.Enums;
using MemoryPack;

namespace Game.Networking.NetMessengerSystem.NetMessages
{
    [Serializable]
    [MemoryPackable]
    public partial class UpdateChosenDirectionNetMessage : NetMessage
    {
        public Direction ChosenDirection;
        
        public UpdateChosenDirectionNetMessage(Direction chosenDirection = Direction.None)
        {
            ChosenDirection = chosenDirection;
        }
    }
}