namespace Game.Networking.NetMessengerSystem
{
    public interface INetMessageListener
    {
        void OnMessageReceived(INetMessage message);
    }
}