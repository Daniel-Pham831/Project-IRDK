using System;
using Game.Enums;
using MemoryPack;

namespace Game.Networking.NetMessengerSystem.NetMessages
{
    [Serializable]
    [MemoryPackable]
    public partial class MoveToNextCellNetMessage : NetMessage
    {
        public Direction CellDirection;

        public MoveToNextCellNetMessage(Direction cellDirection)
        {
            CellDirection = cellDirection;
        }
    }
}