using UniRx;

namespace Maniac.Utils.Extension
{
    public static class ReactivePropertyExtension
    {
        public static void SelfNotify<T>(this ReactiveProperty<T> property)
        {
            property.SetValueAndForceNotify(property.Value);
        }
    }
}