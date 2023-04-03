using Game.CloudProfileSystem;
using Maniac.ProfileSystem;

namespace Game.Networking
{
    public class UserProfile : CloudProfileRecord
    {
        public string DisplayName { get; set; } = "";
    }
}