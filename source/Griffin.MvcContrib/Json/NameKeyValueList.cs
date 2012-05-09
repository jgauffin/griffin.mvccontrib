using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;

namespace Griffin.MvcContrib.Json
{
    /// <summary>
    ///   Double dictionary used by <see cref="ValidationRules" /> .
    /// </summary>
    /// <remarks>Created to get a proper serialization of the items. Feel free to contribute a nicer solution.</remarks>
    public class NameKeyValueList : IDictionary<string, Dictionary<string, string>>
    {
        public Dictionary<string, Dictionary<string, string>> _items =
            new Dictionary<string, Dictionary<string, string>>();

        #region IEnumerable<KeyValuePair<string,Dictionary<string,string>>> Members

        /// <summary>
        ///   Gets the enumerator.
        /// </summary>
        /// <returns> </returns>
        public IEnumerator<KeyValuePair<string, Dictionary<string, string>>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns> An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection. </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        ///   Add a new value
        /// </summary>
        /// <param name="propertyName"> View model property name </param>
        /// <param name="ruleName"> jQuery.validator rule name </param>
        /// <param name="value"> Depends on the usage </param>
        public void Add(string propertyName, string ruleName, string value)
        {
            Dictionary<string, string> inner;
            if (!_items.TryGetValue(propertyName, out inner))
            {
                inner = new Dictionary<string, string>();
                _items.Add(propertyName, inner);
            }
            inner.Add(ruleName, value);
        }

        void ICollection<KeyValuePair<string, Dictionary<string, string>>>.Add(KeyValuePair<string, Dictionary<string, string>> item)
        {
            throw new System.NotImplementedException();
        }

        void ICollection<KeyValuePair<string, Dictionary<string, string>>>.Clear()
        {
            throw new System.NotImplementedException();
        }

        bool ICollection<KeyValuePair<string, Dictionary<string, string>>>.Contains(KeyValuePair<string, Dictionary<string, string>> item)
        {
            throw new System.NotImplementedException();
        }

        void ICollection<KeyValuePair<string, Dictionary<string, string>>>.CopyTo(KeyValuePair<string, Dictionary<string, string>>[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        bool ICollection<KeyValuePair<string, Dictionary<string, string>>>.Remove(KeyValuePair<string, Dictionary<string, string>> item)
        {
            throw new System.NotImplementedException();
        }

        int ICollection<KeyValuePair<string, Dictionary<string, string>>>.Count
        {
            get { throw new System.NotImplementedException(); }
        }

        bool ICollection<KeyValuePair<string, Dictionary<string, string>>>.IsReadOnly
        {
            get { throw new System.NotImplementedException(); }
        }

        bool IDictionary<string, Dictionary<string, string>>.ContainsKey(string key)
        {
            throw new System.NotImplementedException();
        }

        void IDictionary<string, Dictionary<string, string>>.Add(string key, Dictionary<string, string> value)
        {
            throw new System.NotImplementedException();
        }

        bool IDictionary<string, Dictionary<string, string>>.Remove(string key)
        {
            throw new System.NotImplementedException();
        }

        bool IDictionary<string, Dictionary<string, string>>.TryGetValue(string key, out Dictionary<string, string> value)
        {
            throw new System.NotImplementedException();
        }

        Dictionary<string, string> IDictionary<string, Dictionary<string, string>>.this[string key]
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        ICollection<string> IDictionary<string, Dictionary<string, string>>.Keys
        {
            get { throw new System.NotImplementedException(); }
        }

        ICollection<Dictionary<string, string>> IDictionary<string, Dictionary<string, string>>.Values
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}