using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;

namespace Maniac.DataBaseSystem
{
    public class TestConfig : DataBaseConfig
    {
        public bool TestBool;
        public int TestInt;
        public string TestString;
        public CustomData TestCustomData;
    }

    [Serializable]
    public class CustomData
    {
        public long TestLong;
        public string TestString2;
    }
}
