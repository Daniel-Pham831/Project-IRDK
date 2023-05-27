using Game.Networking.NetMessengerSystem;
using Game.Networking.NetMessengerSystem.NetMessages;
using Game.Networking.NormalMessages;
using Maniac.MessengerSystem.Base;
using Maniac.MessengerSystem.Messages;
using Maniac.Utils;
using UniRx;
using Unity.Netcode;

namespace Game.Coin
{
    public class CoinSystem : INetMessageListener , IMessageListener
    {
        public IntReactiveProperty SharedCoin { get; private set; } = new IntReactiveProperty(0);
        public IntReactiveProperty PrivateCoin { get; private set; } = new IntReactiveProperty(0);
    
        private NetMessageTransmitter _netMessageTransmitter => Locator<NetMessageTransmitter>.Instance;

        /// <summary>
        ///  Khởi tạo Hệ thống xử lý coin
        /// </summary>
        public void Init()
        {
            Locator<CoinSystem>.Set(this);
            
            RegisterMessages(true);
        }
        
        private void Reset()
        {
            RegisterMessages(false);
            Locator<CoinSystem>.Remove(this);
        }

        private void RegisterMessages(bool shouldRegister)
        {
            if (shouldRegister)
            {
                // for net messages
                _netMessageTransmitter.Register<UpdateShareCoinNetMessage>(this);
                _netMessageTransmitter.Register<UpdatePrivateCoinToClientNetMessage>(this);
                
                // for normal messages
                Messenger.Register<ApplicationQuitMessage>(this);
                Messenger.Register<TransportFailureMessage>(this);
            }
            else
            {
                _netMessageTransmitter?.UnregisterAll(this);
                Messenger.UnregisterAll(this);
            }
        }

        public void OnNetMessageReceived(NetMessage receivedMessage)
        {
            switch (receivedMessage)
            {
                case UpdateShareCoinNetMessage message:
                    this.HandleUpdateSharedCoinMessage(message);
                    break;
                case UpdatePrivateCoinToClientNetMessage message:
                    this.HandleUpdatePrivateCoinToClientsMessage(message);
                    break;
            }
        }
        
        public void OnMessagesReceived(Message receivedMessage)
        {
            switch (receivedMessage)
            {
                case ApplicationQuitMessage:
                case TransportFailureMessage:
                    Reset();
                    break;
            }
        }

        private void HandleUpdatePrivateCoinToClientsMessage(UpdatePrivateCoinToClientNetMessage message)
        {
            PrivateCoin.Value = message.privateCoin;
        }

        /// <summary>
        /// xử lý coin từ client chả về
        /// </summary>
        /// <param name="message"></param>
        private void HandleUpdateSharedCoinMessage(UpdateShareCoinNetMessage message)
        {
            SharedCoin.Value += message.amount;
        
            var isHost = NetworkManager.Singleton.IsHost;
            if (isHost)
            {
                // update lại cho clients sau khi đã cập nhật shared coin
                _netMessageTransmitter.SendNetMessage(new UpdateShareCoinNetMessage()
                {
                    amount = message.amount
                }, null, true);
            }
        }

        // Cái này sẽ được client gọi để update shared coin lên host
        // sau đó host lắng nghe và update ngược lại shared coin cho clients thong qua hàm HandleUpdateSharedCoinMessage
        public void UpdateSharedCoin(int amount)
        {
            var messageToSend = new UpdateShareCoinNetMessage
            {
                amount = amount
            };

            _netMessageTransmitter.SendNetMessage(messageToSend);
        }
        /// <summary>
        /// Chia xử lý tiền từng màn rồi chia ra cho từng người chơi
        /// </summary>
        public void DivideSharedCoinToClients()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                var allClientIds = NetworkManager.Singleton.ConnectedClientsIds.Count;
                var dividedCoins = SharedCoin.Value / allClientIds;

                var message = new UpdatePrivateCoinToClientNetMessage
                {
                    privateCoin = dividedCoins
                };
                _netMessageTransmitter.SendNetMessage(message);
            }
        }
    }
}

