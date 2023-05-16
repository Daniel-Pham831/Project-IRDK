using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InitCoinSystemCommand : Command
{
    public override async UniTask Execute()
    {
        new CoinSystem().Init();
        await UniTask.CompletedTask;
    }
}