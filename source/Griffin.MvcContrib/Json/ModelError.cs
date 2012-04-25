using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Griffin.MvcContrib.Json
{
    /// <summary>
    ///   Create a json structure from model state
    /// </summary>
    /// <remarks>
    ///   <example>
    ///     Will return the following structure: 
    /// <code>
    /// [
    ///     { "propertyName": ["Field is required", "Must be max 10 chars"]} 
    /// ]</code>
    ///   </example>
    /// </remarks>
    [DataContract(Name = "model-errors", Namespace = "")]
    [XmlRoot("model-errors")]
    public class ModelErrorCollection : IJsonResponseContent, IDictionary<string, List<string>>
    {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        /// <summary>
        ///   Initializes a new instance of the <see cref="ModelError" /> class.
        /// </summary>
        /// <param name="modelState"> Model state. </param>
        /// <example>
        ///   <code>
        ///     [HttpPost]
        ///     public ActionResult Create(YourModel model) 
        ///     {
        ///         if (!ModelState.IsValid)
        ///         {
        ///             if (Request.IsAjax())
        ///             {
        ///                 return Json(new JsonResponse(new ModelError(ModelState)));
        ///             }
        ///         }
        ///     }</code>
        /// </example>
        public ModelErrorCollection(IEnumerable<KeyValuePair<string, ModelState>> modelState)
        {
            foreach (var kvp in modelState)
            {
                if (!kvp.Value.Errors.Any())
                    continue;

                foreach (var error in kvp.Value.Errors)
                {
                    Add(kvp.Key, error.ErrorMessage);
                }
            }
        }

        #region IEnumerable<KeyValuePair<string,List<string>>> Members

        /// <summary>
        ///   Gets the enumerator.
        /// </summary>
        /// <returns> </returns>
        public IEnumerator<KeyValuePair<string, List<string>>> GetEnumerator()
        {
            return _errors.GetEnumerator();
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
        ///   Add a new error message
        /// </summary>
        /// <param name="propertyName"> Name of the view model property. </param>
        /// <param name="errorMessage"> The error message. </param>
        public void Add(string propertyName, string errorMessage)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            if (errorMessage == null) throw new ArgumentNullException("errorMessage");

            List<string> errors;
            if (!_errors.TryGetValue(propertyName, out errors))
            {
                errors = new List<string>();
                _errors.Add(propertyName, errors);
            }

            errors.Add(errorMessage);
        }

        /*

        void ICollection<KeyValuePair<string, List<string>>>.Add(KeyValuePair<string, List<string>> item)
        {
            foreach (var error in item.Value)
            {
                Add(item.Key, error);
            }
            
        }

        void ICollection<KeyValuePair<string, List<string>>>.Clear()
        {
            _errors.Clear();
        }

        bool ICollection<KeyValuePair<string, List<string>>>.Contains(KeyValuePair<string, List<string>> item)
        {
            return _errors.ContainsKey(item.Key);
        }

        void ICollection<KeyValuePair<string, List<string>>>.CopyTo(KeyValuePair<string, List<string>>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<string, List<string>>>.Remove(KeyValuePair<string, List<string>> item)
        {
            return _errors.Remove(item.Key);
        }

        int ICollection<KeyValuePair<string, List<string>>>.Count
        {
            get { return _errors.Count; }
        }

        bool ICollection<KeyValuePair<string, List<string>>>.IsReadOnly
        {
            get { return false; }
        }*/
        void ICollection<KeyValuePair<string, List<string>>>.Add(KeyValuePair<string, List<string>> item)
        {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<string, List<string>>>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<string, List<string>>>.Contains(KeyValuePair<string, List<string>> item)
        {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<string, List<string>>>.CopyTo(KeyValuePair<string, List<string>>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<string, List<string>>>.Remove(KeyValuePair<string, List<string>> item)
        {
            throw new NotImplementedException();
        }

        int ICollection<KeyValuePair<string, List<string>>>.Count
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<KeyValuePair<string, List<string>>>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        bool IDictionary<string, List<string>>.ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        void IDictionary<string, List<string>>.Add(string key, List<string> value)
        {
            throw new NotImplementedException();
        }

        bool IDictionary<string, List<string>>.Remove(string key)
        {
            throw new NotImplementedException();
        }

        bool IDictionary<string, List<string>>.TryGetValue(string key, out List<string> value)
        {
            throw new NotImplementedException();
        }

        List<string> IDictionary<string, List<string>>.this[string key]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        ICollection<string> IDictionary<string, List<string>>.Keys
        {
            get { throw new NotImplementedException(); }
        }

        ICollection<List<string>> IDictionary<string, List<string>>.Values
        {
            get { throw new NotImplementedException(); }
        }
    }
}