using Assets.Game.Coin;
using Game.Networking.NetMessengerSystem;
using Game.Networking.NetMessengerSystem.NetMessages;
using Maniac.MessengerSystem.Messages;
using Maniac.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class CoinSystem : INetMessageListener
{
    public int shareCoin;
    public int pricateCoin; // local
    private NetMessageTransmitter _netMessageTransmitter => Locator<NetMessageTransmitter>.Instance;

    /// <summary>
    ///  Khởi tạo dữ liệu coin
    /// </summary>
    public void Init()
    {
        Locator<CoinSystem>.Set(this);
        _netMessageTransmitter.Register<UpdateShareCoinNetMessage>(this);
        _netMessageTransmitter.Register<UpdateShareCoinToClinetNetMessage>(this);
    }

    public void OnNetMessageReceived(NetMessage receivedMessage)
    {
        switch (receivedMessage)
        {
            case UpdateShareCoinNetMessage message:
                this.HandleUpdateCoin(message);
                break;
            case UpdateShareCoinToClinetNetMessage message:
                this.HandleShareCoinToClient(message);
                break;
        }


    }

    private void HandleShareCoinToClient(UpdateShareCoinToClinetNetMessage message)
    {
        pricateCoin = message.pricateCoin;
        Debug.Log($"Received private coin {pricateCoin}");
    }

    // <summary>
    /// Upadte Coin Host
    /// </summary>
    public void UpdateShareCoid(int amout)
    {
        ///Cộng tiền vào share coid
        shareCoin += amout;
        Debug.Log($"{NetworkManager.Singleton.IsHost} {shareCoin}");
    }
   
    /// <summary>
    /// xử lý coin từ client chả về
    /// </summary>
    /// <param name="message"></param>
    private void HandleUpdateCoin(UpdateShareCoinNetMessage message)
    {
        var ishost = NetworkManager.Singleton.IsHost;
        if (ishost)
        {
            UpdateShareCoid(message.amount);
        }
    }

    public void UpdateSharedCoinToHost(int amount)
    {
        var messageToSend = new UpdateShareCoinNetMessage();
        messageToSend.amount = amount;

        var sendToId = new List<ulong>();
        sendToId.Add(NetworkManager.ServerClientId);

        _netMessageTransmitter.SendNetMessage(messageToSend, sendToId);
    }

    //public void UpdateDivideSharedCoinToHost()
    //{
    //    var messageToSend = new UpdateShareCoinToClinetNetMessage();
    //    messageToSend.pricateCoin = this.pricateCoin;
    //     Debug.Log($"{NetworkManager.Singleton.IsHost},//{messageToSend.pricateCoin}== is oke,{pricateCoin.ToString()}");
    //     var sendToId = new List<ulong>();
    //    sendToId.Add(NetworkManager.ServerClientId);
    //    _netMessageTransmitter.SendNetMessage(messageToSend);
    //}

    //public void DivideSharedCoinToClients()
    //{
    //    if (NetworkManager.Singleton.IsHost)
    //    {
    //        var allClientIds = NetworkManager.Singleton.ConnectedClientsIds.ToArray().Length;
    //        pricateCoin = shareCoin / allClientIds;
    //        Debug.Log($"{NetworkManager.Singleton.IsHost},{ pricateCoin.ToString()}");
    //     
    //    }
    //}

    public void DivideSharedCoinToClients()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            var allClientIds = NetworkManager.Singleton.ConnectedClientsIds.ToArray().Length;
            var dividedCoins = shareCoin / allClientIds;

            var message = new UpdateShareCoinToClinetNetMessage();
            message.pricateCoin = dividedCoins;
            _netMessageTransmitter.SendNetMessage(message);
        }
    }
}

