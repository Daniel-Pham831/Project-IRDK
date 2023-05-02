using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace Game.Networking.NetMessengerSystem
{
    public static class NetMessageCode
    {
        public static Dictionary<ushort,FixedString32Bytes> NetMessageCodes = new Dictionary<ushort, FixedString32Bytes>();
        private static Dictionary<FixedString32Bytes,ushort> _messageCodes = new Dictionary<FixedString32Bytes, ushort>();
        public static Dictionary<FixedString32Bytes,Type> MessageTypes = new Dictionary<FixedString32Bytes, Type>();
        
        public static void Add(ushort code, FixedString32Bytes type, Type subClassType)
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
        
        public static ushort GetUshortFromFixedString32Bytes(FixedString32Bytes type)
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