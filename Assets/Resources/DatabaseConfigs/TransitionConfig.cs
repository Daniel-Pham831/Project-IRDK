using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using Maniac.Utils.Extension;

namespace Maniac.DataBaseSystem
{
    public class TransitionConfig : DataBaseConfig
    {
        public float DefaultTransitionEnterDuration = 1f;
        public float DefaultTransitionExitDuration = 1f;
        public List<TransitionInfo> Transitions;
        
        public TransitionInfo GetRandomTransition()
        {
            return Transitions.TakeRandom();
        }
    }

    [Serializable]
    public class TransitionInfo
    {
        public string Id;
        public List<Sprite> SpriteFrames;
    }
}
