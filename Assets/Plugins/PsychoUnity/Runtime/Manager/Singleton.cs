using System;

namespace PsychoUnity.Manager
{
    public class Singleton<T> where T : new()
    {
        private static readonly Lazy<T> Core = new Lazy<T>(() => new T());
        
        // Making Singleton impossible to new
        private protected Singleton(){}
        
        public static T Instance => Core.Value;
    }
}
