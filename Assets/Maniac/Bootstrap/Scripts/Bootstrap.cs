using Game.CloudProfileSystem;
using Game.Commands;
using Game.Networking.Lobby;
using Game.Networking.Network;
using Game.Networking.Relay;
using Game.Scenes;
using Game.Scenes.MainMenu.Commands;
using Game.Scripts;
using Game.Services;
using Game.Services.UnityServices;
using Maniac.AudioSystem;
using Maniac.DataBaseSystem;
using Maniac.LanguageTableSystem;
using Maniac.ProfileSystem;
using Maniac.RandomSystem;
using Maniac.Services;
using Maniac.SpawnerSystem;
using Maniac.TimeSystem;
using Maniac.UISystem;
using UnityEngine;

namespace Maniac.Bootstrap.Scripts
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private DataBase dataBase;
        [SerializeField] private LanguageTable languageTable;
        
        [Header("UI")]
        [SerializeField] private UIData uiData;
        [SerializeField] private UIManager uiManagerPrefab;
        
        [Header("Audio")]
        [SerializeField] private AudioData audioData;
        [SerializeField] private AudioObjectInstance audioObjectPrefab;

        private async void Start()
        {
            var bootStrapService = CreateBootStrapServiceGroup();
            await bootStrapService.Run();
        }

        private Service CreateBootStrapServiceGroup()
        {
            var essentialServiceGroup = CreateEssentialServiceGroup();
            var bootstrapLoadingServiceGroup = CreateBootstrapLoadingServiceGroup();

            var bootStrap = new SequenceServiceGroup("BootStrap Service");
            bootStrap.Add(essentialServiceGroup);
            bootStrap.Add(new ShowSplashBannerService());
            bootStrap.Add(bootstrapLoadingServiceGroup);

            return bootStrap;
        }

        private Service CreateEssentialServiceGroup()
        {
            var essentialServiceGroup = new SequenceServiceGroup("Maniac Essential Services");
            
            essentialServiceGroup.Add(new LimitFrameRateService());
            essentialServiceGroup.Add(new InitRandomerService());
            essentialServiceGroup.Add(new InitDotweenService());
            essentialServiceGroup.Add(new InitTimeManagerService());
            essentialServiceGroup.Add(new InitDataBaseService(dataBase));
            essentialServiceGroup.Add(new InitUIManagerService(uiData,uiManagerPrefab));
            essentialServiceGroup.Add(new InitSpawnerManagerService());
            essentialServiceGroup.Add(new InitProfileManagerService());
            essentialServiceGroup.Add(new InitLanguageTableService(languageTable)); //this should be behind InitProfileManagerService
            essentialServiceGroup.Add(new InitAudioManagerService(audioData,audioObjectPrefab)); //this should be behind InitProfileManagerService

            return essentialServiceGroup;
        }

        private Service CreateBootstrapLoadingServiceGroup()
        {
            var bootstrapLoadingServiceGroup = new BootstrapLoadingServiceGroup("Loading Services");
            bootstrapLoadingServiceGroup.Add(new InitUnityEventSenderService());
            bootstrapLoadingServiceGroup.Add(new InitUnityServicesService());
            bootstrapLoadingServiceGroup.Add(new InitAuthenticationService());
            bootstrapLoadingServiceGroup.Add(new InitRemoteConfigService());
            bootstrapLoadingServiceGroup.Add(new InitCloudProfileManagerService());
            bootstrapLoadingServiceGroup.Add(new InitNetworkSystemService());
            bootstrapLoadingServiceGroup.Add(new InitLobbySystemService());
            bootstrapLoadingServiceGroup.Add(new InitRelaySystemService());

            var commandServiceGroup = new SequenceCommandServiceGroup("Command Service Group");
            commandServiceGroup.Add( new LoadMainMenuSceneCommand());
            
            bootstrapLoadingServiceGroup.Add(commandServiceGroup);
            return bootstrapLoadingServiceGroup;
        }
    }
}