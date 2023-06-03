using UnityEngine;
using WatStudios.DeepestDungeon.Utility;

namespace WatStudios.DeepestDungeon.Core
{
    public class DatabaseManager : MonoBehaviour
    {
        #region Singleton
        private static DatabaseManager Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Init();
            }
        }
        #endregion

        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField]
        private ScriptableObject[] _elementsToAdd;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private ScriptableObjectDataBase _db;
        #endregion

        #region Private Methods
        private void Init()
        {
            _db = new ScriptableObjectDataBase();
            foreach (var item in _elementsToAdd)
            {
                _db.Add(item);
            }
        }
        #endregion

        #region Public Methods
        public static bool TryGetId<T>(T element, out int id) where T : ScriptableObject
        {
            return Instance._db.TryGetElementId(element, out id);
        }

        public static bool TryGetElement<T>(int id, out T element) where T : ScriptableObject
        {
            return Instance._db.TryGetElement(id, out element);
        }
        #endregion
    }
}
