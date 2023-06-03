using System.Collections.Generic;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Utility
{
    public class ScriptableObjectDataBase
    {
        #region Private Fields
        private HashSet<ScriptableObject> _elements;
        private Dictionary<int, ScriptableObject> _elementsLookup;
        private Dictionary<ScriptableObject, int> _elementsReverseLookup;
        #endregion

        public ScriptableObjectDataBase()
        {
            _elements = new HashSet<ScriptableObject>();
            _elementsLookup = new Dictionary<int, ScriptableObject>();
            _elementsReverseLookup = new Dictionary<ScriptableObject, int>();
        }

        #region Public Methods
        public bool Add<T>(T element) where T : ScriptableObject
        {
            if (_elements.Contains(element))
            {
                return false;
            }
            else
            {
                _elementsLookup.Add(_elements.Count, element);
                _elementsReverseLookup.Add(element, _elements.Count);
                _elements.Add(element);
                return true;
            }
        }

        public bool TryGetElement<T>(int id, out T element) where T : ScriptableObject
        {
            _elementsLookup.TryGetValue(id, out ScriptableObject ele);
            element = ele as T;
            return true;
        }
        public bool TryGetElementId<T>(T element, out int id) where T : ScriptableObject
        {
            return _elementsReverseLookup.TryGetValue(element, out id);
        }
        #endregion
    }
}