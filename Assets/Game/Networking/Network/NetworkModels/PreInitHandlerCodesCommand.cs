using Cysharp.Threading.Tasks;
using Game.Networking.Network.NetworkModels.Handlers;
using Maniac.Command;
using Maniac.Utils.Extension;

namespace Game.Networking.Network.NetworkModels
{
    public class PreInitHandlerCodesCommand : Command
    {
        public override async UniTask Execute()
        {
            var allSubClasses = typeof(NetHandler<>).GetAllSubclasses2();
            ushort ushortCounter = 0;
            foreach (var subClassType in allSubClasses)
            {
                HandlerCode.Add(ushortCounter++, subClassType.Name);
            }
        }
    }
}