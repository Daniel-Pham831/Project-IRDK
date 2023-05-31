using Cysharp.Threading.Tasks;
using Game.Commands;
using Game.Networking.Network.NetworkModels;
using Game.Networking.Network.NetworkModels.Handlers;
using Game.Networking.Network.NetworkModels.Handlers.NetPlayerModel;
using Maniac.Command;
using Maniac.LanguageTableSystem;
using Maniac.Utils;
using Maniac.Utils.Extension;
using UnityEngine;

namespace Game.Networking.Lobby.Commands
{
    public class KickPlayerFromLobbyCommand : Command
    {
        private readonly string _lobbyId;
        private readonly string _playerId;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        private LanguageTable _LanguageTable => Locator<LanguageTable>.Instance;
        private NetModelHub _netModelHub => Locator<NetModelHub>.Instance;
        private NetPlayerModelHandler _netPlayerModelHandler;

        public KickPlayerFromLobbyCommand(string lobbyId, string playerId)
        {
            _lobbyId = lobbyId;
            _playerId = playerId;
        }

        public override async UniTask Execute()
        {
            _netPlayerModelHandler = _netModelHub.GetHandler<NetPlayerModelHandler>();
            
            var playerToKick = _lobbySystem.GetPlayerInJoinedLobby(_playerId);
            if (playerToKick == null) return;
            
            var header = _LanguageTable.Get(LanguageTable.Confirmation_KickPlayerHeader);
            var body = _LanguageTable.Get(LanguageTable.Confirmation_KickPlayerBody);
            var model = await _netPlayerModelHandler.GetModelByPlayerId(_playerId);
            
            var playerNameWithColor = (model != null ? model.Name : $"player").AddColor(Color.red);
            
            var shouldKick = await new ShowConfirmationDialogCommand(
                    header.Format(playerNameWithColor),
                    body.Format(playerNameWithColor))
                .ExecuteAndGetResult();
            if (!shouldKick) return;
            
            var success = await _lobbySystem.KickPlayerFromLobby(_lobbyId, _playerId);
            if (success)
            {
                await ShowKickSuccess(playerNameWithColor);
            }
            else
            {
                await ShowKickFail(playerNameWithColor);
            }
        }

        private async UniTask ShowKickSuccess(string playerName)
        {
            var header = _LanguageTable.Get(LanguageTable.Information_KickSuccessHeader);
            var body = _LanguageTable.Get(LanguageTable.Information_KickSuccessBody);

            await new ShowInformationDialogCommand(header.Format(playerName), body.Format(playerName)).Execute();
        }

        private async UniTask ShowKickFail(string playerName)
        {
            var header = _LanguageTable.Get(LanguageTable.Information_KickFailHeader);
            var body = _LanguageTable.Get(LanguageTable.Information_KickFailBody);

            await new ShowInformationDialogCommand(header.Format(playerName), body.Format(playerName)).Execute();
        }
    }
}