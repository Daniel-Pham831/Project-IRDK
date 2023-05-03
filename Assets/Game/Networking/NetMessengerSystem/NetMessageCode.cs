using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace Game.Networking.NetMessengerSystem
{
    public static class NetMessageCode
    {
        public static Dictionary<ushort,string> NetMessageCodes = new Dictionary<ushort, string>();
        private static Dictionary<string,ushort> _messageCodes = new Dictionary<string, ushort>();
        public static Dictionary<string,Type> MessageTypes = new Dictionary<string, Type>();
        
        public static void Add(ushort code, string type, Type subClassType)
        {
            if(!NetMessageCodes.ContainsKey(code))
            {
                NetMessageCodes.Add(code, type);
                _messageCodes.Add(type,code);
                MessageTypes.Add(type,subClassType);
            }
            else
            {
                Debug.LogError("NetMessageCode already contains this code!");
            }
        }

        public static Type GetMessageTypeFromUshort(ushort code)
        {
            if (NetMessageCodes.ContainsKey(code))
            {
                var type = NetMessageCodes[code];
                if (MessageTypes.ContainsKey(type))
                {
                    return MessageTypes[type];
                }
            }

            return null;
        }
        
        public static string GetMessageNameFromUshort(ushort code)
        {
            if (NetMessageCodes.ContainsKey(code))
            {
                return NetMessageCodes[code];
            }

            return null;
        }
        
        public static ushort GetUshortFromMessageType(Type type)
        {
            if (MessageTypes.ContainsValue(type))
            {
                var key = MessageTypes.FirstOrDefault(x => x.Value == type).Key;
                if (_messageCodes.ContainsKey(key))
                {
                    return _messageCodes[key];
                }
            }

            Debug.LogError("Something went wrong!");
            return 0;
        }
        
        public static ushort GetUshortFromString(string type)
        {
            if (_messageCodes.ContainsKey(type))
            {
                return _messageCodes[type];
            }

            Debug.LogError("Something went wrong!");
            return 0;
        }
    }
}