using UnityEngine;
using Photon.Pun;

namespace WatStudios.DeepestDungeon.Utility.CustomPhoton
{

    [AddComponentMenu("Photon Networking/Photon IK View")]
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(LineRenderer))]
    public class PhotonLineView : MonoBehaviour, IPunObservable
    {
        #region Private Fields

        private PhotonView _photonView;
        private LineRenderer _lineRenderer;

        private Vector3 _positionZero;
        private Vector3 _networkPositionZero;

        private Vector3 _positionOne;
        private Vector3 _networkPositionOne;

        private Vector2 _offset;
        private Vector2 _networkOffset;

        private bool _isEnabled;
        private bool _networkIsEnabled;

        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _photonView = this.gameObject.GetComponent<PhotonView>();
            _lineRenderer = this.gameObject.GetComponent<LineRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!this._photonView.IsMine)
            {
                if (this._networkIsEnabled)
                {
                    this._lineRenderer.SetPosition(0, this._networkPositionZero);
                    this._lineRenderer.SetPosition(1, this._networkPositionOne);
                    this._lineRenderer.GetComponent<Renderer>().material.mainTextureOffset = this._networkOffset;
                    this._lineRenderer.enabled = true;
                }
                else if (!this._networkIsEnabled)
                    this._lineRenderer.enabled = false;
            }
        }
        #endregion

        #region Public Methods
        public void EnableLine(Vector3 positionZero, Vector3 positionOne, Vector2 offset)
        {
            this._isEnabled = true;
            this._positionZero = positionZero;
            this._positionOne = positionOne;
            this._offset = offset;
        }

        public void DisableLine()
        {
            this._isEnabled = false;
        }
        #endregion

        #region IPunObservable implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(this._isEnabled);

                if (this._isEnabled)
                {
                    stream.SendNext(this._positionZero);
                    stream.SendNext(this._positionOne);
                    stream.SendNext(this._offset);
                }

            }
            else
            {
                this._networkIsEnabled = (bool)stream.ReceiveNext();

                if (this._networkIsEnabled)
                {
                    this._networkPositionZero = (Vector3)stream.ReceiveNext();
                    this._networkPositionOne = (Vector3)stream.ReceiveNext();
                    this._networkOffset = (Vector2)stream.ReceiveNext();
                }
            }
        }
        #endregion
    }
}
