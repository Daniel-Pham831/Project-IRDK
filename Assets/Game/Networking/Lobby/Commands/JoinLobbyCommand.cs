using System;
using Cysharp.Threading.Tasks;
using Game.Commands;
using Game.Networking.Network;
using Game.Networking.Relay.Commands;
using Game.Scenes;
using Maniac.Command;
using Maniac.LanguageTableSystem;
using Maniac.Utils;
using Unity.Networking.Transport.Relay;

namespace Game.Networking.Lobby.Commands
{
    public class JoinLobbyCommand : Command
    {
        private readonly string _lobbyIdOrCode;
        private readonly bool _useId;

        private NetworkSystem _networkSystem => Locator<NetworkSystem>.Instance;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        public JoinLobbyCommand(string lobbyIdOrCode,bool useId)
        {
            _lobbyIdOrCode = lobbyIdOrCode;
            _useId = useId;
        }

        public override async UniTask Execute()
        {
            await new ShowConnectToServerCommand().Execute();
            try
            {
                var joinedLobby = _useId
                    ? await _lobbySystem.JoinLobbyById(_lobbyIdOrCode)
                    : await _lobbySystem.JoinLobbyByCode(_lobbyIdOrCode);
                
                var  joinAllocation = await new JoinRelayWithLobbyCommand(joinedLobby).ExecuteAndGetResult();
                _networkSystem.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
                _networkSystem.NetworkManager.StartClient();
            }
            catch
            {
                await ShowJoinFail();
                return;
            }
            
            await new HideConnectToServerCommand().Execute();
            await new LoadEmptySceneCommand().Execute();
            await new LoadSceneCommand(new LoadSceneCommand.Param(SceneName.LobbyRoom,true)).Execute();
        }
        
        private async UniTask ShowJoinFail()
        {
            await new HideConnectToServerCommand().Execute();
            await new ShowInformationDialogCommand(LanguageTable.Information_FailToJoinLobbyHeader,
                LanguageTable.Information_FailToJoinLobbyBody).Execute();
        }
    }
}