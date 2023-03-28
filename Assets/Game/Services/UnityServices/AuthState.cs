namespace Game.Services.UnityServices
{
    public enum AuthState
    {
        Initialized,
        Authenticating,
        Authenticated,
        Error,
        TimedOut
    }
}