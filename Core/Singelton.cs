using System;

namespace Core
{
    /// <summary>
    /// Base class for creation singelton
    /// ATENSION! You need implement method for creation Object, to use 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singelton<T> where T: class
    {
        protected Singelton() //not used
        {
        }
        
        protected static T Object;

        /// <summary>
        /// Instance of Singelton
        /// <exception cref="NullReferenceException">Return if Instance is null</exception>
        /// </summary>
        public static T Instance => Object ?? throw new NullReferenceException("Singelton hasn't been initialized");
    }
}