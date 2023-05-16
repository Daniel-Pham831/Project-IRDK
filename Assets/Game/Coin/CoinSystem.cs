using Assets.Game.Coin;
using Game.Networking.NetMessengerSystem;
using Game.Networking.NetMessengerSystem.NetMessages;
using Maniac.Utils;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinSystem : INetMessageListener
{
    public int shareCoin;
    private NetMessageTransmitter _netMessageTransmitter => Locator<NetMessageTransmitter>.Instance;
    public void AddCoin(int amout)
    {
        shareCoin += amout;
    }
    public void Init()
    {
        _netMessageTransmitter.Register<UpdateShareCoinNetMessage>(this);
        _netMessageTransmitter.SendNetMessage(new UpdateShareCoinNetMessage(), new List<ulong>());
    }
    public void OnNetMessageReceived(NetMessage message)
    {
       switch (message)
        {
            case UpdateShareCoinNetMessage AmoutCoinClinetToHost:
                this.HandleUpdateCoin(AmoutCoinClinetToHost.amout);
                break;

        }    
            
        
    }
    // <summary>
    /// Upadte Coin Host
    /// </summary>
    public void UpdateShareCoid(int amout)
    {



    }
    /// <summary>
    /// Upadte Coin to clinet
    /// </summary>
    public void UpdateShareCoidToClinet()
    {

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="amout"></param>
    private void HandleUpdateCoin(int amout)
    {
        var ishost = NetworkManager.Singleton.IsHost;
        if (ishost)
        {

        }
    }


}
