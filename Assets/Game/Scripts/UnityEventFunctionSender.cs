using System;
using Maniac.MessengerSystem.Base;
using Maniac.MessengerSystem.Messages;
using UnityEngine;

namespace Game.Scripts
{
    public class UnityEventFunctionSender : MonoBehaviour
    {
        private void OnApplicationQuit()
        {
            Messenger.SendMessage(new ApplicationQuitMessage());
        }
    }
    
    public class ApplicationQuitMessage : Message{}
}