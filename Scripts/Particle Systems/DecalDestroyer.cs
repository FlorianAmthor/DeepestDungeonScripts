using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Particles
{
    public class DecalDestroyer : MonoBehaviourPun
    {
        #region Public Fields
        public float lifeTime = 5.0f;
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            if (photonView.Owner == PhotonNetwork.LocalPlayer)
                StartCoroutine(DestroyParticle());
        }
        private IEnumerator DestroyParticle()
        {
            yield return new WaitForSeconds(lifeTime);
            PhotonNetwork.Destroy(gameObject);
        }
        #endregion
    }
}