using UnityEngine;
using Photon.Pun;

namespace WatStudios.DeepestDungeon.Utility.CustomPhoton
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(ParticleSystem))]
    public class PhotonParticleView : MonoBehaviour, IPunObservable
    {
        #region Private Fields

        private PhotonView _photonView;
        private ParticleSystem _particleSystem;

        private Vector3 _particlePosition;
        private Vector3 _particleNetworkPosition;

        private Quaternion _particleRotation;
        private Quaternion _particleNetworkRotation;

        private bool _playParticle;
        private bool _playNetworkParticle;

        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _photonView = this.gameObject.GetComponent<PhotonView>();
            _particleSystem = this.gameObject.GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!this._photonView.IsMine)
            {
                if (this._playNetworkParticle)
                {
                    this._particleSystem.transform.position = _particleNetworkPosition;
                    this._particleSystem.transform.rotation = _particleNetworkRotation;
                    this._particleSystem.Play();

                    if (!this._particleSystem.main.loop)
                    {
                        this._playParticle = false;
                        this._playNetworkParticle = false;
                    }
                }
                else
                {
                    if (!this._particleSystem.main.loop)
                        if (this._particleSystem.isPlaying)
                            this._particleSystem.Stop();
                }
            }
        }
        #endregion

        #region Public Methods
        public void PlayParticleSystem(Vector3 pos, Quaternion rot)
        {
            this._playParticle = true;
            this._particlePosition = pos;
            this._particleRotation = rot;
        }

        public void StopParticleSystem()
        {
            this._playParticle = false;
            this._playNetworkParticle = false;
        }
        #endregion

        #region IPunObservable implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(this._playParticle);

                //if (this._playParticle)
                //{
                    stream.SendNext(this._particlePosition);
                    stream.SendNext(this._particleRotation);
                //}

            }
            else
            {
                this._playNetworkParticle = (bool)stream.ReceiveNext();

                //if (this._playNetworkParticle)
                //{
                    this._particleNetworkPosition = (Vector3)stream.ReceiveNext();
                    this._particleNetworkRotation = (Quaternion)stream.ReceiveNext();
                //}
            }
        }
        #endregion
    }
}
