using UnityEngine;
using Photon.Pun;

namespace WatStudios.DeepestDungeon.Utility.CustomPhoton
{
    [RequireComponent(typeof(PhotonView))]
    public class PhotonRotateView : MonoBehaviour, IPunObservable
    {
        private PhotonView _photonview;

        private Quaternion _networkRotation;

        // Start is called before the first frame update
        void Awake()
        {
            _photonview = this.gameObject.GetComponent<PhotonView>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!this._photonview.IsMine)
                transform.rotation = _networkRotation;
        }

        #region IPunObservable implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(this.transform.rotation);
            }
            else
            {
                this._networkRotation = (Quaternion)stream.ReceiveNext();
            }
        }
        #endregion
    }
}