using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WatStudios.DeepestDungeon.Utility
{
    public class SceneLoader : MonoBehaviour
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private Image _progressBar;
#pragma warning restore 649
        #endregion

        // Start is called before the first frame update
        public void Start()
        {
            StartCoroutine(LoadLevelAsync(SceneLoaderData.Index));
        }

        private IEnumerator LoadLevelAsync(int sceneIndex)
        {
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.LoadLevel(sceneIndex);

            while (PhotonNetwork.LevelLoadingProgress < 1)
            {
                _progressBar.fillAmount = PhotonNetwork.LevelLoadingProgress;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
