using System;

namespace Components
{
    public class Singleton<T> where T : new()
    {
        private static readonly Lazy<T> Core = new Lazy<T>(() => new T());

        public static T Instance => Core.Value;
    }
}
