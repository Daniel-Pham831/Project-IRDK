using Unity.Services.Lobbies.Models;

namespace Game.Networking
{
    public class PlayerDataObject<T> : PlayerDataObject
    {
        public PlayerDataObject(VisibilityOptions visibility, string value = null) : base(visibility, value)
        {
        }
        
        public PlayerDataObject(VisibilityOptions visibility ,T tValue = default) : base(visibility, "")
        {
            TValue = tValue;
        }

        public T TValue { get; set; }
    }
}