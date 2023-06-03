using Photon.Pun;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace WatStudios.DeepestDungeon.Utility
{
    public class TimedObjectDestroyer<T> where T : MonoBehaviourPun
    {
        private Dictionary<T, float> _objToDestroy;

        public TimedObjectDestroyer()
        {
            _objToDestroy = new Dictionary<T, float>();
        }

        /// <summary>
        /// Update Loop for the object destroyer
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            if (_objToDestroy.Count == 0)
                return;
            var keys = new List<T>(_objToDestroy.Keys);
            foreach (var key in keys)
            {
                _objToDestroy[key] = _objToDestroy[key] - deltaTime;
                if (_objToDestroy[key] <= 0.0f)
                {
                    _objToDestroy.Remove(key);
                    if (key.gameObject.GetPhotonView())
                    {
                        if (PhotonNetwork.IsMasterClient)
                            PhotonNetwork.Destroy(key.gameObject);
                    }
                    else
                        Object.Destroy(key.gameObject);
                }
            }
        }

        /// <summary>
        /// Adds the obj to be destroyed with its specified timeToLive
        /// </summary>
        /// <param name="timeBeforeDestroy">Time left before the object is destroyed</param>
        /// <param name="enemyObj">The object to destroy</param>
        public void Add(T enemyObj, float timeBeforeDestroy = 0.0f)
        {
            if (!_objToDestroy.ContainsKey(enemyObj))
                _objToDestroy.Add(enemyObj, timeBeforeDestroy);
        }

        /// <summary>
        /// Removes all all objects that should be destroyed from the timed queue
        /// </summary>
        public void ClearObjects()
        {
            _objToDestroy.Clear();
        }
    }
}