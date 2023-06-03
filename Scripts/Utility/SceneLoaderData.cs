using UnityEngine;
using UnityEngine.SceneManagement;

namespace WatStudios.DeepestDungeon.Utility
{
    [CreateAssetMenu(fileName = "SceneLoaderData", menuName = "ScriptableObjects/SceneLoaderData")]
    public class SceneLoaderData : ScriptableObject
    {
        public static int Index { get; private set; }

        public static string Name { get; private set; }

        public static void Reset()
        {
            Index = -1;
            Name = "";
        }

        public static void SetData(int sceneIndex)
        {
            Index = sceneIndex;
        }

        public static void SetData(string sceneName)
        {
            Name = sceneName;
        }
    }
}
