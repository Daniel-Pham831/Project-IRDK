using System;
using Maniac.MessengerSystem.Messages;

namespace Game.Networking.NetMessages
{
    [Serializable]
    public class ServerMessage : Message
    {
        public ServerMessage(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }
        public object Value { get; private set; } 
    }
}