using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _010.Container._01._DesignPattern._01._Generative
{
    public abstract class Singleton<T> where T : class, new()
    {
        protected Singleton() { }

        private static readonly Lazy<T> instance = new Lazy<T>(() => new T());

        public static T Instance => instance.Value;
    }
}
