using System;

namespace Maniac.MessengerSystem.Messages
{
    /// <summary>
    /// Inherits this class to make a custom Message
    /// </summary>
    [Serializable]
    public abstract class Message
    {
        public string Type => ToString();
        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}