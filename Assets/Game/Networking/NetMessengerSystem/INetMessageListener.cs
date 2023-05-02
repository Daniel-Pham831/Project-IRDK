using Game.Networking.NetMessengerSystem.NetMessages;

namespace Game.Networking.NetMessengerSystem
{
    public interface INetMessageListener
    {
        void OnMessageReceived(INetMessage message);
    }
}