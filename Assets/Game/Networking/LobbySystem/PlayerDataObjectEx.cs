using Unity.Services.Lobbies.Models;

namespace Game.Networking.LobbySystem
{
    public class PlayerDataObjectEx<T> :PlayerDataObject
    {
        public T TValue { get; set; }

        public PlayerDataObjectEx(VisibilityOptions visibility, string value = null) : base(visibility, value)
        {
        }
        
        public PlayerDataObjectEx(VisibilityOptions visibility, T tValue) : base(visibility, "")
        {
            TValue = tValue;
        }
    }
}