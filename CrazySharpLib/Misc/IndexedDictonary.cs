using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrazySharpLib.Misc
{
    class IndexedDictonary<T, U>
    {
        private readonly Dictionary<T, U> _dictionary = new Dictionary<T, U>();
        private readonly List<U> _list = new List<U>();

        public void Add(T key, U value)
        {
            _dictionary.Add(key,value);
            _list.Add(value);
        }

        public U this[int i]
        {
            get { return _list[i]; }
        }

        public U this[T key]
        {
            get { return _dictionary[key]; }
        }

        public bool ContainsKey(T key)
        {
            return _dictionary.ContainsKey(key);
        }
    }
}
