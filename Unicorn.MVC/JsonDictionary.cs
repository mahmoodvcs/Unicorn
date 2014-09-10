using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;

namespace Unicorn.MVC
{
    [Serializable]
    public class JsonDictionary<K, V> : IEnumerable<KeyValuePair<K, V>>, ISerializable, ICollection, IEnumerable
    {
        readonly Dictionary<K, V> dict = new Dictionary<K, V>();

        public JsonDictionary() { }

        protected JsonDictionary(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (K key in dict.Keys)
            {
                info.AddValue(key.ToString(), dict[key]);
            }
        }

        public void Add(K key, V value)
        {
            dict.Add(key, value);
        }

        public V this[K index]
        {
            set { dict[index] = value; }
            get { return dict[index]; }
        }
        public ICollection<K> Keys
        {
            get { return dict.Keys; }
        }

        public bool ContainsKey(K key)
        {
            return dict.ContainsKey(key);
        }

        public bool Remove(K key)
        {
            return dict.Remove(key);
        }

        public bool TryGetValue(K key, out V value)
        {
            return dict.TryGetValue(key, out value);
        }
        public V GetValueOrDefault(K key)
        {
            V ret;
            // Ignore return value
            TryGetValue(key, out ret);
            return ret;
        }

        public ICollection<V> Values
        {
            get { return dict.Values; }
        }


        public void Clear()
        {
            dict.Clear();
        }

        public int Count
        {
            get { return dict.Count; }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        public NameValueCollection ToNameValueCollection()
        {
            NameValueCollection nv = new NameValueCollection();
            foreach (var k in Keys)
                if (this[k] != null)
                    nv.Add(k.ToString(), this[k].ToString());
            return nv;
        }
    }

    public static class DictionaryExtentions
    {
        public static TV GetValueOrDefault<TK, TV>(this IDictionary<TK, TV> dic, TK key)
        {
            TV ret;
            // Ignore return value
            dic.TryGetValue(key, out ret);
            return ret;
        }
        public static NameValueCollection ToNameValueCollection(this IDictionary<string, object> dic)
        {
            NameValueCollection nv = new NameValueCollection();
            foreach (var k in dic.Keys)
                if (dic[k] != null)
                    nv.Add(k.ToString(), dic[k].ToString());
            return nv;
        }
        public static NameValueCollection ToNameValueCollection(this JsonDictionary<string, object> dic)
        {
            NameValueCollection nv = new NameValueCollection();
            foreach (var k in dic.Keys)
                if (dic[k] != null)
                    nv.Add(k.ToString(), dic[k].ToString());
            return nv;
        }
    }
}
