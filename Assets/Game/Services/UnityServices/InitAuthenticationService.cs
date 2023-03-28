using Cysharp.Threading.Tasks;
using Game.Networking;
using Maniac.Services;
using Maniac.Utils;
using ParrelSync;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Game.Services.UnityServices
{
    public class InitAuthenticationService : Service
    {
        private LocalSystem _localSystem => Locator<LocalSystem>.Instance;

        public override async UniTask<IService.Result> Execute()
        {
            var authenticationService = AuthenticationService.Instance;

            authenticationService.SignedIn += async () =>
            {
                _localSystem.LocalPlayer.Id = authenticationService.PlayerId;
            };

            authenticationService.SignInFailed += (err) => { Debug.LogError(err); };

            authenticationService.SignedOut += () => { Debug.Log("Player signed out."); };

            authenticationService.Expired += () => { Debug.Log("Player session could not be refreshed and expired."); };

            await SignInAnonymouslyAsync();

            return IService.Result.Success;
        }

        static async UniTask SignInAnonymouslyAsync(int maxRetries = 5)
        {
            var authenticationService = AuthenticationService.Instance;
            var authenticationState = AuthState.Authenticating;
            var tries = 0;
            while (authenticationState == AuthState.Authenticating && tries < maxRetries)
            {
                try
                {
                    //To ensure staging login vs non staging
                    await authenticationService.SignInAnonymouslyAsync();

                    if (authenticationService.IsSignedIn && authenticationService.IsAuthorized)
                    {
                        authenticationState = AuthState.Authenticated;
                        break;
                    }
                }
                catch (AuthenticationException ex)
                {
                    // Compare error code to AuthenticationErrorCodes
                    // Notify the player with the proper error message
                    Debug.LogError(ex);
                    authenticationState = AuthState.Error;
                }
                catch (RequestFailedException exception)
                {
                    // Compare error code to CommonErrorCodes
                    // Notify the player with the proper error message
                    Debug.LogError(exception);
                    authenticationState = AuthState.Error;
                }

                tries++;
                await UniTask.Delay(1000);
            }

            if (authenticationState != AuthState.Authenticated)
            {
                Debug.LogWarning($"Player was not signed in successfully after {tries} attempts");
                authenticationState = AuthState.TimedOut;
            }
        }
    }
}