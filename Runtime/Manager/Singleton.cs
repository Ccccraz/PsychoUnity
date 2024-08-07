using System;

namespace PsychoUnity.Manager
{
    /// <summary>
    /// Base class for all singleton managers
    /// </summary>
    /// <typeparam name="T"> Pass in the class itself that needs to inherit the base class of the singleton. </typeparam>
    public class Singleton<T> where T : new()
    {
        private static readonly Lazy<T> Core = new Lazy<T>(() => new T());
        
        // Making Singleton impossible to new
        private protected Singleton(){}
        
        /// <summary>
        /// Returns the singleton instance
        /// </summary>
        public static T Instance => Core.Value;
    }
}
