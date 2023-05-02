using System;
using System.Collections.Generic;

namespace Game.Networking.Network.NetworkModels
{
    public static class HandlerCode
    {
        public static Dictionary<ushort,string> HandlerCodes = new Dictionary<ushort, string>();
        private static Dictionary<string,ushort> _handlerCodes = new Dictionary<string, ushort>();
        
        public static void Add(ushort code, string handlerKey)
        {
            if(!HandlerCodes.ContainsKey(code))
            {
                HandlerCodes.Add(code, handlerKey);
                _handlerCodes.Add(handlerKey, code);
            }
        }
        
        public static ushort GetCode(string handlerKey)
        {
            if (_handlerCodes.TryGetValue(handlerKey, out var code))
            {
                return code;
            }

            throw new Exception($"Cannot find {handlerKey}. Please check!");
        }
        
        public static string GetHandlerKey(ushort code)
        {
            if (HandlerCodes.TryGetValue(code, out var handlerKey))
            {
                return handlerKey;
            }

            throw new Exception($"Cannot find {code}. Please check!");
        }
    }
}