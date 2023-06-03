using Cysharp.Threading.Tasks;
using Maniac.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Monster
{

    public class MonsterSystem
    {
        public MonsterSpawner monsterSpawner;

        public async UniTask Init()
        {
            Locator<MonsterSystem>.Set(this, true);

            monsterSpawner = new MonsterSpawner();
        }
    }
}
